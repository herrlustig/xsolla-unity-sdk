﻿using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;

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

		private const String mSummaryUrl = "paystation2/api/cart/summary";
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
					// Запрос на пересчет 
					// Отмена возможного запрос
					CancelInvoke();
					Invoke("GetSummaryRequest", 1);
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
			// Если полное описание пустое, то скрываем ссылку
			if (pItem.GetLongDescription() != "")
				mLongDesc.text = pItem.GetLongDescription();
			else
				mLongDescLink.gameObject.SetActive(false);

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
			SetAmountBlock(pItem.vcAmount,pItem.vcAmountWithoutDiscount,pItem.amount,pItem.amountWithoutDiscount,pItem.currency);
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

		private void SetAmountBlock(Decimal pVcAmount, Decimal pVcAmountWithoutDiscount, Decimal pAmount, Decimal pAmountWithoutDiscount, String pCurrency)
		{
			if (mItem.IsVirtualPayment())
			{
				if (pVcAmount == pVcAmountWithoutDiscount)
					mAmount.text = pVcAmount.ToString("0.00");
				else
					mAmount.text = pVcAmountWithoutDiscount.ToString("0.00") + " " + pVcAmount.ToString("0.00");

				if (mUtils.GetProject().virtualCurrencyIconUrl != "null")
					// если тут придется ошибка с загрузкой, нужно залить альфа канал
					mImgLoader.LoadImage(mVcIcon, mUtils.GetProject().virtualCurrencyIconUrl);
				else
				{
					mAmount.text = mAmount.text + " " + mUtils.GetProject().virtualCurrencyName;
					mVcIcon.gameObject.SetActive(false);
				}
				mCurrency.gameObject.SetActive(false);
			}
			else
			{
				mVcIcon.gameObject.SetActive(false);
				if (pAmount == pAmountWithoutDiscount)
				{
					mAmount.text = CurrencyFormatter.FormatPrice(pCurrency , pAmount.ToString("0.00"));
					if (pCurrency == "RUB")
						mCurrency.enabled = true;
					else
						mCurrency.gameObject.SetActive(false);
				}
				else
				{
					mOldAmount.enabled = true;
					mOldAmount.text = CurrencyFormatter.FormatPrice(pCurrency, pAmountWithoutDiscount.ToString("##.00"));
					mAmount.text = CurrencyFormatter.FormatPrice(pCurrency, pAmount.ToString("##.00"));
					if (pCurrency == "RUB")
						mCurrency.enabled = true;
					else
						mCurrency.gameObject.SetActive(false);
				}
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

		private void SetAmountBlock(XsollaShopItem pItem)
		{	 
			if (pItem.IsVirtualPayment())
			{
				if (pItem.vcAmount == pItem.vcAmountWithoutDiscount)
					mAmount.text = pItem.vcAmount.ToString("##.##");
				else
					mAmount.text = pItem.vcAmountWithoutDiscount.ToString("##.00") + " " + pItem.vcAmount.ToString("##.00");

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

			mBuyBtn.GetComponentInChildren<Text>().text = mUtils.GetTranslations().Get("virtual_item_option_button");
//			if (mUtils.GetSettings().mDesktop.pVirtItems.mButtonWithPrice)
//				mBuyBtn.GetComponentInChildren<Text>().text = "";
//			else
//				mBuyBtn.GetComponentInChildren<Text>().text = mUtils.GetTranslations().Get("virtual_item_option_button");

			mBuyBtn.GetComponent<Button>().onClick.AddListener(delegate
				{
					BuyClick(pItem);
				});

		}

		private void SetStateLongState(bool pState)
		{
			LongDescState = pState;
		}

		private void GetSummaryRequest()
		{
			Logger.Log("Get summary");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("is_virtual_payment", mItem.IsVirtualPayment() ? 1 : 0);	
			lParams.Add(String.Format("sku[{0}]", mItem.GetSku()), ItemQuantity);	
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mSummaryUrl, lParams), SummaryRecived, ErrorRecived);
		}

		private void SummaryRecived(JSONNode pNode)
		{
			Logger.Log("Summary recived");
			XsollaSummaryRecived lSummary = new XsollaSummaryRecived().Parse(pNode) as XsollaSummaryRecived;
			// Заполнить новые цены
			SetAmountBlock(lSummary.mFinance.mTotal.vcAmount, 
						   lSummary.mFinance.mTotalWithOutDiscont.vcAmount, 
						   lSummary.mFinance.mTotal.amount,
						   lSummary.mFinance.mTotalWithOutDiscont.amount,
				lSummary.mFinance.mTotal.currency);
		}

		private void ErrorRecived(JSONNode pErrorNode)
		{
			
		}

		private void BuyClick(XsollaShopItem pItem)
		{
			Logger.Log("Click buy btn " + pItem.GetId());
			Dictionary<string, object> map = new Dictionary<string, object>();
			map.Add ("sku[" + pItem.GetKey() + "]", mCount);
			gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (map, pItem.IsVirtualPayment());
		}
	}

	public class XsollaSummaryRecived: IParseble
	{
		public XsollaApi mApi;
		public XsollaItemFinance mFinance;
		public XsollaItemPurchase mPurchase;
		public bool mSkipConfirmation;

		public IParseble Parse (JSONNode rootNode)
		{
			mFinance = new XsollaItemFinance().Parse(rootNode["finance"]) as XsollaItemFinance;
			mPurchase = new XsollaItemPurchase().Parse(rootNode["purchase"]) as XsollaItemPurchase;
			mSkipConfirmation = rootNode["skip_confirmation"].AsBool;
			mApi = new XsollaApi().Parse(rootNode["api"]) as XsollaApi;
			return this;
		}
	}

	public class XsollaItemFinance: IParseble
	{
		public SummaryTotal mTotal;
		public SummaryTotal mTotalWithOutDiscont;

		public IParseble Parse (JSONNode rootNode)
		{
			mTotal = new SummaryTotal().Parse(rootNode["total"]) as SummaryTotal;
			mTotalWithOutDiscont = new SummaryTotal().Parse(rootNode["total_without_discount"]) as SummaryTotal;
			return this;
		}
	}

	public class SummaryTotal: IParseble
	{
		public Decimal amount;
		public Decimal vcAmount;
		public String currency;

		public IParseble Parse (JSONNode rootNode)
		{
			if (rootNode["amount"] != "null")
				amount = rootNode["amount"].AsDecimal;

			if (rootNode["vc_amount"] != "null")
				vcAmount = rootNode["vc_amount"].AsDecimal;

			if (rootNode["currency"] != "null")
				currency = rootNode["currency"];

			return this;
		}
	}

	public class XsollaItemPurchase: IParseble
	{
		List<XsollaSummaryVirtualItem> mListItems;

		public IParseble Parse (JSONNode rootNode)
		{
			mListItems = new List<XsollaSummaryVirtualItem>();
			JSONNode VirtItemsNode = rootNode["virtual_items"];
			IEnumerator<JSONNode> goodsGroupsEnumerator = VirtItemsNode.Childs.GetEnumerator();
			while(goodsGroupsEnumerator.MoveNext()){
				mListItems.Add(new XsollaSummaryVirtualItem().Parse(goodsGroupsEnumerator.Current) as XsollaSummaryVirtualItem);
			}
			return this;
		}
	}

	public class XsollaSummaryVirtualItem: IParseble
	{
		public String mImgUrl;
		public String mName;
		public int mQuantity;

		public IParseble Parse (JSONNode rootNode)
		{
			mImgUrl = rootNode["image_url"];
			mName = rootNode["name"];
			mQuantity = rootNode["quantity"].AsInt;

			return this;
		}

	}
}

