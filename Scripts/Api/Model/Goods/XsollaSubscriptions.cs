using SimpleJSON;
using System.Collections.Generic;
using System.Text;

namespace Xsolla 
{
	public class XsollaSubscriptions : XsollaObjectsManager<XsollaSubscription>, IParseble {

//		private List<XsollaSubscription> subscriptionList;//"packages":[],
		private XsollaApi api;//							"api":{

		public IParseble Parse (JSONNode subscriptionsNode)
		{
			var packagesNode = subscriptionsNode ["packages"];
			var enumerator = packagesNode.Childs.GetEnumerator ();
			while (enumerator.MoveNext()) 
			{
				AddItem(new XsollaSubscription().Parse(enumerator.Current) as XsollaSubscription);
			}
			api = new XsollaApi().Parse(subscriptionsNode["api"]) as XsollaApi;
			return this;
		}
	}

	public class XsollaSubscription : IXsollaObject, IParseble
	{
		public string id { get; private set;}// 						"id":"5f23c3de",
		public float chargeAmount { get; private set;}//				"charge_amount":19.99,
		public float chargeAmountLocal { get; private set;}//			"charge_amount_local":19.99,
		public float chargeAmountWithoutDiscount{ get; private set;}//	"charge_amount_without_discount":19.99,
		public float chargeAmountWithoutDiscountLocal{ get; private set;}//	"charge_amount_without_discount_local":19.99,
		public string chargeCurrency { get; private set;}//				"charge_currency":"USD",
		public string chargeCurrencyLocal { get; private set;}//		"charge_currency_local":"USD",
		public bool isActive {get; private set;}//						"is_active:false",
		public bool isPossibleRenew {get; private set;}//				"is_possible_renew:true",
		public bool isTrial {get; private set;}//						"is_trial:false",
		public int period { get; private set;}//						"period":1,
		public int periodTrial { get; private set;}//					"period_trial":1,
		public string periodUnit { get; private set;}//					"period_unit":"month",
		public string name { get; private set;}//						"name":"Platinum VIP",
		public string offerLabel { get; private set;}// 				"offer_label: """,
		public string description { get; private set;}//				"description":"10x more experience!",
		public int bonusVirtualCurrency { get; private set;}//			"bonus_virtual_currency":0,
		public List<XsollaBonusItem> bonusVirtualItems { get; private set;}//	"bonus_virtual_items":[]
		public int promotionChargesCount {get; private set;} // 		"promotion_charges_count: "

		public string GetBounusString()
		{
			if (bonusVirtualItems.Count > 0) {
				StringBuilder stringBuilder = new StringBuilder ();
				stringBuilder.Append ("<color=#2DAE7B>");
				stringBuilder.Append ("+ ");
				foreach (XsollaBonusItem bonusItem in bonusVirtualItems) {
					stringBuilder.Append (bonusItem.name).Append (" ");
				}
				stringBuilder.Append ("</color>");
				return stringBuilder.ToString ();
			} 
			else 
			{
				return "";
			}
		}

		public string GetPeriodString(string per)
		{
			return per + " " + period + " " + periodUnit;
		}

		public bool IsSpecial(){
			return chargeAmount != chargeAmountWithoutDiscount;
		}

		public string GetPriceString()
		{
			if (!IsSpecial()) {
				return CurrencyFormatter.FormatPrice(chargeCurrency, chargeAmount.ToString());
			} 
			else 
			{
				string oldPrice = CurrencyFormatter.FormatPrice(chargeCurrency, chargeAmountWithoutDiscount.ToString());
				string newPrice = CurrencyFormatter.FormatPrice(chargeCurrency, chargeAmount.ToString());
				return "<size=10><color=#a7a7a7>" + oldPrice + "</color></size>" + " " + newPrice;
			}
			
		}

		public string GetKey()
		{
			return id.ToString ();
		}

		public string GetName()
		{
			return name;
		}

		public IParseble Parse (JSONNode subscriptionNode)
		{
			id = subscriptionNode ["id"];
			chargeAmount = subscriptionNode ["charge_amount"].AsFloat;
			chargeAmountLocal = subscriptionNode ["charge_amount_local"].AsFloat;
			chargeAmountWithoutDiscount = subscriptionNode ["charge_amount_without_discount"].AsFloat;
			chargeAmountWithoutDiscountLocal = subscriptionNode ["charge_amount_without_discount_local"].AsFloat;
			chargeCurrency = subscriptionNode ["charge_currency"];
			chargeCurrencyLocal = subscriptionNode ["charge_currency_local"];
			isActive = subscriptionNode ["is_active"].AsBool;
			isPossibleRenew = subscriptionNode ["is_possible_renew"].AsBool;
			isTrial = subscriptionNode ["is_trial"].AsBool;
			period = subscriptionNode ["period"].AsInt;
			periodTrial = subscriptionNode ["perios_trial"].AsInt;
			periodUnit = subscriptionNode ["period_unit"];
			name = subscriptionNode ["name"];
			offerLabel = subscriptionNode ["offer_label"];
			description = subscriptionNode ["description"];
			bonusVirtualCurrency = subscriptionNode ["bonus_virtual_currency"].AsInt;
			bonusVirtualItems = XsollaBonusItem.ParseMany (subscriptionNode ["bonus_virtual_items"]);
			promotionChargesCount = subscriptionNode ["promotion_charges_count"].AsInt;
			return this;
		}
	}
}
