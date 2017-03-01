using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class MainFooterController: MonoBehaviour
	{
		public Text mCustomerSupport;
		public Text mContactUs;
		public Text mCopyRight;
		public Text mSecuredConnection;
		public Text mAgreement;

		public void Init(XsollaUtils pUtils)
		{
			if (pUtils != null) {
				XsollaTranslations lTranslatrions = pUtils.GetTranslations ();
				mCustomerSupport.text = lTranslatrions.Get(XsollaTranslations.SUPPORT_CUSTOMER_SUPPORT);
				mContactUs.text = lTranslatrions.Get(XsollaTranslations.SUPPORT_CONTACT_US);
				mCopyRight.text = lTranslatrions.Get(XsollaTranslations.XSOLLA_COPYRIGHT);
				mSecuredConnection.text = lTranslatrions.Get(XsollaTranslations.FOOTER_SECURED_CONNECTION);
				mAgreement.text = lTranslatrions.Get(XsollaTranslations.FOOTER_AGREEMENT);
			}
		}
	}
}

