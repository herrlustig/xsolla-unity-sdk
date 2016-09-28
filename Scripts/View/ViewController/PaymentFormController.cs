using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Xsolla 
{
	public class PaymentFormController : ScreenBaseConroller<object> 
	{

		public delegate void BackButtonHandler();
		public BackButtonHandler OnClickBack;

		public LinearLayout layout;
		public GameObject footer;

		private XsollaForm _form;

		public override void InitScreen (XsollaTranslations translations, object model)
		{
			throw new System.NotImplementedException ();
		}

		public void InitView(XsollaTranslations translations, XsollaForm form)
		{
			_form = form;
			// if have skipCheckout and this checkout form
			if ((form.GetCurrentCommand() == XsollaForm.CurrentCommand.CHECKOUT) && form.GetSkipChekout()){
				string checkoutToken = _form.GetCheckoutToken();
				bool isLinkRequired = checkoutToken != null 
					&& !"".Equals(checkoutToken) 
					&& !"null".Equals(checkoutToken)
					&& !"false".Equals(checkoutToken);
				if (isLinkRequired){
					OnClickPay(isLinkRequired);
					return;
				}
			}

			string pattern = "{{.*?}}";
			Regex regex = new Regex (pattern);
			string title = regex.Replace (translations.Get(XsollaTranslations.PAYMENT_PAGE_TITLE_VIA), form.GetTitle (), 1);
			layout.AddObject (GetTitle (title));
			layout.AddObject (GetError (form.GetError ()));
			layout.AddObject (GetInfo (form.GetMessage ()));
			if (form.GetVisible ().Count > 0) {
				GameObject formView = GetFormView (form, translations);
				layout.AddObject (formView);
			}
			if (form.GetAccountXsolla () != null && !"".Equals (form.GetAccountXsolla ()) && !"null".Equals (form.GetAccountXsolla ()))
				layout.AddObject (GetTwoTextPlate ("Xsolla number", form.GetAccountXsolla ()));
			if (form.GetAccount () != null && !"".Equals (form.GetAccount ()) && !"null".Equals (form.GetAccount ()))
				layout.AddObject (GetTwoTextPlate ("2pay number", form.GetAccount ()));
			if (form.IsValidPaymentSystem ())
				layout.AddObject (GetTextPlate (translations.Get (XsollaTranslations.FORM_CC_EULA)));
			GameObject footerInstance = Instantiate (footer);
			Text[] footerTexts = footerInstance.GetComponentsInChildren<Text> ();
//			footerTexts [0].text = "back";//back
			string nextStep = form.GetNextStepString ();
			footerTexts [2].text = nextStep;//translations.Get (XsollaTranslations.FORM_CONTINUE);//pay now
			Button[] footerButtons = footerInstance.GetComponentsInChildren<Button> ();
			if (OnClickBack != null) {
				footerButtons [0].onClick.AddListener (() => {
					OnBack ();});
			} else {
				footerButtons [0].gameObject.SetActive(false);
			}

			if (form.GetCurrentCommand () == XsollaForm.CurrentCommand.ACCOUNT
			    || form.GetCurrentCommand () == XsollaForm.CurrentCommand.CREATE
			    || form.GetCurrentCommand () == XsollaForm.CurrentCommand.CHECKOUT) {//
				footerTexts [1].text = "";//total
				RectTransform buttonRect = footerButtons [1].GetComponent<RectTransform>();
				Vector2 vecMin = buttonRect.anchorMin;
				vecMin.x = vecMin.x - (buttonRect.anchorMax.x - vecMin.x)/2;
				buttonRect.anchorMin = vecMin;
			} else {
				footerTexts [1].text = translations.Get (XsollaTranslations.TOTAL) + " " + form.GetSumTotal ();//total
			}

			layout.AddObject (footerInstance);
			layout.Invalidate ();

			if (!"".Equals (nextStep) && form.GetCurrentCommand() != XsollaForm.CurrentCommand.ACCOUNT) {
				string checkoutToken = _form.GetCheckoutToken();
				bool isLinkRequired = checkoutToken != null 
					&& !"".Equals(checkoutToken) 
						&& !"null".Equals(checkoutToken)
						&& !"false".Equals(checkoutToken);
				string link = "https://secure.xsolla.com/pages/checkout/?token=" + _form.GetCheckoutToken();
				if(isLinkRequired && Application.platform == RuntimePlatform.WebGLPlayer){
					RectTransform buttonRect = footerButtons [1].GetComponent<RectTransform>();
					int width = (int)(buttonRect.rect.xMax - buttonRect.rect.xMin);
					int height = (int)(buttonRect.rect.yMax - buttonRect.rect.yMin);
					height = height * 8;
					Vector3[] vec = new Vector3[4];
					buttonRect.GetWorldCorners(vec);
					int xPos = (int)vec[0].x;
					int yPos = (int)vec[0].y;
					yPos = yPos/2;
					CreateLinkButtonWebGl(xPos, yPos, width, height, link, "CardPaymeentForm", "Next");
					footerButtons [1].onClick.AddListener (() => {
						OnClickPay (false);});
				} else {
					footerButtons [1].onClick.AddListener (() => {
						OnClickPay (isLinkRequired);});
				}
			} else {
				footerButtons [1].gameObject.SetActive(false);
			}
		}
	
		public GameObject GetFormView(XsollaForm xsollaForm, XsollaTranslations translations){
			bool isCcRender = xsollaForm.GetCurrentCommand() == XsollaForm.CurrentCommand.FORM && xsollaForm.IsCreditCard();
			if (isCcRender)
			{
				return GetCardViewWeb(xsollaForm, translations);
			} else {
				FormElementAdapter adapter = GetComponent<FormElementAdapter>();
				adapter.SetForm (xsollaForm);
				GameObject list = GetList (adapter);
				list.GetComponent<ListView>().DrawList(GetComponent<RectTransform> ());
				return list;
			}
		}

		private bool isMaestro;
		private List<ValidatorInputField> validators;

		public GameObject GetCardViewWeb(XsollaForm xsollaForm, XsollaTranslations translations)
		{
			GameObject cardViewObj = Instantiate (Resources.Load("Prefabs/SimpleView/_ScreenCheckout/CardViewLayoutWeb")) as GameObject;
			InputField[] inputs = cardViewObj.GetComponentsInChildren<InputField>();
			validators = new List<ValidatorInputField> ();
			for(int i = inputs.Length - 1; i >= 0 ; i--)
			{
				XsollaFormElement element = null;
				string newErrorMsg = "Invalid";
				InputField input = inputs[i];
				ValidatorInputField validator = input.GetComponentInChildren<ValidatorInputField>();
				// CVV > *HOLDER* > *ZIP* > YEAR > MONTH > NUMBER
				switch(i)//input.tag)
				{
					case 5://"CardNumber":
						element = xsollaForm.GetItem(XsollaApiConst.CARD_NUMBER);
						newErrorMsg = translations.Get(XsollaTranslations.VALIDATION_MESSAGE_CARDNUMBER);
						CCEditText ccEditText = input.GetComponent<CCEditText>();
						isMaestro = ccEditText.IsMaestro();
						validator.AddValidator(new ValidatorEmpty(newErrorMsg));
						validator.AddValidator(new ValidatorCreditCard(newErrorMsg));
						break;
					case 4://"Month":
						element = xsollaForm.GetItem(XsollaApiConst.CARD_EXP_MONTH);
						newErrorMsg = translations.Get(XsollaTranslations.VALIDATION_MESSAGE_CARD_MONTH);
						validator.AddValidator(new ValidatorEmpty(newErrorMsg));
						validator.AddValidator(new ValidatorMonth(newErrorMsg));
						break;
					case 3://"Year":
						element =  xsollaForm.GetItem(XsollaApiConst.CARD_EXP_YEAR);
						newErrorMsg = translations.Get(XsollaTranslations.VALIDATION_MESSAGE_CARD_YEAR);
						validator.AddValidator(new ValidatorEmpty(newErrorMsg));
						validator.AddValidator(new ValidatorYear(newErrorMsg));
						break; 
					case 2://"Zip":
						element = xsollaForm.GetItem(XsollaApiConst.CARD_ZIP);
						newErrorMsg = translations.Get(XsollaTranslations.VALIDATION_MESSAGE_REQUIRED);
						validator.AddValidator(new ValidatorEmpty(newErrorMsg));
						break;
					case 1://"CardHolder":
						element = xsollaForm.GetItem(XsollaApiConst.CARD_HOLDER);
						newErrorMsg = translations.Get(XsollaTranslations.VALIDATION_MESSAGE_REQUIRED);
						validator.AddValidator(new ValidatorEmpty(newErrorMsg));
						break;
					case 0://"Cvv":
						element = xsollaForm.GetItem(XsollaApiConst.CARD_CVV);
						newErrorMsg = translations.Get(XsollaTranslations.VALIDATION_MESSAGE_CVV);
						validator.AddValidator(new ValidatorEmpty(newErrorMsg));
						validator.AddValidator(new ValidatorCvv(newErrorMsg, isMaestro));
						break;
					default:
						break;
				}

				if(element != null){
					//		input.text = element.GetTitle();
					//						input.GetComponentInChildren<MainValidator>().setErrorMsg(newErrorMsg);
					if (element.GetName() != XsollaApiConst.CARD_CVV)
						input.GetComponentInChildren<Text>().text = element.GetExample();
					// FIX update with unity 5.2 
					input.onValueChanged.AddListener(delegate{OnValueChange(input, element.GetName());});
				} else {
					DestroyImmediate(input.transform.parent.gameObject);
				}
				
				if(validator != null)
					validators.Add(validator);
			}
			// Toggle allowSubscription
			// get toggle object
			Toggle toggle = cardViewObj.GetComponentInChildren<Toggle> ();
			if (xsollaForm.Contains(XsollaApiConst.ALLOW_SUBSCRIPTION))
			{
				XsollaFormElement ToggleElement = null;
				ToggleElement = xsollaForm.GetItem(XsollaApiConst.ALLOW_SUBSCRIPTION);
				// set label name 
				Text lable = toggle.transform.GetComponentInChildren<Text>();
				lable.text = ToggleElement.GetTitle();
				OnValueChange(ToggleElement.GetName(), toggle.isOn?"1":"0");
				toggle.onValueChanged.AddListener ((b) => {
					OnValueChange(ToggleElement.GetName(), b?"1":"0");
				});
			}
			else
			{
				GameObject.Find(toggle.transform.parent.name).SetActive(false);
			}
			return cardViewObj;
		}

		private void OnValueChange(InputField _input, string elemName)
		{
			_form.UpdateElement(elemName, _input.text);
		}

		private void OnValueChange(string elemName, string pValue)
		{
			_form.UpdateElement(elemName, pValue);
		}

		//TODO make new window non blocking
		private void OnClickPay(bool isLinkRequired)
		{
			Logger.Log ("OnClickPay");
			bool isValid = true;
			if (validators != null) {
				foreach (var v in validators) {
					if(isValid)
						isValid = v.IsValid ();
				}
			}
			if(isValid) {
				if(isLinkRequired){
					string link = "https://secure.xsolla.com/pages/checkout/?token=" + _form.GetCheckoutToken();
					if (Application.platform == RuntimePlatform.WebGLPlayer 
					           || Application.platform == RuntimePlatform.OSXWebPlayer 
	       						|| Application.platform == RuntimePlatform.WindowsWebPlayer) {
							Application.ExternalEval("window.open('" + link + "','Window title')");
					} else {
						Application.OpenURL(link);
					}
				}
				gameObject.GetComponentInParent<XsollaPaystationController> ().DoPayment (_form.GetXpsMap ());
			}
		}

		private	void Next(){
			Logger.Log ("OnClickNext");
			if(Application.platform == RuntimePlatform.WebGLPlayer )
				RemoveLinkButtonWebGL();
			gameObject.GetComponentInParent<XsollaPaystationController> ().DoPayment (_form.GetXpsMap ());
		}

		private bool isDivCreated = false;

		public void CreateLinkButtonWebGl(int xPos, int yPos, int width, int heigth, string link, string objName, string functionToCall){
			string js = String.Format(@"var div = document.createElement('a');
						div.id = 'customLinkButton';
						div.style.zIndex = '9999';
						div.style.position = 'absolute';
						div.style.left = '{0}px';
						div.style.bottom = '{1}px';
						div.style.width = '{2}px';
						div.style.height = '{3}px';
						div.href = '{4}';
						div.target = '_blank';
						div.onclick = function() {{ SendMessage('{5}','{6}');}};
						document.getElementById('canvas').parentNode.appendChild(div);",
			                          Math.Abs(xPos), Math.Abs(yPos), Math.Abs(width), Math.Abs(heigth), link, objName, functionToCall);
			Application.ExternalEval(js);
			isDivCreated = true;
		}

		public void RemoveLinkButtonWebGL(){
			if (Application.platform == RuntimePlatform.WebGLPlayer && isDivCreated) { 
				string js = @"var elem = document.getElementById('customLinkButton');
							elem.parentNode.removeChild(elem);";
				Application.ExternalEval (js);
				isDivCreated = false;
			}
		}
		
		private const string PrefabInfo = "Prefabs/SimpleView/_ScreenCheckout/InfoPlate";
		private GameObject GetInfo(XsollaMessage infoText){
			bool isError = infoText != null;
			if (isError) {
				GameObject infoObj = GetObject (PrefabInfo);
				SetText (infoObj, infoText.message);
				return infoObj;
			}
			return null;
		}

		void OnDestroy() {
				RemoveLinkButtonWebGL ();
		}

		private void OnBack()
		{
			if(OnClickBack != null)
				OnClickBack();
		}

	}
}
