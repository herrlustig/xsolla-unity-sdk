using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

namespace Xsolla
{
	public class ShopItemHelper
	{
		public static StyleManager.BaseSprite SetAdBlockItem(XsollaPricepoint pItem, XsollaUtils pUtils, Text pLabel, Image pPanel)
		{
			bool isOffer = (pItem.sum != pItem.sumWithoutDiscount) || (pItem.bonusItems.Count > 0);

			String lDisplaLabel;
			if (isOffer)
				lDisplaLabel = pItem.offerLabel != "" ? pItem.offerLabel : pUtils.GetTranslations().Get("option_offer_desktop");
			else
				lDisplaLabel = pItem.label;

			if (!isOffer && pItem.advertisementType != AXsollaShopItem.AdType.NONE)
			{
				switch (pItem.advertisementType) {
				case AXsollaShopItem.AdType.BEST_DEAL:
					{
						pLabel.text = pItem.label;
						pPanel.enabled = true;
						pPanel.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_bd_panel);
						return StyleManager.BaseSprite.bckg_item_bd;
					}
				case AXsollaShopItem.AdType.RECCOMENDED:
					{
						pLabel.text = pItem.label;
						pPanel.enabled = true;
						pPanel.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_ad_panel);
						return StyleManager.BaseSprite.bckg_item_ad;
					}
				case AXsollaShopItem.AdType.SPECIAL_OFFER:
					{
						pLabel.text = pItem.label;
						pPanel.enabled = true;
						pPanel.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_sales_panel);
						return StyleManager.BaseSprite.bckg_item_sales;
					}
				case AXsollaShopItem.AdType.CUSTOM:
					{
						pLabel.text = pItem.label;
						pPanel.enabled = true;
						pPanel.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_sales_panel);
						return StyleManager.BaseSprite.bckg_item_sales;
					}
				default:
					{
						return StyleManager.BaseSprite.bckg_item;
					}
				}
			}	
			return StyleManager.BaseSprite.bckg_item;
		}

		public static StyleManager.BaseSprite SetAdBlockItem(XsollaShopItem pItem, XsollaUtils pUtils, Text pLabel, Image pPanel)
		{
			bool isOffer = (pItem.amount != pItem.amountWithoutDiscount) || (pItem.vcAmount != pItem.vcAmountWithoutDiscount) || (pItem.bonusVirtualItems.Count > 0) || (pItem.bonusVirtualCurrency.quantity > 0);

			String lDisplaLabel;
			if (isOffer)
				lDisplaLabel = pItem.offerLabel != "" ? pItem.offerLabel : pUtils.GetTranslations().Get("option_offer_desktop");
			else
				lDisplaLabel = pItem.label;

			if (!isOffer && pItem.advertisementType != AXsollaShopItem.AdType.NONE)
			{
				switch (pItem.advertisementType) {
				case AXsollaShopItem.AdType.BEST_DEAL:
					{
						pLabel.text = pItem.label;
						pPanel.enabled = true;
						pPanel.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_bd_panel);
						return StyleManager.BaseSprite.bckg_item_bd;
					}
				case AXsollaShopItem.AdType.RECCOMENDED:
					{
						pLabel.text = pItem.label;
						pPanel.enabled = true;
						pPanel.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_ad_panel);
						return StyleManager.BaseSprite.bckg_item_ad;
					}
				case AXsollaShopItem.AdType.SPECIAL_OFFER:
					{
						pLabel.text = pItem.label;
						pPanel.enabled = true;
						pPanel.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_sales_panel);
						return StyleManager.BaseSprite.bckg_item_sales;
					}
				default:
					{
						pPanel.color = new Color(255,255,255,0);
						pLabel.text = "";
						return StyleManager.BaseSprite.bckg_item;
					}
				}
			}	
			pPanel.color = new Color(255,255,255,0);
			pLabel.text = "";
			return StyleManager.BaseSprite.bckg_item;
		}
	}
}

