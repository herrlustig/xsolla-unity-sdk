
using SimpleJSON;
using System.Collections.Generic;namespace Xsolla {

	public class BuyData : IParseble {
//		public Dictionary<string, object> data;
		public string currency{ get; private set;}//currency: "RUB"
		public decimal sum{ get; private set;}//sum: "0.66"
		public bool enabled{ get; private set;}//enabled: true
		public string example{ get; private set;}//example: null
		public string isMandatory{ get; private set;}//isMandatory: null
		public string isPakets{ get; private set;}//isPakets: null
		public string isReadonly{ get; private set;}//isReadonly: null
		public string isVisible{ get; private set;}//isVisible: "0"
		public string name{ get; private set;}//name: "sub"
		public string options{ get; private set;}//options: null
		public string title{ get; private set;}//title: null
		public string tooltip{ get; private set;}//tooltip: null
		public string type{ get; private set;}//type: "submit"
		public string value{ get; private set;}//value: "Pay now"

		public IParseble Parse(JSONNode buyDataNode)
		{
			if (buyDataNode ["currency"] != null)
				currency = buyDataNode ["currency"];
			if (buyDataNode ["sum"] != null)
				sum = buyDataNode ["sum"].AsDecimal;
			if (buyDataNode ["enabled"] != null)
				enabled = buyDataNode ["enabled"].AsBool;
			if (buyDataNode ["example"] != null)
				example = buyDataNode ["example"];
			if (buyDataNode ["isMandatory"] != null)
				isMandatory = buyDataNode ["isMandatory"];
			if (buyDataNode ["isPakets"] != null)
				isPakets = buyDataNode ["isPakets"];
			if (buyDataNode ["isReadonly"] != null)
				isReadonly = buyDataNode ["isReadonly"];
			if (buyDataNode ["isVisible"] != null)
				isVisible = buyDataNode ["isVisible"];
			if (buyDataNode ["name"] != null)
				name = buyDataNode ["name"];
			if (buyDataNode ["options"] != null)
				options = buyDataNode ["options"];
			if (buyDataNode ["title"] != null)
				title = buyDataNode ["title"];
			if (buyDataNode ["tooltip"] != null)
				tooltip = buyDataNode ["tooltip"];
			if (buyDataNode ["type"] != null)
				type = buyDataNode ["type"];
			if (buyDataNode ["value"] != null)
				value = buyDataNode ["value"];
			return this;
		}


	}
}