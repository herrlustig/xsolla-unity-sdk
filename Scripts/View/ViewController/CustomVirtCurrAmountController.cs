using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public class CustomVirtCurrAmountController: MonoBehaviour
	{
		public InputField 	virtCurrAmount;
		public InputField 	realCurrAmount;
		public Button 		btnPay;
		public Text 		totalAmountTitle;

		public Image		iconVirtCurr;
		public Text			iconRealCurr;	

		private string 		mTotalTitle = "";
		private string 		mCustomCurrency = "";
		private bool 		mSetValues = false;

		public CustomVirtCurrAmountController ()
		{
		}

		public void initScreen(XsollaUtils pUtils, string pCustomCurrency, Action<Dictionary<string, object>> pActionCalc, Action<float> pTryPay)
		{
			if (pUtils.GetProject().isDiscrete)
				virtCurrAmount.contentType = InputField.ContentType.IntegerNumber;
			else
				virtCurrAmount.contentType = InputField.ContentType.DecimalNumber;

			// Set btn Name
			btnPay.gameObject.GetComponentInChildren<Text>().text = pUtils.GetTranslations().Get("form_continue");
			mTotalTitle = pUtils.GetTranslations().Get("payment_summary_total");
			mCustomCurrency = pCustomCurrency;
			ImageLoader imageLoader = FindObjectOfType<ImageLoader>();
			Logger.Log("VirtIcon " + pUtils.GetProject().virtualCurrencyIconUrl);
			imageLoader.LoadImage(iconVirtCurr, "http:" + pUtils.GetProject().virtualCurrencyIconUrl);

			virtCurrAmount.onEndEdit.AddListener(delegate 
				{
					if (!mSetValues)
						pActionCalc(GetParamsForCalc(true));
				});

			realCurrAmount.onEndEdit.AddListener(delegate 
				{
					if (!mSetValues)
						pActionCalc(GetParamsForCalc(false));
				});
					
			btnPay.onClick.AddListener(delegate  
				{
					pTryPay(GetOutAmount());
				});
		}

		private float GetOutAmount()
		{
			float res = 0;
			string value = virtCurrAmount.text;

			float.TryParse(value, out res);
			return res;
		}

		private Dictionary<string, object> GetParamsForCalc(bool pVirtCurr)
		{
			Dictionary<string, object> res = new Dictionary<string, object>();
			res.Add("userInitialCurrency", "");
			res.Add("custom_currency", mCustomCurrency);
			if (pVirtCurr)
				res.Add("custom_vc_amount", virtCurrAmount.text);
			else
				res.Add("custom_amount", realCurrAmount.text);
			return res;
		}
			
		public void setValues(CustomAmountCalcRes pValue)
		{
			mSetValues = true;
			if (pValue.amount != 0)
				totalAmountTitle.text = mTotalTitle + " " + CurrencyFormatter.FormatPrice(pValue.currency, pValue.amount.ToString("N2"));
			else
				totalAmountTitle.text = "";
			
			if (pValue.vcAmount != 0)
				virtCurrAmount.text = pValue.vcAmount.ToString();
			else
				virtCurrAmount.text = "";
			
			if (pValue.amount != 0)
				realCurrAmount.text = pValue.amount.ToString("0.00");
			else
				realCurrAmount.text = "";

			if (pValue.currency == "USD")
				iconRealCurr.text = "$";
			else if (pValue.currency == "EUR")
				iconRealCurr.text = "€";
			else if (pValue.currency == "RUB")
				iconRealCurr.text = "";

			if (pValue.vcAmount > 0)
				btnPay.interactable = true;
			else
				btnPay.interactable = false;

			mSetValues = false;
		}

		public class CustomAmountCalcRes: IParseble
		{
			public double amount;
			public string currency;
			public double vcAmount;
			public Bonus bonus;

			public IParseble Parse(JSONNode pNode)
			{
				amount = pNode["amount"].AsDouble;
				currency = pNode["currency"];
				vcAmount = pNode["vc_amount"].AsDouble;
				bonus = new Bonus().Parse(pNode["bonus"]) as Bonus;
				return this;
			}

			public class Bonus: IParseble
			{
				public IParseble Parse(JSONNode pNode)
				{
					return this;
				}
			}
		}
	}
}

