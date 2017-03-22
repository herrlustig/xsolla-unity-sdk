﻿using System;
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
		public Text 		mTotalAmount;

		public Image		iconVirtCurr;
		public Text			iconRealCurr;	

		private string 		mCustomCurrency = "";
		private bool 		mSetValues = false;
		private XsollaUtils mUtils;
		private bool 		mFirstCalc = true;

		private const String mCalculateUrl = "paystation2/api/pricepoints/calculate";

		public void initScreen(XsollaUtils pUtils, string pCustomCurrency, Action<Dictionary<string, object>> pActionCalc, Action<float> pTryPay)
		{

		}

		//  новая инициализация 
		public void init(XsollaUtils pUtils, Decimal pDefAmount)
		{
			mUtils = pUtils;
			if (pUtils.GetProject().isDiscrete)
				virtCurrAmount.contentType = InputField.ContentType.IntegerNumber;
			else
				virtCurrAmount.contentType = InputField.ContentType.DecimalNumber;

			btnPay.gameObject.GetComponentInChildren<Text>().text = pUtils.GetTranslations().Get("form_continue");
			FindObjectOfType<ImageLoader>().LoadImage(iconVirtCurr, pUtils.GetProject().virtualCurrencyIconUrl);

			virtCurrAmount.onValueChanged.AddListener(delegate 
				{
					if (!mSetValues)
					{
						CancelInvoke();
						Invoke("RecalcVcAmount", 1);
					}
				});

			realCurrAmount.onValueChanged.AddListener(delegate 
				{
					if (!mSetValues)
					{
						CancelInvoke();
						Invoke("RecalcAmount", 1);
					}
				});
			
			btnPay.onClick.AddListener(delegate  
				{
					BuyBtn();
				});

			// Задаем дефолтное значение для обсчета
			virtCurrAmount.text = pDefAmount.ToString();
		}

		private void BuyBtn()
		{
			decimal lVcAmount;
			if (decimal.TryParse(virtCurrAmount.text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out lVcAmount))
			{
				// Покупаем
				Dictionary<string, object> map = new Dictionary<string, object> (1);
				map.Add ("out", lVcAmount);
				gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (map, false);
			}
			else
			{
				// Ошибка парса из поля
			}
		}

		private void RecalcVcAmount()
		{
			Logger.Log("Recalc vc change");
			if (virtCurrAmount.text == "")
			{
				virtCurrAmount.text = "1";
				return;
			}

			if (!mFirstCalc)
			{
				virtCurrAmount.textComponent.color = StyleManager.Instance.GetColor(StyleManager.BaseColor.bonus);
				virtCurrAmount.gameObject.GetComponent<ColorInputController>().pType = StyleManager.BaseSprite.bckg_input_approve;
			}
			Dictionary<string, object> res = new Dictionary<string, object>();
			res.Add("custom_vc_amount", virtCurrAmount.text);
			RequestCalculate(res);
			mFirstCalc = false;
		}

		private void RecalcAmount()
		{
			Logger.Log("Recalc real change");
			if (realCurrAmount.text == "")
			{
				realCurrAmount.text = "0.01";
				return;
			}

			if (!mFirstCalc)
			{
				realCurrAmount.textComponent.color = StyleManager.Instance.GetColor(StyleManager.BaseColor.bonus);
				realCurrAmount.gameObject.GetComponent<ColorInputController>().pType = StyleManager.BaseSprite.bckg_input_approve;
			}
			Dictionary<string, object> res = new Dictionary<string, object>();
			res.Add("custom_amount", realCurrAmount.text);
			RequestCalculate(res);
			mFirstCalc = false;
		}

		private void RequestCalculate(Dictionary<string, object> pParams)
		{
			pParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			pParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			pParams.Add("custom_currency", mCustomCurrency);

			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mCalculateUrl, pParams), CalculateRecived, ErrorRecived, false);
		}

		private void CalculateRecived(JSONNode pNode)
		{
			CustomAmountCalcRes calcRes = new CustomAmountCalcRes().Parse(pNode["calculation"]) as CustomAmountCalcRes;
			setValues(calcRes);
		}

		private void ErrorRecived(JSONNode pNode)
		{
			
		}

		private decimal GetOutAmount()
		{
			decimal res = 0;
			string value = virtCurrAmount.text;

			decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out res);
			return res;
		}

		private void setValues(CustomAmountCalcRes pValue)
		{
			mSetValues = true;
			if (pValue.amount != 0)
			{
				totalAmountTitle.text = mUtils.GetTranslations().Get("form_subtotal");
				mTotalAmount.text = CurrencyFormatter.FormatPrice(pValue.currency, pValue.amount.ToString("N2"));
			}
			else
			{
				totalAmountTitle.text = "";
				mTotalAmount.text = "";
			}
			
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
			public decimal amount;
			public string currency;
			public decimal vcAmount;
			public Bonus bonus;

			public IParseble Parse(JSONNode pNode)
			{
				amount = pNode["amount"].AsDecimal;
				currency = pNode["currency"];
				vcAmount = pNode["vc_amount"].AsDecimal;
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

