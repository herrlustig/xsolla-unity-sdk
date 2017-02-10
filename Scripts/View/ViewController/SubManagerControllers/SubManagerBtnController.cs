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
			
			DateTime lDateNextCharge;
			if (DateTime.TryParse(pSub.mDateNextCharge, out lDateNextCharge))
				mNextInvoice.text = String.Format(nextChargeFormat, mSub.mCharge.ToString(), String.Format("{0:dd/MM/yyyy}", lDateNextCharge));
			else
				throw new Exception("Error DateParser");

			if (pSub.mPaymentMethod != "null")
				mPaymentMethodName.text = pSub.mPaymentMethod + " " + pSub.mPaymentVisibleName;
			else
				mPaymentMethodName.gameObject.SetActive(false);
			
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

	}
}

