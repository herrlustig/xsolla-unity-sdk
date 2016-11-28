using System;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Xsolla
{
	public class XsollaSavedPaymentMethods: XsollaObjectsManager<XsollaSavedPaymentMethod>, IParseble 
	{

		private XsollaApi api;

		public object getApi()
		{
			return api;
		}

		public List<XsollaSavedPaymentMethod> GetSortedItems(string s)
		{
			return itemsList.FindAll (delegate(XsollaSavedPaymentMethod xpm) {
				return xpm.GetName().ToLower().StartsWith (s.ToLower());
			});
		}

		public List<XsollaSavedPaymentMethod> GetItemList()
		{
			return this.itemsList;
		}

		public IParseble Parse (JSONNode paymentListNode)
		{
			IEnumerator<JSONNode> paymentListEnumerator =  paymentListNode["list"].Childs.GetEnumerator();
			while(paymentListEnumerator.MoveNext())
			{
				XsollaSavedPaymentMethod method = new XsollaSavedPaymentMethod().Parse(paymentListEnumerator.Current) as XsollaSavedPaymentMethod;
				AddItem(method);

			}
			api = new XsollaApi().Parse(paymentListNode["api"]) as XsollaApi;
			return this;
		}
	}

	public class XsollaSavedPaymentMethod : IXsollaObject, IParseble
	{
		private long id {get; set;}
		private string type {get; set;}
		private string currency {get; set;}
		private string name {get; set;}
		private long pid {get; set;}
		private string reccurentType {get; set;}
		private SavedMethodForm form {get; set;} // "form":{"paymentSid":"pDEPBqz5qAoFWnBz"}
		private bool replaced {get; set;}
		private string psName {get; set;}
		private string iconSrc {get; set;}
		private bool isSelected {get; set;}

		public string GetImageUrl()
		{
			if(iconSrc.StartsWith("https:"))
				return iconSrc;
			else
				return "https:" + iconSrc;
		}

		public string GetKey()
		{
			return id.ToString();
		}

		public string GetMethodType()
		{
			return type;
		}

		public long GetPid()
		{
			return pid;
		}

		public string GetName()
		{
			return name;
		}

		public string GetPsName()
		{
			return psName;
		}

		public object GetForm()
		{
			return form;
		}

		public string GetFormSid()
		{
			if (form != null)
				return form.getPaymentSid();
			else
				return "";
		}

		public string GetCurrency()
		{
			return currency;
		}

		public IParseble Parse (JSONNode pJsonNode)
		{
			id = pJsonNode["id"].AsInt;
			type = pJsonNode["type"];
			currency = pJsonNode["currency"];
			name = pJsonNode["name"];
			pid = pJsonNode["pid"].AsInt;
			reccurentType = pJsonNode["reccurentType"];
			form = new SavedMethodForm().Parse(pJsonNode["form"]) as SavedMethodForm;
			replaced = pJsonNode["replaced"].AsBool;
			psName = pJsonNode["psName"];
			iconSrc = pJsonNode["iconSrc"];
			isSelected = pJsonNode["isSelected"].AsBool;
			return this;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaSavedPaymentMethod: id={0}, type={1}, currency={2}, name={3}, pid={4}, reccurentType={5}, form={6}, replaced={7}, psName={8}, iconSrc={9}, isSelected={10}]", id, type, currency, name, pid, reccurentType, form, replaced, psName, iconSrc, isSelected);
		}
	}

	public class SavedMethodForm : IParseble
	{
		private string paymentSid;

		public string getPaymentSid()
		{
			return paymentSid;
		}

		public IParseble Parse(JSONNode pJsonNode)
		{
			paymentSid = pJsonNode["paymentSid"];
			return this;
		}
	}


}

