using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Xsolla
{
	public class XsollaSummary : IParseble 
	{

		private List<IXsollaSummaryItem> purchases;	//"purchase":{},
		private XsollaFinance finance;					//"finance":{}

		public  List<IXsollaSummaryItem> GetPurchases()
		{
			return purchases;
		}

		public  XsollaFinance GetFinance()
		{
			return finance;
		}

		public IParseble Parse(JSONNode summaryNode)
		{
			purchases = new List<IXsollaSummaryItem>();
			JSONNode nodeItems = summaryNode ["purchase"] ["virtual_items"];
			JSONNode nodeCurrency = summaryNode ["purchase"] ["virtual_currency"];
			JSONNode nodeSubscriptions = summaryNode ["purchase"] ["subscriptions"];
			if (nodeItems != null) {
				IEnumerator<JSONNode> iEnumerator = nodeItems.Childs.GetEnumerator ();
				while(iEnumerator.MoveNext())
				{
					JSONNode purchaseNode = iEnumerator.Current;
					purchases.Add(new XsollaSummaryItem().Parse(purchaseNode) as IXsollaSummaryItem);
				}
			}

			if (nodeCurrency != null) {
				IEnumerator<JSONNode> iEnumerator = nodeCurrency.Childs.GetEnumerator ();
				while(iEnumerator.MoveNext())
				{
					JSONNode purchaseNode = iEnumerator.Current;
					purchases.Add(new XsollaSummaryItem().Parse(purchaseNode) as IXsollaSummaryItem);
				}
			}

			if (nodeSubscriptions != null) {
				IEnumerator<JSONNode> iEnumerator = nodeSubscriptions.Childs.GetEnumerator ();
				while(iEnumerator.MoveNext())
				{
					JSONNode purchaseNode = iEnumerator.Current;
					purchases.Add(new XsollaSummarySubscription().Parse(purchaseNode) as IXsollaSummaryItem);
				}
			}
//			IEnumerator<JSONNode> enumerator = summaryNode ["purchase"].Childs.GetEnumerator ();
//			IEnumerator<JSONNode> innerEnumerator;
//			while(enumerator.MoveNext()){
//				innerEnumerator = enumerator.Current.Childs.GetEnumerator();
//				if(!enumerator.Current.AsObject.ContainsKey("subscriptions")){
//					while (innerEnumerator.MoveNext()) 
//					{
//						JSONNode purchaseNode = innerEnumerator.Current;
//						purchases.Add(new XsollaSummaryItem().Parse(purchaseNode) as IXsollaSummaryItem);
//					}
//				} else {
//					while (innerEnumerator.MoveNext()) 
//					{
//						JSONNode purchaseNode = innerEnumerator.Current;
//						purchases.Add(new XsollaSummarySubscription().Parse(purchaseNode) as IXsollaSummaryItem);
//					}
//				}
//			}
			finance = new XsollaFinance().Parse(summaryNode["finance"]) as XsollaFinance; 
			return this;
		}

	}

	public interface IXsollaSummaryItem{
		string GetImgUrl();
		string GetName();
		string GetPrice();
		string GetDescription();
		string GetBonus();
	}

	public class XsollaSummaryItem : IParseble, IXsollaSummaryItem
	{

		public float quantity { get; private set;}	//"quantity":100,
		public decimal amount { get; private set;}	//"amount":0.99,
		public string currency { get; private set;}//"currency":"USD",
		public string name { get; private set;}	//"name":"Coins",
		public string imageUrl { get; private set;}//"image_url":"https:\/\/xsolla.cachefly.net\/img\/misc\/images\/f5c585288abb141b179b17333a68fd5b.png",
		public string description { get; private set;}//"description":"+50% extra",
		public string longDescription { get; private set;}//"longDescription":null,
		public bool isBonus { get; private set;}	//"is_bonus":false


		public string GetImgUrl()
		{
			if (imageUrl == null)
				return "";
			if(imageUrl.StartsWith("https:"))
				return imageUrl;
			else 
				return "https:" + imageUrl;
		}

		public string GetName()
		{
			return quantity + " " + name;
		}

		public string GetPrice()
		{
			return CurrencyFormatter.Instance.FormatPrice(currency, amount);
		}


		public string GetDescription()
		{
			return "null".Equals(description) ? "" : description;
		}

		public string GetBonus()
		{
			return isBonus ? "Bonus" : "";
		}

		public IParseble Parse(JSONNode purchaseNode)
		{
			quantity = purchaseNode ["quantity"].AsFloat;
			amount = purchaseNode ["amount"].AsDecimal;
			currency = purchaseNode ["currency"];
			name = purchaseNode ["name"];
			imageUrl = purchaseNode ["image_url"];
			description = purchaseNode ["description"].Value;
			longDescription = purchaseNode ["longDescription"].Value;
			isBonus = purchaseNode ["is_bonus"].AsBool;
			return this;
		}
	}

	public class XsollaSummarySubscription : IParseble, IXsollaSummaryItem
	{
		public decimal amount{ get; private set;}//"amount":0.99,
		public int period{ get; private set;}//"period":1,
		public string currency{ get; private set;}//"currency":"USD",
		public string description{ get; private set;}//"description":"Silver Status",
		public string package_info{ get; private set;}//"package_info":"Silver Status",
		public string period_type{ get; private set;}//"period_type":"month",
		public string expiration_period_type{ get; private set;}//"expiration_period_type":"day",
		public string recurrent_type{ get; private set;}//"recurrent_type":"charge",
		public string date_next_charge{ get; private set;}//"date_next_charge":"2015-07-29",
		public string amount_next_charge{ get; private set;}//"amount_next_charge":"0.9900",
		public string currency_next_charge{ get; private set;}//"currency_next_charge":"USD"

		public string GetImgUrl()
		{
			return "";
		}
		
		public string GetName()
		{
			return period + " " + period_type + " " + description;
		}
		
		public string GetPrice()
		{
			return CurrencyFormatter.Instance.FormatPrice(currency, amount);
		}
		
		
		public string GetDescription()
		{
			return "until " + date_next_charge;
		}
		
		public string GetBonus()
		{
			return "";
		}

		public IParseble Parse(JSONNode purchaseNode)
		{
			amount = purchaseNode ["amount"].AsDecimal;
			period = purchaseNode ["period"].AsInt;
			currency = purchaseNode ["currency"];
			description = purchaseNode ["description"];
			package_info = purchaseNode ["package_info"];
			period_type = purchaseNode ["period_type"];
			expiration_period_type = purchaseNode ["expiration_period_type"];
			recurrent_type = purchaseNode ["recurrent_type"];
			date_next_charge = purchaseNode ["date_next_charge"];
			amount_next_charge = purchaseNode ["amount_next_charge"];
			currency_next_charge = purchaseNode ["currency_next_charge"];
			return this;
		}

	}

	public class XsollaFinance : IParseble
	{

		public FinanceItem subTotal { get; private set;}//"sub_total":{},*
		public FinanceItemBase discount { get; private set;}//"discount":{},
		public FinanceItemBase fee { get; private set;}//"fee":{},
		public FinanceItem xsollaCredits { get; private set;}//"xsolla_credits":{},*
		public FinanceItemBase total { get; private set;}//"total":{},
		public FinanceItemBase vat { get; private set;}//"vat":{}

		public IParseble Parse(JSONNode financyNode)
		{
			if(financyNode["sub_total"] != null)
				subTotal = new FinanceItem().Parse(financyNode["sub_total"]) as FinanceItem;
			if(financyNode["discount"] != null)
				discount = new FinanceItemBase().Parse(financyNode["discount"]) as FinanceItemBase;
			if(financyNode["fee"] != null)
				fee = new FinanceItemBase().Parse(financyNode["fee"]) as FinanceItemBase;
			if(financyNode["xsolla_credits"] != null)
				xsollaCredits = new FinanceItem().Parse(financyNode["xsolla_credits"]) as FinanceItem;
			if(financyNode["total"] != null)
				total = new FinanceItemBase().Parse(financyNode["total"]) as FinanceItemBase;
			if(financyNode["vat"] != null)
				vat = new FinanceItemBase().Parse(financyNode["vat"]) as FinanceItemBase;
			return this;
		}

		
		public class FinanceItemBase : IParseble
		{
			public decimal amount { get; private set;}//"amount":0.99,
			public string currency { get; private set;}//"currency":"USD",

			public FinanceItemBase(){

			}

			public FinanceItemBase(decimal newAmount, string newCurrency):this()
			{
				amount = newAmount;
				currency = newCurrency;
			} 

			public IParseble Parse(JSONNode baseFinanceItemNode)
			{
				amount = baseFinanceItemNode["amount"].AsDecimal;
				currency = baseFinanceItemNode["currency"];
				return this;
			}
		}

		public class FinanceItem : FinanceItemBase
		{
			public float paymentAmount { get; private set;}//"payment_amount":0.99,
			public string paymentCurrency { get; private set;}//"payment_currency":"USD"

			
			public FinanceItem(){
				
			}

			public FinanceItem(decimal newAmount, string newCurrency, 
			                   float newPaymentAmount, string newPaymentCurrency):base(newAmount, newCurrency)
			{
				paymentAmount = newPaymentAmount;
				paymentCurrency = newPaymentCurrency;
			} 

			public new IParseble Parse(JSONNode financeItemNode)
			{
				base.Parse(financeItemNode);
				paymentAmount = financeItemNode["payment_amount"].AsFloat;
				paymentCurrency = financeItemNode["payment_currency"];
				return this;
			}
		}

	}

}
