using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubManagerChargeElemController: MonoBehaviour
	{
		public Text mDateLabel;
		public Text mPaymentType;
		public Text mAmount;

		public GameObject mDevider;

		public void init(String pDate, String pPaymentType, String pAmount, bool pIsTitle = false)
		{
			// показывает разделитель если это заголовок
			mDevider.SetActive(pIsTitle);
			mDateLabel.text = pDate;
			mPaymentType.text = pPaymentType;
			mAmount.text = pAmount;
		}
	}
}

