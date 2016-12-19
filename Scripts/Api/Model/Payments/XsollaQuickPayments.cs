using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Xsolla {
	public class XsollaQuickPayments :  XsollaObjectsManager<XsollaPaymentMethod>, IParseble {

								//		instances: [{id: 1380, name: "Credit cards"}]
		private XsollaApi api;	//		api: {ver: "1.0.0"}

		public object getApi()
		{
			return api;
		}

		public IParseble Parse(JSONNode quickPaymentsNode)
		{
			//var paymentListEnumerator =  quickPaymentsNode["instances"].Childs.GetEnumerator();
			var paymentListEnumerator =  quickPaymentsNode["quick_instances"].Childs.GetEnumerator();
			while(paymentListEnumerator.MoveNext())
			{
				var method = new XsollaPaymentMethod().Parse(paymentListEnumerator.Current) as XsollaPaymentMethod;
				if(method.id != 64 && method.id != 1738){
					AddItem(method);
				}
			}
			api = new XsollaApi().Parse(quickPaymentsNode[XsollaApiConst.R_API]) as XsollaApi;
			return this;
		}
	}
}
