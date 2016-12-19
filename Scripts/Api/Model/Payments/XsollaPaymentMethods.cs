using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Xsolla 
{
	public class XsollaPaymentMethods : XsollaObjectsManager<XsollaPaymentMethod>, IParseble 
	{
		private object lastPayment;//						"lastPayment" : null,
		private XsollaApi api;//							"api" : { ... }

		public object getApi()
		{
			return api;
		}

		public object GetLastPayment(){
			return lastPayment;
		}



		public List<XsollaPaymentMethod> GetListOnType(XsollaPaymentMethod.TypePayment pType)
		{

			return itemsList.FindAll (delegate(XsollaPaymentMethod xpm) 
				{
					return xpm.typePayment == pType;
				});
		}

		public List<XsollaPaymentMethod> GetListOnType()
		{
				return itemsList;	
		}

		public List<XsollaPaymentMethod> GetSortedItems(string s)
		{
			return itemsList.FindAll (delegate(XsollaPaymentMethod xpm) {
				return xpm.name.ToLower().StartsWith (s.ToLower());
			});
		}

		public List<XsollaPaymentMethod> GetRecomendedItems()
		{
			return GetItemsList().FindAll (delegate(XsollaPaymentMethod xpm) {
				return xpm.isHidden == 0 && xpm.isRecommended == 1;
			});

		}

		public IParseble Parse (JSONNode paymentListNode)
		{
			// fetch all quick methods
			IEnumerator<JSONNode> paymentListEnumerator =  paymentListNode["quick_instances"].Childs.GetEnumerator();
			while(paymentListEnumerator.MoveNext())
			{
				XsollaPaymentMethod method = new XsollaPaymentMethod().Parse(paymentListEnumerator.Current) as XsollaPaymentMethod;
				// if count quick methods 3 then add to regular 
				if (this.GetCount() <= 2)
					method.SetType(XsollaPaymentMethod.TypePayment.QUICK);
				else
					method.SetType(XsollaPaymentMethod.TypePayment.REGULAR);
				
				if(method.id != 64 && method.id != 1738){
					AddItem(method);
				}
			}

			// fetch all regular methods
			paymentListEnumerator =  paymentListNode["regular_instances"].Childs.GetEnumerator();
			while(paymentListEnumerator.MoveNext())
			{
				XsollaPaymentMethod method = new XsollaPaymentMethod().Parse(paymentListEnumerator.Current) as XsollaPaymentMethod;
				method.SetType(XsollaPaymentMethod.TypePayment.REGULAR);
				if(method.id != 64 && method.id != 1738){
					AddItem(method);
				}
			}

			lastPayment = null;
			api = new XsollaApi().Parse(paymentListNode["api"]) as XsollaApi;
			return this;
		}
	}

	public class XsollaPaymentMethod : IXsollaObject, IParseble
	{
		public long id {get; private set;}//				"id" : 24,
		public int isHidden {get; private set;}//			"hidden" : 0, show 0
		public string aliases {get; private set;}//			"aliases" : ,
		public int isRecommended {get; private set;}//		"recommended" : 1, show 1
		public int[] cat {get; private set;}//				"cat" : [ ... ],
		public string recurrentType {get; private set;}//	"recurrent_type" : charge,
		public string name {get; private set;}//			"name" : PayPal,
		public string imgUrl {get; private set;}//			"image_url" : //xsolla.cachefly.net/paymentoptions/paystation/theme_33/143x83/24.1421413162.png
		public string imgUrl2x {get; private set;}//			"image_2x_url" : //xsolla.cachefly.net/paymentoptions/paystation/theme_33/143x83/24.1421413162.png
		public bool isVisible {get; private set;} //		"is_visible" // true\false
		public TypePayment typePayment {get; private set;} 

		public enum TypePayment
		{
			QUICK, REGULAR
		}
			
		public string GetImageUrl()
		{
			if(imgUrl2x.StartsWith("https:"))
				return imgUrl2x;
			else 
				return "https:" + imgUrl2x;
		}

		public string GetKey()
		{
			return id.ToString ();
		}

		public string GetName()
		{
			return name;
		}

		public IParseble Parse (JSONNode paymentMethodNode)
		{
			id = paymentMethodNode ["id"].AsInt;
			isHidden = paymentMethodNode["hidden"].AsInt;
			aliases = paymentMethodNode["aliases"];
			isRecommended = paymentMethodNode["recommended"].AsInt;
			IEnumerator<JSONNode> catEnumerator =  paymentMethodNode["cat"].Childs.GetEnumerator();
			cat = new int[paymentMethodNode["cat"].Count];
			int counter = 0;
			while (catEnumerator.MoveNext()) 
			{
				cat[counter] = catEnumerator.Current.AsInt;
				counter++;
			}
			recurrentType = paymentMethodNode["recurrentType"];
			name = paymentMethodNode ["name"];
			imgUrl = paymentMethodNode["image_url"];
			imgUrl2x = paymentMethodNode["image_2x_url"];
			isVisible = paymentMethodNode["is_visible"].AsBool;
			return this;
		}

		public void SetType(TypePayment pType)
		{
			typePayment = pType;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaPaymentMethod: id={0}, isHidden={1}, aliases={2}, isRecommended={3}, cat={4}, recurrentType={5}, name={6}, imgUrl={7}]", id, isHidden, aliases, isRecommended, cat, recurrentType, name, imgUrl);
		}
	}
}
