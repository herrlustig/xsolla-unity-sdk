using System;
using UnityEngine;
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
		public GameObject mStatusPanel;
		public Text mStatusLabel;
		public GameObject mErrorPanel;
		public Text mErrorLabel;

		private XsollaUtils mUtils;
		private const String DOMAIN = "https://secure.xsolla.com";
		private const String mBtnPrefab = "Prefabs/Screens/SubsManager/Simple/SubManagerBtn";
		private const String mDetailPartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailPart";
		private const String mDetailPaymentPartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailPaymentPart";
		private const String mDetailChargePartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailChargesPart";
		private const String mDetailNotifyPartPrefab = "Prefabs/Screens/SubsManager/Detail/SubDetailNotifyPart";
		private const String mDetailBackLinkPartPrefab = "Prefabs/Screens/SubsManager/Detail/BackLinkPart";
		private const String mDetailCancelLinkPartPrefab = "Prefabs/Screens/SubsManager/Detail/SubCancelChoose";

		private XsollaManagerSubDetails mLocalSubDetail;
		private SubManagerCancelPartController cancelSubsCtrl;

		public void initScreen(XsollaUtils pUtils, XsollaManagerSubscriptions pSubsList)
		{
			mUtils = pUtils;
			mTitleScreen.text = mUtils.GetTranslations().Get("user_menu_user_subscription");
			mContinueText.text = mUtils.GetTranslations().Get("balance_back_button");
			mLabel.gameObject.SetActive(true);
			mLabel.text = mUtils.GetTranslations().Get("user_subscription_list_subtitle");

			var children = new List<GameObject>();
			foreach (Transform child in mSubsContainer.transform) 
				children.Add(child.gameObject);
			children.ForEach(child => Destroy(child));

			setNotifyPanels();

			// событие на кнопку возврата 
			mContinueLink.onClick.RemoveAllListeners();
			mContinueLink.onClick.AddListener(OnClickBackShopAction);

			if (pSubsList.GetCount() == 0)
			{
				Logger.Log("Empty List subs");
				mLabel.text = mUtils.GetTranslations().Get("subscription_no_data");
				mLabel.alignment = TextAnchor.MiddleCenter;
			}

			foreach (XsollaManagerSubscription sub in pSubsList.GetItemsList())
			{
				addSubBtn(sub);
			}
		}

		private void setNotifyPanels()
		{
			// событие на кнопку статуса 
			mStatusPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			mStatusPanel.GetComponentInChildren<Button>().onClick.AddListener(() => { mStatusPanel.SetActive(false);});

			// событие на кнопку статуса 
			mErrorPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			mErrorPanel.GetComponentInChildren<Button>().onClick.AddListener(() => { mErrorPanel.SetActive(false);});
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
			if (mUtils.GetUser().userBalance != null)
				lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/subscription", lParams, callbackShowSubDetail);
		}

		private void getApiRequest(String pUrl, Dictionary<string, object> pParams, Action<JSONNode> pRecivedCallBack)
		{
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
			}
			else
			{
				JSONNode rootNode = JSON.Parse(pWww.error);
				showError(String.Format(StringHelper.PrepareFormatString(mUtils.GetTranslations().Get("error_code")), rootNode["error"].Value));
			}
		}

		private bool isLinkPaymentMethod = true;

		private void showStatus(String pLabel)
		{
			mStatusPanel.SetActive(true);
			mStatusLabel.text = pLabel;
		}

		private void showError(String pError)
		{
			mErrorPanel.SetActive(true);
			mErrorLabel.text = pError;
		}

		private void callbackShowSubDetail(JSONNode pNode)
		{
			mLocalSubDetail = new XsollaManagerSubDetails().Parse(pNode) as XsollaManagerSubDetails;
			// зачищаем то что было раньше
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
			controller.getHoldCancelBtn().onClick.AddListener(OnHoldCancelLinkAction);
			controller.getUnholdBtn().onClick.AddListener(OnUnHoldLinkAction);
			controller.getRenewBtn().onClick.AddListener(OnRenewBtnAction);

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

			addBackLinkPart(mUtils.GetTranslations().Get("user_subscription_back_to_subscription_list"), OnClickBackSubsListAction);
		}

		private void ShowLocalSubDetail()
		{
			GetDetailSub(mLocalSubDetail.mId);
		}

		private void addBackLinkPart(String pLabelLink, Action pBackAction, String pConfirmBtnLabel = "", Action pConfirmBtnAction = null)
		{
			GameObject linkBackPart = Instantiate(Resources.Load(mDetailBackLinkPartPrefab)) as GameObject;
			SubBackLinkPart controller = linkBackPart.GetComponent<SubBackLinkPart>();
			controller.init(pLabelLink, pBackAction, pConfirmBtnLabel, pConfirmBtnAction);
			linkBackPart.transform.SetParent(mSubsContainer.transform);
		}

		private void callbackUnlinkMethod(JSONNode pNode)
		{
			if (pNode["status"].Value == "saved")
			{
				// перестроить детализацию и показать статус что отвязали
				ShowLocalSubDetail();
				showStatus(mUtils.GetTranslations().Get("user_subscription_message_unlink_payment_account"));
			}
		}

		private void callbackUnholdMethod(JSONNode pNode)
		{
			if (pNode["status"].Value == "saved")
			{
				// перестроить детализацию и показать что подписка 
				ShowLocalSubDetail();
				showStatus(String.Format(mUtils.GetTranslations().Get("user_subscription_message_unhold_no_active")));
			}
		}

		private void callbackDontrenewMethod(JSONNode pNode)
		{
			if (pNode["status"].Value == "saved")
			{
				// перестроить детализацию и показать что подписка не будет продлеваться
				ShowLocalSubDetail();
				// показать статус
				showStatus(String.Format(StringHelper.PrepareFormatString(mUtils.GetTranslations().Get("user_subscription_message_non_renewing")), StringHelper.DateFormat(mLocalSubDetail.mDateNextCharge)));
			}
		}

		private void callbackDeleteSubMethod(JSONNode pNode)
		{
			if (pNode["status"].Value == "saved")
			{
				// перестроить детализацию и показать статус подписка отменена
				OnClickBackSubsListAction();
				showStatus(String.Format(StringHelper.PrepareFormatString(mUtils.GetTranslations().Get("user_subscription_message_canceled")), mLocalSubDetail.mName, mUtils.GetProject().name));
			}
		}

		private void callbackGetSubsList(JSONNode pNode)
		{
			XsollaManagerSubscriptions lSubsList = new XsollaManagerSubscriptions().Parse(pNode["subscriptions"]) as XsollaManagerSubscriptions;
			initScreen(mUtils, lSubsList);
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
			if (mUtils.GetUser().userBalance != null)
				lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/unlinkpaymentaccount", lParams, callbackUnlinkMethod);
		}

		private void OnRenewBtnAction()
		{
			Logger.Log("Link payment method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("change_account", "0");
			reqParams.Add("id_recurrent_subscription", mLocalSubDetail.mId);
			reqParams.Add("id_payment_account", "");
			reqParams.Add("type_payment_account", "");

			XsollaPaystationController payController = GetComponentInParent<XsollaPaystationController> ();
			payController.ChooseItem(reqParams);
		}

		private void OnClickBackShopAction()
		{
			Logger.Log("On click back shop");
			// уничтожаем объект чтобы отобразить то что под ним
			GameObject.FindObjectOfType<XsollaPaystation>().LoadShop();
			Destroy(this.gameObject);
		}

		private void OnClickBackSubsListAction()
		{
			Logger.Log("On click back to subs");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			if (mUtils.GetUser().userBalance != null)
				lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/subscriptions", lParams, callbackGetSubsList);
		}
			
		private void OnHoldCancelLinkAction()
		{
			// чистим то что было на экране
			var children = new List<GameObject>();
			foreach (Transform child in mSubsContainer.transform) 
				children.Add(child.gameObject);
			children.ForEach(child => Destroy(child));

			GameObject obj = Instantiate(Resources.Load(mDetailCancelLinkPartPrefab)) as GameObject;
			cancelSubsCtrl = obj.GetComponent<SubManagerCancelPartController>();
			cancelSubsCtrl.init(mLocalSubDetail, mUtils);
			cancelSubsCtrl.transform.SetParent(mSubsContainer.transform);

			addBackLinkPart(mUtils.GetTranslations().Get("back_to_user_subscription_info"), ShowLocalSubDetail, mUtils.GetTranslations().Get("hold_subscription_confirm"), OnConfirmCancelSub);
		}

		private void OnConfirmCancelSub()
		{
			Logger.Log("On confirm cancel click");
			if (cancelSubsCtrl != null)
			{
				if (cancelSubsCtrl.isDeleteNow())
					OnDeleteSubAction();
				else
					OnDontRenewAction();
			}
		}

		private void OnUnHoldLinkAction()
		{
			Logger.Log("Unhold click");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("subscription_id", mLocalSubDetail.mId);
			if (mUtils.GetUser().userBalance != null)
				lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/unholdsubscription", lParams, callbackUnholdMethod);
		}
	
		private void OnDontRenewAction()
		{
			Logger.Log("Don't renew");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("subscription_id", mLocalSubDetail.mId);
			if (mUtils.GetUser().userBalance != null)
				lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			lParams.Add("status", "non_renewing");

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/holdsubscription", lParams, callbackDontrenewMethod);

		}

		private void OnDeleteSubAction()
		{
			Logger.Log("Delet now");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("subscription_id", mLocalSubDetail.mId);
			if (mUtils.GetUser().userBalance != null)
				lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			lParams.Add("status", "canceled");

			getApiRequest(DOMAIN + "/paystation2/api/useraccount/holdsubscription", lParams, callbackDeleteSubMethod);
		}
	}
}

