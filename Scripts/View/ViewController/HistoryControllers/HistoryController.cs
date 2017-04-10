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

		private XsollaUtils mUtils;
		private const string mHistoryUrl = "paystation2/api/balance/history";
		private const string PREFAB_HISTORY_ROW  = "Prefabs/SimpleView/HistoryItem";
		private List<HistoryElemController> mList;
		private int mCountMore = 20;
		private bool sortDesc = true;


		private int mLimit = 0;
		private bool isRefresh = false;

		private String mVirtCurrName;

		public bool IsRefresh()
		{
			return isRefresh;
		}

		public void InitScreen(XsollaTranslations pTranslation, XsollaHistoryList pList, String pVirtualCurrName)
		{
			mVirtCurrName = pVirtualCurrName;
			Logger.Log("Init history screen");
			mTitle.text = pTranslation.Get("balance_history_page_title");
				
			mBtnContinue.GetComponent<Text>().text = pTranslation.Get("balance_back_button") + " >";	
			mBtnContinue.GetComponent<Button>().onClick.AddListener(delegate 
				{
					Destroy(this.gameObject);	
					GetComponentInParent<XsollaPaystationController>().NavMenuClick(RadioButton.RadioType.SCREEN_GOODS);
				});

			AddHistoryRow(pTranslation, null, false, true);

			foreach (XsollaHistoryItem item in pList.GetItemsList())
			{
				AddHistoryRow(pTranslation, item, mLimit%2 != 0, false);
				mLimit ++;
			}
				
			isRefresh = false;
		}

		public void Init(XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mTitle.text = mUtils.GetTranslations().Get("balance_history_page_title");
			mBtnContinue.GetComponent<Text>().text = mUtils.GetTranslations().Get("balance_back_button");

			// Делаем запрос на лист
			GetRequestList();
		}

		private void GetRequestList()
		{
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("offset", mList.Count);
			lParams.Add("limit", mCountMore);
			lParams.Add("sortDesc", sortDesc);
			lParams.Add("sortKey", "dateTimestamp");
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mHistoryUrl, lParams), HistoryListRecived, ErrorRecived);
		}

		private void HistoryListRecived(JSONNode pNode)
		{
			XsollaHistoryList lList = new XsollaHistoryList().Parse(pNode["operations"]) as XsollaHistoryList;
			//TODO реализовать заполнение 

		}

		private void ErrorRecived(XsollaErrorRe pError)
		{
			
		}






		public void SortHistory()
		{
			sortDesc = !sortDesc;
			OnRefreshHistory();
		}

		private void ClearList()
		{
			Logger.Log("Clear histroy List");
			mLimit = 0;
			Resizer.DestroyChilds(mHistoryContainer.transform);
			mHistoryContainer.GetComponent<RectTransform>().anchoredPosition = new Vector3(0,0);
			isRefresh = true;
		}

		public void OnRefreshHistory()
		{
			//TODO: Fix error when we click refresh and get list on 40 elements
			Logger.Log("Click refreshBtn");
			ClearList();
			LoadMore();
		}

		public void AddListRows(XsollaTranslations pTranslation, XsollaHistoryList pList)
		{
			foreach (XsollaHistoryItem item in pList.GetItemsList())
			{
				AddHistoryRow(pTranslation, item, mLimit%2 != 0, false);
				mLimit ++;
			}
		}

		public void AddHistoryRow(XsollaTranslations pTranslation, XsollaHistoryItem pItem, Boolean pEven, Boolean pHeader = false)
		{
			Logger.Log("AddHistoryRow");
			GameObject itemRow = Instantiate(Resources.Load(PREFAB_HISTORY_ROW)) as GameObject;
			HistoryElemController controller = itemRow.GetComponent<HistoryElemController>();
			if (controller != null)
			{
				if (pHeader)
				{
					controller.Init(pTranslation, null, mVirtCurrName, pEven, SortHistory, true, sortDesc);
				}
				else
				{
					controller.Init(pTranslation, pItem, mVirtCurrName, pEven, null);
				}
			}
			itemRow.transform.SetParent(mHistoryContainer.transform);
		}

		public void OnScrollChange(Vector2 pVector)
		{
			Logger.Log("Scroll vector" + pVector.ToString());
			if ((pVector == new Vector2(1.0f, 0.0f)) || ((pVector == new Vector2(0.0f, 0.0f))))
			{
				Logger.Log("End scroll");
				if (!isRefresh)
					LoadMore();
			}
		}

		private void LoadMore()
		{
			Logger.Log("Load more history. CurLimit:" + mLimit);
			Dictionary<string, object> lParams = new Dictionary<string, object>();
			// Load History
			lParams.Add("offset", mLimit);
			lParams.Add("limit", mCountMore);
			lParams.Add("sortDesc", sortDesc.ToString().ToLower());
			lParams.Add("sortKey", "dateTimestamp");
			GetComponentInParent<XsollaPaystation> ().LoadHistory(lParams);
		}
	}
}

