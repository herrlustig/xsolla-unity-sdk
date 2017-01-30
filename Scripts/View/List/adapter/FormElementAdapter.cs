using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Xsolla {

	public class FormElementAdapter : IBaseAdapter {
		
		private GameObject labelPrefab, tablePrefab, inputPrefab, checkBoxPrefab, selectPrtefab, selectElementPrefab;

		private XsollaForm form;
		private List<XsollaFormElement> elements;
		private XsollaTranslations _translation;

		public object getSelectElemPrefab()
		{
			return selectElementPrefab;
		}

		public void Awake()
		{
			labelPrefab = Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ElementLabel") as GameObject;
			tablePrefab = Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ElementTable") as GameObject;
			inputPrefab = Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ElementText") as GameObject;
			checkBoxPrefab = Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ElementCheckBox") as GameObject;
			selectPrtefab = Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ElementDropDown") as GameObject;
			selectElementPrefab = Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ElementSelectElement") as GameObject;
		}

		public override int GetElementType(int id)
		{
			return 0;
		}

		public override int GetCount()
		{
			return elements.Count;
		}

		public override GameObject GetView(int position)
		{
			XsollaFormElement element = elements[position];
			//create a new item, name it, and set the parent
			switch (element.GetElementType()) 
			{
				case XsollaFormElement.TYPE_LABEL:
					return DrawLabel(element);
				case XsollaFormElement.TYPE_TEXT:
					return DrawInput(element);
				case XsollaFormElement.TYPE_CHECK:
					return DrawCheckBox(element);
				case XsollaFormElement.TYPE_SELECT:
					return DrawSelect(element);
				case XsollaFormElement.TYPE_TABLE:
					return DrawTable(element);
				default:
					return DrawLabel(element);
			}
		}

		GameObject DrawTable(XsollaFormElement element)
		{
			GameObject newItem = Instantiate(tablePrefab) as GameObject;
			ElementTableController controller = newItem.GetComponent<ElementTableController>();
            controller.InitScreen(element);
			return newItem;
		}

		GameObject DrawLabel(XsollaFormElement element)
        {
			if (element.GetTitle() == "")
				return null;

			GameObject newItem = Instantiate(labelPrefab) as GameObject;
			newItem.GetComponentInChildren<Text> ().text = element.GetTitle ();
			return newItem;
		}

		GameObject DrawInput(XsollaFormElement element)
        {
            // if this promo coupone code then draw another prefab
            if (element.GetName() == "couponCode")
            {
				GameObject newItem = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ContainerPromoCode")) as GameObject;
                PromoCodeController controller = newItem.GetComponent<PromoCodeController>();
				controller.InitScreen(_translation, element);
				controller._inputField.onEndEdit.AddListener(delegate 
					{
						OnEndEdit(element, controller._inputField);
					});

				controller._promoCodeApply.onClick.AddListener(delegate 
					{
						bool isLinkRequired = false;
						if ((form.GetCurrentCommand() == XsollaForm.CurrentCommand.CHECKOUT) && form.GetSkipChekout()){
							string checkoutToken = form.GetCheckoutToken();
							isLinkRequired = checkoutToken != null 
								&& !"".Equals(checkoutToken) 
								&& !"null".Equals(checkoutToken)
								&& !"false".Equals(checkoutToken);
						}
						if(isLinkRequired){
							string link = "https://secure.xsolla.com/pages/checkout/?token=" + form.GetCheckoutToken();
							if (Application.platform == RuntimePlatform.WebGLPlayer 
								//|| Application.platform == RuntimePlatform.OSXWebPlayer 
								//|| Application.platform == RuntimePlatform.WindowsWebPlayer
							) {
								Application.ExternalEval("window.open('" + link + "','Window title')");
							} else {
								Application.OpenURL(link);
							}
						}
						gameObject.GetComponentInParent<XsollaPaystationController> ().ApplyPromoCoupone (form.GetXpsMap ());
					});

                return newItem;
            }
            else
			{
				GameObject newItem = Instantiate(inputPrefab) as GameObject;
				newItem.GetComponentInChildren<Text>().text = element.GetTitle();
				InputField inputField = newItem.GetComponentInChildren<InputField>();
				inputField.GetComponentInChildren<Text>().text = element.GetExample();
				SetupValidation(element.GetName(), inputField);
				//inputField.onValidateInput += ValidateInput;
				inputField.onEndEdit.AddListener(delegate
				{
					OnEndEdit(element, inputField);
				});
				return newItem;
			}
		}

		void SetupValidation(string fieldName, InputField _inputField){
			switch (fieldName) 
			{
				case "card_number":
					_inputField.characterValidation = InputField.CharacterValidation.Integer;
					_inputField.characterLimit = 19;
					//_inputField.onValueChange.AddListener(ValidateCard(_inputField));
					break;
				case "card_year":
					_inputField.characterValidation = InputField.CharacterValidation.Integer;
					break;
				case "card_month":
					_inputField.characterValidation = InputField.CharacterValidation.Integer;
					break;
				case "cvv":
					_inputField.characterValidation = InputField.CharacterValidation.Integer;
					break;
				case "card_holder":
					break;
				case "zip_code":
					break;
				default:
					break;
			}
		}

//		void ValidateCard(InputField _inputField){
//			string text = _inputField.text;
//		}

        public void OnEndEdit(XsollaFormElement element, InputField _input)
        {
			form.UpdateElement(element.GetName(), _input.text);
        }

		public void OnEndEdit(string fieldName, string newValue)
		{
			form.UpdateElement(fieldName, newValue);
		}


		GameObject DrawCheckBox(XsollaFormElement element)
		{
			GameObject newItem = Instantiate(checkBoxPrefab) as GameObject;
			Toggle toggle = newItem.GetComponentInChildren<Toggle> ();
			OnEndEdit(element.GetName(), toggle.isOn?"1":"0");
			toggle.onValueChanged.AddListener ((b) => {
				OnEndEdit(element.GetName(), b?"1":"0");
			});
			newItem.GetComponentInChildren<Text> ().text = element.GetTitle ();
			return newItem;
		}

		GameObject DrawSelect(XsollaFormElement element)
		{
			GameObject newSelect = Instantiate(selectPrtefab) as GameObject;
			DropDownController controller = newSelect.GetComponent<DropDownController> ();
			newSelect.GetComponentInChildren<Text>().text = element.GetTitle ();
			List<Xsolla.XsollaFormElement.Option> options = element.GetOptions ();
			List<string> titles = new List<string> (options.Count);
			foreach(Xsolla.XsollaFormElement.Option o in options){
				titles.Add(o.GetLabel());
			}
			controller.SetParentForScroll (gameObject.transform.parent.parent);
			controller.OnItemSelected += (position, title) => {
				OnEndEdit(element.GetName(), options[position].GetValue());
			};
			controller.SetData (titles);
//			GameObject panel = newSelect.GetComponentInChildren<VerticalLayoutGroup> ().gameObject;
//			List<Xsolla.XsollaFormElement.Option> options = element.GetOptions ();
//			foreach (var option in options)
//			{
//				GameObject go = Instantiate(selectElementPrefab) as GameObject;
//				go.GetComponentInChildren<Text>().text = option.GetLabel();
//				go.transform.SetParent(panel.transform);
//			}
			return newSelect;
		}
		
		public override GameObject GetPrefab()
		{
			return inputPrefab;
		}


		public void SetForm(XsollaForm form, XsollaTranslations pTranslation = null)
		{
			this.form = form;
			this.elements = form.GetVisible ();
			this._translation = pTranslation;
		}

		public override GameObject GetNext ()
		{
			throw new NotImplementedException ();
		}

	}

}
