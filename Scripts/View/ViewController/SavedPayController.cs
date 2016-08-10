﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Xsolla
{
	public class SavedPayController : MonoBehaviour
	{
		public ImageLoader imageLoader;
		public GameObject title;
		public GameObject methodsGrid;
		public GameObject showQuickPaymentMethods;
		public GameObject back;
		public GameObject self;

		private XsollaUtils utilsLink;

		public void InitScreen(XsollaUtils utils)
		{
			utilsLink = utils;
			title.GetComponent<Text>().text = utilsLink.GetTranslations().Get(XsollaTranslations.SAVEDMETHOD_PAGE_TITLE);
			showQuickPaymentMethods.GetComponent<Text>().text = utilsLink.GetTranslations().Get(XsollaTranslations.PAYMENT_LIST_SHOW_QUICK);
			back.GetComponent<Text>().text = utilsLink.GetTranslations().Get(XsollaTranslations.BACK_TO_SPECIALS);
		}

		public void SetSavedMethods(XsollaSavedPaymentMethods pMethods)
		{
			if (pMethods != null)
			{
				List<XsollaSavedPaymentMethod> paymentList = pMethods.GetItemList();
				// For each method we create btn
				foreach(XsollaSavedPaymentMethod method in paymentList)
				{
					CreateMethodBtn(method);
				}
				self.gameObject.SetActive(true);
				SetUpNavButtons();
			}
			else
			{
				// if methods list is empty, we hide all window
				self.gameObject.SetActive(false);
			}
		}

		private void CreateMethodBtn(XsollaSavedPaymentMethod pMethod)
		{
			// Create object
			GameObject methodBtn = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/SavedMethodBtn")) as GameObject;
			methodBtn.transform.SetParent(methodsGrid.transform);
			SavedMethodBtnController controller = methodBtn.GetComponent<SavedMethodBtnController>();

			// Set method
			controller.setMethod(pMethod);
			// Set name 
			controller._nameMethod.text = pMethod.GetName();
			// Set Type
			controller._nameType.text = pMethod.GetPsName();
			// Set icon
			imageLoader.LoadImage(controller._iconMethod, pMethod.GetImageUrl());		
			// Set BtnList
			controller._btnMethod.onClick.AddListener(() => onMethodClick(controller.getMethod()));
		}

		private void onMethodClick(XsollaSavedPaymentMethod pMethod)
		{
			Dictionary<string, object> purchase = new Dictionary<string, object>();
			purchase.Add("saved_method_id", pMethod.GetKey());
			purchase.Add("pid", pMethod.GetPid());
			purchase.Add("paymentWithSavedMethod", 1);
			purchase.Add("paymentSid", pMethod.GetFormSid());
			purchase.Add("userInitialCurrency", pMethod.GetCurrency());
			GetComponentInParent<XsollaPaystationController> ().ChoosePaymentMethod (purchase);
		}
			
		public void SetUpNavButtons()
		{
			showQuickPaymentMethods.GetComponent<Button>().onClick.AddListener (() => { 
				GetComponentInParent<PaymentListScreenController>().OpenQuickPayments();
			});
			back.GetComponent<Button>().onClick.AddListener (() => { 
				GetComponentInParent<XsollaPaystationController>().LoadShopPricepoints();
			});
		}

	}
}

