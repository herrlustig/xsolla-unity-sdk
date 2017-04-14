
using SimpleJSON;
using System.Collections.Generic;

namespace Xsolla
{
	public class XsollaBonusItem : IParseble {

		public string name {get; private set;}// 	"name":"Milk",
		public int quantity {get; private set;}//"quantity":"1"

		public static List<XsollaBonusItem> ParseMany(JSONNode bonusItemsNode)
		{
			var bonusItems = new List<XsollaBonusItem>(bonusItemsNode.Count);
			var bonusItemsEnumerator = bonusItemsNode.Childs.GetEnumerator ();
			while(bonusItemsEnumerator.MoveNext())
			{
				bonusItems.Add(new XsollaBonusItem().Parse(bonusItemsEnumerator.Current) as XsollaBonusItem);
			}
			return bonusItems;
		}

		public IParseble Parse (JSONNode bonusItemsNode)
		{
			name = bonusItemsNode["name"];
			quantity = bonusItemsNode["quantity"].AsInt;
			return this;
		}
	}

}
