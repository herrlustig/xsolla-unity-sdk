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

		// for replace
		public GameObject mReplacePanelMethod;
		public GameObject mPanelForReplacedMethods;
		public GameObject mLinkGetAnotherMethods;
		public GameObject mLinkBack;
		public GameObject mBtnContinue;

		// for edit
		public GameObject mDelPanelMethod;
		public GameObject mPanelForDelMethod;
		public Text 	  mQuestionLabel;
		public GameObject mLinkCancel;
		public GameObject mLinkDelete;
		public GameObject mBtnReplace;
		public GameObject mDelStatusPanel;

		private XsollaUtils mUtilsLink;
		private ArrayList 	mListBtnsObjs;
		private XsollaPaystationController.ActiveScreen mPrevScreen;
		private Action<XsollaPaystationController.ActiveScreen> mOnClose;

		private ArrayList mListReplacedMethods;
		private string mSelectedMethod;


		public PaymentManagerController ()
		{
		}

		public void setPrevScreen(XsollaPaystationController.ActiveScreen pScreen)
		{
			mPrevScreen = pScreen;
		}

		public void setOnCloseMethod(Action<XsollaPaystationController.ActiveScreen> pAction)
		{
			mOnClose = pAction;
		}

		public void initScreen(XsollaUtils pUtils, XsollaSavedPaymentMethods pMethods, Action pAddPaymentMethod)
		{
			if (mListBtnsObjs == null)
				mListBtnsObjs = new ArrayList();
			else
				mListBtnsObjs.Clear();
			
			mUtilsLink = pUtils;
			mTitle.text = pUtils.GetTranslations().Get("payment_account_page_title");
			mInformationTitle.text = pUtils.GetTranslations().Get("payment_account_add_title");
			mInformation.text = pUtils.GetTranslations().Get("payment_account_add_info");
			mContinueLink.text = pUtils.GetTranslations().Get("payment_account_back_button") + " >";

			Button continueBtn = mContinueLink.GetComponent<Button>();
			continueBtn.onClick.RemoveAllListeners();
			continueBtn.onClick.AddListener(() => 
				{
					Destroy(this.gameObject);
					mOnClose(mPrevScreen);
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
				mReplacePanelMethod.SetActive(false);
				mInfoPanel.SetActive(true);

				Button btnAddPayment = mBtnAddPaymentObj.GetComponent<Button>();
				btnAddPayment.onClick.RemoveAllListeners();
				btnAddPayment.onClick.AddListener(() => pAddPaymentMethod());
			}
			else
			{
				mInfoPanel.SetActive(false);
				mDelPanelMethod.SetActive(false);
				mReplacePanelMethod.SetActive(false);
				mContainer.SetActive(true);
				foreach (XsollaSavedPaymentMethod item in pMethods.GetItemList())
				{
					// Create prefab on btn saved method, set parent and set controller on them
					GameObject methodBtn = Instantiate(Resources.Load("Prefabs/SimpleView/_PaymentFormElements/SavedMethodBtnNew")) as GameObject;
					methodBtn.transform.SetParent(mBtnGrid.transform);

					// Add objects btn on list
					mListBtnsObjs.Add(methodBtn);

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
			mReplacePanelMethod.SetActive(false);
			mDelPanelMethod.SetActive(true);

			// clone object to panel edit
			SavedMethodBtnController controller = Instantiate(pMethodObj);
			controller.setMethod(pMethodObj.getMethod());
			controller.setDeleteBtn(false);
			controller.setMethodBtn(false);

			RectTransform methodPanelrecttransform = controller.GetComponent<RectTransform>();
			// clear currency state
			for(int i = 0; i < mPanelForDelMethod.transform.childCount; i++)
			{
				Logger.Log("Destroy child on panel for edit saved payment method with ind - " + i);
				Destroy(mPanelForDelMethod.transform.GetChild(i).gameObject);
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
			mBtnReplace.GetComponent<Button>().onClick.AddListener(() => onClickReplacePeymentMethod(controller.getMethod()));
		}

		private void onClickReplacePeymentMethod(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Click replace method");
			mListReplacedMethods = new ArrayList();

			// open form with replaced methods
			mInfoPanel.SetActive(false);
			mDelPanelMethod.SetActive(false);
			mContainer.SetActive(false);
			mReplacePanelMethod.SetActive(true);

			for(int i = 0; i < mPanelForReplacedMethods.transform.childCount; i++)
			{
				Logger.Log("Destroy child on panel for edit saved payment method with ind - " + i);
				Destroy(mPanelForReplacedMethods.transform.GetChild(i).gameObject);
			}

			// TODO
			// if we don't have account on replace we must click on another method


			// set all 
			foreach (GameObject btnObj in mListBtnsObjs)
			{
				// check if this method not method those replaced
				SavedMethodBtnController controller = btnObj.GetComponent<SavedMethodBtnController>();
				if (controller.getMethod().GetKey() == pMethod.GetKey())
					continue;

				// add this obj on panel
				SavedMethodBtnController controllerClone = Instantiate(controller);
				controllerClone.setMethod(controller.getMethod());
				controllerClone.setDeleteBtn(false);
				controllerClone.setMethodBtn(false);
				controllerClone.setToggleObj(true, onToggleChange);

				controllerClone.transform.SetParent(mPanelForReplacedMethods.transform);
				RectTransform methodPanelrecttransform = controllerClone.GetComponent<RectTransform>();
				methodPanelrecttransform.anchorMin = new Vector2(0, 0);
				methodPanelrecttransform.anchorMax = new Vector2(1, 1);
				methodPanelrecttransform.pivot = new Vector2(0.5f, 0.5f);
				methodPanelrecttransform.offsetMin = new Vector2(0,0);
				methodPanelrecttransform.offsetMax = new Vector2(0,0);

				mListReplacedMethods.Add(controllerClone);
			}

			// set titles for replace screen
			mLinkGetAnotherMethods.GetComponent<Text>().text = mUtilsLink.GetTranslations().Get("savedmethod_other_change_account_label");
			Button linkAnotherMethod = mLinkGetAnotherMethods.GetComponent<Button>();
			linkAnotherMethod.onClick.RemoveAllListeners();
			linkAnotherMethod.onClick.AddListener(() => onClickConfirmReplacedAnotherMethod(pMethod.GetKey()));

			mLinkBack.GetComponent<Text>().text = mUtilsLink.GetTranslations().Get("back_to_paymentaccount");
			Button linkBack = mLinkBack.GetComponent<Button>();
			linkBack.onClick.RemoveAllListeners();
			linkBack.onClick.AddListener(() => onClickCancelEditMethod());

			mBtnContinue.GetComponentInChildren<Text>().text = mUtilsLink.GetTranslations().Get("form_continue");
			Button btnContinue = mBtnContinue.GetComponent<Button>();
			btnContinue.onClick.RemoveAllListeners();
			btnContinue.onClick.AddListener(() => onClickConfirmReplaced(pMethod.GetKey()));

		}

		private void onToggleChange(string pMethodKey, bool pState)
		{
			Logger.Log("Method with key " + pMethodKey + " get state " + pState.ToString());
			foreach(SavedMethodBtnController method in mListReplacedMethods)
			{
				if ((method.getMethod().GetKey() == pMethodKey) && (pState))
				{
					mSelectedMethod = pMethodKey;
					continue;
				}
				method.setToggleState(false);
			}
		}

		private void onClickConfirmReplaced(string pMethodKey)
		{
			Logger.Log("Raplaced existing method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("id_payment_account", pMethodKey);
			reqParams.Add("saved_method_id", mSelectedMethod);
			reqParams.Add("paymentWithSavedMethod", "1");

			XsollaPaystationController controller = gameObject.GetComponentInParent<XsollaPaystationController>();
			controller.ReplacedOnSavedMethod(reqParams);
		}

		private void onClickConfirmReplacedAnotherMethod(string pMethodKey)
		{
			Logger.Log("Raplaced existing method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("id_payment_account", pMethodKey);

			// load all methods 

		}

		private void onClickConfirmDeletePaymentMethod(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Delete payment method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();

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
			mReplacePanelMethod.SetActive(false);
			mContainer.SetActive(true);

			// set title on main screen 
			mTitle.text = mUtilsLink.GetTranslations().Get("payment_account_page_title");
		}

		private void onClickDeletePaymentMethod(SavedMethodBtnController pMethodObj)
		{
			Logger.Log("Click Btn to Delete saved method");
			initDeleteMethodPanel(pMethodObj);
		}
	}
}

