using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Xsolla
{
	public class AllPaymentsController : MonoBehaviour {

		public PaymentListView paymenListView;
		public GameObject dropDownContainer;
		public DropDownController dropDownController;
		public GameObject back;
		public GameObject containerBtns;
		private XsollaCountries _countries;
		public ImageLoader imageLoader;
		private XsollaUtils utilsLink;
		private List<ShopPaymentBtnController> listBtns;

		public XsollaCountries GetCountrysList()
		{
			return _countries;
		}

		public void SetPaymentMethods(XsollaPaymentMethods paymentMethods){
			paymenListView.SetPaymentMethods(paymentMethods);
			SetUpNavButtons();
		}

		public void SetPaymentMethods(List<XsollaPaymentMethod> pList)
		{
			if (listBtns == null)
				listBtns = new List<ShopPaymentBtnController>();
			else
				ClearBtnContainer();
			
			List<XsollaPaymentMethod> listToShow = pList.FindAll(delegate(XsollaPaymentMethod method)
				{
					return method.isVisible == true;
				});
					
			foreach(XsollaPaymentMethod item in listToShow)
			{
				CreatePaymentBtn(item);
			}
			SetUpNavButtons();
		}

		private void CreatePaymentBtn(XsollaPaymentMethod pMethod)
		{
			GameObject popularBtn = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ShopPaymentBtn")) as GameObject;
			popularBtn.transform.SetParent(containerBtns.transform);
			ShopPaymentBtnController controller = popularBtn.GetComponent<ShopPaymentBtnController>();
			listBtns.Add(controller);
			// Set method
			controller.setMethod(pMethod);
			// Set icon
			controller.setIcon(imageLoader);
//			if (pMethod.GetImageUrl() != "")
//				imageLoader.LoadImage(controller._icon, pMethod.GetImageUrl());
			controller._btn.onClick.AddListener(() => OnChoosePaymentMethod(controller.getMethod().id));

		}

		private void ClearBtnContainer()
		{
			listBtns.ForEach(delegate(ShopPaymentBtnController btn)
				{
					Destroy(btn._self);
				});
			listBtns.Clear();
		}

		public void Sort(string pInput)
		{
			listBtns.ForEach(delegate(ShopPaymentBtnController btn)
				{
					btn._self.SetActive(btn.getMethod().name.ToLower().StartsWith(pInput.ToLower()));
				});
		}

		public void OnChoosePaymentMethod(long paymentMethodId)
		{
			GetComponentInParent<PaymentListScreenController> ().ChoosePaymentMethod (paymentMethodId);
		}


		public void InitScreen(XsollaUtils pUtils)
		{
			utilsLink = pUtils;
			back.GetComponent<Text>().text = utilsLink.GetTranslations().Get(XsollaTranslations.BACK_TO_LIST);
		}

		public void SetCountries(string currentCountryIso, XsollaCountries countries)
		{
			_countries = countries;
			dropDownController.gameObject.SetActive (true);//Show countries dropdown
			dropDownController.SetParentForScroll (gameObject.transform.parent.parent.parent);
			dropDownController.OnItemSelected += (position, title) => {
				OnChangeCountry (position);
			};
			dropDownController.SetData (countries.GetTitleList(), _countries.GetItemByKey(currentCountryIso).name);

		}

		public void UpdatePaymentMethods(XsollaPaymentMethods paymentMethods){
			paymenListView.SetPaymentMethods(paymentMethods);
		}
		public void OnChangeCountry(string key)
		{
			XsollaCountry country = _countries.GetItemByKey(key);
			dropDownContainer.SetActive (false);
			GetComponentInParent<PaymentListScreenController>().ChangeCountry(country.iso);
		}

		public void OnChangeCountry(int position)
		{
			XsollaCountry country = _countries.GetItemsList()[position];
			dropDownContainer.SetActive (false);
			GetComponentInParent<PaymentListScreenController>().ChangeCountry(country.iso);
		}

		public void SetUpNavButtons()
		{
			back.GetComponent<Button>().onClick.AddListener (() => { GetComponentInParent<PaymentListScreenController>().OpenQuickPayments(); });
		}

	}
}
