
using SimpleJSON;

namespace Xsolla 
{
	public class XsollaSettings : IParseble 
	{

		public string version;//"version":"mobile"
		public XsollaPaystation2 paystation2;//"paystation2":{}
		public Components components;
		public XsollaSettingDescktop mDesktop;
		public string theme;

		public string GetTheme(){
			if (theme != null && !"null".Equals (theme) && !"theme".Equals (theme))
				if("default".Equals(theme) || "dark".Equals(theme))
					return theme;
				else 
					return "dark";
			else
				return "dark";
		}

		public IParseble Parse (JSONNode rootNode)
		{
			version = rootNode["version"];
			var paystation2Node = rootNode["paystation2"];
			paystation2 = new XsollaPaystation2 ().Parse (paystation2Node) as XsollaPaystation2;
			theme = rootNode["theme"];
			mDesktop = new XsollaSettingDescktop().Parse(rootNode["desktop"]) as XsollaSettingDescktop;
			components = new Components().Parse(rootNode["components"]) as Components;
			return this;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaSettings]");
		}
	}

	public class XsollaSettingDescktop: IParseble
	{
		public XsollaVirtualItems pVirtItems;

		public IParseble Parse(JSONNode pRootNode)
		{
			pVirtItems = new XsollaVirtualItems().Parse(pRootNode["virtual_item_list"]) as XsollaVirtualItems;
			return this;
		}
	}

	public class XsollaVirtualItems: IParseble
	{
		public bool mButtonWithPrice;
		public string pLayout;


		public IParseble Parse(JSONNode pRootNode)
		{
			mButtonWithPrice = pRootNode["button_with_price"].AsBool;
			pLayout = pRootNode["layout"].Value;
			return this;
		}
	}

	public class Components: IParseble
	{
		public VirtualCurrency virtualCurreny;

		public IParseble Parse(JSONNode pRootNode)
		{
			virtualCurreny = new VirtualCurrency().Parse(pRootNode["virtual_currency"]) as VirtualCurrency;
			return this;
		}

	}

	public class VirtualCurrency: IParseble
	{
		public bool customAmount;

		public IParseble Parse(JSONNode pRootNode)
		{
			customAmount = pRootNode["custom_amount"].AsBool;
			return this;
		}
	}

	public class XsollaPaystation2 : IParseble
	{
		public string iconUrl {get; private set;}// "icon_url":"paystation\/theme_33\/143x83\/%id%.png"
		public string formIcopnUrl {get; private set;}// "form_icon_url":"paystation\/theme_33\/84x45\/%id%.png"
		public string pricepointsAtFirst {get; private set;}// "pricepoints_at_first":"1"
		public string subscriptionAtFirst {get; private set;}// "subscriptions_at_first":"1"
		public string bonusTimerShow {get; private set;}// "bonus_timer_show":"1"
		public string goodsAtFirst {get; private set;}// "goods_at_first":"1"
		public string countryRemove {get; private set;}// "country_remove":"KP"
		public string statusRowExclude {get; private set;}// "status_rows_exclude":"details"

		public IParseble Parse (JSONNode paystation2Node)
		{
			iconUrl = paystation2Node ["icon_url"];
			formIcopnUrl = paystation2Node ["form_icon_url"];
			pricepointsAtFirst = paystation2Node ["pricepoints_at_first"];
			subscriptionAtFirst = paystation2Node ["subscriptions_at_first"];
			bonusTimerShow = paystation2Node ["bonus_timer_show"];
			goodsAtFirst = paystation2Node ["goods_at_first"];
			countryRemove = paystation2Node ["country_remove"];
			statusRowExclude = paystation2Node ["status_rows_exclude"];
			return this;
		}
	}
}