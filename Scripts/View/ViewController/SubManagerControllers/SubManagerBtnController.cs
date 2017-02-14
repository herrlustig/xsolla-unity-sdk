using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubManagerBtnController: MonoBehaviour
	{
		public Text mSubName;
		public Text mNextInvoice;
		public Text mPaymentMethodName;
		public Text mDetailText;
		public Button mDetailBtn;
		public Button mSelfBtn;
		private XsollaManagerSubscription mSub;
	
		public SubManagerBtnController ()
		{
		}

		public void init(XsollaManagerSubscription pSub, XsollaTranslations pTranslation)
		{
			mSub = pSub;
			mSubName.text = mSub.GetName();
			String nextChargeFormat = pTranslation.Get("next_charge"); // next_charge:"Next invoice: {{amount}}, {{date}}↵"

			int indx = 0;
			while (nextChargeFormat.Contains("{{"))
			{
				String replacedPart = nextChargeFormat.Substring(nextChargeFormat.IndexOf("{{", 0) + 1, nextChargeFormat.IndexOf("}}", 0) - nextChargeFormat.IndexOf("{{", 0));
				nextChargeFormat = nextChargeFormat.Replace(replacedPart, indx.ToString());  
				indx ++;
			}

			if (pSub.mStatus == "active")
				mNextInvoice.text = String.Format(nextChargeFormat, mSub.mCharge.ToString(), pSub.mDateNextCharge.ToString("d"));
			else
				mNextInvoice.gameObject.SetActive(false);

			if (pSub.mPaymentMethod != "null")
				mPaymentMethodName.text = pSub.mPaymentMethod + " " + pSub.mPaymentVisibleName;
			else
			{
				switch (pSub.mStatus)
				{
					case "freeze":
					{
						mPaymentMethodName.text = String.Format(prepareFormatString(pTranslation.Get("user_subscription_hold_to")),pSub.mHoldDates.dateTo.ToString("d"));
						break;
					}
					case "non_renewing":
					{
						mPaymentMethodName.text = String.Format(prepareFormatString(pTranslation.Get("user_subscription_non_renewing")), pSub.mDateNextCharge.ToString("d")) ;
						break;
					}
					default:
					{
						mPaymentMethodName.gameObject.SetActive(false);	
						break;
					}
				}

			}

			mDetailText.text = pTranslation.Get("user_subscription_to_details");
		}

		public void SetDetailAction(Action<XsollaManagerSubscription> pAction)
		{
			mDetailBtn.onClick.AddListener(delegate 
				{
					pAction(mSub);
				});
			mSelfBtn.onClick.AddListener(delegate 
				{
					pAction(mSub);
				});
		}

		private String prepareFormatString(String pInnerString)
		{
			String res = pInnerString;
			int indx = 0;
			while (res.Contains("{{"))
			{
				String replacedPart = res.Substring(res.IndexOf("{{", 0) + 1, res.IndexOf("}}", 0) - res.IndexOf("{{", 0));
				res = res.Replace(replacedPart, indx.ToString());  
				indx ++;
			}
			return res;
		}
	}
}

