using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;

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
		public Button mCloseNotify;

		// wait screen
		public GameObject mWaitChangeScreen;
		public GameObject mCancelWaitBtn;
		public Text       mCanceltext;
		public MyRotation mProgressBar;

		private XsollaUtils mUtilsLink;
		private Action 		mActionAddPayment;
		private ArrayList 	mListBtnsObjs;
		private Action mOnClose;
		public XsollaSavedPaymentMethods mListMethods { get;  set;}


		private ArrayList mListReplacedMethods;
		private XsollaSavedPaymentMethod mSelectedMethod;

		public PaymentManagerController ()
		{
		}

		public void setOnCloseMethod(Action pAction)
		{
			mOnClose = pAction;
		}

		private Action _addPaymentMethod;
		private XsollaSavedPaymentMethods _MethodsOnWaitLoop = null;

		public void initWaitScreen(XsollaUtils pUtils, Action pAddPaymentMethod)
		{
			mUtilsLink = pUtils;
			_addPaymentMethod = pAddPaymentMethod;
			mWaitChangeScreen.SetActive(true);
			mProgressBar.SetLoading(true);
			_MethodsOnWaitLoop = null;

			// Start wait change loop
			InvokeRepeating("StartGetSavedMethodLoop", 0f, 5f);
			mCancelWaitBtn.GetComponent<Button>().onClick.AddListener(() => CancelWait());
			mCanceltext.text = pUtils.GetTranslations().Get("cancel");
		}

		private void StartGetSavedMethodLoop()
		{
			GetSavedMethod();
		}

		private void CancelWait()
		{
			mProgressBar.SetLoading(false);
			mWaitChangeScreen.SetActive(false);
			CancelInvoke("StartGetSavedMethodLoop");
			initScreen(mUtilsLink, _MethodsOnWaitLoop, _addPaymentMethod, false);
		}

		private void GetSavedMethod(bool pAddState = false, bool pInitAfter = false)
		{
			WWWForm form = new WWWForm();
			string url = "https://secure.xsolla.com/paystation2/api/savedmethods";
			Dictionary<string, object> post = new Dictionary<string, object>();
			StringBuilder sb = new StringBuilder ();

			post.Add(XsollaApiConst.ACCESS_TOKEN, mUtilsLink.GetAcceessToken());

			foreach(KeyValuePair<string,object> post_arg in post)
			{
				string argValue = post_arg.Value != null ? post_arg.Value.ToString() : "";
				sb.Append(post_arg.Key).Append("=").Append(argValue).Append("&");
				form.AddField(post_arg.Key, argValue);
			}

			Debug.Log (url);
			Debug.Log (sb.ToString());
			WWW www = new WWW(url, form);
			StartCoroutine(GetListSavedMethod(www, pInitAfter, pAddState));
		}

		private IEnumerator GetListSavedMethod(WWW www, bool pInitAfter, bool pAddMethod)
		{
			Debug.Log("Wait saved account list");
			yield return www;
			// check for errors
			if (www.error == null)
			{
				Debug.Log("WWW_request -> " + www.text);
				JSONNode rootNode = JSON.Parse(www.text);
				if(rootNode != null && rootNode.Count > 2 || rootNode["error"] == null) 
				{
					XsollaSavedPaymentMethods tempOnWaitLoop = new XsollaSavedPaymentMethods();
					tempOnWaitLoop.Parse(rootNode);
					if (pInitAfter)
					{
						initScreen(mUtilsLink, tempOnWaitLoop, mActionAddPayment, false);
						if (pAddMethod)
							SetStatusAddOk();
						else
							SetStatusReplaceOk();
						yield break;
					}
					if (_MethodsOnWaitLoop == null)
						_MethodsOnWaitLoop = tempOnWaitLoop;
					else
					{
						if (tempOnWaitLoop.Equals(_MethodsOnWaitLoop))
							_MethodsOnWaitLoop = tempOnWaitLoop;
						else
						{
							Logger.Log("Stop wait end show result");
							_MethodsOnWaitLoop = tempOnWaitLoop;
							SetStatusAddOk();
							CancelWait();
						}
					}
				}
			}
		}

		public void initScreen(XsollaUtils pUtils, XsollaSavedPaymentMethods pMethods, Action pAddPaymentMethod, bool pAddState)
		{
			mUtilsLink = pUtils;
			mActionAddPayment = pAddPaymentMethod;

			if (pMethods != null)
				mListMethods = pMethods;
			else
			{
				GetSavedMethod(pAddState ,true);
				return;
			}

			if (mListBtnsObjs == null)
				mListBtnsObjs = new ArrayList();
			else
				mListBtnsObjs.Clear();
			
			mTitle.text = pUtils.GetTranslations().Get("payment_account_page_title");
			mInformationTitle.text = pUtils.GetTranslations().Get("payment_account_add_title");
			mInformation.text = pUtils.GetTranslations().Get("payment_account_add_info");
			mContinueLink.text = pUtils.GetTranslations().Get("payment_account_back_button");
			mCanceltext.text = pUtils.GetTranslations().Get("cancel");
			mCloseNotify.onClick.AddListener(CloseStatus);

			Button continueBtn = mContinueLink.GetComponent<Button>();
			continueBtn.onClick.RemoveAllListeners();
			continueBtn.onClick.AddListener(() => 
				{
					Destroy(this.gameObject);
					mOnClose();
				});
			Text textBtn = mBtnAddPaymentObj.GetComponentInChildren<Text>();
			textBtn.text = pUtils.GetTranslations().Get("payment_account_add_button");

			// clear btn Grid
			for(int i = 0; i < mBtnGrid.transform.childCount; i++ )
			{
				Destroy(mBtnGrid.transform.GetChild(i).gameObject);
			}

			if (mListMethods.GetCount() == 0)
			{
				mContainer.SetActive(false);
				mDelPanelMethod.SetActive(false);
				mReplacePanelMethod.SetActive(false);
				mInfoPanel.SetActive(true);

				Button btnAddPayment = mBtnAddPaymentObj.GetComponent<Button>();
				btnAddPayment.onClick.RemoveAllListeners();
				btnAddPayment.onClick.AddListener(() => { CloseStatus(); mActionAddPayment();});
			}
			else
			{
				mInfoPanel.SetActive(false);
				mDelPanelMethod.SetActive(false);
				mReplacePanelMethod.SetActive(false);
				mContainer.SetActive(true);
				foreach (XsollaSavedPaymentMethod item in mListMethods.GetItemList())
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
					controller.getBtnDelete().onClick.AddListener(() => { CloseStatus(); onClickDeletePaymentMethod(controller);});
				}

				// Add button "Add payment metnod"
				GameObject objAddMethodClone = Instantiate<GameObject>(mBtnAddPaymentObj);
				objAddMethodClone.transform.SetParent(mBtnGrid.transform);
				//set onclickListener
				Button btnAddMethod = objAddMethodClone.GetComponent<Button>();
				btnAddMethod.onClick.RemoveAllListeners();
				btnAddMethod.onClick.AddListener(() => { CloseStatus(); mActionAddPayment();});

			}
		}

		public void SetStatusDeleteOk()
		{
			string statusText = mUtilsLink.GetTranslations().Get("payment_account_message_delete_account_successfully");
			ShowStatusBar(statusText);
		}

		public void SetStatusAddOk()
		{
			string statusText = mUtilsLink.GetTranslations().Get("payment_account_message_success");
			ShowStatusBar(statusText);
		}

		public void SetStatusReplaceOk()
		{
			string statusText = mUtilsLink.GetTranslations().Get("payment_account_message_success_replace");
			ShowStatusBar(statusText);
		}
			
		private void ShowStatusBar(string pStatus)
		{
			mDelStatusPanel.GetComponentInChildren<Text>().text = pStatus;
			mDelStatusPanel.SetActive(true);
		}

		private void CloseStatus()
		{
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
			linkAnotherMethod.onClick.AddListener(() => onClickConfirmReplacedAnotherMethod(pMethod));

			mLinkBack.GetComponent<Text>().text = mUtilsLink.GetTranslations().Get("back_to_paymentaccount");
			Button linkBack = mLinkBack.GetComponent<Button>();
			linkBack.onClick.RemoveAllListeners();
			linkBack.onClick.AddListener(() => onClickCancelEditMethod());

			mBtnContinue.GetComponentInChildren<Text>().text = mUtilsLink.GetTranslations().Get("form_continue");
			Button btnContinue = mBtnContinue.GetComponent<Button>();
			btnContinue.onClick.RemoveAllListeners();
			btnContinue.onClick.AddListener(() => onClickConfirmReplaced(pMethod));

		}

		private void onToggleChange(string pMethodKey, bool pState)
		{
			Logger.Log("Method with key " + pMethodKey + " get state " + pState.ToString());
			foreach(SavedMethodBtnController method in mListReplacedMethods)
			{
				if ((method.getMethod().GetKey() == pMethodKey) && (pState))
				{
					mSelectedMethod = method.getMethod();
					continue;
				}
				method.setToggleState(false);
			}
		}

		private void onClickConfirmReplaced(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Raplaced existing method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("id_payment_account", pMethod.GetKey());

			reqParams.Add("saved_method_id", mSelectedMethod.GetKey());
			reqParams.Add("pid", mSelectedMethod.GetPid());
			reqParams.Add("paymentWithSavedMethod", 1);
			reqParams.Add("paymentSid", pMethod.GetFormSid());
			reqParams.Add("type_payment_account", pMethod.GetMethodType());

			Dictionary<string, object> replacedParam = new Dictionary<string, object>();
			replacedParam.Add("replace_payment_account", 1);

			XsollaPaystationController payController = GetComponentInParent<XsollaPaystationController> ();
			payController.FillPurchase(ActivePurchase.Part.PAYMENT_MANAGER_REPLACED, replacedParam);
			payController.ChoosePaymentMethod (reqParams);
		}

		private void onClickConfirmReplacedAnotherMethod(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Raplaced existing method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();
			reqParams.Add("id_payment_account", pMethod.GetKey());
			reqParams.Add("replace_payment_account", 1);
			reqParams.Add("type_payment_account", pMethod.GetMethodType());

			XsollaPaystationController payController = GetComponentInParent<XsollaPaystationController> ();
			payController.ChooseItem(reqParams);
		}

		private void onClickConfirmDeletePaymentMethod(XsollaSavedPaymentMethod pMethod)
		{
			Logger.Log("Delete payment method");
			Dictionary<string, object> reqParams = new Dictionary<string, object>();

			reqParams.Add("id", pMethod.GetKey());
			reqParams.Add("type", pMethod.GetMethodType());

			XsollaPaystationController controller = gameObject.GetComponentInParent<XsollaPaystationController>();
			controller.DeleteSavedPaymentMethod(reqParams);
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

//		public void WaitChangeLoop()
//		{
//			WWWForm form = new WWWForm();
//			string url = "https://secure.xsolla.com/paystation2/api/savedmethods";
//			Dictionary<string, object> post = new Dictionary<string, object>();
//			StringBuilder sb = new StringBuilder ();
//
//			post.Add(XsollaApiConst.ACCESS_TOKEN, mUtilsLink.GetAcceessToken());
//
//			foreach(KeyValuePair<string,object> post_arg in post)
//			{
//				string argValue = post_arg.Value != null ? post_arg.Value.ToString() : "";
//				sb.Append(post_arg.Key).Append("=").Append(argValue).Append("&");
//				form.AddField(post_arg.Key, argValue);
//			}
//
//			Debug.Log (url);
//			Debug.Log (sb.ToString());
//			WWW www = new WWW(url, form);
//			StartCoroutine(GetListSavedMethod(www));
//		}
//
//		private IEnumerator GetListSavedMethod(WWW www)
//		{
//			Debug.Log("Wait saved account list");
//			yield return www;
//			// check for errors
//			if (www.error == null)
//			{
//				Debug.Log("WWW_request -> " + www.text);
//			}
//		}
	}
}

