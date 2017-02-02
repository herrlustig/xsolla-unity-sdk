using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Xsolla
{
	public class XsollaManagerSubDetails: IParseble
	{

		public XsollaApi mApi;
		public List<XsollaSubDetailCharge> mCharges;
		public XsollaSubCharge mCharge;
		public DateTime mDateCreate;
		public DateTime mDateNextCharge;
		public String mDesc;
		public String mHoldDates;
		public String mId;
		public String mIdExternal;
		public Boolean mIsCancelPossible;
		public Boolean mIsChangePlanAllowed;
		public Boolean mIsHoldPossible;
		public Boolean mIsRenewPossible;
		public Boolean mIsSheduledHoldExist;
		public XsollaSubLimitHoldPerios mLimitHoldPeriod;
		public String mName;
		public XsollaSubCharge mNextCharge;
		public String mNextPeriodPlanChange;
		public XsollaSubDetailPaymentAcc mPaymentAccount;
		public String mPaymentIcoSrc;
		public String mPaymentMethodName;
		public String mPaymentMethodType;
		public String mPaymentMethodVisName;
		public XsollaSubDetailPeriod mPeriod;
		public String mSheduledHoldDates;
		public String mStatus;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mApi = new XsollaApi().Parse(rootNode["api"]) as XsollaApi;

			mCharges = new List<XsollaSubDetailCharge>();
			var listCharges = rootNode["charges"].Childs.GetEnumerator();
			while (listCharges.MoveNext())
			{
				mCharges.Add(new XsollaSubDetailCharge().Parse(listCharges.Current) as XsollaSubDetailCharge);
			}

			JSONNode subsNode = rootNode["subscription"];

			mCharge = new XsollaSubCharge().Parse(subsNode["charge"]) as XsollaSubCharge;
			mDateCreate = DateTime.Parse(subsNode["date_create"].Value);
			mDateNextCharge = DateTime.Parse(subsNode["date_next_charge"].Value);
			mDesc = subsNode["description"].Value;
			mHoldDates = subsNode["hold_dates"].Value;
			mId = subsNode["id"].Value;
			mIdExternal = subsNode["id_external"].Value;
			mIsCancelPossible = subsNode["is_cancel_possible"].AsBool;
			mIsChangePlanAllowed = subsNode["is_change_plan_allowed"].AsBool;
			mIsHoldPossible = subsNode["is_hold_possible"].AsBool;
			mIsRenewPossible = subsNode["is_renew_possible"].AsBool;
			mIsSheduledHoldExist = subsNode["is_scheduled_hold_exist"].AsBool;
			mLimitHoldPeriod = new XsollaSubLimitHoldPerios().Parse(subsNode["limit_hold_period"]) as XsollaSubLimitHoldPerios;
			mName = subsNode["name"].Value;
			mNextCharge = new XsollaSubCharge().Parse(subsNode["next_charge"]) as XsollaSubCharge;
			mNextPeriodPlanChange = subsNode["next_period_plan_change"];
			mPaymentAccount = new XsollaSubDetailPaymentAcc().Parse(subsNode["payment_account"]) as XsollaSubDetailPaymentAcc;
			mPaymentIcoSrc = subsNode["payment_icon_src"].Value;
			mPaymentMethodName = subsNode["payment_method"].Value;
			mPaymentMethodType = subsNode["payment_type"].Value;
			mPaymentMethodVisName = subsNode["payment_visible_name"].Value;
			mPeriod = new XsollaSubDetailPeriod().Parse(subsNode["period"]) as XsollaSubDetailPeriod;
			mSheduledHoldDates = subsNode["scheduled_hold_dates"].Value;
			mStatus = subsNode["status"].Value;

			return this;
		}

	}

	public class XsollaSubDetailPeriod: IParseble
	{
		public String mUnit;
		public int mValue;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mUnit = rootNode["unit"].Value;
			mValue = rootNode["value"].AsInt;
			return this;
		}
	}

	public class XsollaSubDetailPaymentAcc: IParseble
	{
		public int mId;
		public String mType;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mId = rootNode["id"].AsInt;
			mType = rootNode["type"].Value;
			return this;
		}
	}

	public class XsollaSubLimitHoldPerios: IParseble
	{
		public int maxDays;
		public int minDays;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			maxDays = rootNode["max_days"].AsInt;
			minDays = rootNode["min_days"].AsInt;
			return this;
		}

	}

	public class XsollaSubDetailCharge: IParseble
	{
		public XsollaSubCharge mCharge;
		public String mDateCreate;
		public String mPaymentMethod;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mCharge = new XsollaSubCharge().Parse(rootNode["charge"]) as XsollaSubCharge;
			mDateCreate = rootNode["date_create"].Value;
			mPaymentMethod = rootNode["payment_method"].Value;
			return this;
		}

	}
}

