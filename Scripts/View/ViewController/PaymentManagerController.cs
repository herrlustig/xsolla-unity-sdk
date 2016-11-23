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

		private XsollaUtils mUtilsLink;

		public PaymentManagerController ()
		{
		}

		public Button GetAddAccountBtn()
		{
			return mBtnAddPaymentObj.GetComponent<Button>();
		}

		public void initScreen(XsollaUtils pUtils, XsollaSavedPaymentMethods pMethods)
		{
			mUtilsLink = pUtils;
			mTitle.text = pUtils.GetTranslations().Get("payment_account_page_title");
			mInformationTitle.text = pUtils.GetTranslations().Get("payment_account_add_title");
			mInformation.text = pUtils.GetTranslations().Get("payment_account_add_info");
			mContinueLink.text = pUtils.GetTranslations().Get("payment_account_back_button") + " >";
			mContinueLink.GetComponent<Button>().onClick.AddListener(delegate 
				{
					Logger.Log("Destroy history");
					Destroy(this.gameObject);	
				});
			Text textBtn = mBtnAddPaymentObj.GetComponentInChildren<Text>();
			textBtn.text = pUtils.GetTranslations().Get("payment_account_add_button");
			if (pMethods.GetCount() == 0)
			{
				mContainer.SetActive(false);
				mInfoPanel.SetActive(true);
			}
			else
			{
				mInfoPanel.SetActive(false);
				mContainer.SetActive(true);
				foreach (XsollaSavedPaymentMethod item in pMethods.GetItemList())
				{
					// Create prefab on btn saved method, set parent and set controller on them
					GameObject methodBtn = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/SavedMethodBtn")) as GameObject;
					methodBtn.transform.SetParent(mBtnGrid.transform);
					SavedMethodBtnController controller = methodBtn.GetComponent<SavedMethodBtnController>();

					// Activated btn delete
					controller.setDeleteBtn(true);
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
					controller.getBtnDelete().onClick.AddListener(() => deletePaymentMethod(methodBtn));
				}

				// Add button "Add payment metnod"
				Instantiate<GameObject>(mBtnAddPaymentObj).transform.SetParent(mBtnGrid.transform);
			}
				
		}

		private void initDeleteMethodPanel(GameObject pMethodObj)
		{
			// show edit panel
			mContainer.SetActive(false);
			mDelPanelMethod.SetActive(true);

			// clone object to panel edit
			GameObject methodPanel = Instantiate(pMethodObj);
			methodPanel.transform.SetParent(mPanelForDelMethod.transform);

			// set text 
			mTitle.text = mUtilsLink.GetTranslations().Get("delete_payment_account_page_title");
			mQuestionLabel.text = mUtilsLink.GetTranslations().Get("payment_account_delete_confirmation_question");
			mLinkCancel.GetComponent<Text>().text = mUtilsLink.GetTranslations().Get("cancel");
			mLinkDelete.GetComponent<Text>().text = mUtilsLink.GetTranslations().Get("delete_payment_account_button");
			mBtnReplace.GetComponentInChildren<Text>().text = mUtilsLink.GetTranslations().Get("replace_payment_account_button");

		}

		private void deletePaymentMethod(GameObject pMethodObj)
		{
			Logger.Log("Click Btn to Delete saved method");
			initDeleteMethodPanel(pMethodObj);
		}
	}
}

