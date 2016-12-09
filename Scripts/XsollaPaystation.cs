using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;
using System.Linq;

namespace  Xsolla
{
	public abstract class XsollaPaystation : MonoBehaviour {

		private string BaseParams;
		private bool IsSandbox;
		protected XsollaUtils Utils;
		private ActivePurchase currentPurchase;
		private bool cancelStatusCheck = false;
		private bool isSimple = false;
		public string _countryCurr = "";

		private XsollaPaymentImpl __payment;
		private XsollaPaymentImpl Payment
		{
			get
			{
				if (__payment == null)
				{
					__payment = gameObject.GetComponent <XsollaPaymentImpl>();
					if (__payment == null)
					{
						__payment = gameObject.AddComponent <XsollaPaymentImpl>() as XsollaPaymentImpl;
					}
				}
				return __payment;
			}
			set
			{
				__payment = value;
			}
		}

		protected XsollaResult result;

		public bool isSandbox()
		{
			return IsSandbox;
		}

		protected abstract void ShowPricepoints (XsollaUtils utils, XsollaPricepointsManager pricepoints);

		protected abstract void ShowGoodsGroups (XsollaGroupsManager groups);
		protected abstract void UpdateGoods (XsollaGoodsManager goods);

		protected abstract void ShowQuickPaymentsList (XsollaUtils utils, XsollaQuickPayments paymentMethods);
		protected abstract void ShowPaymentsList (XsollaPaymentMethods paymentMethods);
		protected abstract void ShowSavedPaymentsList(XsollaSavedPaymentMethods savedPaymentsMethods);
		protected abstract void ShowCountries (XsollaCountries paymentMethods);
		protected abstract void ApplyPromoCouponeCode(XsollaForm pForm);
		protected abstract void ShowHistory(XsollaHistoryList pList);
		protected abstract void UpdateCustomAmount(CustomVirtCurrAmountController.CustomAmountCalcRes pRes);

		protected abstract void ShowPaymentForm (XsollaUtils utils, XsollaForm form);

		protected abstract void ShowPaymentStatus (XsollaTranslations translations, XsollaStatus status);
		protected abstract void CheckUnfinishedPaymentStatus (XsollaStatus status, XsollaForm form);

		protected abstract void ShowVPSummary (XsollaUtils utils, XVirtualPaymentSummary summary);
		protected abstract void ShowVPError (XsollaUtils utils, string error);
		protected abstract void ShowVPStatus (XsollaUtils utils, XVPStatus status);
		protected abstract void GetCouponErrorProceed(XsollaCouponProceedResult presult);
		protected abstract void PaymentManagerRecieved(XsollaSavedPaymentMethods pResult);
		protected abstract void DeleteSavedPaymentMethodRecieved();

		//{"user":{ "id":{ "value":"1234567","hidden":true},"email":{"value":"support@xsolla.com"},"name":{"value":"Tardis"},"country":{"value":"US"} },"settings":{"project_id":15764,"language":"en","currency":"USD"}}
		//jafS6nqbzRpZzA38
		// BGKkyK2VetScsLgOcnchTB3r1XdkQaW4 - sandbox
		//KVvI4jVlPaTbre4IAD2chJWTBRqQPkCD
		public void OpenPaystation (string accessToken, bool isSandbox)
		{
//			AddHttpRequestObj();
			SetLoading (isSandbox);
			Logger.isLogRequired = true;//isSandbox;
			Logger.Log ("Paystation initiated current mode sandbox");
			currentPurchase = new ActivePurchase();
			JSONNode rootNode = JSON.Parse(accessToken);
			Dictionary<string, object> dict = new Dictionary<string, object> ();
			if (rootNode == null) {
				isSimple = false;
				dict.Add ("access_token", accessToken);
				BaseParams = "access_token=" + accessToken;
			} else {
				isSimple = true;
				dict.Add ("access_data", accessToken);
				BaseParams = "access_data=" + accessToken;
			}
			StartPayment (dict, isSandbox);
		}

		public static void AddHttpRequestObj(){
			GameObject loader = GameObject.Find(HttpTlsRequest.loaderGameObjName);
			if (loader == null)
			{
				GameObject loaderObj = Instantiate(Resources.Load("Prefabs/GameLoader")) as GameObject;
				loaderObj.name = HttpTlsRequest.loaderGameObjName;
			}
		}
			
		private void StartPayment(Dictionary<string, object> dict, bool isSandbox){
			Logger.Log ("Request prepared");
			currentPurchase.Add(ActivePurchase.Part.TOKEN, dict);
			IsSandbox = isSandbox;
			if(isSimple)
				CheckUnfinished ();
			Payment.UtilsRecieved += RecieveUtils;
			
			Payment.FormReceived += (form) => ShowPaymentForm(Utils, form);
			Payment.StatusReceived += (status, form) => {
				//TODO make better solution
				FillPurchase(ActivePurchase.Part.XPS, form.GetXpsMap());
				ShowPaymentStatus (Utils.GetTranslations (), status); 
			};
			Payment.ApplyCouponeCodeReceived += (form) => ApplyPromoCouponeCode(form);
			Payment.StatusChecked += (status) => WaitingStatus(status);
			
			Payment.QuickPaymentMethodsRecieved += (quickpayments) => ShowQuickPaymentsList(Utils, quickpayments);
			Payment.PaymentMethodsRecieved += ShowPaymentsList;
			Payment.SavedPaymentMethodsRecieved += ShowSavedPaymentsList;
			Payment.CountriesRecieved += ShowCountries;
			Payment.HistoryRecieved += ShowHistory;

			Payment.PricepointsRecieved += (pricepoints) => ShowPricepoints(Utils, pricepoints);
			Payment.GoodsGroupsRecieved += (goods) => ShowGoodsGroups(goods);
			Payment.GoodsRecieved += (goods) => UpdateGoods(goods);
			Payment.CustomAmountCalcRecieved += (calcRes) => UpdateCustomAmount(calcRes);

			Payment.VirtualPaymentSummaryRecieved += (summary) => ShowVPSummary(Utils, summary);
			Payment.VirtualPaymentProceedError += (error) => ShowVPError(Utils, error);
			Payment.VirtualPaymentStatusRecieved += (status) => ShowVPStatus(Utils, status);

			Payment.CouponProceedErrorRecived += (proceed) => GetCouponErrorProceed(proceed);
			Payment.PaymentManagerMethods += (savedMethods) => PaymentManagerRecieved(savedMethods);
			Payment.DeleteSavedPaymentMethodRespond += () => DeleteSavedPaymentMethodRecieved();
			
			Payment.ErrorReceived += ShowPaymentError;
			Payment.SetModeSandbox (isSandbox);
			Payment.InitPaystation(currentPurchase.GetMergedMap());
		}

		private void CheckUnfinished(){
			Logger.Log ("Check unfinished payments");
			if (TransactionHelper.CheckUnfinished ()) {
				Logger.Log ("Have unfinished payments");
				Payment.StatusReceived += CheckUnfinishedPaymentStatus;
				var request = TransactionHelper.LoadRequest();
				if(request != null) {
					Payment.GetStatus(request);
				} else {
					TransactionHelper.Clear();
					Payment = null;
				}
			}
		}

		protected void NextPaymentStep(Dictionary<string, object> xpsMap)
		{
			Logger.Log ("Next Payment Step request");
			SetLoading (true);
			Payment.NextStep (xpsMap);
		}

		public void LoadShopPricepoints()
		{	
			Logger.Log ("Load Pricepoints request");
			SetLoading (true);
			Payment.GetPricePoints (currentPurchase.GetMergedMap());
		}
		
		public void LoadGoodsGroups()
		{
			Logger.Log ("Load Goods Groups request");
			SetLoading (true);
			Payment.GetItemsGroups (currentPurchase.GetMergedMap());
		}

		public void LoadGoods(long groupId)
		{
			Logger.Log ("Load Goods request");
			Payment.GetItems (groupId, currentPurchase.GetMergedMap());
		}

		public void LoadFavorites()
		{
			Logger.Log ("Load Favorites request");
			Payment.GetFavorites (currentPurchase.GetMergedMap());
		}

		public void GetCouponProceed(string pCouponCode)
		{
			Dictionary<string, object> map = new Dictionary<string, object>();
			map.Add("coupon_code", pCouponCode);
			Payment.GetCouponProceed(map);
		}

		public void SetFavorite(Dictionary<string, object> items)
		{
			foreach (var kvPair in currentPurchase.GetPart(ActivePurchase.Part.TOKEN)) {
				items.Add(kvPair.Key, kvPair.Value);
			}
			Payment.SetFavorite (items);
		}

		public void LoadQuickPayment()
		{
			Logger.Log ("Load Quick Payments request");
			if (currentPurchase != null && currentPurchase.counter > 2) {
				currentPurchase.Remove (ActivePurchase.Part.PID);
				currentPurchase.Remove (ActivePurchase.Part.XPS);
			}

			LoadSavedPaymentMethods();
			LoadPaymentMethods ();
			LoadCountries ();
			SetLoading (true);
		}

		public void LoadPaymentMethods()
		{
			Logger.Log ("Load Payment Methods request");
			SetLoading (true);	
			Payment.GetPayments (_countryCurr, currentPurchase.GetMergedMap());
		}
			

		public void LoadSavedPaymentMethods()
		{
			Logger.Log ("Load saved payments methods");
			SetLoading(true);
			Payment.GetSavedPayments(currentPurchase.GetMergedMap());
		}

		public void LoadCountries()
		{
			Logger.Log ("Load Countries request");
			SetLoading (true);
			Payment.GetCountries (currentPurchase.GetMergedMap());
		}

		public void LoadHistory(Dictionary<string, object> pParams)
		{
			Payment.GetHistory(pParams);
		}

		public void LoadPaymentManager()
		{
			Logger.Log("Show Payment manager");
			Dictionary<string, object> lParams = new Dictionary<string, object>();
			lParams.Add("userInitialCurrency", "");
			Payment.GetSavedPaymentsForManager(lParams);
		}

		public void UpdateCountries(string countryIso)
		{
			Logger.Log ("Update Countries request");
			_countryCurr = countryIso;
			Payment.GetPayments (countryIso, currentPurchase.GetMergedMap());
		}

		public void ChooseItem(Dictionary<string, object> items)
		{
			Logger.Log ("Choose item request");
			if(isSimple)
				TransactionHelper.SavePurchase (items);
			result = new XsollaResult (new Dictionary<string, object>(items));
			currentPurchase.Remove (ActivePurchase.Part.PID);
			currentPurchase.Remove (ActivePurchase.Part.XPS);
			FillPurchase (ActivePurchase.Part.ITEM, items);
			TryPay();
		}

		public void ChooseItem(Dictionary<string, object> items, bool isVirtualPayment)
		{
			Logger.Log ("Choose item request");
			if(isSimple)
				TransactionHelper.SavePurchase (items);
			result = new XsollaResult (new Dictionary<string, object>(items));
			currentPurchase.Remove (ActivePurchase.Part.PID);
			currentPurchase.Remove (ActivePurchase.Part.XPS);
			FillPurchase (ActivePurchase.Part.ITEM, items);
			if (!isVirtualPayment)
				TryPay ();
			else {
				SetLoading (true);
				Payment.GetVPSummary (currentPurchase.GetMergedMap ());
			}
		}

		public void ProceedVirtualPayment(Dictionary<string, object> items)
		{
			Logger.Log ("Proceed VirtualPayment");
			FillPurchase (ActivePurchase.Part.PROCEED, items);
			SetLoading (true);			
			Payment.ProceedVPayment (currentPurchase.GetMergedMap ());
		}


		public void ChoosePaymentMethod(Dictionary<string, object> items)
		{
			Logger.Log ("Choose payment method request");
			items.Add ("returnUrl", "https://secure.xsolla.com/paystation3/#/desktop/return/?" + BaseParams);
			FillPurchase (ActivePurchase.Part.PID, items);
			TryPay();
		}

		public void ApplyPromoCoupone(Dictionary<string, object> items)
		{
			Logger.Log("Apply promo-coupone");
			FillPurchase(ActivePurchase.Part.XPS, items);
			TryApplyCoupone();
			
		}

		public void DeleteSavedPaymentMethod(Dictionary<string, object> pParams)
		{
			Logger.Log("Delete method");
			Payment.DeleteSavedMethod(pParams);
		}

		public void ReplacedOnSavedMethod(Dictionary<string, object> pParams)
		{
			Logger.Log("Replaced saved method on saved method");
			FillPurchase(ActivePurchase.Part.XPS, pParams);
			TryPay();
		}

		public void DoPayment(Dictionary<string, object> items)
		{
			Logger.Log ("Do payment");
			currentPurchase.Remove (ActivePurchase.Part.INVOICE);
			FillPurchase (ActivePurchase.Part.XPS, items);
			TryPay();
		}

		public void GetStatus(Dictionary<string, object> items)
		{
			Logger.Log ("Get Status");
			FillPurchase (ActivePurchase.Part.INVOICE, items);
			Payment.NextStep (currentPurchase.GetMergedMap());
		}

		public void CalcCustomAmount(Dictionary<string, object> pParam)
		{
			Logger.Log("Calc custom amount");
			Payment.CalculateCustomAmount(pParam);
		}

		protected void Restart (){
			Logger.Log ("Restart payment");
			currentPurchase.RemoveAllExceptToken ();
			cancelStatusCheck = true;
		}

		public void RetryPayment()
		{
			Logger.Log ("Retry payment");
			TryPay();
		}
		
		public void FillPurchase(ActivePurchase.Part part, Dictionary<string, object> items)
		{
			if (currentPurchase == null) {
				currentPurchase = new ActivePurchase();
				currentPurchase.Add(part, new Dictionary<string, object>(items));
			} else {
				currentPurchase.Remove(part);
				currentPurchase.Add(part, new Dictionary<string, object>(items));
			}
		}

		public void RemovePurchasePart(ActivePurchase.Part pPart)
		{
			Logger.Log("Remove purchase part: " + pPart.ToString());
			if ((currentPurchase != null) && (currentPurchase.ContainsKey(pPart)))
			{
				currentPurchase.Remove(pPart);		
			}
			else
				Logger.Log("purchase is null or don't contain key");
		}

		private void TryApplyCoupone()
		{
			Dictionary<string, object> items = currentPurchase.GetMergedMap();
			if (!items.ContainsKey(XsollaApiConst.XPS_FIX_COMMAND))
				items.Add(XsollaApiConst.XPS_FIX_COMMAND, XsollaApiConst.COMMAND_CALCULATE);
			else
				items[XsollaApiConst.XPS_FIX_COMMAND] = XsollaApiConst.COMMAND_CALCULATE;

			if (!items.ContainsKey(XsollaApiConst.XPS_CHANGE_ELEM))
				items.Add(XsollaApiConst.XPS_CHANGE_ELEM, XsollaApiConst.COUPON_CODE);
			else 
				items[XsollaApiConst.XPS_CHANGE_ELEM] = XsollaApiConst.COUPON_CODE;
			Payment.ApplyPromoCoupone(items);
		}

		private void TryPay()
		{
			Logger.Log ("Try pay");
			if (Utils.GetPurchase () != null) {
				if (currentPurchase.counter >= 2) {
					NextPaymentStep (currentPurchase.GetMergedMap());
				} else {
					LoadQuickPayment ();
				}
			} else {
				if (currentPurchase.counter >= 3) {
					NextPaymentStep (currentPurchase.GetMergedMap());
				} else {
					LoadQuickPayment ();
				}
			}
		}

		protected virtual void RecieveUtils (XsollaUtils utils){
			Logger.Log ("Utils recived");
			Utils = utils;
			if(isSimple) {
				BaseParams += "&access_token=" + utils.GetAcceessToken();
				currentPurchase.GetPart(ActivePurchase.Part.TOKEN).Remove("access_data");
				currentPurchase.GetPart(ActivePurchase.Part.TOKEN).Add("access_token", utils.GetAcceessToken());
			}
			XsollaPurchase xsollaPurchase = utils.GetPurchase ();
			if (xsollaPurchase != null) 
			{
				bool isPurchase = xsollaPurchase.IsPurchase();
				if(xsollaPurchase.paymentSystem != null && isPurchase){
					NextPaymentStep(currentPurchase.GetMergedMap());
				} else if(isPurchase){ 
					LoadQuickPayment();
				} else {
					LoadShop(utils);
				}
			} 
			else 
			{
				LoadShop(utils);
			}
			SetLoading (false);
		}

		private void LoadShop(XsollaUtils utils){
			Logger.Log ("Load Shop request");
			XsollaPaystation2 paystation2 = utils.GetSettings ().paystation2;
			if (paystation2.goodsAtFirst != null && paystation2.goodsAtFirst.Equals("1"))
			{
				LoadGoodsGroups();
			} else 
			if (paystation2.pricepointsAtFirst != null && paystation2.pricepointsAtFirst.Equals("1")){
				LoadShopPricepoints();
			}
		}

		public void LoadShop(){
			Logger.Log ("Load Shop request");
			if (Utils != null) {
				XsollaPaystation2 paystation2 = Utils.GetSettings ().paystation2;
				if (paystation2.goodsAtFirst != null && paystation2.goodsAtFirst.Equals ("1")) {
					LoadGoodsGroups ();
				} 
				else if (paystation2.pricepointsAtFirst != null && paystation2.pricepointsAtFirst.Equals ("1"))
				{
					LoadShopPricepoints ();
				}
			}
		}

		protected void WaitingStatus (XsollaStatusPing pStatus) {
			Logger.Log ("Waiting payment status");
			if (XsollaStatus.Group.DONE != pStatus.GetGroup() && XsollaStatus.Group.TROUBLED != pStatus.GetGroup() && pStatus.GetElapsedTiem() < 1200) {
				if (pStatus.isFinal()) {
//					Payment.InitPaystation(currentPurchase.GetMergedMap());
					LoadShopPricepoints ();
					cancelStatusCheck = false;
				} else {
					StartCoroutine (Test ());
				}
//			} else if ("delivering".Equals (status)) {
//				StartCoroutine (Test ());
			} else {
				currentPurchase.Remove(ActivePurchase.Part.INVOICE);
				TryPay();
			}
		}

		private IEnumerator Test(){
			yield return new WaitForSeconds(2);
			Payment.NextStep (currentPurchase.GetMergedMap());
		}

		protected abstract void ShowPaymentError (XsollaError error);

		protected abstract void SetLoading (bool isLoading);

	}
}
