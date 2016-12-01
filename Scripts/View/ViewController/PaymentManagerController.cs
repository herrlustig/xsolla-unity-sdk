using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Xsolla
{
	public class PaymentManagerController: MonoBehaviour
	{
		public Text mTitle;
		public Text mContinueLink;
		public GameObject mInfoPanel;
		public Text mInformationTitle;
		public Text mInformation;
		public GameObject mContainer;
		public GameObject mBtnGrid;
		public GameObject mBtnAddPaymentObj;
		public ImageLoader mImgLoader; 

		// for edit
		public GameObject mDelPanelMethod;
		public GameObject mPanelForDelMethod;
		public Text 	  mQuestionLabel;
		public GameObject mLinkCancel;
		public GameObject mLinkDelete;
		public GameObject mBtnReplace;
		public GameObject mDelStatusPanel;

		private XsollaUtils mUtilsLink;

		public PaymentManagerController ()
		{
		}

		public void initScreen(XsollaUtils pUtils, XsollaSavedPaymentMethods pMethods, Action pAddPaymentMethod)
		{
			mUtilsLink = pUtils;
			mTitle.text = pUtils.GetTranslations().Get("payment_account_page_title");
			mInformationTitle.text = pUtils.GetTranslations().Get("payment_account_add_title");
			mInformation.text = pUtils.GetTranslations().Get("payment_account_add_info");
			mContinueLink.text = pUtils.GetTranslations().Get("payment_account_back_button") + " >";

			Button continueBtn = mContinueLink.GetComponent<Button>();
			continueBtn.onClick.RemoveAllListeners();
			continueBtn.onClick.AddListener(delegate 
				{
					Destroy(this.gameObject);	
				});
			Text textBtn = mBtnAddPaymentObj.GetComponentInChildren<Text>();
			textBtn.text = pUtils.GetTranslations().Get("payment_account_add_button");

			// clear btn Grid
			for(int i = 0; i < mBtnGrid.transform.childCount; i++ )
			{
				Destroy(mBtnGrid.transform.GetChild(i).gameObject);
			}

			if (pMethods.GetCount() == 0)
			{
				mContainer.SetActive(false);
				mDelPanelMethod.SetActive(false);
				mInfoPanel.SetActive(true);

				Button btnAddPayment = mBtnAddPaymentObj.GetComponent<Button>();
				btnAddPayment.onClick.RemoveAllListeners();
				btnAddPayment.onClick.AddListener(() => pAddPaymentMethod());
			}
			else
			{
				mInfoPanel.SetActive(false);
				mDelPanelMethod.SetActive(false);
				mContainer.SetActive(true);
				foreach (XsollaSavedPaymentMethod item in pMethods.GetItemList())
				{
					// Create prefab on btn saved method, set parent and set controller on them
					GameObject methodBtn = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/SavedMethodBtn")) as GameObject;
					methodBtn.transform.SetParent(mBtnGrid.transform);
					SavedMethodBtnController controller = methodBtn.GetComponent<SavedMethodBtnController>();

					// Activated btn delete
					controller.setDeleteBtn(true);
					controller.setMethodBtn(false);
					controller.setDeleteBtnName(pUtils.GetTranslations().Get("delete_payment_account_button"));

					// Set btn property
					// Set method
					controller.setMethod(item);
					// Set name 
					controller.setNameMethod(item.GetName());
					// Set Type
					controller.setNameType(item.GetPsName());
					// Set icon
					mImgLoader.LoadImage(controller._iconMethod, item.GetImageUrl());		
					// Set BtnDelAction
					controller.getBtnDelete().onClick.AddListener(() => onClickDeletePaymentMethod(controller));
				}

				// Add button "Add payment metnod"
				GameObject objAddMethodClone = Instantiate<GameObject>(mBtnAddPaymentObj);
				objAddMethodClone.transform.SetParent(mBtnGrid.transform);
				//set onclickListener
				Button btnAddMethod = objAddMethodClone.GetComponent<Button>();
				btnAddMethod.onClick.RemoveAllListeners();
				btnAddMethod.onClick.AddListener(() => pAddPaymentMethod());

			}
				
		}

		public void SetStatusDeleteOk()
		{
			string statusText = mUtilsLink.GetTranslations().Get("payment_account_message_delete_account_successfully");
			StartCoroutine(ShowStatusBar(statusText,3));
		}

		private IEnumerator ShowStatusBar(string pStatus,float time)
		{
			mDelStatusPanel.GetComponentInChildren<Text>().text = pStatus;
			mDelStatusPanel.SetActive(true);
			yield return new WaitForSeconds(time);
			mDelStatusPanel.SetActive(false);
		}

		private void initDeleteMethodPanel(SavedMethodBtnController pMethodObj)
		{
			// show edit panel
			mContainer.SetActive(false);
			mDelPanelMethod.SetActive(true);

			// clone object to panel edit
			SavedMethodBtnController controller = Instantiate(pMethodObj);
			controller.setMethod(pMethodObj.getMethod());
			controller.setDeleteBtn(false);
			controller.setMethodBtn(false);

			RectTransform methodPanelrecttransform = controller.GetComponent<RectTransform>();
			// clear currency state
			if (mPanelForDelMethod.transform.hasChanged)
			{
				for(int i = 0; i < mPanelForDelMethod.transform.childCount; i++)
				{
					Logger.Log("Destroy child on panel for edit saved payment method with ind - " + i);
					Destroy(mPanelForDelMethod.transform.GetChild(i).gameObject);
				}
			}

			controller.transform.SetParent(mPanelForDelMethod.transform);
			methodPanelrecttransform.anchorMin = new Vector2(0, 0);
			methodPanelrecttransform.anchorMax = new Vector2(1, 1);
			methodPanelrecttransform.pivot = new Vector2(0.5f, 0.5f);
			methodPanelrecttransform.offsetMin = new Vector2(0,0);
			methodPanelrecttransform.offsetMax = new Vector2(0,0);

			// set text 
			mTitle.text = mUtilsLink.GetTranslations().Get("delete_payment_account_page_title");
			mQuestionLabel.text = mUtilsLink.GetTranslations().Get("payment_account_delete_confirmation_question");

			mLinkCancel.GetComponent<Text>().text = mUtilsLink.GetTranslations().Get("cancel");
			mLinkCancel.GetComponent<Button>().onClick.RemoveAllListeners();
			mLinkCancel.GetComponent<Button>().onClick.AddListener(() => onClickCancelEditMethod());
	
			mLinkDelete.GetComponent<Text>().text = mUtilsLink.GetTranslations().Get("delete_payment_account_button");
			mLinkDelete.GetComponent<Button>().onClick.RemoveAllListeners();
			mLinkDelete.GetComponent<Button>().onClick.AddListener(() => onClickConfirmDeletePaymentMethod(controller.getMethod()));

			mBtnReplace.GetComponentInChildren<Text>().text = mUtilsLink.GetTranslations().Get("replace_payment_account_button");
			mBtnReplace.GetComponent<Button>().onClick.RemoveAllListeners();
			mBtnReplace.GetComponent<Button>().onClick.AddListener(() => onCliceReplacePeymentMethod(controller.getMethod()));

		}

		private void onCliceReplacePeymentMethod(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Click replace method");
		}

		private void onClickConfirmDeletePaymentMethod(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Delete payment method");
			Dictionary<String, object> reqParams = new Dictionary<string, object>();

			reqParams.Add("id", pMethod.GetKey());
			reqParams.Add("type", pMethod.GetMethodType());

			XsollaPaystationController controller = gameObject.GetComponentInParent<XsollaPaystationController>();
			controller.DeleteSavedPaymentMethod(reqParams);

// 			https://secure.xsolla.com/paystation2/api/savedmethods/delete
//			access_token:W8SaoL896NvrC3NfhN42gS7tgklgC2lN
//			id:1957827
//			type:paypal

		}

		private void onClickCancelEditMethod()
		{
			Logger.Log("Cancel edit method");

			mInfoPanel.SetActive(false);
			mDelPanelMethod.SetActive(false);
			mContainer.SetActive(true);
		}

		private void onClickDeletePaymentMethod(SavedMethodBtnController pMethodObj)
		{
			Logger.Log("Click Btn to Delete saved method");
			initDeleteMethodPanel(pMethodObj);
		}
	}
}

