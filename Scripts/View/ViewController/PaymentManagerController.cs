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
		public GameObject mBtnAddPaymentObj;

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
			mContinueLink.text = pUtils.GetTranslations().Get("payment_account_back_button");
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
					
				}
			}
				
		}
	}
}

