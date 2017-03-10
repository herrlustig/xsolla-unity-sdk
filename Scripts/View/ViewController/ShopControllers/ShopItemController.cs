using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class ShopItemController: MonoBehaviour
	{
		public Image mMainBckg;
		public GameObject mAdPanel;
		public Image mItemImg;
		public Text mItemName;
		public Text mFav;
		public Text mShortDesc;
		public Text mLongDescLink;

		public GameObject mAmountPanel;
		public Text mAmount;
		public Image mVcIcon;

		public GameObject mBuyBtn;

		public void init(XsollaShopItem pItem, XsollaUtils pUtils)
		{
			// Задаем название 
			mItemName.text = pItem.GetName();
			// Задаем короткое описание 
			mShortDesc.text = pItem.GetDescription();
			// Задаем иконку любимого товара
			mFav.text = pItem.IsFavorite() ? "" : "";

			// Задаем название для длинного оописания
			mLongDescLink.text = pUtils.GetTranslations().Get("carousel_btn_learn_more");

			// Рекламный блок
			switch (pItem.advertisementType) {
			case AXsollaShopItem.AdType.BEST_DEAL:
				{
					mAdPanel.GetComponentInChildren<Text>().text = pItem.label;
					mAdPanel.GetComponent<Image>().sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_bd_panel);
					mMainBckg.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_item_bd);
					break;
				}
			case AXsollaShopItem.AdType.RECCOMENDED:
				{
					mAdPanel.GetComponentInChildren<Text>().text = pItem.label;
					mAdPanel.GetComponent<Image>().sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_ad_panel);
					mMainBckg.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_item_ad);
					break;
				}
			default:
				{
					mAdPanel.GetComponent<Image>().enabled = false;
					mAdPanel.GetComponentInChildren<Text>().enabled = false;
					mMainBckg.sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_item);
					break;
				}
			}

			// Ценовой блока 
			mVcIcon.gameObject.SetActive(pItem.IsVirtualPayment());
			mAmount.text = pItem.GetPriceString();

		}

		public ShopItemController ()
		{
		}
	}
}

