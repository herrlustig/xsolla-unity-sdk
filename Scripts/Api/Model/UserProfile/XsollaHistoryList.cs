using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

namespace Xsolla
{
	public class XsollaHistoryList: XsollaObjectsManager<XsollaHistoryItem>, IParseble
	{
		public IParseble Parse (JSONNode pNode)
		{
			var enumerator = pNode.Childs.GetEnumerator ();
			while (enumerator.MoveNext()) 
			{
				AddItem(new XsollaHistoryItem().Parse(enumerator.Current) as XsollaHistoryItem);
			}
			return this;
		}
	}

	public class XsollaHistoryItem: IXsollaObject, IParseble
	{
		public string id {get; private set;}
		public string comment {get; private set;} //comment:null
		public string couponeCode {get; private set;} //couponCode:null
		public DateTime date{get; private set;}// date:"2016-10-04T12:36:03+03:00"
		public int invoiceId{get; private set;}// invoiceId:231266644
		public string operationType{get; private set;}// operationType:"cancellation"
		public decimal paymentAmount{get; private set;}// paymentAmount:0.5
		public string paymentCurrency{get; private set;}// paymentCurrency:"USD"
		public string paymentName{get; private set;}// paymentName:"Apple Pay"
		public string statusCode{get; private set;}// statusCode:"troubled"
		public string statusText{get; private set;}// statusText:"Canceled"
		public float userBalance{get; private set;}// userBalance:50
		public string userCustom{get; private set;}// userCustom:""
		public float vcAmount{get; private set;}// vcAmount:-50
		public XsollaHistoryVirtualItems virtualItems{get; private set;}// virtualItems:[]
		public string virtualItemsOperationType{get; private set;}// virtualItemsOperationType:"remove"

		public IParseble Parse(JSONNode pNode)
		{
			comment = pNode["comment"];
			couponeCode = pNode["couponCode"];
			date = DateTime.Parse(pNode["date"]);
			invoiceId = pNode["invoiceId"].AsInt;
			operationType = pNode["operationType"];
			paymentAmount = pNode["paymentAmount"].AsDecimal;
			paymentCurrency = pNode["paymentCurrency"];
			paymentName = pNode["paymentName"];
			statusCode = pNode["statusCode"];
			statusText = pNode["statusText"];
			userBalance = pNode["userBalance"].AsFloat;
			userCustom = pNode["userCustom"];
			vcAmount = pNode["vcAmount"].AsFloat;
			virtualItems = new XsollaHistoryVirtualItems().Parse(pNode["virtualItems"]) as XsollaHistoryVirtualItems;
			virtualItemsOperationType = pNode["virtualItemsOperationType"];
			id = Guid.NewGuid().ToString();

			return this;
		}

		public string GetKey()
		{
			return id;
		}

		public string GetName()
		{
			return comment;
		}
	}

	public class XsollaHistoryVirtualItems: IParseble
	{
		public string virtualItemsOperationType{get; private set;}//virtualItemsOperationType:"add"
		public XsollaHistoryVirtualItemsList items;

		public IParseble Parse(JSONNode pNode)
		{
			virtualItemsOperationType = pNode["virtualItemsOperationType"];
			items = new XsollaHistoryVirtualItemsList().Parse(pNode) as XsollaHistoryVirtualItemsList;
			return this;
		}
	}

	public class XsollaHistoryVirtualItemsList: XsollaObjectsManager<XsollaHistoryVirtualItem>, IParseble
	{
		public IParseble Parse(JSONNode pNode)
		{
			var enumerator = pNode.Childs.GetEnumerator ();
			while (enumerator.MoveNext()) 
			{
				AddItem(new XsollaHistoryVirtualItem().Parse(enumerator.Current) as XsollaHistoryVirtualItem);
			}
			return this;
		}
	}

	public class XsollaHistoryVirtualItem: IXsollaObject,  IParseble
	{
		public int id{get; private set;} //id:"1469" 
		public string name{get; private set;} //name:"Chicken"

		public IParseble Parse(JSONNode pNode)
		{
			id = pNode["id"].AsInt;
			name = pNode["name"];
			return this;
		}

		public string GetKey()
		{
			return id.ToString ();
		}

		public string GetName()
		{
			return name;
		}

	}
}

