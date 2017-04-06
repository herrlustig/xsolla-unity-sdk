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
			// масштабирование
			subObj.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			SubscriptionBtnController controller = subObj.GetComponent<SubscriptionBtnController>();
			controller.InitBtn(pSub,pTranslation);

		}

		public SubscriptionsViewController ()
		{
		}
	}
}

