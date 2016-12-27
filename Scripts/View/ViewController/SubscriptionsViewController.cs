using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubscriptionsViewController: MonoBehaviour
	{
		XsollaSubscriptions 	_listSubs;
		public Text 			_titlePage;
		public GameObject 		_listSubsView;
		public Text 			_timeAlert;

		private const string PREFAB_SPEC_SUBS = "Prefabs/SimpleView/_ScreenShop/ShopItemSubscriptionSpecial";

		public void InitScreen(XsollaTranslations pTranslation,XsollaSubscriptions pSubs)
		{
			// for current design we can't show all allert, because we don't have button Continue or some else
//			if (!pSubs.GetActivePackage().Equals(null))
//			{
//				if (pSubs.GetActivePackage()._isPossibleRenew)
//					_timeAlert.text = string.Format(pTranslation.Get("subscription_active_note"), pSubs.GetActivePackage()._dateNextCharge.ToShortDateString());
//				else	
//					_timeAlert.text = string.Format(pTranslation.Get("subscription_active_note_no_renew"), pSubs.GetActivePackage()._dateNextCharge.ToShortDateString());
//			}

			_titlePage.text = pTranslation.Get(XsollaTranslations.SUBSCRIPTION_PAGE_TITLE);

			_listSubs = pSubs;
			foreach(XsollaSubscription sub in _listSubs.GetItemsList())
			{
				AddSubs(sub,pTranslation);
			}
		}

		private void AddSubs(XsollaSubscription pSub, XsollaTranslations pTranslation)
		{
			GameObject subObj = Instantiate(Resources.Load(PREFAB_SPEC_SUBS)) as GameObject;
			subObj.transform.SetParent(_listSubsView.transform);
			SubscriptionBtnController controller = subObj.GetComponent<SubscriptionBtnController>();
			controller.InitBtn(pSub,pTranslation);

		}

		public SubscriptionsViewController ()
		{
		}
	}
}

