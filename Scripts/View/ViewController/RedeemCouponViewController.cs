using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla;
using System.Text.RegularExpressions;

namespace Xsolla
{
	public class RedeemCouponViewController: MonoBehaviour
	{

		public Text _title;
		public GameObject _errorPanel;
		public Text _coupounNotif;
		public InputField _inputField;
		public Text _nameInputField;
		public Text _inputFieldExample;
		public Button _btnApply;
		private XsollaUtils _utiliLink;

		public void InitScreen(XsollaUtils pUtils)
		{
			_utiliLink = pUtils;
			// Set titles
			_title.text = _utiliLink.GetTranslations().Get(XsollaTranslations.COUPON_PAGE_TITLE);
			_coupounNotif.text = _utiliLink.GetTranslations().Get(XsollaTranslations.COUPON_DESCRIPTION);
			_nameInputField.text = _utiliLink.GetTranslations().Get(XsollaTranslations.COUPON_CODE_TITLE);
			_inputFieldExample.text = _utiliLink.GetTranslations().Get(XsollaTranslations.COUPON_CODE_EXAMPLE);
			_inputField.onValidateInput += delegate(string input, int charIndex, char addedChar) 
			{ 
				return MyValidate(addedChar); 
			};

			_inputField.onEndEdit.AddListener(delegate 
				{
					setAproveInput();
				});

			Text btnText = _btnApply.GetComponentInChildren<Text>();
			btnText.text = _utiliLink.GetTranslations().Get(XsollaTranslations.COUPON_CONTROL_APPLY);
		}

		private char MyValidate(char charToValidate)
		{
			if(Regex.IsMatch(charToValidate.ToString(), "[^a-zA-Z0-9\\-\\_]"))
			{
				// не латинский и не цифровой и не - и _
				return '\0';
			}
			return charToValidate;
		}

		public void ShowError(string pErrMsg)
		{
			Text textErr = _errorPanel.GetComponentInChildren<Text>();
			textErr.text = pErrMsg;
			_errorPanel.SetActive(true);	
		}

		public void HideError()
		{
			_errorPanel.SetActive(false);
		}

		public string GetCode()
		{
			return _inputField.text;
		}

		public void setAproveInput()
		{
			// Задаем апрувные цвета поля
			_inputField.gameObject.GetComponent<ColorInputController>().pType = StyleManager.BaseSprite.bckg_input_approve;
			// Задаем цвета
			_nameInputField.color = StyleManager.Instance.GetColor(StyleManager.BaseColor.bonus);
		}

		public RedeemCouponViewController ()
		{
		}
	}
}

