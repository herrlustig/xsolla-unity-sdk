using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public class SubManagerDetailChargesPartController: MonoBehaviour
	{
		public Text mPartTitle;
		public GameObject mDetailsContainer;

		private const String mChargeItemTextPrefab = "Prefabs/Screens/SubsManager/Simple/ChargeItem";
		private XsollaUtils mUtils;
		private XsollaManagerSubDetails mSubDetail;

		public void init(XsollaManagerSubDetails pSubDetail, XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mSubDetail = pSubDetail;

			mPartTitle.text = mUtils.GetTranslations().Get("user_subscription_charges_title");

			String lDateTitle = mUtils.GetTranslations().Get("virtualstatus_check_time");
			String lPaymenyTypeTitle = mUtils.GetTranslations().Get("user_subscription_payment_header");
			String lAmountTitle = mUtils.GetTranslations().Get("user_subscription_payment");

			// добавляем заголовки 
			addChargeElem(lDateTitle, lPaymenyTypeTitle, lAmountTitle, true);

			// добавляем историю
			foreach (XsollaSubDetailCharge charge in mSubDetail.mCharges)
			{
				addChargeElem(charge.mDateCreate.ToString("d"), charge.mPaymentMethod, charge.mCharge.ToString());	
			}
		}

		private void addChargeElem(String pDate, String pPaymetntype, String pAmount, bool pIsTitle = false)
		{
			GameObject obj = Instantiate(Resources.Load(mChargeItemTextPrefab)) as GameObject;
			SubManagerChargeElemController controller = obj.GetComponent<SubManagerChargeElemController>() as SubManagerChargeElemController;
			controller.init(pDate, pPaymetntype, pAmount, pIsTitle);
			controller.transform.SetParent(mDetailsContainer.transform);
		}

	}
}

