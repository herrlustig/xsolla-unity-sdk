using SimpleJSON;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Xsolla
{
	public class XsollaGoodsManager : XsollaObjectsManager<XsollaShopItem>, IParseble {


		public IParseble Parse (JSONNode goodsNode)
		{
			JSONNode itemsNode = goodsNode ["virtual_items"];//virtual_items <- NEW | OLD -> items
			IEnumerator<JSONNode> goodsEnumerator = itemsNode.Childs.GetEnumerator ();
			while(goodsEnumerator.MoveNext())
			{
				XsollaShopItem item = new XsollaShopItem().Parse(goodsEnumerator.Current) as XsollaShopItem;
				AddItem(item);
			}
			return this;
		}
			
		public void setItemVirtCurrName(string pName){
			foreach(XsollaShopItem item in this.GetItemsList()){
				item.SetVirtCurrName(pName);
			}
		}
	}

	public class XsollaGroupsManager : XsollaObjectsManager<XsollaGoodsGroup>, IParseble
	{
		public IParseble Parse (JSONNode groupsNode)
		{
			JSONNode goodsGroupsNode = groupsNode["groups"];//["goodsgroups"];
			IEnumerator<JSONNode> goodsGroupsEnumerator = goodsGroupsNode.Childs.GetEnumerator ();
			while(goodsGroupsEnumerator.MoveNext()){
				AddItem(new XsollaGoodsGroup().Parse(goodsGroupsEnumerator.Current) as XsollaGoodsGroup);
			}
			return this;
		}
	}

	public class XsollaGoodsGroup : IXsollaObject, IParseble
	{
		public int id; // "id":"119",
		public string name; // "name":"Top Items",
		public int mLevel;
		public string mExternalId;
		public string mChildren;

		public string GetKey()
		{
			return id.ToString ();
		}

		public string GetName()
		{
			return name;
		}

		public IParseble Parse (JSONNode goodsGroupNode)
		{
			id = goodsGroupNode ["id"].AsInt;
			name = goodsGroupNode ["name"];
			mLevel = goodsGroupNode ["level"].AsInt;
			mExternalId = goodsGroupNode ["external_id"].Value;
			mChildren = goodsGroupNode ["children"].Value;
			return this;
		}
	}

	public class XsollaShopItem : AXsollaShopItem, IParseble
	{
			
		private long 	id;									//	id: 1468,

		private string 	sku;								//	sku: "1468",
		private string 	name;								//	name: "Кролик",
		private string 	imageUrl;							//	image_url: "https://xsolla.cachefly.net/img/3906561d617cb3.png",
		private string 	description;						//	description: "Кролики — это маленькие млекопитающие семейства зайцевых.",
		private string 	descriptionLong;					//	long_description: "There are eight different genera in the family classified as rabbits...",
		public string 	currency;							//	currency: "USD",
		private string  virtCurrency = "Coins";				//  item virtual currency. Today not allowed different currency for each item.

		public decimal 	amount;								//	amount: 0.39,
		public decimal 	amountWithoutDiscount;				//	amount_without_discount: 0.39,
		public decimal 	vcAmount;							//	vc_amount: 0,
		public decimal 	vcAmountWithoutDiscount;			//	vc_amount_without_discount: 0,

		private int 	quantityLimit;						//	quantity_limit: 1,
		private int 	isFavorite;							//	is_favorite: 0,

		private string[] 				unsatisfiedUserAttributes;	//	unsatisfied_user_attributes: []
		private XsollaBonusItem 		bonusVirtualCurrency;		//	bonus_virtual_currency: {},
		private List<XsollaBonusItem> 	bonusVirtualItems;			//	bonus_virtual_items: [],

		public void SetVirtCurrName(string pName){
			virtCurrency = pName;
		}

		public long GetId(){
			return id;
		}

		public int GetQuantityLimit()
		{
			return quantityLimit;
		}

		public string GetBounusString()
		{
			if (bonusVirtualItems.Count > 0) {
				StringBuilder stringBuilder = new StringBuilder ();
				stringBuilder.Append ("<color=#2DAE7B>");
				stringBuilder.Append ("+ ");
				foreach (XsollaBonusItem bonusItem in bonusVirtualItems) {
					stringBuilder.Append (bonusItem.name).Append (" free ");
				}
				stringBuilder.Append ("</color>");
				return stringBuilder.ToString ();
			} else if (bonusVirtualCurrency != null && bonusVirtualCurrency.quantity != null && !"".Equals(bonusVirtualCurrency.quantity)) {
				StringBuilder stringBuilder = new StringBuilder ();
				stringBuilder.Append ("<color=#2DAE7B>");
				stringBuilder.Append ("+ ");
				stringBuilder.Append (bonusVirtualCurrency.quantity).Append (bonusVirtualCurrency.name).Append (" free ");
				stringBuilder.Append ("</color>");
				return stringBuilder.ToString ();
			} else {
				return "";
			}
		}

		public string GetImageUrl()
		{
			if (imageUrl != null) {
				if (imageUrl.StartsWith ("https:"))
					return imageUrl;
				else 
					return "https:" + imageUrl;
			} else {
				return null;
			}
		}

		public string GetPriceString()
		{
			if (!IsVirtualPayment()) {
				if (amount == amountWithoutDiscount) {
					return CurrencyFormatter.FormatPrice (currency, amount.ToString ());
				} else {
					string oldPrice = CurrencyFormatter.FormatPrice (currency, amountWithoutDiscount.ToString ());
					string newPrice = CurrencyFormatter.FormatPrice (currency, amount.ToString ());
					return "<size=10><color=#a7a7a7>" + oldPrice + "</color></size>" + " " + newPrice;
				}
			} else {
				if (vcAmount == vcAmountWithoutDiscount) {
					// FIX name currency must by localizated  
					return CurrencyFormatter.FormatPrice (virtCurrency, vcAmount.ToString ());
				} else {
					string oldPrice = CurrencyFormatter.FormatPrice (virtCurrency, vcAmountWithoutDiscount.ToString ());
					string newPrice = CurrencyFormatter.FormatPrice (virtCurrency, vcAmount.ToString ());
					return "<size=10><color=#a7a7a7>" + oldPrice + "</color></size>" + " " + newPrice;
				}
			}
			
		}

		public bool IsVirtualPayment() {
			return vcAmount > 0 || vcAmountWithoutDiscount > 0;
		}

		public string GetSku(){
			return sku;
		}

		public override string GetKey()
		{
			return sku.ToString ();//sku <- NEW | OLD -> id
		}

		public override string GetName()
		{
			return name;
		}

		public string GetDescription(){
			return description;
		}

		public string GetLongDescription(){
			return descriptionLong;
		}

		public bool IsFavorite(){
			return isFavorite == 0 ? false : true;
		}

		public IParseble Parse (JSONNode shopItemNode)
		{
			id 						= shopItemNode ["id"].AsInt;
			sku 					= shopItemNode ["sku"].Value;
			name 					= shopItemNode ["name"].Value;
			description 			= shopItemNode ["description"].Value;
			descriptionLong 		= shopItemNode ["long_description"].Value;
			imageUrl 				= shopItemNode ["image_url"].Value;//image_url <- NEW | OLD -> image
			amount 					= shopItemNode ["amount"].AsDecimal;
			amountWithoutDiscount 	= shopItemNode ["amount_without_discount"].AsDecimal;//amount_without_discount <- NEW | OLD -> amountWithoutDiscount
			vcAmount 				= shopItemNode ["vc_amount"].AsDecimal;
			vcAmountWithoutDiscount = shopItemNode ["vc_amount_without_discount"].AsDecimal;//amount_without_discount <- NEW | OLD -> amountWithoutDiscount
			currency 				= shopItemNode ["currency"].Value;
			bonusVirtualItems 		= XsollaBonusItem.ParseMany (shopItemNode ["bonus_virtual_items"]);
			var bvc 				= new XsollaBonusItem ();
			bvc.Parse (shopItemNode ["bonus_virtual_currency"]);
			bonusVirtualCurrency 	= bvc;
			label 					= shopItemNode ["label"].Value;
			isFavorite 				= shopItemNode ["is_favorite"].AsInt;

			offerLabel 				= shopItemNode ["offer_label"].Value;
			string advertisementTypeString = shopItemNode ["advertisement_type"].Value;
			advertisementType = AdType.NONE;
			if (amount != amountWithoutDiscount || bonusVirtualItems.Count > 0) {
				advertisementType = AdType.SPECIAL_OFFER;
			} else {
				if("best_deal".Equals(advertisementTypeString)) {
					advertisementType = AdType.BEST_DEAL;
				} else if("recommended".Equals(advertisementTypeString)) {
					advertisementType = AdType.RECCOMENDED;
				}
			}
			return this;
		}

	}

}
