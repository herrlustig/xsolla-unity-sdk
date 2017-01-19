using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Xsolla 
{
	public class ShopViewController : MonoBehaviour {

		public ScrollableListCustom menu;
		public RadioGroupController radioGroup;
		public Text 				title;
		public GridView 			content;

		public PricePointsAdapter 	pAdapter;
		public SubscriptionsAdapter sAdapter;
		public GoodsAdapter 		gAdapter;

		public GameObject 			CustomAmountLink;
		public GameObject 			CustomAmountScreen;
		public GameObject			ShopPanel;


		public void OpenPricepoints(string title, XsollaPricepointsManager pricepoints, string virtualCurrencyName, string buyBtnText, bool pCustomHref = false, XsollaUtils pUtils = null)
		{
			Resizer.ResizeToParrent (gameObject);
			menu.transform.parent.parent.gameObject.SetActive (false);
			SetTitle (title);
			// if we have custom amount we need show link object 
			if (pCustomHref)
			{
				CustomAmountLink.SetActive(true);
				string customAmountShowTitle = pUtils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_CUSTOM_AMOUNT_SHOW_TITLE);
				string customAmountHideTitle = pUtils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_CUSTOM_AMOUNT_HIDE_TITLE);

				Text titleCustomAmount = CustomAmountLink.GetComponent<Text>();
				titleCustomAmount.text = customAmountShowTitle;

				Toggle toggle = CustomAmountLink.GetComponent<Toggle>();
				toggle.onValueChanged.AddListener((value) =>   
					{
						if (value)
						{
							titleCustomAmount.text = customAmountHideTitle;
						}
						else
							titleCustomAmount.text = customAmountShowTitle;

						CustomAmountScreen.SetActive(value);
						ShopPanel.SetActive(!value);

						Logger.Log("Change value toggle " + value.ToString());
					});
						
				CustomVirtCurrAmountController controller = CustomAmountScreen.GetComponent<CustomVirtCurrAmountController>() as CustomVirtCurrAmountController;
				controller.initScreen(pUtils, pricepoints.GetItemByPosition(1).currency, CalcCustomAmount, TryPayCustomAmount);
			}
			else
			{
				CustomAmountLink.SetActive(false);
			}

			pAdapter.SetManager (pricepoints, virtualCurrencyName, buyBtnText);
			if (pAdapter.OnBuyPricepoints == null) {
				pAdapter.OnBuyPricepoints += (outAmount) => {
					Dictionary<string, object> map = new Dictionary<string, object> (1);
					map.Add ("out", outAmount);
					OpenPaymentMethods (map, false);
				};
			}
			DrawContent (pAdapter, 3);
		}

		public void TryPayCustomAmount(float pOutAmount)
		{
			Dictionary<string, object> map = new Dictionary<string, object> (1);
			map.Add ("out", pOutAmount);
			OpenPaymentMethods (map, false);
		}

		public void CalcCustomAmount(Dictionary<string, object> pParam)
		{
			gameObject.GetComponentInParent<XsollaPaystationController> ().CalcCustomAmount(pParam);
		}
		
		public void OpenSubscriptions(string title, XsollaSubscriptions subscriptions)
		{
			Resizer.ResizeToParrent (gameObject);
			menu.transform.parent.parent.gameObject.SetActive (false);
			SetTitle (title);
			sAdapter.SetManager (subscriptions);
			if (sAdapter.OnBuySubscription == null) {
				sAdapter.OnBuySubscription += (subscriptionId) => {
					Dictionary<string, object> map = new Dictionary<string, object> (1);
					map.Add ("id_package", subscriptionId);
					OpenPaymentMethods (map, false);
				};
			}
			DrawContent (sAdapter, 1);
		}
		
		public void OpenGoods(XsollaGroupsManager groups)
		{
			Resizer.ResizeToParrent (gameObject);
			menu.transform.parent.parent.gameObject.SetActive (true);
			SetTitle (groups.GetItemByPosition(0).name);
			menu.SetData ((groupId) => {
				XsollaGoodsGroup group = groups.GetItemByKey(groupId);
				radioGroup.SelectItem (groups.GetItemsList().IndexOf(group));
				ChooseItemsGroup(group.id, group.name);
			}, groups.GetNamesDict ());
			radioGroup.SetButtons(menu.GetItems ());
			radioGroup.SelectItem (0);
		}

		public void UpdateGoods(XsollaGoodsManager goods, string buyBtnText)
		{
			gAdapter.SetManager (goods, buyBtnText);
			if (gAdapter.OnBuy == null) {
				gAdapter.OnBuy += (sku, isVirtualPayment) => {
					Dictionary<string, object> map = new Dictionary<string, object> (1);
					map.Add (sku, 1);
					OpenPaymentMethods (map, isVirtualPayment);
				};
			}
			if (gAdapter.OnFavorite == null) {
				gAdapter.OnFavorite += (isFavorite, sku, id) => {
					Dictionary<string, object> map = new Dictionary<string, object> (1);
					map.Add ("is_favorite", isFavorite ? 1 : 0);
					map.Add (sku, 1);
					map.Add ("virtual_item_id", id);
					SetFavorite (map);
				};
			}
			DrawContent (gAdapter, 3);
		}

		public void SetTitle(string s)
		{
			if (title.gameObject != null)
				title.text = s;
		}

		private void DrawContent(IBaseAdapter adapter, int columnsCount){
			content.SetAdapter (adapter, columnsCount);
		}

		private void ChooseItemsGroup(long groupId, string groupName)
		{
			SetTitle (groupName);
			gameObject.GetComponentInParent<XsollaPaystationController> ().LoadGoods (groupId);	
		}

		private void OpenVirtualPayment(Dictionary<string, object> purchase)
		{
			purchase.Add ("is_virtual_payment", 1);
			gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (purchase);
		}

		private void OpenPaymentMethods(Dictionary<string, object> purchase, bool isVirtualPayment)
		{
//			if(isVirtualPayment)
//				purchase.Add ("is_virtual_payment", 1);
			gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (purchase, isVirtualPayment);
		}

		private void SetFavorite(Dictionary<string, object> purchase)
		{
			gameObject.GetComponentInParent<XsollaPaystationController> ().SetFavorite (purchase);
		}

	}
}