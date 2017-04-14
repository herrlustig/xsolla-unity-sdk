
using SimpleJSON;
using System.Collections.Generic;
using System.Collections;

namespace Xsolla 
{
	public class XsollaPurchase : IParseble 
	{
		public VirtualCurrency virtualCurrency{get; private set;}//"virtual_currency" : +{ ... },
		public VirtualItems virtualItems{get; private set;}//"virtual_items": +{ ... }
		public Subscription subscription{get; private set;}//"subscription" : +{ ... },
		public Checkout checkout;
		public PaymentSystem paymentSystem{get; private set;}//"payment_system": +{ ... },

		public bool IsPurchase()
		{
			return virtualCurrency != null || virtualItems != null || subscription != null || checkout != null;
		}

		public bool IsPaymentSystem(){
			return paymentSystem != null;
		}

		public IParseble Parse (JSONNode purchaseNode)
		{
			if (purchaseNode.Count == 0)
				return null;
			if(purchaseNode ["virtual_currency"] != null)
				virtualCurrency = new VirtualCurrency ().Parse (purchaseNode ["virtual_currency"]) as VirtualCurrency;
			if(purchaseNode ["virtual_items"] != null)
				virtualItems = new VirtualItems().Parse(purchaseNode ["virtual_items"]) as VirtualItems;
			if(purchaseNode ["subscription"] != null)
				subscription = new Subscription ().Parse (purchaseNode ["subscription"]) as Subscription;
			if(purchaseNode ["payment_system"] != null)
				paymentSystem = new PaymentSystem().Parse(purchaseNode ["payment_system"]) as PaymentSystem;
			if (purchaseNode ["checkout"] != null)
				checkout = new Checkout ().Parse(purchaseNode ["checkout"]) as Checkout;;
			return this;
		}

		public class VirtualCurrency : IParseble
		{
			public int 	quantity{get; private set;}
			public bool allowModify{get; private set;}


			public IParseble Parse (JSONNode virtualCurrencyNode)
			{
				quantity = virtualCurrencyNode["quantity"].AsInt;
				allowModify = virtualCurrencyNode ["allow_modify"].AsBool;
				return this;
			}
		}

		public class Subscription : IParseble 
		{
			public string id{get; private set;}//		
			public bool allowModify{get; private set;}

			public IParseble Parse (JSONNode subscriptionNode)
			{
				id = subscriptionNode ["plan_id"];
				allowModify = subscriptionNode ["allow_modify"].AsBool;
				return this;
			}
		}

		public class VirtualItems : IParseble 
		{

			public bool allowModify{get; private set;}//"allow_modify":false
			public List<Item> items{get; private set;}//"items":[]

			public IParseble Parse(JSONNode itemsNode)
			{
				allowModify = itemsNode ["allow_modify"].AsBool;
				IEnumerator<JSONNode> enumerator = itemsNode ["items"].Childs.GetEnumerator();
				items = new List<Item> ();
				while (enumerator.MoveNext()) {
					Item item = new Item();
					item.sku = enumerator.Current["sku"].AsInt;
					item.amount = enumerator.Current["amount"].AsInt;
					items.Add(item);
				}
				return this;
			}

			public struct Item
			{
				public long sku;//"sku":12,
				public int amount;//"amount":1
			}
		}

		public class Checkout : IParseble
		{
			public int 		amount {get; private set;}//	"quantity" : 1,
			public string 	currency {get; private set;}//"allow_modify" : false
			
			
			public IParseble Parse (JSONNode virtualCurrencyNode)
			{
				amount = virtualCurrencyNode["amount"].AsInt;
				currency = virtualCurrencyNode ["currency"].Value;
				return this;
			}
		}

		public class PaymentSystem : IParseble 
		{
			public long id{get; private set;}//"id":26,
			public bool allowModify{get; private set;}//"allow_modify":false
			
			public IParseble Parse (JSONNode paymentSystemNode)
			{
				id = paymentSystemNode["id"].AsInt;
				allowModify = paymentSystemNode ["allow_modify"].AsBool;
				return this;
			}
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaPurchase: virtualCurrency={0}, virtualItems={1}, subscription={2}, paymentSystem={3}]", virtualCurrency, virtualItems, subscription, paymentSystem);
		}
	}
}
