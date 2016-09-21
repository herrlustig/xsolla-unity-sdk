using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Xsolla
{
	public class SubscriptionBtnController: MonoBehaviour
	{
		private XsollaSubscription _sub;
		public Text 		_subName;
		public Text			_bonusText;
		public GameObject	_desc;
		public Text			_price;
		public Text			_period;
		public Button		_btnSub;

		public GameObject 	_offerText;

		public void InitBtn(XsollaSubscription pSub)
		{
			_sub = pSub;
			_subName.text = _sub.GetName();

			if (_sub.description != null)
			{
				_desc.GetComponent<Text>().text = _sub.description;
				_desc.SetActive(true);
			}
			else
			{
				_desc.SetActive(false);
			}
			_price.text = _sub.GetPriceString();
			_period.text = _sub.GetPeriodString("Every");

			if (_sub.offerLabel == null)
				SetOffer("");
			else
				SetOffer(_sub.offerLabel);

			_btnSub.onClick.AddListener(() => {
				Dictionary<string,object> purchase = new Dictionary<string, object>();
				purchase.Add("id_package", _sub.id);

				gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (purchase, false);
			});
		}

		private void SetOffer(string pOffer)
		{
			if (_offerText != null)
			{
				_offerText.GetComponent<Text>().text = pOffer;
				_offerText.SetActive(pOffer.Equals("")?false:true);
			}
		}

		public SubscriptionBtnController ()
		{
		}
	}
}

