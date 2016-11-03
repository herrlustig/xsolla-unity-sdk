using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Xsolla{
	public class QuickPaymentsController : MonoBehaviour {

		public ImageLoader imageLoader;
		//public GameObject[] quiсk;// max 2
		public GameObject[] popular;//max 6
		public GameObject quickPanel;
		public GameObject recPanel;
		public GameObject showMore;
		public GameObject back;
		public GameObject title;

		private XsollaUtils utilsLink;
		private int countQuickBtn = 3;
		private int countPopBtn = 6;
		private List<QuickPaymentBtnController> listQuickBtns;
		private List<ShopPaymentBtnController> listPopularBtns;

		public void DrawLayout(XsollaPaymentMethods paymentMethods){
		}

		public void InitScreen(XsollaUtils utils)
		{
			utilsLink = utils;
			showMore.GetComponent<Text>().text = utilsLink.GetTranslations().Get(XsollaTranslations.PAYMENT_LIST_SHOW_MORE);
			back.GetComponent<Text>().text = utilsLink.GetTranslations().Get(XsollaTranslations.BACK_TO_SPECIALS);
		}

		public void SetQuickMethods(List<XsollaPaymentMethod> pQuickPayments)
		{
			if (pQuickPayments != null)
			{
				if (listQuickBtns == null)
					listQuickBtns = new List<QuickPaymentBtnController>();
				else
					ClearBtnQuickContainer();

				List<XsollaPaymentMethod> paymentList = pQuickPayments;
				for (int i=0; i < countQuickBtn; i++)
				{
					if (i < paymentList.Count)
					{
						CreateQuickBtn(paymentList[i]);	
					}
					else
					{
						CreateQuickBtn(null);	
					}
				}
			}
		}

		private void ClearBtnQuickContainer()
		{
			listQuickBtns.ForEach(delegate(QuickPaymentBtnController btn)
				{
					Destroy (btn._self);
				});
			listQuickBtns.Clear();
		}

		private void CreateQuickBtn(XsollaPaymentMethod pMethod)
		{
			GameObject quickBtn = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/QuickPaymentBtn")) as GameObject;
			quickBtn.transform.SetParent(quickPanel.transform);
			QuickPaymentBtnController controller = quickBtn.GetComponent<QuickPaymentBtnController>();
			listQuickBtns.Add(controller);
			if (pMethod == null)
			{
				controller.Hide();
				return;
			}

			// Set method
			controller.setMethod(pMethod);
			// Set name 
			controller.setLable(pMethod.GetName());
			// Set icon
			controller.setIcon(pMethod.id, imageLoader);
			controller._btnMethod.onClick.AddListener(() => OnChoosePaymentMethod(controller.getMethod().id));
		}
			
		public void SetAllMethods(List<XsollaPaymentMethod> paymentMethods)
		{
			if (paymentMethods != null)
			{
				if (listPopularBtns == null)
					listPopularBtns = new List<ShopPaymentBtnController>();
				else
					ClearBtnPopularConatiner();

				for (int i = 0; i < countPopBtn; i++)
				{
					CreatePopularBtn(paymentMethods[i]);
				}
			}
			SetUpNavButtons ();
			return;
		}

		private void CreatePopularBtn(XsollaPaymentMethod pMethod)
		{
			GameObject popularBtn = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/ShopPaymentBtn")) as GameObject;
			popularBtn.transform.SetParent(recPanel.transform);
			ShopPaymentBtnController controller = popularBtn.GetComponent<ShopPaymentBtnController>();

			if (!pMethod.isVisible)
			{
				controller.Hide();
				return;
			}

			listPopularBtns.Add(controller);
			// Set method
			controller.setMethod(pMethod);
			// Set icon
			controller.setIcon(imageLoader);
			controller._btn.onClick.AddListener(() => OnChoosePaymentMethod(controller.getMethod().id));
		}

		private void ClearBtnPopularConatiner()
		{
			listPopularBtns.ForEach(delegate(ShopPaymentBtnController btn)
				{
					Destroy (btn._self);
				});
			listPopularBtns.Clear();
		}

		public void SetUpNavButtons()
		{
			showMore.GetComponent<Button>().onClick.AddListener (() => { 
				GetComponentInParent<PaymentListScreenController>().OpenAllPayments();
			});
			back.GetComponent<Button>().onClick.AddListener (() => { 
				GetComponentInParent<XsollaPaystationController>().LoadShopPricepoints();
			});
		}

		public void OnChoosePaymentMethod(long paymentMethodId)
		{
			GetComponentInParent<PaymentListScreenController> ().ChoosePaymentMethod (paymentMethodId);
		}

		public void ShowMorePaymentMethod()
		{

		}


		public void Back()
		{

		}

	}
}
