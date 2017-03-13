using System;
using SimpleJSON;

namespace Xsolla
{
	public class XsollaBonus: IParseble
	{
		public string mDesc;//		description:""
		public bool mHasSales;//		has_sales:true
		public string mImgFolder;//		imgFolder:"https://cdn.xsolla.com/paymentoptions/paystation/theme_33/84x45/"
		public string mName;//		name:""
		public string mOffers;//		offers:[]
		public int mSeconds;//		seconds:null
		public string mSpecials;//		specials:null

		public IParseble Parse (JSONNode rootNode)
		{
			mDesc = rootNode["description"].Value;
			mHasSales = rootNode["has_sales"].AsBool;
			mImgFolder = rootNode["imgFolder"].Value;
			mName = rootNode["name"].Value;
			mOffers = rootNode["offers"].Value;
			mSeconds = rootNode["seconds"].AsInt;
			mSpecials = rootNode["specials"].Value;

			return this;
		}
	}
}

