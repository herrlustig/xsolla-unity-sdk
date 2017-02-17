using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public class MainHeaderController: MonoBehaviour
	{
		public Text _titleProj;
		public GameObject _btnDropDownObj;
		public Text _userName;
		public Button _pMenuBtnComponent;
		private const string PREFAB_VIEW_MENU_ITEM_EMPTY = "Prefabs/SimpleView/ProfileBtn";

		public void InitScreen(XsollaUtils pUtils)
		{
			// set title 
			_titleProj.text = pUtils.GetProject().name;

			// user name 
			_userName.text = pUtils.GetUser().GetName();
		
			if (pUtils.GetUser().virtualCurrencyBalance != null)
			{
				GameObject obj = Instantiate(Resources.Load(PREFAB_VIEW_MENU_ITEM_EMPTY)) as GameObject;
				UserProfileBtnController controller = obj.GetComponentInChildren<UserProfileBtnController>();
				controller.InitScreen(pUtils.GetTranslations().Get("user_menu_balance"), ShowHistory);
				obj.transform.SetParent(_btnDropDownObj.transform);
				_pMenuBtnComponent.enabled = true;
			}

			if (!pUtils.GetUser().IdAllowModify())
			{
				GameObject obj = Instantiate(Resources.Load(PREFAB_VIEW_MENU_ITEM_EMPTY)) as GameObject;
				UserProfileBtnController controller = obj.GetComponentInChildren<UserProfileBtnController>();
				controller.InitScreen(pUtils.GetTranslations().Get("user_menu_payment_accounts"), ShowPaymentManager);
				obj.transform.SetParent(_btnDropDownObj.transform);

				GameObject objSubs = Instantiate(Resources.Load(PREFAB_VIEW_MENU_ITEM_EMPTY)) as GameObject;
				UserProfileBtnController controllerSubs = objSubs.GetComponentInChildren<UserProfileBtnController>();
				controllerSubs.InitScreen(pUtils.GetTranslations().Get("user_menu_user_subscription"), ShowSubscriptionManager);
				objSubs.transform.SetParent(_btnDropDownObj.transform);
			}
			else
				_pMenuBtnComponent.enabled = false;
		}

		private void ShowPaymentManager()
		{
			GetComponentInParent<XsollaPaystation> ().LoadPaymentManager();
		}

		private void ShowHistory()
		{
			Logger.Log("Show user history");
			Dictionary<string, object> lParams = new Dictionary<string, object>();
			// Load History
			lParams.Add("offset", 0);
			lParams.Add("limit", 20);
			lParams.Add("sortDesc", true);
			lParams.Add("sortKey", "dateTimestamp");
			GetComponentInParent<XsollaPaystation> ().LoadHistory(lParams);
		}

		private void ShowSubscriptionManager()
		{
			GetComponentInParent<XsollaPaystation> ().LoadSubscriptionsManager();
		}

		public void setStateUserMenu(bool pState)
		{
			Logger.Log("Set user menu to state " + pState);
			_pMenuBtnComponent.gameObject.SetActive(pState);
		}

		public MainHeaderController ()
		{
		}
	}
}

