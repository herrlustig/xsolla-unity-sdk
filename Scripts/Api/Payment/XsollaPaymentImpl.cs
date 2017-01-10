using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
using System.Text;

namespace Xsolla 
{
	public class XsollaPaymentImpl : MonoBehaviour, IXsollaPayment {

		private string DOMAIN = "https://secure.xsolla.com";

		private const string SDK_VERSION = "1.3.4";

		private const int TRANSLATIONS 		 	= 0;
		private const int DIRECTPAYMENT_FORM 	= 1;
		private const int DIRECTPAYMENT_STATUS 	= 2;
		private const int PRICEPOINTS 			= 3;
		private const int GOODS 				= 5;
		private const int GOODS_GROUPS 			= 51;
		private const int GOODS_ITEMS 			= 52;
		private const int PAYMENT_LIST 			= 6;
		private const int SAVED_PAYMENT_LIST	= 61;
		private const int QUICK_PAYMENT_LIST 	= 7;
		private const int COUNTRIES 			= 8;
		private const int VIRTUAL_PAYMENT_SUMMARY 	= 9;
		private const int VIRTUAL_PROCEED 			= 10;
		private const int VIRTUAL_STATUS 			= 11;

		private const int APPLY_PROMO_COUPONE		= 12;
		private const int COUPON_PROCEED			= 13;
		private const int HISTORY					= 22;
		private const int PAYMENT_MANAGER_LIST		= 15;
		private const int CALCULATE_CUSTOM_AMOUNT   = 14;
		private const int ACTIVE_SUBS				= 16;
		private const int DELETE_SAVED_METHOD 		= 17;


		public Action<XsollaUtils> 					UtilsRecieved;
		public Action<XsollaTranslations> 			TranslationRecieved;
		public Action<XsollaHistoryList>			HistoryRecieved;

		public Action<XsollaPricepointsManager> 	PricepointsRecieved;
		public Action<XsollaGroupsManager> 			GoodsGroupsRecieved;
		public Action<XsollaGoodsManager> 			GoodsRecieved;
		public Action<XsollaSubscriptions>			SubsReceived;

		public Action<XsollaPaymentMethods> 		PaymentMethodsRecieved;
		public Action<XsollaSavedPaymentMethods>    SavedPaymentMethodsRecieved;
		public Action<XsollaQuickPayments> 			QuickPaymentMethodsRecieved;
		public Action<XsollaQuickPayments>			QuickPaymentMethodsRecievedNew;
		public Action<XsollaCountries> 				CountriesRecieved;

		public Action<XsollaForm> 					FormReceived;
		public Action<XsollaStatus, XsollaForm> 	StatusReceived;
		public Action<XsollaForm>					ApplyCouponeCodeReceived;
		public Action<XsollaStatusPing> 			StatusChecked;
		public Action<XsollaError> 					ErrorReceived;
		public Action<XsollaCouponProceedResult>	CouponProceedErrorRecived;
		public Action<CustomVirtCurrAmountController.CustomAmountCalcRes> CustomAmountCalcRecieved;

		public Action<XVirtualPaymentSummary> 		VirtualPaymentSummaryRecieved;
		public Action<string> 						VirtualPaymentProceedError;
		public Action<XVPStatus> 					VirtualPaymentStatusRecieved;

		public Action<XsollaSavedPaymentMethods>	PaymentManagerMethods;
		public Action								DeleteSavedPaymentMethodRespond;

		//TODO CHANGE PARAMS
		protected string _accessToken;
		protected Dictionary<string, object> baseParams;
		protected HttpTlsRequest httpreq;

		public XsollaPaymentImpl(){
		}

		public XsollaPaymentImpl(string accessToken){
			this._accessToken = accessToken;
		}

		public void InitPaystation(XsollaWallet xsollaWallet)
		{
			this._accessToken = xsollaWallet.GetToken ();
			GetUtils (null);
//			GetNextStep (new Dictionary<string, object> ());
		}

		public void InitPaystation(string accessToken)
		{
			this._accessToken = accessToken;
			GetUtils (null);
		}

		public void InitPaystation(Dictionary<string, object> pararams)
		{

//			this._accessToken = pararams[XsollaApiConst.ACCESS_TOKEN].ToString();
			baseParams = new Dictionary<string, object> (pararams);
			GetUtils (pararams);
		}

		public void StartPaymentWithoutUtils(XsollaWallet xsollaWallet)
		{
			this._accessToken = xsollaWallet.GetToken ();
			GetNextStep (new Dictionary<string, object> ());
		}

		public void NextStep(Dictionary<string, object> xpsMap)
		{
			GetNextStep (xpsMap);
		}

		public void Status(string token, long invoice)
		{

		}

		public void SetModeSandbox(bool isSandbox)
		{
			if (!isSandbox) { 
				DOMAIN = "https://secure.xsolla.com";
			} else {
				DOMAIN = "https://sandbox-secure.xsolla.com";
			}
		}

		public void SetDomain(string domain)
		{
			DOMAIN = domain;
		}

		public void SetToken (string token)
		{
			this._accessToken = token;
		}

		
		// ---------------------------------------------------------------------------

		private void OnUtilsRecieved(XsollaUtils utils)
		{
			if(UtilsRecieved != null)
				UtilsRecieved(utils);
		}

		private void OnPricepointsRecieved(XsollaPricepointsManager pricepoints)
		{
			if(PricepointsRecieved != null)
				PricepointsRecieved(pricepoints);
		}

		private void OnGoodsRecieved(XsollaGoodsManager goods)
		{
			if(GoodsRecieved != null)
				GoodsRecieved(goods);
		}

		private void OnGoodsGroupsRecieved(XsollaGroupsManager groups)
		{
			if(GoodsGroupsRecieved != null)
				GoodsGroupsRecieved(groups);
		}

		private void OnSubscriptionsReceived(XsollaSubscriptions pSubs)
		{
			if(SubsReceived != null)
				SubsReceived(pSubs);
		}

		// ---------------------------------------------------------------------------

		private void OnVPSummaryRecieved(XVirtualPaymentSummary summary)
		{
			if(VirtualPaymentSummaryRecieved != null)
				VirtualPaymentSummaryRecieved(summary);
		}

		private void OnVPProceedError(string error)
		{
			if(VirtualPaymentProceedError != null)
				VirtualPaymentProceedError(error);
		}

		private void OnVPStatusRecieved(XVPStatus status)
		{
			if (VirtualPaymentStatusRecieved != null)
				VirtualPaymentStatusRecieved (status);
		}

		private void OnPaymentMethodsRecieved(XsollaPaymentMethods paymentMethods)
		{
			if(PaymentMethodsRecieved != null)
				PaymentMethodsRecieved(paymentMethods);
		}

		private void OnSavedPaymentMethodsRecieved(XsollaSavedPaymentMethods pMethods)
		{
			if (SavedPaymentMethodsRecieved != null)
				SavedPaymentMethodsRecieved(pMethods);
		}

		private void OnQuickPaymentMethodsRecieved(XsollaQuickPayments quickPayments)
		{
			if(QuickPaymentMethodsRecieved != null)
				QuickPaymentMethodsRecieved(quickPayments);
		}

		private void OnQuickPaymentMethodsRecievedNew(XsollaQuickPayments quickPayments)
		{
			if(QuickPaymentMethodsRecievedNew != null)
				QuickPaymentMethodsRecievedNew(quickPayments);
		}

		private void OnCountriesRecieved(XsollaCountries countries)
		{
			if(CountriesRecieved != null)
				CountriesRecieved(countries);
		}

		protected virtual void OnTranslationRecieved(XsollaTranslations translations) 
		{
			if (TranslationRecieved != null)
				TranslationRecieved(translations);
		}

		protected virtual void OnHistoryRecieved(XsollaHistoryList pHistoryList)
		{
			if (HistoryRecieved != null)
				HistoryRecieved(pHistoryList);
		}
		
		// ---------------------------------------------------------------------------

		protected virtual void OnFormReceived(XsollaForm form) 
		{
			if (FormReceived != null)
				FormReceived(form);
		}

		protected virtual void OnStatusReceived(XsollaStatus status, XsollaForm form) 
		{
			if (StatusReceived != null)
				StatusReceived(status, form);
		}

		protected virtual void OnApplyCouponeReceived(XsollaForm pForm)
		{
			if (ApplyCouponeCodeReceived != null)
				ApplyCouponeCodeReceived(pForm);
		}

		protected virtual void OnStatusChecked(XsollaStatusPing pStatus)
		{
			if (StatusChecked != null)
				StatusChecked(pStatus);
		}

		protected virtual void OnErrorReceived(XsollaError error) 
		{
			if (ErrorReceived != null)
				ErrorReceived(error);
		}

		protected virtual void OnCouponProceedErrorRecived(XsollaCouponProceedResult pCouponObj)
		{
			if (CouponProceedErrorRecived != null)
				CouponProceedErrorRecived(pCouponObj);
		}

		protected virtual void OnCustomAmountResRecieved(CustomVirtCurrAmountController.CustomAmountCalcRes pRes)
		{
			if (CustomAmountCalcRecieved != null)
				CustomAmountCalcRecieved(pRes);
		}

		protected virtual void OnPaymentManagerMethod(XsollaSavedPaymentMethods pRes)
		{
			if (PaymentManagerMethods != null)
				PaymentManagerMethods(pRes);
		}

		protected virtual void OnDeleteSavedPaymentMethod()
		{
			if (DeleteSavedPaymentMethodRespond != null)
				DeleteSavedPaymentMethodRespond();
		}
		
		// ---------------------------------------------------------------------------

		public void GetVPSummary(Dictionary<string, object> pararams) {
			if (!pararams.ContainsKey ("is_virtual_payment"))
				pararams.Add ("is_virtual_payment", 1);
			else
				pararams ["is_virtual_payment"] = 1;
			StartCoroutine(POST (VIRTUAL_PAYMENT_SUMMARY, GetCartSummary(), pararams));
		}

		public void ProceedVPayment(Dictionary<string, object> pararams) {
			StartCoroutine(POST (VIRTUAL_PROCEED, ProceedVirtualPaymentLink (), pararams));
		}

		public void VPaymentStatus(Dictionary<string, object> pararams) {
			StartCoroutine(POST (VIRTUAL_STATUS, GetVirtualPaymentStatusLink (), pararams));
		}

		void GetUtils(Dictionary<string, object> pararams)
		{
			//Dictionary<string, object> pararams = new Dictionary<string, object> ();
			StartCoroutine(POST (TRANSLATIONS, GetUtilsLink(), pararams));
		}

		void GetNextStep(Dictionary<string, object> nextStepParams)
		{
			if (nextStepParams.Count == 0) {
				nextStepParams.Add ("pid", 26);
			}
			if (!nextStepParams.ContainsKey ("paymentWithSavedMethod")) {
				nextStepParams.Add ("paymentWithSavedMethod", 0);
//				nextStepParams.Add ("returnUrl", "");
			}
			StartCoroutine(POST (DIRECTPAYMENT_FORM, GetDirectpaymentLink(), nextStepParams));
		}

		public void GetStatus(Dictionary<string, object> statusParams)
		{
			StartCoroutine(POST (DIRECTPAYMENT_STATUS, GetStatusLink(), statusParams));
		}

		public void GetPricePoints(Dictionary<string, object> requestParams)
		{
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			StartCoroutine(POST (PRICEPOINTS, GetPricepointsUrl(), requestParams));
		}

		public void GetGoods(Dictionary<string, object> requestParams)
		{
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			StartCoroutine(POST (GOODS, GetGoodsUrl(), requestParams));
		}
		public void GetItemsGroups(Dictionary<string, object> requestParams)
		{
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			StartCoroutine(POST (GOODS_GROUPS, GetItemsGroupsUrl(), requestParams));
		}

		public void GetItems(long groupId, Dictionary<string, object> requestParams)
		{
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			requestParams.Add ("group_id", groupId );//group_id <- NEW | OLD -> requestParams.Add ("id_group",groupId );
			StartCoroutine(POST (GOODS_ITEMS, GetItemsUrl(), requestParams));
		}

		public void GetSubscriptions()
		{
			Dictionary<string,object> param = new Dictionary<string, object>();
			param.Add(XsollaApiConst.ACCESS_TOKEN, baseParams[XsollaApiConst.ACCESS_TOKEN]);
			StartCoroutine(POST(ACTIVE_SUBS, GetSubsUrl(), baseParams));
		}

		public void GetFavorites(Dictionary<string, object> requestParams)
		{
			StartCoroutine(POST (GOODS_ITEMS, GetFavoritsUrl(), requestParams));
		}

		public void GetCouponProceed(Dictionary<string, object> pParams)
		{
			if (!pParams.ContainsKey(XsollaApiConst.ACCESS_TOKEN) && (baseParams.ContainsKey(XsollaApiConst.ACCESS_TOKEN)))
				pParams.Add(XsollaApiConst.ACCESS_TOKEN, baseParams[XsollaApiConst.ACCESS_TOKEN]);

			StartCoroutine(POST(COUPON_PROCEED, GetCouponProceed(), pParams));
		}	

		public void SetFavorite(Dictionary<string, object> requestParams)
		{
			StartCoroutine(POST (999, SetFavoritsUrl(), requestParams));
		}

		public void GetPaymentsInfo(Dictionary<string, object> requestParams)
		{
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			StartCoroutine(POST (QUICK_PAYMENT_LIST, GetQuickPaymentsUrl(), requestParams));
			StartCoroutine(POST (PAYMENT_LIST, GetPaymentListUrl(), requestParams));
			StartCoroutine(POST (COUNTRIES, GetCountriesListUrl(), requestParams));
		}

		public void GetQuickPayments(string countryIso, Dictionary<string, object> requestParams)
		{
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			if (countryIso != null && !"".Equals (countryIso)) {
				requestParams["country"] = countryIso;
			}
			StartCoroutine(POST (QUICK_PAYMENT_LIST, GetQuickPaymentsUrl(), requestParams));
		}

		public void GetPayments(string countryIso, Dictionary<string, object> requestParams)
		{
//			GetVirtualPaymentSummary (requestParams);
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			if (countryIso != null && !"".Equals (countryIso)) {
				requestParams["country"] = countryIso;
			}
			StartCoroutine(POST (PAYMENT_LIST, GetPaymentListUrl(), requestParams));
		}

		public void GetSavedPayments(Dictionary<string, object> requestParams)
		{
			StartCoroutine(POST(SAVED_PAYMENT_LIST, GetSavedPaymentListUrl(), requestParams));
		}

		public void GetSavedPaymentsForManager(Dictionary<string, object> pParams)
		{
			StartCoroutine(POST(PAYMENT_MANAGER_LIST, GetSavedPaymentListUrl(), pParams));
		}

		public void GetCountries(Dictionary<string, object> requestParams)
		{
//			Dictionary<string, object> requestParams = new Dictionary<string, object>();
			StartCoroutine(POST (COUNTRIES, GetCountriesListUrl(), requestParams));
		}
			
		public void GetHistory(Dictionary<string, object> pParams)
		{
			if (!pParams.ContainsKey(XsollaApiConst.ACCESS_TOKEN))
				pParams.Add(XsollaApiConst.ACCESS_TOKEN, baseParams[XsollaApiConst.ACCESS_TOKEN]);

			StartCoroutine(POST(HISTORY, GetHistoryUrl(), pParams));
		}
		public void ApplyPromoCoupone(Dictionary<string, object> pParams)
		{
			StartCoroutine(POST(APPLY_PROMO_COUPONE, GetDirectpaymentLink(), pParams));
		}

		public void CalculateCustomAmount(Dictionary<string, object> pParams)
		{
			if (!pParams.ContainsKey(XsollaApiConst.ACCESS_TOKEN))
				pParams.Add(XsollaApiConst.ACCESS_TOKEN, baseParams[XsollaApiConst.ACCESS_TOKEN]);
			StartCoroutine(POST(CALCULATE_CUSTOM_AMOUNT, GetCalculateCustomAmountUrl(), pParams));
		}
			
		public void DeleteSavedMethod(Dictionary<string, object> pParam)
		{
			if (!pParam.ContainsKey(XsollaApiConst.ACCESS_TOKEN))
				pParam.Add(XsollaApiConst.ACCESS_TOKEN, baseParams[XsollaApiConst.ACCESS_TOKEN]);
			StartCoroutine(POST(DELETE_SAVED_METHOD, GetDeleteSavedPaymentMethodUrl(), pParam));
		}
			
		public IEnumerator POST(int type, string url, Dictionary<string, object> post)
		{
			WWWForm form = new WWWForm();
			StringBuilder sb = new StringBuilder ();
			if (!post.ContainsKey (XsollaApiConst.ACCESS_TOKEN) && !post.ContainsKey ("project") && !post.ContainsKey ("access_data") && baseParams != null)
			{
				foreach (KeyValuePair<string, object> kv in baseParams)
					post.Add (kv.Key, kv.Value);
				if (!post.ContainsKey(XsollaApiConst.ACCESS_TOKEN) && !baseParams.ContainsKey(XsollaApiConst.ACCESS_TOKEN) && (_accessToken != ""))
					post.Add(XsollaApiConst.ACCESS_TOKEN, _accessToken);

			}
			if (type == DIRECTPAYMENT_STATUS)
				TransactionHelper.SaveRequest (post);
			if(!post.ContainsKey("alternative_platform"))
				post.Add ("alternative_platform", "unity/" + SDK_VERSION);

			foreach(KeyValuePair<string,object> post_arg in post)
			{
				string argValue = post_arg.Value != null ? post_arg.Value.ToString() : "";
				sb.Append(post_arg.Key).Append("=").Append(argValue).Append("&");
				form.AddField(post_arg.Key, argValue);
			}
				
			Debug.Log (url);
			Debug.Log (sb.ToString());
			WWW www = new WWW(url, form);

			yield return StartCoroutine(WaitForRequest(type, www, post));	
		}
			
		private IEnumerator WaitForRequest(int pType, WWW www, Dictionary<string, object> post)
		{
			Logger.Log("Start get www");
			yield return www;
			// check for errors
			if (www.error == null)
			{
				Debug.Log("Type -> " + pType);
				Debug.Log("WWW_request -> " + www.text);


				string data = www.text;


				JSONNode rootNode = JSON.Parse(www.text);
				if(rootNode != null && rootNode.Count > 2 || rootNode["error"] == null) {
					switch(pType)
					{
					case TRANSLATIONS: 
						{
							if(rootNode.Count > 2){
								XsollaUtils utils = new XsollaUtils().Parse(rootNode) as XsollaUtils;
								projectId = utils.GetProject().id.ToString();

								OnUtilsRecieved(utils);
								// if base param not containKey access token, then add token from util
								if (!baseParams.ContainsKey(XsollaApiConst.ACCESS_TOKEN))
									_accessToken = utils.GetAcceessToken();
								OnTranslationRecieved(utils.GetTranslations());
							} else {
								XsollaError error = new XsollaError();
								error.Parse(rootNode);
								OnErrorReceived(error);
							}
						}
						break;
						case DIRECTPAYMENT_FORM:
						{
							if(rootNode.Count > 8) {
								XsollaForm form = new XsollaForm();
								form.Parse(rootNode);
								switch (form.GetCurrentCommand()) {
								case XsollaForm.CurrentCommand.STATUS:
									// if we replaced or add saved account, we must start loop on get list saved account
									if (post.ContainsKey("save_payment_account_only") || (post.ContainsKey("replace_payment_account")))
									{
										break;
									}
									GetStatus(form.GetXpsMap());
									break;
								case XsollaForm.CurrentCommand.CHECKOUT:
								case XsollaForm.CurrentCommand.CHECK:
								case XsollaForm.CurrentCommand.FORM:
								case XsollaForm.CurrentCommand.CREATE:
								case XsollaForm.CurrentCommand.ACCOUNT:
									OnFormReceived(form);
									break;
								case XsollaForm.CurrentCommand.UNKNOWN:
									if(rootNode.Count > 10)
									{
										OnFormReceived(form);
									} else {
										XsollaError error = new XsollaError();
										error.Parse(rootNode);
										OnErrorReceived(error);
									}
									break;
								default:
									break;
								}
							} else {
								XsollaStatusPing statusPing = new XsollaStatusPing();
								statusPing.Parse(rootNode);
								OnStatusChecked(statusPing);
							}
						}
						break;
						case DIRECTPAYMENT_STATUS:
						{
							XsollaForm form = new XsollaForm();
							form.Parse(rootNode);
							XsollaStatus status = new XsollaStatus();
							status.Parse(rootNode);
							OnStatusReceived(status, form);
						}
						break;
						case PRICEPOINTS:
						{
							XsollaPricepointsManager pricepoints = new XsollaPricepointsManager();
							pricepoints.Parse(rootNode);
							OnPricepointsRecieved(pricepoints);
						}
						break;
						case GOODS:
						{
							XsollaGoodsManager goods = new XsollaGoodsManager();
							goods.Parse(rootNode);
							OnGoodsRecieved(goods);
						}
						break;
						case GOODS_GROUPS:
						{
							XsollaGroupsManager groups = new XsollaGroupsManager();
							groups.Parse(rootNode);
							OnGoodsGroupsRecieved(groups);
						}
						break;
						case GOODS_ITEMS:
						{
							XsollaGoodsManager goods = new XsollaGoodsManager();
							goods.Parse(rootNode);
							OnGoodsRecieved(goods);
						}
						break;
						case PAYMENT_LIST:
						{
							XsollaPaymentMethods paymentMethods = new XsollaPaymentMethods();
							paymentMethods.Parse(rootNode);
							OnPaymentMethodsRecieved(paymentMethods);
						}
						break;
						case SAVED_PAYMENT_LIST:
						{
							XsollaSavedPaymentMethods savedPaymentsMethods = new XsollaSavedPaymentMethods();
							savedPaymentsMethods.Parse(rootNode);
							OnSavedPaymentMethodsRecieved(savedPaymentsMethods);
						}
						break;
						case QUICK_PAYMENT_LIST:
						{
							XsollaQuickPayments quickPayments = new XsollaQuickPayments();
							quickPayments.Parse(rootNode);
							OnQuickPaymentMethodsRecieved(quickPayments);
						}
						break;
						case COUNTRIES:
						{
							XsollaCountries countries = new XsollaCountries();
							countries.Parse(rootNode);
							OnCountriesRecieved(countries);
						}
						break;
						case VIRTUAL_PAYMENT_SUMMARY:
						{
							XVirtualPaymentSummary summary = new XVirtualPaymentSummary();
							summary.Parse(rootNode);
							Logger.Log("VIRTUAL_PAYMENT_SUMMARY " + summary.ToString());
							if(summary.IsSkipConfirmation) {
								Logger.Log("IsSkipConfirmation true");
								post.Add("dont_ask_again", 0);
								ProceedVPayment(post);
							} else {
								Logger.Log("IsSkipConfirmation false");							
								OnVPSummaryRecieved(summary);
							}
						}
						break;
						case VIRTUAL_PROCEED:
						{
							XProceed proceed = new XProceed();
							proceed.Parse(rootNode);
							Logger.Log ("VIRTUAL_PROCEED " + proceed.ToString());
							if(proceed.IsInvoiceCreated) {
								Logger.Log ("VIRTUAL_PROCEED 1");
								long operationId = proceed.OperationId;
								post.Add("operation_id", operationId);
								VPaymentStatus(post);
							} else {
								Logger.Log ("VIRTUAL_PROCEED 0 ");
								OnVPProceedError(proceed.Error);
							}
						}
						break;
						case VIRTUAL_STATUS:
						{
							XVPStatus vpStatus = new XVPStatus();
							vpStatus.Parse(rootNode);
							//{"errors":[ {"message":"Insufficient balance to complete operation"} ], "api":{"ver":"1.0.1"}, "invoice_created":"false", "operation_id":"0", "code":"0"}
							Logger.Log ("VIRTUAL_STATUS" + vpStatus.ToString());
							OnVPStatusRecieved(vpStatus);
						}
						break;
						case APPLY_PROMO_COUPONE:
						{
							XsollaForm form = new XsollaForm();
							form.Parse(rootNode);
							OnApplyCouponeReceived(form);
						}
						break;
						case COUPON_PROCEED:
						{	
							XsollaCouponProceedResult couponProceed = new XsollaCouponProceedResult();
							couponProceed.Parse(rootNode);
							if (couponProceed._error != null)
							{
								Logger.Log("COUPON_PROCEED ERROR: " + couponProceed._error);
								OnCouponProceedErrorRecived(couponProceed);
							}
							else
							{
								long operationId = couponProceed._operationId;
								if (post.ContainsKey("coupon_code"))
									post.Remove("coupon_code");
								post.Add("operation_id", operationId);

								VPaymentStatus(post);
							}
						}
						break;
						case HISTORY:
						{
							XsollaHistoryList history = new XsollaHistoryList().Parse(rootNode["operations"]) as XsollaHistoryList;
							OnHistoryRecieved(history);
						}
						break;
						case CALCULATE_CUSTOM_AMOUNT:
						{
							CustomVirtCurrAmountController.CustomAmountCalcRes res = new CustomVirtCurrAmountController.CustomAmountCalcRes().Parse(rootNode["calculation"]) as CustomVirtCurrAmountController.CustomAmountCalcRes;
							OnCustomAmountResRecieved(res);

						}
						break;
					case ACTIVE_SUBS:
						{
							XsollaSubscriptions subs = new XsollaSubscriptions();
							subs.Parse(rootNode);
							OnSubscriptionsReceived(subs);
						}
						break;
						case PAYMENT_MANAGER_LIST:
						{
							XsollaSavedPaymentMethods res = new XsollaSavedPaymentMethods().Parse(rootNode) as XsollaSavedPaymentMethods;
							OnPaymentManagerMethod(res);
						}
						break;
						case DELETE_SAVED_METHOD:
						{
							if (rootNode["error"] != null)
							{
								// has error
								Logger.Log("Respond on delete savedPaymentMethod has error");
							}
							else
								OnDeleteSavedPaymentMethod();

							//XsollaDeleteSavedMethodRespond res = new XsollaDeleteSavedMethodRespond().Parse(rootNode) as XsollaDeleteSavedMethodRespond;
						}
						break;
						default:
						break;
					}
				} 
				else 
				{
					XsollaError error = new XsollaError();
					error.Parse(rootNode);
					OnErrorReceived(error);
				}
			} 
			else 
			{
				JSONNode errorNode = JSON.Parse(www.text);
				string errorMsg = errorNode["errors"].AsArray[0]["message"].Value 
					+ ". Support code " + errorNode["errors"].AsArray[0]["support_code"].Value;
				int errorCode = 0;
				if(www.error.Length > 3)
					errorCode = int.Parse(www.error.Substring(0, 3));
				else
					errorCode = int.Parse(www.error);
				OnErrorReceived(new XsollaError(errorCode, errorMsg));
			}    
			if(projectId != null && !"".Equals(projectId))
				LogEvent ("UNITY " + SDK_VERSION + " REQUEST", projectId, www.url);
			else 
				LogEvent ("UNITY " + SDK_VERSION + " REQUEST", "undefined", www.url);
		}

		private string GetUtilsLink(){
			return DOMAIN + "/paystation2/api/utils";
		}

		private string GetDirectpaymentLink(){
			return DOMAIN + "/paystation2/api/directpayment";
		}

		private string GetStatusLink(){
			return DOMAIN + "/paystation2/api/directpayment";
		}

		/*		PAYMENT METHODS LINKS	 */
		private string GetPricepointsUrl(){
			return DOMAIN + "/paystation2/api/pricepoints";
		}
		
		private string GetGoodsUrl(){
			return DOMAIN + "/paystation2/api/digitalgoods";
		}

		private string GetFavoritsUrl(){
			return DOMAIN + "/paystation2/api/virtualitems/favorite";
		}

		private string SetFavoritsUrl(){
			return DOMAIN + "/paystation2/api/virtualitems/setfavorite";
		}
		
		private string GetItemsGroupsUrl(){
			return DOMAIN + "/paystation2/api/virtualitems/groups";
		}

		private string GetCouponProceed()
		{
			return DOMAIN + "/paystation2/api/coupons/proceed";
		}
		
		private string GetItemsUrl(){
			return DOMAIN + "/paystation2/api/virtualitems/items";
		}

		private string GetHistoryUrl()
		{
			return DOMAIN + "/paystation2/api/balance/history";
		}


		/*		PAYMENT METHODS LINKS	 */

		// TODO New version API 
		private string GetPaymentListUrl()
		{
			return DOMAIN + "/paystation2/api/paymentlist/payment_methods";
		}

//		private string GetPaymentListUrl(){
//			return DOMAIN + "/paystation2/api/paymentlist";
//		}

		private string GetSavedPaymentListUrl(){
			return DOMAIN + "/paystation2/api/savedmethods";
		}

		private string GetQuickPaymentsUrl(){
			return DOMAIN + "/paystation2/api/paymentlist/quick_payments";
		}

		private string GetCountriesListUrl(){
			return DOMAIN + "/paystation2/api/country";
		}

		private string GetSubsUrl()
		{
			return DOMAIN + "/paystation2/api/recurring/active";
		}
		
		/*		BUY WITH VIRTUAL CURERENCY	 */
		private string GetCartSummary() {
			return DOMAIN + "/paystation2/api/cart/summary";
		}

		private string ProceedVirtualPaymentLink() {
			return DOMAIN + "/paystation2/api/virtualpayment/proceed";
		}

		private string GetVirtualPaymentStatusLink() {
			return DOMAIN + "/paystation2/api/virtualstatus";
		}

		private string GetCalculateCustomAmountUrl() 
		{
			return DOMAIN + "/paystation2/api/pricepoints/calculate";
		}

		private string GetDeleteSavedPaymentMethodUrl()
		{
			return DOMAIN + "/paystation2/api/savedmethods/delete";
		}

		//*** GA SECTION START ***
		private string propertyID = "UA-62372273-1";
		
		private static XsollaPaymentImpl instance;
		
		private string bundleID = "Unity";
		private string appName;
		private string projectId = "";
		
		private string screenRes;
		private string clientID;
		
//		void Awake()
//		{
//			if(instance)
//				DestroyImmediate(gameObject);
//			else
//			{
//				DontDestroyOnLoad(gameObject);
//				instance = this;
//			}
//		}
		
		void Start() 
		{
			appName = GetProjectName ();
			screenRes = Screen.width + "x" + Screen.height;
			
			#if UNITY_IPHONE
			clientID = SystemInfo.deviceUniqueIdentifier;// iPhoneSettings.uniqueIdentifier;
			#else
			clientID = SystemInfo.deviceUniqueIdentifier;
			#endif
			
		}
		
		public void LogScreen(string title)
		{
//			Debug.Log ("Google Analytics - Screen --> " + title);
			
			title = WWW.EscapeURL(title);
			
			var url = "https://www.google-analytics.com/collect?v=1&ul=en-us&t=appview&sr="+screenRes+"&an="+WWW.EscapeURL(appName)+"&a=448166238&tid="+propertyID+"&aid="+bundleID+"&cid="+WWW.EscapeURL(clientID)+"&_u=.sB&av="+SDK_VERSION+"&_v=ma1b3&cd="+title+"&qt=2500&z=185";
			
			StartCoroutine( Process(new WWW(url)) );
		}
		
		/*  MOBILE EVENT TRACKING:  https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide */
		public void LogEvent(string titleCat, string titleAction, string actionLable)
		{
			titleCat = WWW.EscapeURL(titleCat);
			titleAction = WWW.EscapeURL(titleAction);
			actionLable = WWW.EscapeURL (actionLable);
			//LOG Google analitic
			//Debug.Log ("Google Analytics - Event --> " + titleAction + " -->" + titleCat + " -->" + actionLable);

			var url = "https://www.google-analytics.com/collect?v=1&ul=en-us&t=event&sr="+screenRes+"&an="+WWW.EscapeURL(appName)+"&a=448166238&tid="+propertyID+"&aid="+bundleID+"&cid="+WWW.EscapeURL(clientID)+"&_u=.sB&av="+SDK_VERSION+"&_v=ma1b3&ec="+titleCat+"&ea="+titleAction+"&el"+actionLable+"&qt=2500&z=185";
			
			StartCoroutine( Process(new WWW(url)) );
		}

		public void LogEvent(string titleCat, string actionLable)
		{
			//LOG Google analitic
			//Debug.Log ("Google Analytics - Event --> " + actionLable);
			
			titleCat = WWW.EscapeURL(titleCat);
			if (projectId == null) {
				projectId = GetProjectName();//PlayerSettings.companyName + " | " + PlayerSettings.productName;
				projectId = WWW.EscapeURL (projectId);
			}
			actionLable = WWW.EscapeURL (actionLable);
			
			var url = "https://www.google-analytics.com/collect?v=1&ul=en-us&t=event&sr="+screenRes+"&an="+WWW.EscapeURL(appName)+"&a=448166238&tid="+propertyID+"&aid="+bundleID+"&cid="+WWW.EscapeURL(clientID)+"&_u=.sB&av="+SDK_VERSION+"&_v=ma1b3&ec="+titleCat+"&ea="+projectId+"&el="+actionLable+"&qt=2500&z=185";
			
			StartCoroutine( Process(new WWW(url)) );
		}
		
		
		
		private IEnumerator Process(WWW www)
		{
			yield return www;
			
			if(www.error == null)
			{
				if (www.responseHeaders.ContainsKey("STATUS"))
				{
					//LOG Google analitic
					if (www.responseHeaders["STATUS"] == "HTTP/1.1 200 OK")	
					{
						//Debug.Log ("GA Success");
					} else {
						//Debug.LogWarning(www.responseHeaders["STATUS"]);	
					}
				}else{
					Debug.LogWarning("Event failed to send to Google");	
				}
			}else{
				Debug.LogWarning(www.error.ToString());	
			}
			
			www.Dispose();
		}
		
		//*** GA SECTION END ***

		public string GetProjectName()
		{
			string[] s = Application.dataPath.Split('/');
			string projectName = s[s.Length - 2];
//			Debug.Log("project = " + projectName);
			return projectName;
		}
		
	}
}
