using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public class SubManagerDetailChargesPartController: MonoBehaviour
	{
		public Text mPartTitle;
		public GameObject mDetailsContainer;
		public GameObject mMorePanel;
		public Text mShowMoreLabel;
		public Button mShowMoreBtn;


		private const String mChargeItemTextPrefab = "Prefabs/Screens/SubsManager/Simple/ChargeItem";
		private XsollaUtils mUtils;
		private XsollaManagerSubDetails mSubDetail;
		private List<GameObject> mListCharges;
		private bool mShowState = false;

		public void init(XsollaManagerSubDetails pSubDetail, XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mSubDetail = pSubDetail;
			mListCharges = new List<GameObject>();

			mPartTitle.text = mUtils.GetTranslations().Get("user_subscription_charges_title");

			String lDateTitle = mUtils.GetTranslations().Get("virtualstatus_check_time");
			String lPaymenyTypeTitle = mUtils.GetTranslations().Get("user_subscription_payment_header");
			String lAmountTitle = mUtils.GetTranslations().Get("user_subscription_payment");

			// добавляем заголовки 
			addChargeElem(lDateTitle, lPaymenyTypeTitle, lAmountTitle, true);

			mSubDetail.mCharges.Sort(delegate(XsollaSubDetailCharge charge1, XsollaSubDetailCharge charge2) 
				{
					if (charge1.mDateCreate < charge2.mDateCreate)
						return 1;
					else
						return -1;
				});

			mMorePanel.SetActive(mSubDetail.mCharges.Count > 4);
			mShowMoreBtn.onClick.AddListener(OnMoreBtnAction);

			// добавляем историю
			foreach (XsollaSubDetailCharge charge in mSubDetail.mCharges)
			{
				mListCharges.Add(addChargeElem(StringHelper.DateFormat(charge.mDateCreate), charge.mPaymentMethod, charge.mCharge.ToString()));	
			}

			ShowList(mShowState);
		}

		private GameObject addChargeElem(String pDate, String pPaymetntype, String pAmount, bool pIsTitle = false)
		{
			GameObject obj = Instantiate(Resources.Load(mChargeItemTextPrefab)) as GameObject;
			SubManagerChargeElemController controller = obj.GetComponent<SubManagerChargeElemController>() as SubManagerChargeElemController;
			controller.init(pDate, pPaymetntype, pAmount, pIsTitle);
			controller.transform.SetParent(mDetailsContainer.transform);
			return obj;
		}

		private void OnMoreBtnAction()
		{
			ShowList(!mShowState);
		}

		private void ShowList(bool pMore)
		{
			if (pMore)
				mListCharges.ForEach((obj) => { obj.SetActive(true);});
			else
			{
				if (mListCharges.Count > 4)
					mListCharges.GetRange(3, mListCharges.Count - 3).ForEach((obj) => { obj.SetActive(false);});
			}
			mShowMoreLabel.text = mUtils.GetTranslations().Get(pMore?"user_subscription_expand_link_expanded":"user_subscription_expand_link");
			mShowState = pMore;
		}
	}
}

