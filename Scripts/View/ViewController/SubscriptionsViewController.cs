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

		private const string PREFAB_SPEC_SUBS = "Prefabs/SimpleView/_ScreenShop/ShopItemSubscriptionSpecial";

		public void InitScreen(XsollaSubscriptions pSubs)
		{
			_listSubs = pSubs;
			foreach(XsollaSubscription sub in _listSubs.GetItemsList())
			{
				AddSubs(sub);
			}
		}

		private void AddSubs(XsollaSubscription pSub)
		{
			GameObject subObj = Instantiate(Resources.Load(PREFAB_SPEC_SUBS)) as GameObject;
			subObj.transform.SetParent(_listSubsView.transform);
			SubscriptionBtnController controller = subObj.GetComponent<SubscriptionBtnController>();
			controller.InitBtn(pSub);
		}

		public SubscriptionsViewController ()
		{
		}
	}
}

