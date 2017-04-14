using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Xsolla
{
	public class HistoryController: MonoBehaviour
	{
		public Text mTitle;
		public GameObject mHistoryContainer;
		public Button mBtnRefresh;
		public GameObject mBtnContinue;
		public MyRotation mProgressBar;
		public Text mEmptyItems;

		private XsollaUtils mUtils;
		private const string mHistoryUrl = "paystation2/api/balance/history";
		private const string PREFAB_HISTORY_ROW  = "Prefabs/SimpleView/HistoryItem";
		private List<HistoryElemController> mList;
		private int mCountMore = 20;
		private bool mSortDesc = true;

		/// <summary>
		/// Init the specified pUtils.
		/// </summary>
		/// <param name="pUtils">P utils.</param>
		public void Init(XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mList = new List<HistoryElemController>();
			mTitle.text = mUtils.GetTranslations().Get("balance_history_page_title");
			mEmptyItems.text = mUtils.GetTranslations().Get("balance_history_no_data");
			mBtnContinue.GetComponent<Text>().text = mUtils.GetTranslations().Get("balance_back_button");
			mBtnContinue.GetComponent<Button>().onClick.AddListener(delegate 
				{
					Destroy();
				});

			// Добавляем заголовок
			AddHeader();

			// Делаем запрос на лист
			GetRequestList();
		}

		/// <summary>
		/// Gets the request list.
		/// </summary>
		private void GetRequestList()
		{
			mProgressBar.SetLoading(true);
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("offset", mList.Count);
			lParams.Add("limit", mCountMore);
			lParams.Add("sortDesc", mSortDesc.ToString().ToLower());
			lParams.Add("sortKey", "dateTimestamp");
			Logger.Log("Request on history: offset - " + mList.Count + " Limit - " + mCountMore + " Sort - " + mSortDesc.ToString());
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mHistoryUrl, lParams), HistoryListRecived, ErrorRecived, false);
		}

		/// <summary>
		/// Histories the list recived.
		/// </summary>
		/// <param name="pNode">P node.</param>
		private void HistoryListRecived(JSONNode pNode)
		{
			XsollaHistoryList lList = new XsollaHistoryList().Parse(pNode["operations"]) as XsollaHistoryList;
			for (int idx = 0; idx < lList.Count ; idx ++)
			{
				AddHistoryRow(lList.GetItemByPosition(idx));
			}

			// Если лист пустой
			mEmptyItems.gameObject.SetActive(lList.Count == 0);
			mHistoryContainer.transform.parent.gameObject.SetActive(lList.Count != 0);

			mProgressBar.SetLoading(false);
		}

		/// <summary>
		/// Errors the recived.
		/// </summary>
		/// <param name="pError">P error.</param>
		private void ErrorRecived(XsollaErrorRe pError)
		{
			
		}

		/// <summary>
		/// Sorts the history.
		/// </summary>
		private void SortHistory()
		{
			mSortDesc = !mSortDesc;
			OnRefreshHistory();
		}

		/// <summary>
		/// Clears the list.
		/// </summary>
		private void ClearList()
		{
			Logger.Log("Clear histroy List");

			List<GameObject> children = new List<GameObject>();
			foreach (Transform child in mHistoryContainer.transform) 
			{
				if (child != mProgressBar.transform)
					children.Add(child.gameObject);
			}
			
			children.ForEach(child => Destroy(child));

			mHistoryContainer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,0);
			mList.Clear();
		}

		/// <summary>
		/// Raises the refresh history event.
		/// </summary>
		public void OnRefreshHistory()
		{
			mProgressBar.SetLoading(true);
			ClearList();
			AddHeader();
			GetRequestList();
		}

		/// <summary>
		/// Adds the header row.
		/// </summary>
		private void AddHeader()
		{
			GameObject itemRow = Instantiate(Resources.Load(PREFAB_HISTORY_ROW)) as GameObject;
			HistoryElemController controller = itemRow.GetComponent<HistoryElemController>();
			controller.Init(mUtils, null, false, SortHistory, true, mSortDesc);
			itemRow.transform.SetParent(mHistoryContainer.transform);
			Resizer.SetDefScale(itemRow);
			MoveProgresBarOnLastChild();
		}

		/// <summary>
		/// Adds the history row.
		/// </summary>
		/// <param name="pItem">P history row.</param>
		private void AddHistoryRow(XsollaHistoryItem pItem)
		{
			GameObject itemRow = Instantiate(Resources.Load(PREFAB_HISTORY_ROW)) as GameObject;
			HistoryElemController controller = itemRow.GetComponent<HistoryElemController>();
			if (controller != null)
				controller.Init(mUtils, pItem, (mList.Count + 1)%2 != 0, null);
			itemRow.transform.SetParent(mHistoryContainer.transform);
			Resizer.SetDefScale(itemRow);
			MoveProgresBarOnLastChild();
			mList.Add(controller);
		}

		/// <summary>
		/// Raises the scroll change event.
		/// </summary>
		/// <param name="pVector">P vector.</param>
		public void OnScrollChange(Vector2 pVector)
		{
			if ((pVector == new Vector2(1.0f, 0.0f)) || ((pVector == new Vector2(0.0f, 0.0f))))
			{
				if (!mProgressBar.isActiveAndEnabled)
					GetRequestList();
			}
		}

		/// <summary>
		/// Moves the progres bar on last child.
		/// </summary>
		private void MoveProgresBarOnLastChild()
		{
			mProgressBar.transform.SetAsLastSibling();
		}
			
		/// <summary>
		/// Destroy this instance.
		/// </summary>
		private void Destroy()
		{
			mList.Clear();
			Destroy(this.gameObject);
		}
	}
}

