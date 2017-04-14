using UnityEngine;
using System.Collections;

namespace Xsolla {
	public abstract class AXsollaShopItem : IXsollaObject {
		public string label { get; protected set;}// 						"label":"",
		public string offerLabel { get; protected set;}// 				"offer_label":"",
		public AdType advertisementType { get; protected set;}// 			"advertisementType":null,

		public abstract string GetKey();
		public abstract string GetName();

		public string GetLabel(){
			return !"".Equals (offerLabel) && !"null".Equals (offerLabel) ? offerLabel : !"null".Equals (label) ? label : "SPECIAL OFFER";
		}
		
		public AdType GetAdvertisementType(){
			return advertisementType;
		}


		public enum AdType {
			NONE,
			SPECIAL_OFFER,
			RECCOMENDED,
			BEST_DEAL,
			CUSTOM
		}

		public override string ToString ()
		{
			return string.Format ("[AXsollaShopItem: label={0}, offerLabel={1}, advertisementType={2}]", label, offerLabel, advertisementType);
		}
	}
}
