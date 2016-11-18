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

		public PaymentManagerController ()
		{
		}

		public Button GetAddAccountBtn()
		{
			return mBtnAddPaymentObj.GetComponent<Button>();
		}

		public void initScreen(XsollaUtils pUtils, XsollaSavedPaymentMethods pMethods)
		{
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
					controller.getBtnDelete().onClick.AddListener(() => deletePaymentMethod(controller.getMethod()));
				}

				// Add button "Add payment metnod"
				Instantiate<GameObject>(mBtnAddPaymentObj).transform.SetParent(mBtnGrid.transform);
			}
				
		}

		private void deletePaymentMethod(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Click Btn to Delete saved method");
		}
	}
}

