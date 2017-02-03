using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;

namespace Xsolla
{
	public class SubsManagerController: MonoBehaviour
	{
		public Text mTitleScreen;
		public Text mContinueText;
		public Button mContinueLink;
		public Text mLabel;
		public GameObject mSubsContainer;

		private XsollaUtils mUtils;
		private const String DOMAIN = "https://secure.xsolla.com";
		private const String mBtnPrefab = "Prefabs/SimpleView/_PaymentFormElements/SubManagerBtn";

		public SubsManagerController ()
		{
		}

		public void initScreen(XsollaUtils pUtils, XsollaManagerSubscriptions pSubsList)
		{
			mUtils = pUtils;
			mTitleScreen.text = mUtils.GetTranslations().Get("user_menu_user_subscription");
			mContinueText.text = mUtils.GetTranslations().Get("balance_back_button");
			mLabel.text = mUtils.GetTranslations().Get("user_subscription_list_subtitle");

			mContinueLink.onClick.AddListener(OnClickBackShop);

			foreach (XsollaManagerSubscription sub in pSubsList.GetItemsList())
			{
				addSubBtn(sub);
			}
		}

		private void OnClickBackShop()
		{
			Logger.Log("On click back shop");
		}

		private void addSubBtn(XsollaManagerSubscription pSub)
		{
			GameObject objBtn = Instantiate(Resources.Load(mBtnPrefab)) as GameObject;
			SubManagerBtnController controller = objBtn.GetComponent<SubManagerBtnController>();
			controller.init(pSub, mUtils.GetTranslations());
			controller.SetDetailAction(onDetailBtnClick);
			objBtn.transform.SetParent(mSubsContainer.transform);
		}

		private void onDetailBtnClick(XsollaManagerSubscription pSub)
		{
			Logger.Log("On Detail click. Id: " + pSub.GetKey());
			GetDetailSub(pSub.GetId());
		}

		private void GetDetailSub(int pSubId)
		{
			Dictionary<string, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("subscription_id",pSubId);
			lParams.Add("userInitialCurrency",pSubId);

			// send params
			String lUrl = DOMAIN + "/paystation2/api/useraccount/subscription";
			WWWForm lForm = new WWWForm();
			StringBuilder sb = new StringBuilder ();
			foreach(KeyValuePair<string, object> pair in lParams)
			{
				string argValue = pair.Value != null ? pair.Value.ToString() : "";
				sb.Append(pair.Key).Append("=").Append(argValue).Append("&");
				lForm.AddField(pair.Key, argValue);
			}

			WWW lwww = new WWW(lUrl, lForm);
			StartCoroutine(getSubscriptionDetail(lwww));
		}


		private IEnumerator getSubscriptionDetail(WWW pWww)
		{
			yield return pWww;
			if (pWww.error == null)
			{
				JSONNode rootNode = JSON.Parse(pWww.text);
				XsollaManagerSubDetails subDetail = new XsollaManagerSubDetails().Parse(rootNode) as XsollaManagerSubDetails;
				showSubDetail(subDetail);

			}
			else
			{
				//TODO show error 
			}
		}

		private void showSubDetail(XsollaManagerSubDetails pSubDetail)
		{
			// убрать то что было на панели и построить новое?
			// скрыть то что было на панели и построить новое?
			// после возвращения обратно, перестраивать полностью подписки?


			// TODO кнопки Unhold | Hold or Cancel | Change Plan


			// TODO кнопки Delete 



		}

	}
}

