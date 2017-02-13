using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubManagerCancelPartController: MonoBehaviour
	{
		public Toggle mDontRenew;
		public Toggle mDeletNow;
		public Text mDontRenewLabel;
		public Text mDontRenewDesc;
		public Text mDeleteNowLabel;
		public Text mDeleteNowDesc;
		public Text mTitle;

		public bool isDontRenew()
		{
			return mDontRenew.isOn;
		}

		public bool isDeleteNow()
		{
			return mDeletNow.isOn;
		}

		public void init(XsollaManagerSubDetails pSubDetail, XsollaUtils pUtils)
		{
			mTitle.text = pUtils.GetTranslations().Get("user_hold_subscription_title");

			mDontRenewLabel.text = pUtils.GetTranslations().Get("hold_subscription_dont_renew_label");
			mDontRenewDesc.text = pUtils.GetTranslations().Get("hold_subscription_dont_renew_label_description");

			mDeleteNowLabel.text = pUtils.GetTranslations().Get("hold_subscription_cancel_label");
			mDeleteNowDesc.text = pUtils.GetTranslations().Get("hold_subscription_cancel_label_description");
		}
	}
}

