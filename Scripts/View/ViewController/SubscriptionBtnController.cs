using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubscriptionBtnController: MonoBehaviour
	{
		private XsollaSubscription _sub;
		public Text 		_subName;
		public Text			_bonusText;
		public Text			_desc;
		public Text			_price;
		public Text			_period;

		public void InitBtn(XsollaSubscription pSub)
		{
			_sub = pSub;
			_subName.text = _sub.GetName();
			//_desc.text = _sub.description;
			_price.text = _sub.GetPriceString();
			_period.text = _sub.GetPeriodString("Every");
		}

		public SubscriptionBtnController ()
		{
		}
	}
}

