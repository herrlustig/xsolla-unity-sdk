using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

namespace Xsolla
{
	public class PricePointItemController: MonoBehaviour
	{
		public Image mMainBckg;
		public GameObject mAdPanel;
		public Image mItemImg;
		public Text mItemName;
		public Text mVcCurr;
		public Text mShortDesc;

		public GameObject mAmountPanel;
		public Text mAmount;
		public Text mOldAmount;
		public Text mCurrency;

		public ImageLoader mImgLoader;

		public Image mListImagePanel;

		public GameObject mBuyBtn;

		private XsollaPricepoint mItem;
		private XsollaUtils mUtils;

		public void init(XsollaPricepoint pItem, XsollaUtils pUtils)
		{
			mItem = pItem;
			mUtils = pUtils;

			// Загружаем картинку
			if (pItem.image != "null")
				mImgLoader.LoadImage(mItemImg, pItem.GetImageUrl());
			else
				mItemImg.gameObject.transform.parent.gameObject.SetActive(false);

			// Задаем короткое описание 
			mShortDesc.text = pItem.GetDescription();

			// Задаем название
			mItemName.text = pItem.outAmount.ToString();
			mVcCurr.text = mUtils.GetProject().virtualCurrencyName;

			// Рекламный блок
			SetAdBlock(pItem);
			// Ценовой блок
			SetAmountBlock(pItem.sum, pItem.sumWithoutDiscount, pItem.currency);
		}

		private void SetAdBlock(XsollaPricepoint pItem)
		{
			switch (pItem.advertisementType) {
			case AXsollaShopItem.AdType.BEST_DEAL:
				{
					mAdPanel.GetComponentInChildren<Text>().text = pItem.label;
					mAdPanel.GetComponent<Image>().sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_bd_panel);
					SetSpecialAdBkcg(StyleManager.BaseSprite.bckg_item_bd);
					break;
				}
			case AXsollaShopItem.AdType.RECCOMENDED:
				{
					mAdPanel.GetComponentInChildren<Text>().text = pItem.label;
					mAdPanel.GetComponent<Image>().sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_ad_panel);
					SetSpecialAdBkcg(StyleManager.BaseSprite.bckg_item_ad);
					break;
				}
			case AXsollaShopItem.AdType.SPECIAL_OFFER:
				{
					mAdPanel.GetComponentInChildren<Text>().text = pItem.label;
					mAdPanel.GetComponent<Image>().sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_sales_panel);
					SetSpecialAdBkcg(StyleManager.BaseSprite.bckg_item_sales);
					break;
				}
			default:
				{
					if (pItem.offerLabel != "")
					{
						mAdPanel.GetComponentInChildren<Text>().text = pItem.offerLabel;
						mAdPanel.GetComponent<Image>().sprite = StyleManager.Instance.GetSprite(StyleManager.BaseSprite.bckg_sales_panel);
						SetSpecialAdBkcg(StyleManager.BaseSprite.bckg_item_sales);
					}
					else
					{
						mAdPanel.GetComponent<Image>().enabled = false;
						mAdPanel.GetComponentInChildren<Text>().enabled = false;
						SetSpecialAdBkcg(StyleManager.BaseSprite.bckg_item);
					}
					break;
				}
			}
		}

		private void SetSpecialAdBkcg(StyleManager.BaseSprite pSprite)
		{
			mMainBckg.sprite = StyleManager.Instance.GetSprite(pSprite);
			if (mListImagePanel != null)
				mListImagePanel.sprite = StyleManager.Instance.GetSprite(pSprite);	
		}

		private void SetAmountBlock(Decimal pAmount, Decimal pAmountWithoutDiscount, String pCurrency)
		{
			
			if (pAmount == pAmountWithoutDiscount)
			{
				mAmount.text = CurrencyFormatter.FormatPrice(pCurrency , pAmount.ToString("0.00"));
				if (pCurrency == "RUB")
					mCurrency.enabled = true;
			}
			else
			{
				mOldAmount.enabled = true;
				mOldAmount.text = CurrencyFormatter.FormatPrice(pCurrency, pAmountWithoutDiscount.ToString("0.00"));
				mAmount.text = CurrencyFormatter.FormatPrice(pCurrency, pAmount.ToString("0.00"));
				if (pCurrency == "RUB")
					mCurrency.enabled = true;
			}


			mBuyBtn.GetComponentInChildren<Text>().text = mUtils.GetTranslations().Get("virtual_item_option_button");
//			if (mUtils.GetSettings().mDesktop.pVirtItems.mButtonWithPrice)
//				mBuyBtn.GetComponentInChildren<Text>().text = "";
//			else
//				mBuyBtn.GetComponentInChildren<Text>().text = mUtils.GetTranslations().Get("virtual_item_option_button");

			mBuyBtn.GetComponent<Button>().onClick.AddListener(delegate
				{
					BuyClick(mItem);
				});
		}

		private void BuyClick(XsollaPricepoint pItem)
		{
			Logger.Log("Click buy btn ");
			Dictionary<string, object> map = new Dictionary<string, object> (1);
			map.Add ("out", pItem.outAmount);
			gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (map, false);
		}

	}
}

