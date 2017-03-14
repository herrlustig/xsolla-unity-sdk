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
		public GameObject mLongDescLink;
		public Canvas mLongCanvas;
		public GameObject mLongPanel;
		public Text mLongDesc;

		public GameObject mAmountPanel;
		public Text mAmount;
		public Image mVcIcon;
		public Text mOldAmount;
		public Text mCurrency;

		public ImageLoader mImgLoader;

		public Image mListImagePanel;
		public Text mQuantityLabel;
		public Button mQuantityMinus;
		public Button mQuantityPlus;
		public Text mQuantityCount;
		public Text mOldAmountStrange;

		public GameObject mBuyBtn;

		private XsollaShopItem mItem;
		private XsollaUtils mUtils;
		private int mCount = 1;
		public int ItemQuantity
		{
			get 
			{
				return mCount;
			}
			set 
			{
				if (value == 1)
				{
					mQuantityMinus.interactable = false;
				}
				else
					mQuantityMinus.interactable = true;

				if (value == mItem.GetQuantityLimit())
				{
					mQuantityPlus.interactable = false;
				}
				else
					mQuantityPlus.interactable = true;

				if ((value >= 1) || (value <= mItem.GetQuantityLimit()))
				{
					mCount = value;
					mQuantityCount.text = mCount.ToString();
				}
			}
		}

		public Action mCollapseAnotherDesc;
		private bool mLongDescState = false; 
		public bool LongDescState 
		{
			get
			{
				return mLongDescState;
			}
			set
			{
				if (value)
					mCollapseAnotherDesc();
				mLongDescState = value;
				mLongPanel.SetActive(value);
				mLongDescLink.GetComponent<Text>().text = value ? mUtils.GetTranslations().Get("option_description_collapse") : mUtils.GetTranslations().Get("carousel_btn_learn_more");
			}
		}
			
		public void init(XsollaShopItem pItem, XsollaUtils pUtils)
		{
			mItem = pItem;
			mUtils = pUtils;
			// Загружаем картинку
			mImgLoader.LoadImage(mItemImg, pItem.GetImageUrl());
			// Задаем название 
			mItemName.text = pItem.GetName();
			// Задаем короткое описание 
			mShortDesc.text = pItem.GetDescription();
			// Задаем полное описание 
			if (mLongCanvas != null)
				Resizer.ResizeToParrentRe(mLongCanvas.gameObject);
			mLongDesc.text = pItem.GetLongDescription();
			// Задаем иконку любимого товара
			mFav.text = pItem.IsFavorite() ? "" : "";
			// Задаем состояние длинного описания
			LongDescState = false;
			mLongDescLink.GetComponent<Button>().onClick.AddListener(delegate {SetStateLongState(!mLongDescState);});

			// Задаем поля для лэндинга list
			SetListLandingItem(pItem);
			// Рекламный блок
			SetAdBlock(pItem);
			// Ценовой блок
			SetAmountBlock(pItem);
		}

		private void SetListLandingItem(XsollaShopItem pItem)
		{
			// Блок кол-ва
			if (mQuantityLabel != null && mQuantityCount != null)
			{
				mQuantityLabel.text = mUtils.GetTranslations().Get("quantity_label");
				mQuantityCount.text = mCount.ToString();
			}
			// События для кнопок изменения кол-ва товара	
			if (mQuantityMinus != null && mQuantityPlus != null)
			{
				mQuantityMinus.onClick.AddListener(MinusQuantity);
				mQuantityPlus.onClick.AddListener(AddQuantity);
			}
		}

		private void AddQuantity()
		{
			ItemQuantity++;
		}

		private void MinusQuantity()
		{
			ItemQuantity--;
		}

		private void SetAdBlock(XsollaShopItem pItem)
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

		private void SetAmountBlock(XsollaShopItem pItem)
		{	 
			if (pItem.IsVirtualPayment())
			{
				if (pItem.vcAmount == pItem.vcAmountWithoutDiscount)
					mAmount.text = pItem.vcAmount.ToString("##.##");
				else
					mAmount.text = "<s>" + pItem.vcAmountWithoutDiscount.ToString("##.00") + "</s>" + " " + pItem.vcAmount.ToString("##.00");

				if (mUtils.GetProject().virtualCurrencyIconUrl != "null")
					mImgLoader.LoadImage(mVcIcon, mUtils.GetProject().virtualCurrencyIconUrl);
				else
				{
					mAmount.text = mAmount.text + " " + mUtils.GetProject().virtualCurrencyName;
					mVcIcon.gameObject.SetActive(false);
				}
			}
			else
			{
				mVcIcon.gameObject.SetActive(false);
				if (pItem.amount == pItem.amountWithoutDiscount)
				{
					mAmount.text = CurrencyFormatter.FormatPrice(pItem.currency, pItem.amount.ToString("##.00"));
					if (pItem.currency == "RUB")
						mCurrency.enabled = true;
				}
				else
				{
					mOldAmount.enabled = true;
					mOldAmount.text = CurrencyFormatter.FormatPrice(pItem.currency, pItem.amountWithoutDiscount.ToString("##.00"));
					mAmount.text = CurrencyFormatter.FormatPrice(pItem.currency, pItem.amount.ToString("##.00"));
					if (pItem.currency == "RUB")
						mCurrency.enabled = true;
				}
			}
		}

		private void SetStateLongState(bool pState)
		{
			LongDescState = pState;
		}
	}
}

