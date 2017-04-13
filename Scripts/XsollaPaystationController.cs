
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;

namespace Xsolla
{
	public class XsollaPaystationController : XsollaPaystation {

		private const string PREFAB_SCREEN_STATUS 		 = "Prefabs/SimpleView/_ScreenStatus/ScreenStatusNew";
		private const string PREFAB_SCREEN_ERROR 		 = "Prefabs/Screens/ScreenError";
		private const string PREFAB_SCREEN_ERROR_MAIN 	 = "Prefabs/Screens/MainScreenError";
		private const string PREFAB_SCREEN_CHECKOUT 	 = "Prefabs/SimpleView/_ScreenCheckout/ScreenCheckout";
		private const string PREFAB_SCREEN_VP_SUMMARY 	 = "Prefabs/SimpleView/_ScreenVirtualPaymentSummary/ScreenVirtualPaymentSummary";
		private const string PREFAB_SCREEN_REDEEM_COUPON = "Prefabs/SimpleView/_ScreenShop/RedeemCouponView";
		private const string PREFAB_SCREEN_HISTORY_USER  = "Prefabs/SimpleView/_ScreenShop/HistoryView";
		private const string PREFAB_SCREEN_SUBSCRIPTIONS = "Prefabs/SimpleView/_ScreenShop/SubscriptionsView";

		private const string PREFAB_SCREEN_GOODS_SHOP = "Prefabs/SimpleView/_ScreenShop/GoodsViewRe";
		private const string PREFAB_SCREEN_PRICEPOINT_SHOP = "Prefabs/SimpleView/_ScreenShop/PricePointViewRe";


		private const string PREFAB_SCREEN_PAYMENT_MANAGER = "Prefabs/Screens/ScreenPaymentManager";
		private const string PREFAB_SCREEN_SUBSCRIPTION_MANAGER = "Prefabs/Screens/SubsManager/ScreenSubsManager";

		public event Action<XsollaResult> 	OkHandler;
		public event Action<XsollaError> 	ErrorHandler;

		public GameObject 					mainScreen;
		public MyRotation 					progressBar;
		private bool 						isMainScreenShowed = false;

		public GameObject 					shopScreenPrefab;
		public GameObject 					paymentListScreenPrefab;
		public GameObject 					container;
		
		private MainNavMenuController       mNavMenuController;
		private MainFooterController 		mFooterController;
		private PaymentListScreenController _paymentListScreenController;
		private ShopViewController 			_shopViewController;
		private RedeemCouponViewController  _couponController;
		private SubscriptionsViewController _subsController;

		private PaymentManagerController 	_SavedPaymentController;
		private SubsManagerController 		_SubsManagerController;

		private MainScreenController 		mMainScreenController;

		private static ActiveScreen 		currentActive = ActiveScreen.UNKNOWN;
		private GameObject 					mainScreenContainer;

		public enum ActiveScreen
		{
			SHOP, P_LIST, VP_PAYMENT, PAYMENT, STATUS, ERROR, UNKNOWN, FAV_ITEMS_LIST, REDEEM_COUPONS, HISTORY_LIST, SUBSCRIPTIONS, PAYMENT_MANAGER
		}
			
		protected override void RecieveUtils (XsollaUtils utils)
		{
			StyleManager.Instance.ChangeTheme(utils.GetSettings().GetTheme());
			mainScreen = Instantiate (mainScreen);
			mainScreen.transform.SetParent (container.transform);
			mainScreen.SetActive (true);
			mMainScreenController = mainScreen.GetComponent<MainScreenController>();
			mainScreenContainer = mMainScreenController.mMainContainer;
			Resizer.ResizeToParrent (mainScreen);
			//base.RecieveUtils(utils);
			base.Utils = utils;
			InitHeader(utils);
			InitFooter(utils);
			InitNavMenu(utils);

			// Выделяем первый элемент
			mNavMenuController.onNavMenuItemClick(0);
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
				GameObject paymentListScreen = Instantiate (paymentListScreenPrefab);
				_paymentListScreenController = paymentListScreen.GetComponent<PaymentListScreenController> ();
				_paymentListScreenController.transform.SetParent (mainScreenContainer.transform);
				Resizer.SetDefScale(paymentListScreen);
				_paymentListScreenController.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				mainScreenContainer.GetComponentInParent<ScrollRect> ().content = _paymentListScreenController.GetComponent<RectTransform> ();
			}
		}

		protected override void ShowQuickPaymentsList (XsollaUtils utils, XsollaQuickPayments quickPayments)
		{
		}

		protected override void ShowPaymentsList (XsollaPaymentMethods paymentMethods)
		{
			DrawPaymentListScreen ();
			_paymentListScreenController.InitScreen(base.Utils);
			_paymentListScreenController.SetPaymentsMethods (paymentMethods);
			_paymentListScreenController.OpenPayments();

			SetLoading (false);
			return;
		}

		protected override void ShowSavedPaymentsList (XsollaSavedPaymentMethods savedPaymentsMethods)
		{
			DrawPaymentListScreen ();
			_paymentListScreenController.SetSavedPaymentsMethods(savedPaymentsMethods);
		}

		protected override void ShowCountries (XsollaCountries countries)
		{
			DrawPaymentListScreen ();
			_paymentListScreenController.SetCountries (_countryCurr, countries, Utils);
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
			
		protected override void ApplyPromoCouponeCode (XsollaForm pForm)
		{
			Logger.Log("Apply promo recieved");
			PromoCodeController promoController = mainScreenContainer.GetComponentInChildren<PromoCodeController>();
			if (pForm.GetError() != null)
			{
				if (pForm.GetError().elementName == XsollaApiConst.COUPON_CODE)
				{
					promoController.SetError(pForm.GetError());
					return;
				}
				return;
			}

			RightTowerController controller = mainScreenContainer.GetComponentInChildren<RightTowerController>();
			// update rigth tower info, if we get rigth tower controller
			if (controller != null)
				controller.UpdateDiscont(Utils.GetTranslations(),pForm.GetSummary());

			// update total amount on payment form total
			PaymentFormController paymentController = mainScreenContainer.GetComponentInChildren<PaymentFormController>();
			if (paymentController != null)
			{
				Text[] footerTexts = paymentController.layout.objects[paymentController.layout.objects.Count - 1].gameObject.GetComponentsInChildren<Text> ();
				footerTexts[1].text = Utils.GetTranslations().Get(XsollaTranslations.TOTAL) + " " + pForm.GetSumTotal ();
			}
			promoController.ApplySuccessful();
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

		protected override void UpdateCustomAmount (CustomVirtCurrAmountController.CustomAmountCalcRes pRes)
		{
			//			 find custom amount controller 
//			CustomVirtCurrAmountController controller = FindObjectOfType<CustomVirtCurrAmountController>();
//			if (controller != null)
//				//controller.setValues(pRes);
//			else
//				Logger.Log("Custom amount controller not found");	
		}

		protected override void PaymentManagerRecieved (XsollaSavedPaymentMethods pResult, bool pAddState)
		{
			if (_SavedPaymentController == null)
			{
				Resizer.DestroyChilds(mainScreenContainer.transform);
				GameObject paymentManager = Instantiate(Resources.Load(PREFAB_SCREEN_PAYMENT_MANAGER)) as GameObject;
				_SavedPaymentController = paymentManager.GetComponent<PaymentManagerController>();
				_SavedPaymentController.setOnCloseMethod(() => 
					{
						//ShowGoodsShop();
						//LoadGoodsGroups();
						NavMenuClick(RadioButton.RadioType.SCREEN_GOODS);
					});
				paymentManager.transform.SetParent (mainScreenContainer.transform);
				paymentManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
				Resizer.ResizeToParrent (paymentManager);
				currentActive = ActiveScreen.PAYMENT_MANAGER;
			}
		
			SetLoading(false);
			// Remove purchase part 
			Restart();
			_SavedPaymentController.initScreen(Utils, pResult, AddPaymentAccount, pAddState);
			
		}

		protected override void DeleteSavedPaymentMethodRecieved()
		{
			// Send message on delete to form
			if ((currentActive == ActiveScreen.PAYMENT_MANAGER) && (_SavedPaymentController != null))
			{
				_SavedPaymentController.SetStatusDeleteOk();
				// Reload savedMethods
				LoadPaymentManager();
			}
		}

		protected override void WaitChangeSavedMethod ()
		{
			if (_SavedPaymentController == null)
			{
				GameObject paymentManager = Instantiate(Resources.Load(PREFAB_SCREEN_PAYMENT_MANAGER)) as GameObject;
				_SavedPaymentController = paymentManager.GetComponent<PaymentManagerController>();
				_SavedPaymentController.setOnCloseMethod(() => 
					{
						//LoadGoodsGroups();
						NavMenuClick(RadioButton.RadioType.SCREEN_GOODS);
					});
				paymentManager.transform.SetParent (mainScreenContainer.transform);
				paymentManager.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
				Resizer.ResizeToParrent (paymentManager);
				currentActive = ActiveScreen.PAYMENT_MANAGER;
			}

			SetLoading(false);
			// Remove purchase part 
			Restart();
			_SavedPaymentController.initWaitScreen(Utils, AddPaymentAccount);
		}

		protected override void SubsManagerListRecived (XsollaManagerSubscriptions pSubsList)
		{
			if (_SubsManagerController == null)
			{
				GameObject obj = Instantiate(Resources.Load(PREFAB_SCREEN_SUBSCRIPTION_MANAGER)) as GameObject;
				_SubsManagerController = obj.GetComponent<SubsManagerController>();
				_SubsManagerController.initScreen(Utils, pSubsList);

				obj.transform.SetParent(mainScreenContainer.transform);
				obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
				Resizer.ResizeToParrent(obj);
			}
			else
			{
				_SubsManagerController.initScreen(Utils, pSubsList);
			}
			SetLoading(false);
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

			if (utils.GetSettings().components.virtualCurreny.customAmount)
				_shopViewController.OpenPricepoints(title, pricepoints, vcName, buyText, true, utils);
			else
				_shopViewController.OpenPricepoints(title, pricepoints, vcName, buyText);
		}
		
		public void OpenGoods(XsollaGroupsManager groups)
		{
			DrawShopScreen ();
			LoadGoods (groups.GetItemByPosition(0).id);
			_shopViewController.OpenGoods(groups);
		}

		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< SHOP <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

		private void DrawStatus(XsollaTranslations translations, XsollaStatus status)
		{
			currentActive = ActiveScreen.STATUS;
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_STATUS)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			StatusViewController controller = statusScreen.GetComponent<StatusViewController> ();
			controller.StatusHandler += OnUserStatusExit;
			controller.InitScreen(translations, status);
		}

		/// <summary>
		/// Shows the redeem coupon.
		/// </summary>
		public void ShowRedeemCoupon()
		{
			currentActive = ActiveScreen.REDEEM_COUPONS;
			GameObject screenRedeemCoupons = Instantiate(Resources.Load(PREFAB_SCREEN_REDEEM_COUPON)) as GameObject;
			Resizer.DestroyChilds(mainScreenContainer.transform);
			Resizer.SetParentToFullScreen(screenRedeemCoupons, mainScreenContainer);

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

		private void AddPaymentAccount()
		{
			Logger.Log("Click addAccount");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("save_payment_account_only",1);
			FillPurchase(ActivePurchase.Part.PAYMENT_MANAGER, reqParams);
			// load payment methods
			LoadQuickPayment();
		}

		private void setCurrentScreenValue(ActiveScreen pValue)
		{
			currentActive = pValue;
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
				texts[1].text = "Something went wrong";
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
			Resizer.SetDefScale(checkoutScreen);
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
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_VP_SUMMARY)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			Resizer.SetDefScale(statusScreen);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			ScreenVPController screenVpController = statusScreen.GetComponent<ScreenVPController> ();
			screenVpController.DrawScreen(utils, summary);
		}

		private void DrawVPError(XsollaUtils utils, string error) {
			currentActive = ActiveScreen.VP_PAYMENT;
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_VP_SUMMARY)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			Resizer.SetDefScale(statusScreen);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			ScreenVPController screenVpController = statusScreen.GetComponent<ScreenVPController> ();
			screenVpController.DrawScreen(utils, _summary);
			screenVpController.ShowError (error);
		}
					
		private void DrawVPStatus (XsollaUtils utils, XVPStatus status) {
			currentActive = ActiveScreen.STATUS;
			GameObject statusScreen = Instantiate (Resources.Load(PREFAB_SCREEN_STATUS)) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			Resizer.SetDefScale(statusScreen);
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
			
		// Новая инициализация
		/// <summary>
		/// Inits the header.
		/// </summary>
		/// <param name="utils">Utils.</param>
		private void InitHeader(XsollaUtils utils)
		{
			MainHeaderController controller = mainScreen.GetComponentInChildren<MainHeaderController>();
			controller.InitScreen(utils);
		}
			
		/// <summary>
		/// Inits the nav menu.
		/// </summary>
		/// <param name="pUtils">Utils.</param>
		private void InitNavMenu(XsollaUtils pUtils)
		{
			mNavMenuController = mainScreen.GetComponentInChildren<MainNavMenuController>();
			mNavMenuController.Init(pUtils, NavMenuClick); 
		}

		/// <summary>
		/// Inits the footer.
		/// </summary>
		/// <param name="pUtils">Utils.</param>
		private void InitFooter(XsollaUtils pUtils)
		{
			mFooterController = mainScreen.GetComponentInChildren<MainFooterController>();
			mFooterController.Init(pUtils);
		}

		/// <summary>
		/// Shows the goods shop.
		/// </summary>
		private void ShowGoodsShop()
		{
			GameObject goodsShop = Instantiate(Resources.Load(PREFAB_SCREEN_GOODS_SHOP)) as GameObject;
			Resizer.DestroyChilds(mainScreenContainer.transform);
			ShopViewControllerRe controller = goodsShop.GetComponent<ShopViewControllerRe>();
			controller.init(Utils);

			// задаем родителя и заполняем 
			Resizer.SetParentToFullScreen(goodsShop, mainScreenContainer);
			// Выделяем элемент меню
			mNavMenuController.SelectRadioItem(RadioButton.RadioType.SCREEN_GOODS);
		}

		/// <summary>
		/// Shows the fav goods shop.
		/// </summary>
		private void ShowFavGoodsShop()
		{
			ShopViewControllerRe shopController = GameObject.FindObjectOfType<ShopViewControllerRe>();
			if (shopController != null)
				shopController.ShowFavItems();
		}

		/// <summary>
		/// Shows the price point shop.
		/// </summary>
		private void ShowPricePointShop()
		{
			GameObject pricePointShop = Instantiate(Resources.Load(PREFAB_SCREEN_PRICEPOINT_SHOP)) as GameObject;
			Resizer.DestroyChilds(mainScreenContainer.transform);
			PricePointShopControllerRe controller = pricePointShop.GetComponent<PricePointShopControllerRe>();
			controller.SetProgressBarAction(delegate {progressBar.SetLoading(true);}, delegate {progressBar.SetLoading(false);});
			controller.init(Utils);

			// задаем родителя и заполняем 
			Resizer.SetParentToFullScreen(pricePointShop, mainScreenContainer);
			// Выделяем элемент меню
			mNavMenuController.SelectRadioItem(RadioButton.RadioType.SCREEN_PRICEPOINT);
		}

		/// <summary>
		/// Draws the subscriptions.
		/// </summary>
		private void DrawSubscriptions()
		{
			GameObject screenSubs = Instantiate(Resources.Load(PREFAB_SCREEN_SUBSCRIPTIONS)) as GameObject;
			Resizer.DestroyChilds(mainScreenContainer.transform);
			SubscriptionsViewController controller = screenSubs.GetComponent<SubscriptionsViewController>();
			controller.init(Utils);
//			_subsController = screenSubs.GetComponent<SubscriptionsViewController>();
//			_subsController.init(Utils);

			// задаем родителя и заполняем 
			Resizer.SetParentToFullScreen(screenSubs, mainScreenContainer);
			// Выделяем элемент меню
			mNavMenuController.SelectRadioItem(RadioButton.RadioType.SCREEN_SUBSCRIPTION);
		}

		/// <summary>
		/// Shows the history.
		/// </summary>
		private void ShowHistory()
		{
			GameObject historyScreen = Instantiate(Resources.Load(PREFAB_SCREEN_HISTORY_USER)) as GameObject;
			HistoryController controller = historyScreen.GetComponent<HistoryController>();
			controller.Init(Utils);

			// задаем родителя и заполняем 
			Resizer.SetParentToFullScreen(historyScreen, mainScreenContainer);
		}

		/// <summary>
		/// Click on nav menu items by type.
		/// </summary>
		/// <param name="pType">Type menu item.</param>
		public void NavMenuClick(RadioButton.RadioType pType)
		{
			switch (pType) {
			case RadioButton.RadioType.SCREEN_GOODS:
				{
					ShowGoodsShop();
					mNavMenuController.SetVisibleBtn(true, RadioButton.RadioType.SCREEN_FAVOURITE);
					break;
				}
			case RadioButton.RadioType.SCREEN_PRICEPOINT:
				{
					ShowPricePointShop();
					mNavMenuController.SetVisibleBtn(false, RadioButton.RadioType.SCREEN_FAVOURITE);
					break;
				}
			case RadioButton.RadioType.SCREEN_SUBSCRIPTION:
				{
					//LoadSubscriptions();
					DrawSubscriptions();
					mNavMenuController.SetVisibleBtn(false, RadioButton.RadioType.SCREEN_FAVOURITE);
					break;
				}
			case RadioButton.RadioType.SCREEN_REDEEMCOUPON:
				{
					ShowRedeemCoupon();
					mNavMenuController.SetVisibleBtn(false, RadioButton.RadioType.SCREEN_FAVOURITE);
					break;
				}
			case RadioButton.RadioType.SCREEN_FAVOURITE:
				{
					ShowFavGoodsShop();
					break;
				}
			case RadioButton.RadioType.SCREEN_HISTORY:
				{
					ShowHistory();
					break;
				}
			default:
				break;
			}
		}

		private void TryAgain(){
			SetLoading (true);
			Restart ();
		}

		private void OnUserStatusExit(XsollaStatus.Group group, string invoice, Xsolla.XsollaStatusData.Status status, Dictionary<string, object> pPurchase = null)
		{
			Logger.Log ("On user exit status screen");
			switch (group){
				case XsollaStatus.Group.DONE:
					Logger.Log ("Status Done");
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
				{
					Logger.Log ("Handle chancel");
					if (ErrorHandler != null) 
						ErrorHandler (XsollaError.GetCancelError());
					break;
				}
			}
		}
	}
}
