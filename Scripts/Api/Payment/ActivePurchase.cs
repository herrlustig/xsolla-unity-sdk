using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace Xsolla 
{
	public class ActivePurchase {
	
		private Dictionary<Part, Dictionary<string, object>> purchase;
		public Part lastAdded{ get; private set;}
		public int counter{ get; private set;}

		public enum Part
		{
			TOKEN, INFO, ITEM, PID, XPS, INVOICE, PROCEED, NULL, PAYMENT_MANAGER
		}

		public ActivePurchase(){
			purchase = new Dictionary<Part, Dictionary<string, object>> ();
			lastAdded = Part.NULL;
			counter = 0;
		}

		public void Add(Part part, Dictionary<string, object> map)
		{
			purchase.Add (part, map);
			lastAdded = part;
			counter++;
		}

		
		public void RemoveAllExceptToken(){
			List<Part> keyList = new List<Part>(purchase.Keys);
			foreach (var key in keyList) {
				if(key != Part.TOKEN)
					Remove(key);
			}
			lastAdded = Part.TOKEN;
			counter = 1;
		}

		public void RemoveLast(){
			if (IsActive () && purchase.ContainsKey (lastAdded)) {
				Remove (lastAdded);
				counter--;
			} else {
			}
		}

		public void Remove(Part part){
			if (purchase.ContainsKey (part)) {
				purchase.Remove (part);
				counter--;
			} else {
			}
		}


		public bool ContainsKey(Part part){
			return purchase.ContainsKey (part);
		}

		public Dictionary<string, object> GetPart(Part part){
			return purchase [part];
		}

		public Dictionary<string, object> GetMergedMap(){
			Dictionary<string, object> finalPurchase = new Dictionary<string, object>();
			IEnumerator<KeyValuePair<Part, Dictionary<string, object>>> enumerator = purchase.GetEnumerator ();
			while(enumerator.MoveNext())
			{
				Logger.Log(enumerator.Current.ToString());
				finalPurchase = finalPurchase.Concat(enumerator.Current.Value)
					.ToDictionary (d => d.Key, d => d.Value);
//					.GroupBy(d => d.Key)
//					.ToDictionary (d => d.Key, d => d.First().Value);
			}
			return finalPurchase;
		}

		public bool IsActive()
		{
			return lastAdded != Part.NULL;
		}

	}
}
