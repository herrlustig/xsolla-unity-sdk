
using SimpleJSON;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Xsolla 
{
	public class XsollaPricepointsManager : XsollaObjectsManager<XsollaPricepoint>, IParseble 
	{

//		private List<XsollaPricepoint> list;// 			"list":[],
		private Dictionary<string, object> formParams;//"formParams":[],
		private string projectCurrency;// 				"projectCurrency":"Coins",
//		private XsollaApi api;// 						"api":{}

		public string GetProjectCurrency(){
			return projectCurrency;
		}

		public IParseble Parse (JSONNode pricepointsNode)
		{
			var listNode = pricepointsNode ["list"];
			var pricepointsEnumerator = listNode.Childs.GetEnumerator ();
			while (pricepointsEnumerator.MoveNext()) 
			{
				AddItem(new XsollaPricepoint().Parse(pricepointsEnumerator.Current) as XsollaPricepoint);
			}

			JSONNode formParamsNode = pricepointsNode ["formParams"];
			formParams = new Dictionary<string, object> (formParamsNode.Count);
			IEnumerator<JSONNode> formParamsEnumerator = formParamsNode.Childs.GetEnumerator ();
			while (formParamsEnumerator.MoveNext()) 
			{
				JSONNode current = formParamsEnumerator.Current;
				formParams.Add(current["name"], current["value"]);
			}

			projectCurrency = pricepointsNode ["projectCurrency"];

//			api = new XsollaApi ().Parse (pricepointsNode [XsollaApiConst.R_API]) as XsollaApi;

			return this;
		}

		public struct FormParam
		{
			public string name { get; private set;}// "name":"theme",
			public object value { get; private set;}// "value":100
			
			public FormParam(string newName, object newValue):this()
			{
				name = newName;
				value = newValue;
			} 
		}
	}

	public class XsollaPricepoint : AXsollaShopItem, IParseble
	{
		public float outAmount { get; private set;}// 					"out":100,
		public float outWithoutDiscount { get; private set;}// 			"outWithoutDiscount":100,
		public float bonusOut { get; private set;}// 						"bonusOut":0,
		public float sum { get; private set;}// 						"sum":0.99,
		public float sumWithoutDiscount { get; private set;}// 			"sumWithoutDiscount":0.99,
		public string currency { get; private set;}// 					"currency":"USD",
		public string image { get; private set;}// 						"image":"\/\/livedemo.xsolla.com\/paystation\/img\/1.png",
		public string desc { get; private set;}// 						"desc":"",
		public List<XsollaBonusItem> bonusItems { get; private set;}//	"bonusItems":[],
		public bool selected { get; private set;}// 					"selected":true

		public string GetImageUrl()
		{
			if(image.StartsWith("https:"))
				return image;
			else 
				return "https:" + image;
		}

		public string GetOutString()
		{
			return outAmount.ToString ();
		}

		public string GetPriceString()
		{
			if (sum == sumWithoutDiscount) {
				return CurrencyFormatter.FormatPrice(currency, sum.ToString());
			} 
			else 
			{
				string oldPrice = CurrencyFormatter.FormatPrice(currency, sumWithoutDiscount.ToString());
				string newPrice = CurrencyFormatter.FormatPrice(currency, sum.ToString());
				return "<size=10><color=#a7a7a7>" + oldPrice + "</color></size>" + " " + newPrice;
			}

		}

		public bool IsSpecialOffer()
		{
			return sum != sumWithoutDiscount || bonusItems.Count > 0;
		}

		public string GetDescription(){
			return desc.ToString ();
		}

		public override string GetKey()
		{
			return outAmount.ToString ();
		}

		public override string GetName()
		{
			return outAmount.ToString ();
		}



		public IParseble Parse (JSONNode pricepointNode)
		{
			outAmount = pricepointNode["out"].AsFloat;
			outWithoutDiscount = pricepointNode["outWithoutDiscount"].AsFloat;
			bonusOut = pricepointNode["bonusOut"].AsFloat;
			sum = pricepointNode["sum"].AsFloat;
			sumWithoutDiscount = pricepointNode["sumWithoutDiscount"].AsFloat;
			currency = pricepointNode["currency"].Value;
			image = pricepointNode["image"].Value;
			desc = pricepointNode["description"].Value;
			bonusItems = XsollaBonusItem.ParseMany (pricepointNode ["bonusItems"]);
			label = pricepointNode["label"].Value;
			offerLabel = pricepointNode["offerLabel"].Value;
			selected = pricepointNode["selected"].AsBool;

			string advertisementTypeString = pricepointNode ["advertisementType"].Value;
			advertisementType = AdType.NONE;
			if (sum != sumWithoutDiscount || outAmount != outWithoutDiscount || bonusItems.Count > 0 || bonusOut > 0) {
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
