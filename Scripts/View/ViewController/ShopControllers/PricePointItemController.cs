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
				if (pUtils.GetProject().virtualCurrencyIconUrl != "null")
					mImgLoader.LoadImage(mItemImg, pUtils.GetProject().virtualCurrencyIconUrl);
				else
					mItemImg.color = new Color(255,255,255,0);

			// Задаем короткое описание 
			mShortDesc.text = pItem.GetDescription();

			// Задаем название
			mItemName.text = pUtils.GetProject().isDiscrete ? pItem.outAmount.ToString("N2") : pItem.outAmount.ToString("##.00");
			mVcCurr.text = mUtils.GetProject().virtualCurrencyName;

			// Рекламный блок
			SetAdBlock(pItem);
			// Ценовой блок
			SetAmountBlock(pItem.sum, pItem.sumWithoutDiscount, pItem.currency);
		}

		private void SetAdBlock(XsollaPricepoint pItem)
		{
			StyleManager.BaseSprite lItemBckg = ShopItemHelper.SetAdBlockItem(pItem, mUtils, mAdPanel.GetComponentInChildren<Text>(), mAdPanel.GetComponent<Image>());
			SetSpecialAdBkcg(lItemBckg);
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
				mAmount.text = CurrencyFormatter.Instance.FormatPrice(pCurrency , pAmount);
			}
			else
			{
				mOldAmount.enabled = true;
				mOldAmount.text = CurrencyFormatter.Instance.FormatPrice(pCurrency, pAmountWithoutDiscount);
				mAmount.text = CurrencyFormatter.Instance.FormatPrice(pCurrency, pAmount);
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

