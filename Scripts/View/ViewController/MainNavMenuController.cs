using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Xsolla
{
	public class MainNavMenuController: MonoBehaviour
	{
		public GameObject mNavMenuPanel;

		private const string PREFAB_VIEW_MENU_ITEM		 = "Prefabs/SimpleView/MenuItem";
		private const string PREFAB_VIEW_MENU_ITEM_ICON	 = "Prefabs/SimpleView/MenuItemIcon";
		private const string PREFAB_VIEW_MENU_ITEM_EMPTY = "Prefabs/SimpleView/MenuItemEmpty";
		private RadioGroupController mRadioGroupController;
		private Action<RadioButton.RadioType> mActionNavClick;

		public void Init(XsollaUtils pUtils, Action<RadioButton.RadioType> pActionNavClick)
		{
			// Инициализация навигационного меню
			mActionNavClick = pActionNavClick;
			if(pUtils.GetPurchase() == null || !pUtils.GetPurchase().IsPurchase())
			{
				InitMenu(pUtils);
			}

			// Выбираем первый элемент из всего меню
			mRadioGroupController.radioButtons[0].Select();
		}

		private void InitMenu(XsollaUtils pUtils)
		{
			mRadioGroupController = mNavMenuPanel.gameObject.AddComponent<RadioGroupController> ();
			GameObject menuItemPrefab 		= Resources.Load (PREFAB_VIEW_MENU_ITEM) as GameObject;
			GameObject menuItemIconPrefab 	= Resources.Load (PREFAB_VIEW_MENU_ITEM_ICON) as GameObject;
			GameObject menuItemEmptyPrefab 	= Resources.Load (PREFAB_VIEW_MENU_ITEM_EMPTY) as GameObject;
			Dictionary<string, XComponent> components = pUtils.GetProject().components;
			if(components.ContainsKey("items") && components ["items"].IsEnabled)
			{
				GameObject menuItemGoods = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemGoods.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = (components ["items"].Name != "null") ? components ["items"].Name : pUtils.GetTranslations().Get(XsollaTranslations.VIRTUALITEM_PAGE_TITLE);
				menuItemGoods.GetComponent<RadioButton>().setType(RadioButton.RadioType.SCREEN_GOODS);
				menuItemGoods.GetComponent<Button>().onClick.AddListener(delegate 
					{
						mRadioGroupController.SelectItem(RadioButton.RadioType.SCREEN_GOODS);
						mActionNavClick(RadioButton.RadioType.SCREEN_GOODS);
					});
				menuItemGoods.transform.SetParent(mNavMenuPanel.transform);
				mRadioGroupController.AddButton(menuItemGoods.GetComponent<RadioButton>());
			}

			if (components.ContainsKey("virtual_currency") && components ["virtual_currency"].IsEnabled)
			{
				GameObject menuItemPricepoints = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemPricepoints.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = (components ["virtual_currency"].Name != "null") ? components["virtual_currency"].Name : pUtils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_TITLE); 
				menuItemPricepoints.GetComponent<RadioButton>().setType(RadioButton.RadioType.SCREEN_PRICEPOINT);
				menuItemPricepoints.GetComponent<Button>().onClick.AddListener(delegate 
					{
						mRadioGroupController.SelectItem(RadioButton.RadioType.SCREEN_PRICEPOINT);
						mActionNavClick(RadioButton.RadioType.SCREEN_PRICEPOINT);
					});
				menuItemPricepoints.transform.SetParent(mNavMenuPanel.transform);	
				mRadioGroupController.AddButton(menuItemPricepoints.GetComponent<RadioButton>());
			} 

			if (components.ContainsKey("subscriptions") && components["subscriptions"].IsEnabled)
			{
				GameObject menuItemSubs = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemSubs.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = (components["subscriptions"].Name != "null") ? components["subscriptions"].Name : pUtils.GetTranslations().Get(XsollaTranslations.SUBSCRIPTION_PAGE_TITLE);  
				menuItemSubs.GetComponent<RadioButton>().setType(RadioButton.RadioType.SCREEN_SUBSCRIPTION);
				menuItemSubs.GetComponent<Button>().onClick.AddListener(delegate 
					{
						mRadioGroupController.SelectItem(RadioButton.RadioType.SCREEN_SUBSCRIPTION);
						mActionNavClick(RadioButton.RadioType.SCREEN_SUBSCRIPTION);
						//LoadSubscriptions();
					});
				menuItemSubs.transform.SetParent(mNavMenuPanel.transform);	
				mRadioGroupController.AddButton(menuItemSubs.GetComponent<RadioButton>());

			}

			if (components.ContainsKey("coupons") && components["coupons"].IsEnabled)
			{
				GameObject menuItemCoupons = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemCoupons.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = (components["coupons"].Name != "null") ? components["coupons"].Name : pUtils.GetTranslations().Get(XsollaTranslations.COUPON_PAGE_TITLE); 
				menuItemCoupons.GetComponent<RadioButton>().setType(RadioButton.RadioType.SCREEN_REDEEMCOUPON);
				menuItemCoupons.GetComponent<Button>().onClick.AddListener(delegate 
					{
						mRadioGroupController.SelectItem(RadioButton.RadioType.SCREEN_REDEEMCOUPON);
						mActionNavClick(RadioButton.RadioType.SCREEN_REDEEMCOUPON);
					});
				menuItemCoupons.transform.SetParent(mNavMenuPanel.transform);	
				mRadioGroupController.AddButton(menuItemCoupons.GetComponent<RadioButton>());
			}

			GameObject menuItemEmpty = Instantiate (menuItemEmptyPrefab);
			menuItemEmpty.transform.SetParent (mNavMenuPanel.transform);

			GameObject menuItemFavorite = Instantiate (menuItemIconPrefab);
			menuItemFavorite.GetComponentInChildren<Text> ().text = "";
			menuItemFavorite.GetComponent<RadioButton>().setType(RadioButton.RadioType.SCREEN_FAVOURITE);
			menuItemFavorite.GetComponent<Button>().onClick.AddListener(delegate 
				{
					mRadioGroupController.SelectItem(RadioButton.RadioType.SCREEN_FAVOURITE);
					mActionNavClick(RadioButton.RadioType.SCREEN_FAVOURITE);
				});
			menuItemFavorite.transform.SetParent (mNavMenuPanel.transform);
			mRadioGroupController.AddButton(menuItemFavorite.GetComponent<RadioButton>());

		}

		public void SetVisibleBtn(bool pVisible, RadioButton.RadioType pType)
		{
			mRadioGroupController.radioButtons.Find(x => x.getType() == pType).visibleBtn(pVisible);
		}

		public void SelectRadioItem(RadioButton.RadioType pType)
		{
			if (mRadioGroupController != null)
				mRadioGroupController.SelectItem(pType);
		}
	}
}

