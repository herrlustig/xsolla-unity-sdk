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
		private const String mBtnPrefab = "Prefabs/Screens/SubsManager/Simple/SubManagerBtn";
		private const String mDetailPartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailPart";
		private const String mDetailPaymentPartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailPaymentPart";
		private const String mDetailChargePartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailChargesPart";
		private const String mDetailNotifyPartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailNotifyPart";

		private XsollaManagerSubDetails mLocalSubDetail;

		public SubsManagerController ()
		{
		}

		public void initScreen(XsollaUtils pUtils, XsollaManagerSubscriptions pSubsList)
		{
			mUtils = pUtils;
			mTitleScreen.text = mUtils.GetTranslations().Get("user_menu_user_subscription");
			mContinueText.text = mUtils.GetTranslations().Get("balance_back_button");
			mLabel.text = mUtils.GetTranslations().Get("user_subscription_list_subtitle");

			mContinueLink.onClick.AddListener(OnClickBackShopAction);

			foreach (XsollaManagerSubscription sub in pSubsList.GetItemsList())
			{
				addSubBtn(sub);
			}
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
			lParams.Add("subscription_id", pSubId);
			lParams.Add("userInitialCurrency", mUtils.GetUser().userBalance.currency);

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/subscription", lParams, callbackShowSubDetail);
		}

		private void getApiRequest(String pUrl, Dictionary<string, object> pParams, Action<JSONNode> pRecivedCallBack)
		{
			// send params
			WWWForm lForm = new WWWForm();
			StringBuilder sb = new StringBuilder ();
			foreach(KeyValuePair<string, object> pair in pParams)
			{
				string argValue = pair.Value != null ? pair.Value.ToString() : "";
				sb.Append(pair.Key).Append("=").Append(argValue).Append("&");
				lForm.AddField(pair.Key, argValue);
			}
			WWW lwww = new WWW(pUrl, lForm);
			StartCoroutine(getRequest(lwww, pRecivedCallBack));
		}
			
		private IEnumerator getRequest(WWW pWww, Action<JSONNode> pCallback)
		{
			yield return pWww;
			if (pWww.error == null)
			{
				JSONNode rootNode = JSON.Parse(pWww.text);
				pCallback(rootNode);
				//showSubDetail(rootNode);
			}
			else
			{
				//TODO show error 
			}
		}

		private bool isLinkPaymentMethod = true;

		private void callbackShowSubDetail(JSONNode pNode)
		{
			mLocalSubDetail = new XsollaManagerSubDetails().Parse(pNode) as XsollaManagerSubDetails;
			// убрать то что было на панели и построить новое?
			var children = new List<GameObject>();
			foreach (Transform child in mSubsContainer.transform) 
				children.Add(child.gameObject);
			children.ForEach(child => Destroy(child));

			// скрыть заголовок
			mLabel.gameObject.SetActive(false);
			mTitleScreen.text = mUtils.GetTranslations().Get("user_subscription_info_page_title");

			// если в типе метода идет notify то нужно выдать уведомление о том что метод оплаты не привязан и дать ссылку на линку метода
			if ((mLocalSubDetail.mStatus != "non_renewing") && (mLocalSubDetail.mPaymentMethodName == "null")) // TODO добавить условие с allow_recurrent
			{
				isLinkPaymentMethod = false;
				GameObject notifyObj = Instantiate(Resources.Load(mDetailNotifyPartPrefab)) as GameObject;
				SubManagerNotifyPartController notifyController = notifyObj.GetComponent<SubManagerNotifyPartController>() as SubManagerNotifyPartController;
				notifyController.init(mUtils.GetTranslations().Get("user_subscription_payment_not_link_account_message"), mUtils.GetTranslations().Get("user_subscription_add"), mLocalSubDetail, OnLinkPaymentMethodAction);            
				notifyController.transform.SetParent(mSubsContainer.transform);
			}
				
			// добавить часть детализации 
			GameObject detailPart = Instantiate(Resources.Load(mDetailPartPrefab)) as GameObject;
			SubManagerDetailPartController controller = detailPart.GetComponent<SubManagerDetailPartController>() as SubManagerDetailPartController;
			controller.initScreen(mLocalSubDetail, mUtils);
			detailPart.transform.SetParent(mSubsContainer.transform);

			// добавить префаб платежного метода
			if (mLocalSubDetail.mStatus != "non_renewing")
			{
				GameObject detailPaymentPart = Instantiate(Resources.Load(mDetailPaymentPartPrefab)) as GameObject;
				SubManagerDetailPaymentPartController paymentPartController = detailPaymentPart.GetComponent<SubManagerDetailPaymentPartController>() as SubManagerDetailPaymentPartController;
				if (isLinkPaymentMethod)
					paymentPartController.init(mLocalSubDetail, mUtils, OnUnlinkPaymentMethodAction);
				else
					paymentPartController.init(mLocalSubDetail, mUtils, OnLinkPaymentMethodAction);
				paymentPartController.transform.SetParent(mSubsContainer.transform);
			}

			// добавить префаб истории платежей
			if (mLocalSubDetail.mCharges != null)
			{
				GameObject detailCharges = Instantiate(Resources.Load(mDetailChargePartPrefab)) as GameObject;
				SubManagerDetailChargesPartController chargesController = detailCharges.GetComponent<SubManagerDetailChargesPartController>() as SubManagerDetailChargesPartController;
				chargesController.init(mLocalSubDetail, mUtils);
				chargesController.transform.SetParent(mSubsContainer.transform);
			}

			// TODO добавить ссылку на обратно 
			//OnClickBackShopAction();


		}

		private void callbackUnlinkMethod(JSONNode pNode)
		{
			if (pNode["status"].Value == "saved")
			{
				// перестроить детализацию и показать статус что отвязали
				GetDetailSub(mLocalSubDetail.mId);
			}
		}

		private void OnLinkPaymentMethodAction(XsollaManagerSubDetails pSubDetail)
		{
			Logger.Log("Link payment method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("change_account", "1");
			reqParams.Add("id_recurrent_subscription", pSubDetail.mId);
			reqParams.Add("id_payment_account", "");
			reqParams.Add("subscription_payment_type", "charge");

			XsollaPaystationController payController = GetComponentInParent<XsollaPaystationController> ();
			payController.ChooseItem(reqParams);
		}

		private void OnUnlinkPaymentMethodAction(XsollaManagerSubDetails pSubDetail)
		{
			Logger.Log("Unlink payment method");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("subscription_id", pSubDetail.mId);
			lParams.Add("userInitialCurrency", mUtils.GetUser().userBalance.currency);

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/unlinkpaymentaccount", lParams, callbackUnlinkMethod);

			//https://secure.xsolla.com/paystation2/api/useraccount/unlinkpaymentaccount
			//access_token:7g46L7ZZQoQhmhobCvH9q3Dc0w59eYN8
			//subscription_id:9676670
			//userInitialCurrency:USD
		}

		private void OnClickBackShopAction()
		{
			Logger.Log("On click back shop");
			// TODO сделать возврат
		}
	}
}

