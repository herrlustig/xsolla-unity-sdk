using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public class SubsManagerController: MonoBehaviour
	{
		public Text mTitleScreen;
		public Text mContinueText;
		public Button mContinueLink;
		public Text mLabel;
		public GameObject mSubsContainer;

		private XsollaUtils mUtils;
		private const String mBtnPrefab = "Prefabs/SimpleView/_PaymentFormElements/SubManagerBtn";

		public SubsManagerController ()
		{
		}

		public void initScreen(XsollaUtils pUtils, XsollaManagerSubscriptions pSubsList)
		{
			mUtils = pUtils;
			mTitleScreen.text = mUtils.GetTranslations().Get("user_menu_user_subscription");
			mContinueText.text = mUtils.GetTranslations().Get("balance_back_button");
			mLabel.text = mUtils.GetTranslations().Get("user_subscription_list_subtitle");

			mContinueLink.onClick.AddListener(OnClickBackShop);

			foreach (XsollaManagerSubscription sub in pSubsList.GetItemsList())
			{
				addSubBtn(sub);
			}
		}

		private void OnClickBackShop()
		{
			Logger.Log("On click back shop");
		}

		private void addSubBtn(XsollaManagerSubscription pSub)
		{
			GameObject objBtn = Instantiate(Resources.Load(mBtnPrefab)) as GameObject;
			SubManagerBtnController controller = objBtn.GetComponent<SubManagerBtnController>();
			controller.init(pSub, mUtils.GetTranslations());
			controller.SetDetailAction(onDetailBtnClick);
			objBtn.transform.SetParent(mSubsContainer.transform);
		}

		private void onDetailBtnClick(XsollaManagerSubscription pSub)
		{
			Logger.Log("On Detail click. Id: " + pSub.GetKey());
		}

		private void GetDetailSub(int pSubId)
		{
			Dictionary<string, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("subscription_id",pSubId);
			lParams.Add("userInitialCurrency",pSubId);

			String lUrl = "";
			WWWForm lForm = new WWWForm();

			WWW lwww = new WWW(lUrl, lForm);
		
			StartCoroutine(getSubscriptionDetail(lwww));

		}


		private IEnumerator getSubscriptionDetail(WWW pWww)
		{
			yield return pWww;


		}

	}
}

