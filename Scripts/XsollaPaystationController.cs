
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Xsolla
{
	public class XsollaPaystationController : XsollaPaystation {

		private const string PREFAB_SCREEN_STATUS 		 = "Prefabs/SimpleView/_ScreenStatus/ScreenStatusNew";
		private const string PREFAB_SCREEN_ERROR 		 = "Prefabs/Screens/ScreenError";
		private const string PREFAB_SCREEN_ERROR_MAIN 	 = "Prefabs/Screens/MainScreenError";
		private const string PREFAB_SCREEN_CHECKOUT 	 = "Prefabs/SimpleView/_ScreenCheckout/ScreenCheckout";
		private const string PREFAB_SCREEN_VP_SUMMARY 	 = "Prefabs/SimpleView/_ScreenVirtualPaymentSummary/ScreenVirtualPaymentSummary";
		private const string PREFAB_SCREEN_REDEEM_COUPON = "Prefabs/SimpleView/_ScreenShop/RedeemCouponView";

		private const string PREFAB_VIEW_MENU_ITEM		 = "Prefabs/SimpleView/MenuItem";
		private const string PREFAB_VIEW_MENU_ITEM_ICON	 = "Prefabs/SimpleView/MenuItemIcon";
		private const string PREFAB_VIEW_MENU_ITEM_EMPTY = "Prefabs/SimpleView/MenuItemEmpty";

		public event Action<XsollaResult> 	OkHandler;
		public event Action<XsollaError> 	ErrorHandler;

		public GameObject 					mainScreen;
		public MyRotation 					progressBar;
		private bool 						isMainScreenShowed = false;

		public GameObject 					shopScreenPrefab;
		public GameObject 					paymentListScreebPrefab;
		public GameObject 					container;

		private PaymentListScreenController _paymentListScreenController;
		private ShopViewController 			_shopViewController;
		private RedeemCouponViewController  _couponController;
		private RadioGroupController 		_radioController;

		private static ActiveScreen 		currentActive = ActiveScreen.UNKNOWN;
		private Transform 					menuTransform;
		private GameObject 					mainScreenContainer;

		enum ActiveScreen
		{
			SHOP, P_LIST, VP_PAYMENT, PAYMENT, STATUS, ERROR, UNKNOWN, FAV_ITEMS_LIST, REDEEM_COUPONS
		}

		protected override void RecieveUtils (XsollaUtils utils)
		{
			StyleManager.Instance.ChangeTheme (utils.GetSettings().GetTheme());
			mainScreen = Instantiate (mainScreen);
			mainScreen.transform.SetParent (container.transform);
			mainScreen.SetActive (true);
			mainScreenContainer = mainScreen.GetComponentsInChildren<ScrollRect> ()[0].gameObject;
			menuTransform = mainScreen.GetComponentsInChildren<RectTransform> ()[5].transform;
			Resizer.ResizeToParrent (mainScreen);
			base.RecieveUtils(utils);
			InitHeader(utils);
			InitFooter (utils);
			if(utils.GetPurchase() == null || !utils.GetPurchase().IsPurchase())
				InitMenu(utils);
		}

		protected override void ShowPricepoints (XsollaUtils utils, XsollaPricepointsManager pricepoints)
		{
			Logger.Log ("Pricepoints recived");
			OpenPricepoints (utils, pricepoints);
			SetLoading (false);
		}

		protected override void ShowGoodsGroups (XsollaGroupsManager groups)
		{
			Logger.Log ("Goods Groups recived");
			OpenGoods (groups);
//			SetLoading (false);
		}

		protected override void UpdateGoods (XsollaGoodsManager goods)
		{
			Logger.Log ("Goods recived");
			// SetVirtual curr name
			goods.setItemVirtCurrName(Utils.GetProject().virtualCurrencyName);
			_shopViewController.UpdateGoods(goods, Utils.GetTranslations().Get(XsollaTranslations.VIRTUAL_ITEM_OPTION_BUTTON));
			SetLoading (false);
		}

		protected override void ShowPaymentForm (XsollaUtils utils, XsollaForm form)
		{
			Logger.Log ("Payment Form recived");
			DrawForm (utils, form);
			SetLoading (false);
		}
			
		protected override void ShowPaymentStatus (XsollaTranslations translations, XsollaStatus status)
		{
			Logger.Log ("Status recived");
			SetLoading (false);
			DrawStatus (translations, status);
		}

		protected override void CheckUnfinishedPaymentStatus (XsollaStatus status, XsollaForm form)
		{
			Logger.Log ("Check Unfinished Payment Status");
			if (status.GetGroup () == XsollaStatus.Group.DONE) {
				var purchase = TransactionHelper.LoadPurchase();
				XsollaResult result = new XsollaResult(purchase);
				result.invoice = status.GetStatusData().GetInvoice ();
				result.status = status.GetStatusData().GetStatus ();
				Logger.Log("Ivoice ID " + result.invoice);
				Logger.Log("Bought", purchase);
				if(TransactionHelper.LogPurchase(result.invoice)) {
					if (OkHandler != null)
						OkHandler (result);
				} else {
						Logger.Log("Alredy added");
				}
				TransactionHelper.Clear();
			}
		}

		protected override void ShowPaymentError (XsollaError error)
		{
			Logger.Log ("Show Payment Error " + error);
			SetLoading (false);
			DrawError (error);
		}

		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>> PAYMENT METHODS >>>>>>>>>>>>>>>>>>>>>>>>>>>> 
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		private void DrawPaymentListScreen(){
			currentActive = ActiveScreen.P_LIST;
			if (_paymentListScreenController == null) {
				GameObject paymentListScreen = Instantiate (paymentListScreebPrefab);
				_paymentListScreenController = paymentListScreen.GetComponent<PaymentListScreenController> ();
				_paymentListScreenController.transform.SetParent (mainScreenContainer.transform);
				_paymentListScreenController.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				mainScreenContainer.GetComponentInParent<ScrollRect> ().content = _paymentListScreenController.GetComponent<RectTransform> ();
			}
		}

		protected override void ShowQuickPaymentsList (XsollaUtils utils, XsollaQuickPayments quickPayments)
		{
			DrawPaymentListScreen ();
			if (!_paymentListScreenController.IsQuiqckPayments()) {
				_paymentListScreenController.InitScreen (utils);
				_paymentListScreenController.SetQuickPayments (quickPayments);
//				LoadPaymentMethods ();
			} else {
				_paymentListScreenController.UpdateQuick(quickPayments);
			}
			if(_paymentListScreenController.IsAllLoaded())
				SetLoading (false);
		}

		protected override void ShowPaymentsList (XsollaPaymentMethods paymentMethods)
		{
			DrawPaymentListScreen ();
			if (!_paymentListScreenController.IsAllPayments ()) {
				_paymentListScreenController.SetPaymentsMethods (paymentMethods);
//				LoadCountries ();
			} else {
				//_paymentListScreenController.SetPaymentsMethods(paymentMethods);
				_paymentListScreenController.UpdateRecomended(paymentMethods);
			}
			if(_paymentListScreenController.IsAllLoaded())
				SetLoading (false);
		}

		protected override void ShowSavedPaymentsList (XsollaSavedPaymentMethods savedPaymentsMethods)
		{
			DrawPaymentListScreen ();
			if (!_paymentListScreenController.IsSavedPayments())
				_paymentListScreenController.SetSavedPaymentsMethods(savedPaymentsMethods);
			else
				_paymentListScreenController.UpdateSavedMethods(savedPaymentsMethods);
			if(_paymentListScreenController.IsAllLoaded())
				SetLoading (false);
		}

		protected override void ShowCountries (XsollaCountries countries)
		{
			DrawPaymentListScreen ();
			_paymentListScreenController.SetCountries (countries);
			if(_paymentListScreenController.IsAllLoaded())
				SetLoading (false);
//			throw new System.NotImplementedException ();
		}

		protected override void ShowVPSummary(XsollaUtils utils, XVirtualPaymentSummary summary) {
			SetLoading (false);
			DrawVPSummary (utils, summary);
		}

		protected override void ShowVPError(XsollaUtils utils, string error) {
			SetLoading (false);
			DrawVPError (utils, error);
		}
		
		protected override void ShowVPStatus (XsollaUtils utils, XVPStatus status) {
			SetLoading (false);
			DrawVPStatus (utils, status);
		}

		protected override void GetCouponErrorProceed (XsollaCouponProceedResult pResult)
		{
			Logger.Log(pResult.ToString());
			if(_couponController != null)
			{
				_couponController.ShowError(pResult._error);
				return;
			}
		}
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<< PAYMENT METHODS <<<<<<<<<<<<<<<<<<<<<<<<<<<< 
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> SHOP >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		private void DrawShopScreen(){
			currentActive = ActiveScreen.SHOP;
			if (_shopViewController == null) {
				GameObject paymentListScreen = Instantiate (shopScreenPrefab);
				_shopViewController = paymentListScreen.GetComponent<ShopViewController> ();
				_shopViewController.transform.SetParent (mainScreenContainer.transform);
				_shopViewController.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				mainScreenContainer.GetComponentInParent<ScrollRect> ().content = _shopViewController.GetComponent<RectTransform> ();
			}
		}

		public void OpenPricepoints(XsollaUtils utils, XsollaPricepointsManager pricepoints)
		{
			DrawShopScreen ();
			string title = utils.GetTranslations ().Get (XsollaTranslations.PRICEPOINT_PAGE_TITLE);
			string vcName = utils.GetProject ().virtualCurrencyName;
			string buyText = utils.GetTranslations ().Get (XsollaTranslations.VIRTUAL_ITEM_OPTION_BUTTON);
			_shopViewController.OpenPricepoints(title, pricepoints, vcName, buyText);
		}
		
		public void OpenGoods(XsollaGroupsManager groups)
		{
			DrawShopScreen ();
			LoadGoods (groups.GetItemByPosition(0).id);
			_shopViewController.OpenGoods(groups);
			_radioController.SelectItem(0);
		}

		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< SHOP <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

		private void DrawStatus(XsollaTranslations translations, XsollaStatus status)
		{
			currentActive = ActiveScreen.STATUS;
			menuTransform.gameObject.SetActive (false);
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_STATUS)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			StatusViewController controller = statusScreen.GetComponent<StatusViewController> ();
			controller.StatusHandler += OnUserStatusExit;
			controller.InitScreen(translations, status);
		}

		public void ShowRedeemCoupon()
		{
			currentActive = ActiveScreen.REDEEM_COUPONS;
			GameObject screenRedeemCoupons = Instantiate(Resources.Load(PREFAB_SCREEN_REDEEM_COUPON)) as GameObject;
			// clear container
			Resizer.DestroyChilds(mainScreenContainer.transform);
			screenRedeemCoupons.transform.SetParent (mainScreenContainer.transform);
			screenRedeemCoupons.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
			Resizer.ResizeToParrent (screenRedeemCoupons);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = screenRedeemCoupons.GetComponent<RectTransform> ();
			_couponController = screenRedeemCoupons.GetComponent<RedeemCouponViewController>();
			_couponController.InitScreen(base.Utils);
			_couponController._btnApply.onClick.AddListener(delegate
				{
					CouponApplyClick(_couponController.GetCode());
				});
		}

		private void CouponApplyClick(string pCode)
		{
            _couponController.HideError();
			Logger.Log("ClickApply" + " - " + pCode);
			GetCouponProceed(pCode);
		}
			
		private void DrawError(XsollaError error)
		{
			if (mainScreenContainer != null) {
				currentActive = ActiveScreen.ERROR;
				GameObject errorScreen = Instantiate (Resources.Load (PREFAB_SCREEN_ERROR)) as GameObject;
				errorScreen.transform.SetParent (mainScreenContainer.transform);
				errorScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				mainScreenContainer.GetComponentInParent<ScrollRect> ().content = errorScreen.GetComponent<RectTransform> ();
				ScreenErrorController controller = errorScreen.GetComponent<ScreenErrorController> ();
				controller.ErrorHandler += OnErrorRecivied;
				controller.DrawScreen (error);
			} else {
				GameObject errorScreen = Instantiate (Resources.Load (PREFAB_SCREEN_ERROR_MAIN)) as GameObject;
				errorScreen.transform.SetParent (container.transform);
				Text[] texts = errorScreen.GetComponentsInChildren<Text>();
				texts[1].text = "Somthing went wrong";
				texts[2].text = error.errorMessage;
				texts[3].text = error.errorCode.ToString();
				texts[3].gameObject.SetActive(false);
				Resizer.ResizeToParrent (errorScreen);
			}
		}

		private void DrawForm(XsollaUtils utils, XsollaForm form)
		{
			currentActive = ActiveScreen.PAYMENT;
			GameObject checkoutScreen = Instantiate (Resources.Load(PREFAB_SCREEN_CHECKOUT)) as GameObject;
			checkoutScreen.transform.SetParent(mainScreenContainer.transform);
			checkoutScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			//scroll.content = paymentScreen.GetComponent<RectTransform> ();
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = checkoutScreen.GetComponent<RectTransform> ();
			ScreenCheckoutController controller = checkoutScreen.GetComponent<ScreenCheckoutController> ();
			controller.InitScreen(utils, form);
		}


		XVirtualPaymentSummary _summary;
		private void DrawVPSummary(XsollaUtils utils, XVirtualPaymentSummary summary)
		{
			_summary = summary;
			currentActive = ActiveScreen.VP_PAYMENT;
			menuTransform.gameObject.SetActive (true);
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_VP_SUMMARY)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			ScreenVPController screenVpController = statusScreen.GetComponent<ScreenVPController> ();
			screenVpController.DrawScreen(utils, summary);
		}

		private void DrawVPError(XsollaUtils utils, string error) {
			currentActive = ActiveScreen.VP_PAYMENT;
			menuTransform.gameObject.SetActive (true);
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_VP_SUMMARY)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			ScreenVPController screenVpController = statusScreen.GetComponent<ScreenVPController> ();
			screenVpController.DrawScreen(utils, _summary);
			screenVpController.ShowError (error);
		}

		
		private void DrawVPStatus (XsollaUtils utils, XVPStatus status) {
			currentActive = ActiveScreen.STATUS;
			menuTransform.gameObject.SetActive (false);
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_STATUS)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			StatusViewController controller = statusScreen.GetComponent<StatusViewController> ();
			controller.StatusHandler += OnUserStatusExit;
			controller.DrawVpStatus(utils, status);
		}
		
		protected override void SetLoading (bool isLoading)
		{
			if (!isMainScreenShowed) {
				if (isLoading) {
					mainScreen.SetActive (false);
				} else {
					mainScreen.SetActive (true);
					isMainScreenShowed = true;
				}
			} else {
				if (isLoading) {
					Resizer.DestroyChilds(mainScreenContainer.transform);
				}
			}
			progressBar.SetLoading (isLoading);
		}

		private void InitHeader(XsollaUtils utils)
		{
			Text[] texts = mainScreen.GetComponentsInChildren<Text> ();
			texts [0].text = utils.GetProject ().name;
		}

		//TODO minimize
		private void InitMenu(XsollaUtils utils)
		{
			_radioController = menuTransform.gameObject.AddComponent<RadioGroupController> ();
			GameObject menuItemPrefab 		= Resources.Load (PREFAB_VIEW_MENU_ITEM) as GameObject;
			GameObject menuItemIconPrefab 	= Resources.Load (PREFAB_VIEW_MENU_ITEM_ICON) as GameObject;
			GameObject menuItemEmptyPrefab 	= Resources.Load (PREFAB_VIEW_MENU_ITEM_EMPTY) as GameObject;
//			menuTransform = mainScreen.GetComponentInChildren<HorizontalLayoutGroup> ().gameObject.transform;
			Dictionary<string, XComponent> components = utils.GetProject().components;
			XsollaPaystation2 paystation2 			  = utils.GetSettings ().paystation2;
			bool isGoodsRequred = components.ContainsKey("items") && components ["items"].IsEnabled;
			if(isGoodsRequred)
			{
				GameObject menuItemGoods = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemGoods.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = utils.GetTranslations().Get(XsollaTranslations.VIRTUALITEM_PAGE_TITLE);
				menuItemGoods.GetComponent<Button>().onClick.AddListener(delegate {
					_radioController.SelectItem(menuItemGoods.GetComponent<RadioButton>());
					LoadGoodsGroups();
				});
				menuItemGoods.transform.SetParent(menuTransform);
				_radioController.AddButton(menuItemGoods.GetComponent<RadioButton>());
			}
			//HACK with Unity 5.3
			//bool isPricepointsRequired = components.ContainsKey("virtual_currency") && components ["virtual_currency"].IsEnabled;
			if (paystation2.pricepointsAtFirst != null && paystation2.pricepointsAtFirst.Equals("1"))
			{
				GameObject menuItemPricepoints = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemPricepoints.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = utils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_TITLE);
				menuItemPricepoints.GetComponent<Button>().onClick.AddListener(delegate {
					_radioController.SelectItem(menuItemPricepoints.GetComponent<RadioButton>());
					LoadShopPricepoints();
				});
				menuItemPricepoints.transform.SetParent(menuTransform);	
				_radioController.AddButton(menuItemPricepoints.GetComponent<RadioButton>());
			} 

			if (components.ContainsKey("subscriptions") && components["subscriptions"].IsEnabled)
			{
				GameObject menuItemSubs = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemSubs.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = utils.GetTranslations().Get(XsollaTranslations.SUBSCRIPTION_MOBILE_PAGE_TITLE);
				menuItemSubs.GetComponent<Button>().onClick.AddListener(delegate {
					//_shopViewController.OpenSubscriptions();
					_radioController.SelectItem(menuItemSubs.GetComponent<RadioButton>());
				});
				menuItemSubs.transform.SetParent(menuTransform);	
				_radioController.AddButton(menuItemSubs.GetComponent<RadioButton>());

			}

			if (components.ContainsKey("coupons") && components["coupons"].IsEnabled)
			{
				GameObject menuItemCoupons = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemCoupons.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = utils.GetTranslations().Get(XsollaTranslations.COUPON_PAGE_TITLE);
				menuItemCoupons.GetComponent<Button>().onClick.AddListener(delegate {
					_radioController.SelectItem(menuItemCoupons.GetComponent<RadioButton>());
					ShowRedeemCoupon();
				});
				menuItemCoupons.transform.SetParent(menuTransform);	
				_radioController.AddButton(menuItemCoupons.GetComponent<RadioButton>());
			}

			GameObject menuItemEmpty = Instantiate (menuItemEmptyPrefab);
			menuItemEmpty.transform.SetParent (menuTransform);

			
			GameObject menuItemFavorite = Instantiate (menuItemIconPrefab);
			menuItemFavorite.GetComponentInChildren<Text> ().text = "";
			menuItemFavorite.GetComponent<Button>().onClick.AddListener(delegate {
				_shopViewController.SetTitle(utils.GetTranslations().Get(XsollaTranslations.VIRTUALITEMS_TITLE_FAVORITE));
				_radioController.SelectItem(menuItemFavorite.GetComponent<RadioButton>());
				LoadFavorites();
			});
			menuItemFavorite.transform.SetParent (menuTransform);
			_radioController.AddButton(menuItemFavorite.GetComponent<RadioButton>());

		}

		private void InitFooter(XsollaUtils utils)
		{
			if (utils != null) {
				Text[] texts = mainScreen.GetComponentsInChildren<Text> ();
				XsollaTranslations translatrions = utils.GetTranslations ();
				texts [2].text = translatrions.Get (XsollaTranslations.SUPPORT_CUSTOMER_SUPPORT);
				texts [3].text = translatrions.Get (XsollaTranslations.SUPPORT_CONTACT_US);
				texts [5].text = translatrions.Get (XsollaTranslations.FOOTER_SECURED_CONNECTION);
				texts [6].text = translatrions.Get (XsollaTranslations.FOOTER_AGREEMENT);
			}
		}

		private void OnUserStatusExit(XsollaStatus.Group group, string invoice, Xsolla.XsollaStatusData.Status status, Dictionary<string, object> pPurchase = null)
		{
			Logger.Log ("On user exit status screen");
			switch (group){
				case XsollaStatus.Group.DONE:
					Logger.Log ("Status Done");
					menuTransform.gameObject.SetActive (true);
					if (result == null)
						result = new XsollaResult();
					result.invoice = invoice;
					result.status = status;
					if (pPurchase != null)
						result.purchases = pPurchase;
					Logger.Log("Ivoice ID " + result.invoice);
					Logger.Log("Status " + result.status);
					Logger.Log("Bought", result.purchases);
					TransactionHelper.Clear ();
				
					if (OkHandler != null)
						OkHandler (result);
					else 
						Logger.Log ("Have no OkHandler");
					break;
				case XsollaStatus.Group.TROUBLED:
				case XsollaStatus.Group.INVOICE:
				case XsollaStatus.Group.UNKNOWN:
				default:
					result.invoice = invoice;
					result.status = status;
					Logger.Log("Ivoice ID " + result.invoice);
					Logger.Log("Status " + result.status);
					Logger.Log("Bought", result.purchases);
					TransactionHelper.Clear ();
					if (OkHandler != null)
						OkHandler (result);
					else 
						Logger.Log ("Have no OkHandler");
					break;
			}
		}

		private void TryAgain(){
			SetLoading (true);
			menuTransform.gameObject.SetActive (true);
			Restart ();
		}

		private void OnErrorRecivied(XsollaError xsollaError)
		{
			Logger.Log("ErrorRecivied " + xsollaError.ToString());
			if (ErrorHandler != null)
				ErrorHandler (xsollaError);
			else 
				Logger.Log ("Have no ErrorHandler");
		}

		void OnDestroy(){
			Logger.Log ("User close window");
			switch (currentActive) 
			{
				case ActiveScreen.STATUS:
					Logger.Log ("Check payment status");
					StatusViewController controller = GetComponentInChildren<StatusViewController>();
					if(controller != null)
						controller.statusViewExitButton.onClick.Invoke();
					break;
				default:
					Logger.Log ("Handle chancel");
					ErrorHandler (XsollaError.GetCancelError());
					break;
			}
		}
	}
}
