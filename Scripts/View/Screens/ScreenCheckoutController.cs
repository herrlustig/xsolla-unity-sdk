using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Xsolla
{
	public class ScreenCheckoutController : MonoBehaviour {

		public Button back;
		public PaymentFormController paymentForm;
		public RightTowerController tower;
		private bool isPrevStepPaymentList = true;

		public void InitScreen(XsollaUtils utils, XsollaForm form)
		{
			XsollaTranslations translations = utils.GetTranslations ();
			Resizer.ResizeToParrent (gameObject);
			bool isPurchaseNull = utils.GetPurchase () == null;
			if (isPurchaseNull || !utils.GetPurchase ().IsPurchase () || !utils.GetPurchase ().IsPaymentSystem ()) {
				if(!isPurchaseNull)
					isPrevStepPaymentList = !utils.GetPurchase ().IsPaymentSystem ();
				paymentForm.OnClickBack += () => {
					Back ();};
			}
			paymentForm.InitView (translations, form);
			if (form.GetSummary () != null)
				tower.InitView (translations, form.GetSummary ());
			else 
				tower.gameObject.SetActive (false);
		}

		public void Back()
		{
			if(isPrevStepPaymentList)
				GetComponentInParent<XsollaPaystationController>().LoadQuickPayment();
			else
				GetComponentInParent<XsollaPaystationController>().NavMenuClick(RadioButton.RadioType.SCREEN_GOODS);
		}

	}
}
