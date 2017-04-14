using System;
using System.Text;

namespace Xsolla 
{
	public class XsollaUser : IParseble 
	{

		private Requisites requisites;// "requisites":{},
		private string local;// "local":"en",
		private Country country;// "country":{},
		private int savedPaymentMethodCount;// "savedPaymentMethodCount":0,
		private string acceptLanguage;// "acceptLanguage":"ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4,cs;q=0.2",
		private string acceptEncoding; // "acceptEncoding":"gzip, deflate"
		public VirtualUserBalance userBalance; //user_balance:{amount: 0, currency: "USD"}
		public VirtualCurrencyBalance virtualCurrencyBalance; // "virtual_currency_balance":{amount: 250}

		public string GetCountryIso()
		{
			return country.value;
		}

		public bool IsAllowChangeCountry()
		{
			return country.allowModify;
		}

		public string GetName()
		{
			return requisites.value;
		}

		public bool IdAllowModify()
		{
			return requisites.idAllowModify;
		}

		public IParseble Parse (SimpleJSON.JSONNode userNode)
		{
			if(userNode ["requisites"].Count > 1)
				requisites = new Requisites (userNode ["requisites"] ["value"], userNode ["requisites"] ["isVisible"].AsBool, userNode ["requisites"] ["id_allow_modify"].AsBool);
			country = new Country (userNode ["country"] ["value"], userNode ["country"] ["allow_modify"].AsBool);
			local = userNode ["local"];
			savedPaymentMethodCount = userNode ["savedPaymentMethodCount"].AsInt;
			acceptLanguage = userNode ["acceptLanguage"];
			acceptEncoding = userNode ["acceptEncoding"];
			if ((userNode["user_balance"]["currency"] != null) && (userNode["user_balance"]["amount"] != null))
				userBalance = new VirtualUserBalance(userNode["user_balance"]["currency"], userNode["user_balance"]["amount"].AsDecimal);

			if (userNode["virtual_currency_balance"]["amount"] != null)
				if (userNode["virtual_currency_balance"]["amount"] != null)
					virtualCurrencyBalance = new VirtualCurrencyBalance(userNode["virtual_currency_balance"]["amount"].AsDouble);

			return this;
		}

		public struct Requisites
		{
			public string value { get; private set;}// "value":"John Smith",
			public bool isVisible { get; private set;}// "isVisible":true
			public bool idAllowModify { get; private set;} //id_allow_modify:false

			public Requisites(string newValue, bool newIsVisible, bool pIdAllowModify):this()
			{
				value = newValue;
				isVisible = newIsVisible;
				idAllowModify = pIdAllowModify;
			} 

		}

		public class VirtualCurrencyBalance
		{
			public double amount;

			public VirtualCurrencyBalance(double pAmount)
			{
				amount = pAmount;
			}
		}

		public class VirtualUserBalance
		{
			public String currency;
			public decimal amount;

			public VirtualUserBalance(String pCurrency, decimal pAmount)
			{
				currency = pCurrency;
				amount = pAmount;
			}
		}

		public struct Country
		{
			public string value{ get; private set;}// "value":"US",
			public bool allowModify{ get; private set;}// "allow_modify":true

			public Country(string newValue, bool newAllowModify):this()
			{
				value = newValue;
				allowModify = newAllowModify;
			} 

		}

		public override string ToString ()
		{
			return string.Format ("[XsollaUser] "
			                      + "\n requisites {0}"
			                      + "\n local {1}"
			                      + "\n country {2}"
			                      + "\n savedPaymentMethodCount {3}"
			                      + "\n acceptLanguage {4}"
			                      + "\n acceptEncoding {5}",
			                      requisites, local, country, 
			                      savedPaymentMethodCount, acceptLanguage, acceptEncoding);
	}
	}
}
