using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Xsolla
{
	public class MainNavMenuController: MonoBehaviour
	{
		public GameObject mNavMenuPanel;
		public ToggleGroup mGroup;

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
		}

		private void InitMenu(XsollaUtils pUtils)
		{
			mRadioGroupController = mNavMenuPanel.gameObject.AddComponent<RadioGroupController> ();
			GameObject menuItemEmptyPrefab 	= Resources.Load (PREFAB_VIEW_MENU_ITEM_EMPTY) as GameObject;
			Dictionary<string, XComponent> components = pUtils.GetProject().components;
			if(components.ContainsKey("items") && components ["items"].IsEnabled)
			{
				string lName = (components ["items"].Name != "null") ? components ["items"].Name : pUtils.GetTranslations().Get(XsollaTranslations.VIRTUALITEM_PAGE_TITLE);
				addMenuBtn("", lName, RadioButton.RadioType.SCREEN_GOODS);
			}

			if (components.ContainsKey("virtual_currency") && components ["virtual_currency"].IsEnabled)
			{
				string lName = (components ["virtual_currency"].Name != "null") ? components["virtual_currency"].Name : pUtils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_TITLE); 
				addMenuBtn("", lName, RadioButton.RadioType.SCREEN_PRICEPOINT);
			} 

			if (components.ContainsKey("subscriptions") && components["subscriptions"].IsEnabled)
			{
				string lName = (components["subscriptions"].Name != "null") ? components["subscriptions"].Name : pUtils.GetTranslations().Get("state_name_subscription");  
				addMenuBtn("", lName, RadioButton.RadioType.SCREEN_SUBSCRIPTION);
			}

			if (components.ContainsKey("coupons") && components["coupons"].IsEnabled)
			{
				string lName = (components["coupons"].Name != "null") ? components["coupons"].Name : pUtils.GetTranslations().Get(XsollaTranslations.COUPON_PAGE_TITLE); 
				addMenuBtn("", lName, RadioButton.RadioType.SCREEN_REDEEMCOUPON);
			}

			GameObject menuItemEmpty = Instantiate (menuItemEmptyPrefab);
			menuItemEmpty.transform.SetParent (mNavMenuPanel.transform);

			addMenuBtn("", "", RadioButton.RadioType.SCREEN_FAVOURITE);
		}

		private void addMenuBtn(string pIcon, string pName, RadioButton.RadioType pType)
		{
			GameObject menuItemPrefab = Instantiate(Resources.Load (PREFAB_VIEW_MENU_ITEM)) as GameObject;
			RadioButton controller = menuItemPrefab.GetComponent<RadioButton>();
			controller.init(pIcon, pName, pType, delegate { onNavMenuItemClick(pType); });

			menuItemPrefab.transform.SetParent(mNavMenuPanel.transform);
			mRadioGroupController.AddButton(menuItemPrefab.GetComponent<RadioButton>());
		}

		public void onNavMenuItemClick(RadioButton.RadioType pType)
		{
			mRadioGroupController.UnselectAll();
			mActionNavClick(pType);
		}

		public void onNavMenuItemClick(int pIdx)
		{
			mRadioGroupController.radioButtons[pIdx].mBtn.onClick.Invoke();
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

		public void SelectRadioItem(int pIdx)
		{
			if (mRadioGroupController != null)
				mRadioGroupController.SelectItem(pIdx);
		}
	}
}

