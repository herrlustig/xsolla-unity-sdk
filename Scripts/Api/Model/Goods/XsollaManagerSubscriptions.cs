using System;

namespace Xsolla
{
	public class XsollaManagerSubscriptions: XsollaObjectsManager<XsollaManagerSubscription>, IParseble
	{
		private XsollaApi api;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			api = new XsollaApi().Parse(rootNode["api"]) as XsollaApi;

			var enumerator = rootNode.Childs.GetEnumerator();

			while (enumerator.MoveNext()) 
			{
				AddItem(new XsollaManagerSubscription().Parse(enumerator.Current) as XsollaManagerSubscription);
			}

			return this;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaManagerSubscriptions: api={0}]", api);
		}
	}

	public class XsollaManagerSubscription: IXsollaObject, IParseble
	{
		public XsollaSubCharge mCharge;			//		charge:{amount: 0.3, currency: "USD"}
		public String mDateNextCharge;		//		date_next_charge:"2017-02-06T12:41:40+03:00"
		public String mDesc;		//		description:"7 days"
		String mHoldDates;		//		hold_dates:null
		int mId;				//		id:9510073
		String mIdExternal;		//		id_external:"187fc9f4"
		public String mName;		//		name:"7 days"
		public String mPaymentMethod;		//		payment_method:"PayPal"
		String mPaymentType;		//		payment_type:"charge"
		public String mPaymentVisibleName;		//		payment_visible_name:"qualityqontrol@xsolla.com"
		XsollaSubPeriod mPeriod;		//		period:{value: 7, unit: "day"}
		int mValue;		//		value:7
		String mStatus;		//		status:"active"

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mCharge = new XsollaSubCharge().Parse(rootNode["charge"]) as XsollaSubCharge;
			mDateNextCharge = rootNode["date_next_charge"];
			mDesc = rootNode["description"];
			mHoldDates = rootNode["hold_dates"];
			mId = rootNode["id"].AsInt;
			mIdExternal = rootNode["id_external"];
			mName = rootNode["name"];
			mPaymentMethod = rootNode["payment_method"];
			mPaymentType = rootNode["payment_type"];
			mPaymentVisibleName = rootNode["payment_visible_name"];
			mPeriod = new XsollaSubPeriod().Parse(rootNode["period"]) as XsollaSubPeriod;
			mValue = rootNode["value"].AsInt;
			mStatus = rootNode["status"];

			return this;
		}

		public string GetKey ()
		{
			return mId.ToString();
		}

		public int GetId()
		{
			return mId;
		}

		public string GetName ()
		{
			return mName;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaManagerSubscription: mCharge={0}, mDateNextCharge={1}, mDesc={2}, mHoldDates={3}, mId={4}, mIdExternal={5}, mName={6}, mPaymentMethod={7}, mPaymentType={8}, mPaymentVisibleName={9}, mPeriod={10}, mValue={11}, mStatus={12}]", mCharge, mDateNextCharge, mDesc, mHoldDates, mId, mIdExternal, mName, mPaymentMethod, mPaymentType, mPaymentVisibleName, mPeriod, mValue, mStatus);
		}
	}

	public class XsollaSubCharge: IParseble
	{
		decimal mAmount;
		String mCurrency;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mAmount = rootNode["amount"].AsDecimal;
			mCurrency = rootNode["currency"].Value;
			return this;
		}

		public override string ToString ()
		{
			return CurrencyFormatter.FormatPrice(mCurrency, mAmount.ToString("0.00"));
		}
	}

	public class XsollaSubPeriod: IParseble
	{
		int mValue;
		String mUnit;

		public IParseble Parse (SimpleJSON.JSONNode rootNode)
		{
			mValue = rootNode["value"].AsInt;
			mUnit = rootNode["unit"].Value;
			return this;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaSubPeriod: mValue={0}, mUnit={1}]", mValue, mUnit);
		}
		

		public XsollaSubPeriod ()
		{
		}
	}
}

