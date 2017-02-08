using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubManagerDetailPartController: MonoBehaviour
	{
		public Text mPartTitle;
		public Text mBtnRenewName;
		public Button mBtnRenew;
		public GameObject mDetailsContainer;
		public GameObject mLinkUnhold;
		public GameObject mLinkHoldCancel;
		public GameObject mLinkChangePlan;

		private const String mLabeltextPrefab = "Prefabs/Screens/SubsManager/Simple/DetailLabelValue";
		private XsollaUtils mUtils;
		private XsollaManagerSubDetails mSubDetail;

		public Button getUnholdBtn()
		{
			return mLinkUnhold.GetComponent<Button>();
		}
		public Button getHoldCancelBtn()
		{
			return mLinkHoldCancel.GetComponent<Button>();
		}
		public Button getChangePlanBtn()
		{
			return mLinkChangePlan.GetComponent<Button>();
		}

		public void initScreen(XsollaManagerSubDetails pSubDetail, XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mSubDetail = pSubDetail;
			mPartTitle.text = pUtils.GetTranslations().Get("user_subscription_details_title");
			mBtnRenewName.text = pUtils.GetTranslations().Get("user_subscription_renew");

			mLinkUnhold.GetComponent<Text>().text = pUtils.GetTranslations().Get("user_subscription_unhold");
			mLinkHoldCancel.GetComponent<Text>().text = pUtils.GetTranslations().Get("user_subscription_hold");
			mLinkChangePlan.GetComponent<Text>().text = pUtils.GetTranslations().Get("user_subscription_change_plan");

			// кнопка обновления
			mBtnRenew.gameObject.SetActive(mSubDetail.mIsRenewPossible);
			// Unhold
			mLinkUnhold.SetActive(mSubDetail.mStatus == "freeze");
			// Hold or Cancel
			mLinkHoldCancel.SetActive((mSubDetail.mIsHoldPossible || mSubDetail.mStatus != "non_renewing") && (mSubDetail.mStatus != "freeze"));
			// Change Plan
			mLinkChangePlan.SetActive(mSubDetail.mIsChangePlanAllowed);

			// добавляем поля датализации 
			List<LabelValue> listFileds = getImportDetails();
			foreach (LabelValue item in listFileds)
			{
				GameObject obj = Instantiate(Resources.Load(mLabeltextPrefab)) as GameObject;
				LabelValueController controller = obj.GetComponent<LabelValueController>();
				controller.init(item.label, item.value, item.actionLabel, item.action);
				obj.transform.SetParent(mDetailsContainer.transform);
			}

			getUnholdBtn().onClick.AddListener(onUnholdLinkClick);
			getHoldCancelBtn().onClick.AddListener(onHoldCancelLinkClick);
			getChangePlanBtn().onClick.AddListener(onChangePlanLinkClick);
		}

		private void onUnholdLinkClick()
		{
		}
		private void onHoldCancelLinkClick()
		{
		}
		private void onChangePlanLinkClick()
		{
		}

		public List<LabelValue> getImportDetails()
		{
			List<LabelValue> list = new List<LabelValue>();
			XsollaTranslations translation = mUtils.GetTranslations();
			// ПОЛЯ ДЕТАЛИЗАЦИИ
			// имя
			list.Add(new LabelValue(translation.Get("user_subscription_name"), mSubDetail.mName));
			// статус
			list.Add(new LabelValue(translation.Get("user_subscription_status"), translation.Get("user_subscription_status_" + mSubDetail.mStatus)));
			// ценa
			list.Add(new LabelValue(translation.Get("user_subscription_charge"), mSubDetail.mCharge.ToString()));
			// цикл платежа
			list.Add(new LabelValue(translation.Get("user_subscription_period"), formattedPeriod(mSubDetail.mPeriod.mValue.ToString(), mSubDetail.mPeriod.mUnit)));

			if (mSubDetail.mStatus == "non_renewing")
				list.Add(new LabelValue(translation.Get("user_subscription_end_bill_date"), string.Format("{0:dd/MM/yyyy}", mSubDetail.mDateNextCharge)));  
				 
			if (mSubDetail.mNextPeriodPlanChange != null)
				list.Add(new LabelValue(translation.Get("user_subscription_new_plan"), string.Format(prepareFormatString(translation.Get("user_subscription_next_period_plan_change")), mSubDetail.mNextPeriodPlanChange.name, mSubDetail.mNextPeriodPlanChange.date.ToString("d"))));  
			
			if (mSubDetail.mIsSheduledHoldExist && (mSubDetail.mSheduledHoldDates != null) || (mSubDetail.mStatus == "freeze") && (mSubDetail.mHoldDates != null))
			{

				String lDateFrom = "", lDateTo = "";
				if (mSubDetail.mStatus == "freeze")
				{
					if (mSubDetail.mHoldDates != null)
					{
						lDateFrom = mSubDetail.mHoldDates.dateFrom.ToString("d");
						lDateTo = mSubDetail.mHoldDates.dateTo.ToString("d");
					}
				}
				else
				{
					if (mSubDetail.mSheduledHoldDates != null)
					{
						lDateFrom = mSubDetail.mSheduledHoldDates.dateFrom.ToString("d");
						lDateTo = mSubDetail.mSheduledHoldDates.dateTo.ToString("d");
					}
				}
					
				if (mSubDetail.mIsSheduledHoldExist)
					list.Add(new LabelValue(translation.Get("user_subscription_hold_dates"), lDateFrom + " - " + lDateTo, translation.Get("cancel"), cancelHoldDates));
				else
					list.Add(new LabelValue(translation.Get("user_subscription_hold_dates"), lDateFrom + " - " + lDateTo));
			}

			return list;
		}

		private String prepareFormatString(String pInnerString)
		{
			String res = pInnerString;
			int indx = 0;
			while (res.Contains("{{"))
			{
				String replacedPart = res.Substring(res.IndexOf("{{", 0) + 1, res.IndexOf("}}", 0) - res.IndexOf("{{", 0));
				res = res.Replace(replacedPart, indx.ToString());  
				indx ++;
			}
			return res;
		}

		private String formattedPeriod(String pValue, String pUnit)
		{
			String translateKey = "period_" + pUnit + pValue;
			String unit = mUtils.GetTranslations().Get(translateKey);
			if (unit == "")
				unit = mUtils.GetTranslations().Get("period_" + pUnit + "s");

			return pValue + " " + unit;
		}

		private void cancelHoldDates()
		{
			Logger.Log("Cancel hold dates with id - " + mSubDetail.mId);
		}
	}

	public struct LabelValue
	{
		public String label;
		public String value;
		public String actionLabel;
		public Action action;

		public LabelValue(String pLabel, String pValue, String pActionLable = null, Action pAction = null)
		{
			label = pLabel;
			value = pValue;
			actionLabel = pActionLable;
			action = pAction;
		}
	}
}

