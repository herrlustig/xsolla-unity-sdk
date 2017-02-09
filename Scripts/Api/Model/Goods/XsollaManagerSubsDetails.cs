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
		public XsollaSubHoldDates mHoldDates;
		public String mId;
		public String mIdExternal;
		public Boolean mIsCancelPossible;
		public Boolean mIsChangePlanAllowed;
		public Boolean mIsHoldPossible;
		public Boolean mIsRenewPossible;
		public Boolean mIsSheduledHoldExist;
		public XsollaSubLimitHoldPeriod mLimitHoldPeriod;
		public String mName;
		public XsollaSubCharge mNextCharge;
		public XsollaSubNextPeriodPlanChange mNextPeriodPlanChange;
		public XsollaSubDetailPaymentAcc mPaymentAccount;
		public String mPaymentIcoSrc;
		public String mPaymentMethodName;
		public String mPaymentMethodType;
		public String mPaymentMethodVisName;
		public XsollaSubDetailPeriod mPeriod;
		public XsollaSubHoldDates mSheduledHoldDates;
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
			mHoldDates = new XsollaSubHoldDates().Parse(subsNode["hold_dates"]) as XsollaSubHoldDates;
			mId = subsNode["id"].Value;
			mIdExternal = subsNode["id_external"].Value;
			mIsCancelPossible = subsNode["is_cancel_possible"].AsBool;
			mIsChangePlanAllowed = subsNode["is_change_plan_allowed"].AsBool;
			mIsHoldPossible = subsNode["is_hold_possible"].AsBool;
			mIsRenewPossible = subsNode["is_renew_possible"].AsBool;
			mIsSheduledHoldExist = subsNode["is_scheduled_hold_exist"].AsBool;
			mLimitHoldPeriod = new XsollaSubLimitHoldPeriod().Parse(subsNode["limit_hold_period"]) as XsollaSubLimitHoldPeriod;
			mName = subsNode["name"].Value;
			mNextCharge = new XsollaSubCharge().Parse(subsNode["next_charge"]) as XsollaSubCharge;
			mNextPeriodPlanChange = new XsollaSubNextPeriodPlanChange().Parse(subsNode["next_period_plan_change"]) as XsollaSubNextPeriodPlanChange;
			mPaymentAccount = new XsollaSubDetailPaymentAcc().Parse(subsNode["payment_account"]) as XsollaSubDetailPaymentAcc;
			mPaymentIcoSrc = subsNode["payment_icon_src"].Value;
			mPaymentMethodName = subsNode["payment_method"].Value;
			mPaymentMethodType = subsNode["payment_type"].Value;
			mPaymentMethodVisName = subsNode["payment_visible_name"].Value;
			mPeriod = new XsollaSubDetailPeriod().Parse(subsNode["period"]) as XsollaSubDetailPeriod;
			mSheduledHoldDates = new XsollaSubHoldDates().Parse(subsNode["scheduled_hold_dates"]) as XsollaSubHoldDates;
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
			if (rootNode == "null")
				return null;

			mUnit = rootNode["unit"].Value;
			mValue = rootNode["value"].AsInt;
			return this;
		}

		public override string ToString ()
		{
			

			return string.Format ("{1} {0}", mUnit, mValue);
		}
		
	}

	public class XsollaSubDetailPaymentAcc: IParseble
	{
		public int mId;
		public String mType;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			if (rootNode.Value == "null")
				return null;

			mId = rootNode["id"].AsInt;
			mType = rootNode["type"].Value;
			return this;
		}
	}

	public class XsollaSubNextPeriodPlanChange: IParseble
	{
		public String name;
		public DateTime date;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			if (rootNode.Value == "null")
				return null;
			
			if (rootNode["name"] != null)
				name = rootNode["name"].Value;
			if (rootNode["date"] != null)
			date = DateTime.Parse(rootNode["date"].Value);
			return this;
		}
	}

	public class XsollaSubHoldDates: IParseble
	{
		public DateTime dateFrom;
		public DateTime dateTo;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			if (rootNode.Value == "null")
				return null;
			
			if (rootNode["date_from"] != null)
				dateFrom = DateTime.Parse(rootNode["date_from"].Value);
			if (rootNode["date_to"] != null)
				dateTo = DateTime.Parse(rootNode["date_to"].Value);
			return this;
		}

		public override string ToString ()
		{
			return string.Format ("{0} - {1}", dateFrom.ToString("d"), dateTo.ToString("d"));
		}
		
	}

	public class XsollaSubLimitHoldPeriod: IParseble
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
		public DateTime mDateCreate;
		public String mPaymentMethod;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mCharge = new XsollaSubCharge().Parse(rootNode["charge"]) as XsollaSubCharge;
			if (rootNode["date_create"] != null)
				mDateCreate = DateTime.Parse(rootNode["date_create"].Value);
			mPaymentMethod = rootNode["payment_method"].Value;
			return this;
		}

	}
}

