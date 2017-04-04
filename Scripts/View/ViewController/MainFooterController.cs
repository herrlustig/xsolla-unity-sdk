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

		private const String mUrl = "http://xsolla.com/termsandconditions/?lang=en&ca=15924";

		public void Init(XsollaUtils pUtils)
		{
			if (pUtils != null) {
				XsollaTranslations lTranslatrions = pUtils.GetTranslations ();
				mCustomerSupport.text = lTranslatrions.Get(XsollaTranslations.SUPPORT_CUSTOMER_SUPPORT);
				mContactUs.text = lTranslatrions.Get(XsollaTranslations.SUPPORT_CONTACT_US);
				mCopyRight.text = lTranslatrions.Get(XsollaTranslations.XSOLLA_COPYRIGHT);
				mSecuredConnection.text = lTranslatrions.Get(XsollaTranslations.FOOTER_SECURED_CONNECTION);

				mAgreement.text = pUtils.GetProject().eula != "null" ? pUtils.GetProject().eula : lTranslatrions.Get(XsollaTranslations.FOOTER_AGREEMENT);
			
				mAgreement.gameObject.GetComponent<Button>().onClick.AddListener(delegate 
						{
						new OpenUrlHelper().OpenUrl(pUtils.GetProject().eulaUrl != "null" ? pUtils.GetProject().eulaUrl : mUrl);
						});
			}
		}
	}
}

