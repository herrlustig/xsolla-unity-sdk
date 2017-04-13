using System;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections;
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
		public GameObject mQuantityPanel;
		public Button mQuantityMinus;
		public Button mQuantityPlus;
		public Text mQuantityCount;
		public Text mOldAmountStrange;

		public GameObject mBuyBtn;
		public Text mBtnBuyText;
		public MyRotation mBtnProgressBar;

		private const String mSummaryUrl = "paystation2/api/cart/summary";
		private const String mFavoriteUrl = "paystation2/api/virtualitems/setfavorite";
		private XsollaShopItem mItem;
		private XsollaUtils mUtils;
		private int mGroupId;
		private Action<int, bool> mActionResetCacheGroup;
		private int mCount = 1;
		/// <summary>
		/// Gets or sets the item quantity.
		/// </summary>
		/// <value>The item quantity.</value>
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
					ProgressBarBuyBtn(true);
				}
			}
		}
		public Action mCollapseAnotherDesc;
		private bool mLongDescState = false; 
		private bool mIsListLayoutItem;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Xsolla.ShopItemController"/> long desc state.
		/// </summary>
		/// <value><c>true</c> if long desc state; otherwise, <c>false</c>.</value>
		public bool LongDescState 
		{
			get
			{
				return mLongDescState;
			}
			set
			{
				if ((value) && (!mIsListLayoutItem))
					mCollapseAnotherDesc();
				mLongDescState = value;
				mLongPanel.SetActive(value);
				mLongDescLink.GetComponent<Text>().text = value ? mUtils.GetTranslations().Get("option_description_collapse") : mUtils.GetTranslations().Get("carousel_btn_learn_more");
			}
		}
			
		/// <summary>
		/// Init the specified pItem, pUtils, pGroupId, pActionResetCacheGroup and pList.
		/// </summary>
		/// <param name="pItem">P item.</param>
		/// <param name="pUtils">P utils.</param>
		/// <param name="pGroupId">P group identifier.</param>
		/// <param name="pActionResetCacheGroup">P action reset cache group.</param>
		/// <param name="pList">If set to <c>true</c> p list.</param>
		public void init(XsollaShopItem pItem, XsollaUtils pUtils, int pGroupId, Action<int, bool> pActionResetCacheGroup, bool pList)
		{
			mItem = pItem;
			mUtils = pUtils;
			mGroupId = pGroupId;
			mActionResetCacheGroup = pActionResetCacheGroup;
			mIsListLayoutItem = pList;

			// Загружаем картинку
			mImgLoader.LoadImage(mItemImg, pItem.GetImageUrl());

			// Задаем название 
			mItemName.text = pItem.GetName();

			// Задаем короткое описание 
			mShortDesc.text = pItem.GetDescription();

			// Дефолтное состояние прогресс бара
			ProgressBarBuyBtn(false);

			// Задаем полное описание 
			if (mLongCanvas != null)
				Resizer.ResizeToParrentRe(mLongCanvas.gameObject);
			// Если полное описание пустое, то скрываем ссылку
			if (pItem.GetLongDescription() != "")
				mLongDesc.text = pItem.GetLongDescription();
			else
				mLongDescLink.gameObject.SetActive(false);

			// Задаем иконку любимого товара
			SetFavoriteState();
			mFav.gameObject.GetComponent<Button>().onClick.AddListener(delegate {ChangeFavState();});

			// Задаем состояние длинного описания
			LongDescState = false;
			mLongDescLink.GetComponent<Button>().onClick.AddListener(delegate {SetStateLongState(!mLongDescState);});

			// Блокируем минус
			if (mQuantityMinus != null)
				mQuantityMinus.interactable = false;

			// Задаем поля для лэндинга list
			SetListLandingItem(pItem);
			// Рекламный блок
			SetAdBlock(pItem);
			// Ценовой блок
			SetAmountBlock(pItem.vcAmount,pItem.vcAmountWithoutDiscount,pItem.amount,pItem.amountWithoutDiscount,pItem.currency);
		}

		/// <summary>
		/// Sets the state of the favorite.
		/// </summary>
		private void SetFavoriteState()
		{
			// Проверка на бессерверную интеграцию
			mFav.gameObject.SetActive(mUtils.GetUser().userBalance != null);
			mFav.text = mItem.IsFavorite() ? "" : "";
		}

		/// <summary>
		/// Changes the state of the fav.
		/// </summary>
		private void ChangeFavState()
		{
			Logger.Log("Get summary");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			if (mUtils.GetUser().userBalance != null)
				lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			lParams.Add("virtual_item_id", mItem.GetId());
			lParams.Add("is_favorite", mItem.IsFavorite()? "0" : "1");
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mFavoriteUrl, lParams), FavoriteRecived, ErrorRecived, false);
		}

		/// <summary>
		/// Favorites the recived.
		/// </summary>
		/// <param name="pNode">P node.</param>
		private void FavoriteRecived(JSONNode pNode)
		{
			int lFavState = pNode["is_favorite"].AsInt;
			mItem.isFavorite = lFavState;
			SetFavoriteState();
			// Сбросим статус кэша для группы в которой находится этот товар
			Logger.Log("Next respond on groupID - " + mGroupId + " without Cache");
			mActionResetCacheGroup(mGroupId, false);
		}
			
		/// <summary>
		/// Sets the list landing item.
		/// </summary>
		/// <param name="pItem">P item.</param>
		private void SetListLandingItem(XsollaShopItem pItem)
		{
			if (mQuantityLabel == null)
				return;

			if ((pItem.GetQuantityLimit() > 1) || (pItem.GetQuantityLimit() == 0))
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
			else 
			{
				// убираем панель с кол-вом 
				mQuantityLabel.gameObject.SetActive(false);
				mQuantityPanel.SetActive(false);
			}
		}

		/// <summary>
		/// Adds the quantity.
		/// </summary>
		private void AddQuantity()
		{
			ItemQuantity++;
		}

		/// <summary>
		/// Minuses the quantity.
		/// </summary>
		private void MinusQuantity()
		{
			ItemQuantity--;
		}

		/// <summary>
		/// Sets the ad block.
		/// </summary>
		/// <param name="pItem">P item.</param>
		private void SetAdBlock(XsollaShopItem pItem)
		{
			StyleManager.BaseSprite lItemBckg = ShopItemHelper.SetAdBlockItem(pItem, mUtils, mAdPanel.GetComponentInChildren<Text>(), mAdPanel.GetComponent<Image>());
			if ((lItemBckg == StyleManager.BaseSprite.bckg_item) && (mIsListLayoutItem))
			{
				mAdPanel.SetActive(false);
			}
			SetSpecialAdBkcg(lItemBckg);
		}

		/// <summary>
		/// Sets the special ad bkcg.
		/// </summary>
		/// <param name="pSprite">P sprite.</param>
		private void SetSpecialAdBkcg(StyleManager.BaseSprite pSprite)
		{
			mMainBckg.sprite = StyleManager.Instance.GetSprite(pSprite);
			if (mListImagePanel != null)
				mListImagePanel.sprite = StyleManager.Instance.GetSprite(pSprite);	
		}

		/// <summary>
		/// Sets the amount block.
		/// </summary>
		/// <param name="pVcAmount">P vc amount.</param>
		/// <param name="pVcAmountWithoutDiscount">P vc amount without discount.</param>
		/// <param name="pAmount">P amount.</param>
		/// <param name="pAmountWithoutDiscount">P amount without discount.</param>
		/// <param name="pCurrency">P currency.</param>
		private void SetAmountBlock(Decimal pVcAmount, Decimal pVcAmountWithoutDiscount, Decimal pAmount, Decimal pAmountWithoutDiscount, String pCurrency)
		{
			// Виртуальная покупка или нет
			if (mItem.IsVirtualPayment())
			{
				if (pVcAmount == pVcAmountWithoutDiscount)
					mAmount.text = pVcAmount.ToString("N2");
				else
					mAmount.text = pVcAmountWithoutDiscount.ToString("N2") + " " + pVcAmount.ToString("N2");

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
					mAmount.text = CurrencyFormatter.FormatPrice(pCurrency , pAmount.ToString("N2"));
					if (pCurrency == "RUB")
						mCurrency.enabled = true;
					else
						mCurrency.gameObject.SetActive(false);
				}
				else
				{
					mOldAmount.enabled = true;
					mOldAmount.text = CurrencyFormatter.FormatPrice(pCurrency, pAmountWithoutDiscount.ToString("N2"));
					mAmount.text = CurrencyFormatter.FormatPrice(pCurrency, pAmount.ToString("N2"));
					if (pCurrency == "RUB")
						mCurrency.enabled = true;
					else
						mCurrency.gameObject.SetActive(false);
				}
			}


			// Название для кнопки покупки
			mBtnBuyText.text = mUtils.GetTranslations().Get("virtual_item_option_button");

			// Если цена должна быть в кнопке
			if (mUtils.GetSettings().mDesktop.pVirtItems.mButtonWithPrice)
			{
				// Скрываем текст самой кнопки
				mBuyBtn.GetComponentInChildren<Text>().gameObject.SetActive(false);
				// Переносим ценовой блок в кнопку
				mAmountPanel.transform.SetParent(mBuyBtn.transform);
				// Растягиваем 
				Resizer.ResizeToParrentRe(mAmountPanel);
				// Меняем цвет текстов
				IEnumerator lEnum = mAmountPanel.transform.GetComponentsInChildren<ColorController>().GetEnumerator();
				while(lEnum.MoveNext())
					(lEnum.Current as ColorController).itemsToColor[0].color = StyleManager.BaseColor.txt_white;
			}

			mBuyBtn.GetComponent<Button>().onClick.AddListener(delegate
				{
					BuyClick(mItem);
				});
		}

		/// <summary>
		/// Progresses the bar buy button.
		/// </summary>
		/// <param name="pState">If set to <c>true</c> p state.</param>
		private void ProgressBarBuyBtn(bool pState)
		{
			mBtnProgressBar.SetLoading(pState);
			mBtnBuyText.gameObject.SetActive(!pState);
			mAmountPanel.SetActive(!pState);
			mBuyBtn.GetComponent<Image>().color = pState ? new Color(255,255,255,0) : new Color(255,255,255,255);
		}

		/// <summary>
		/// Sets the state of the state long.
		/// </summary>
		/// <param name="pState">If set to <c>true</c> p state.</param>
		private void SetStateLongState(bool pState)
		{
			LongDescState = pState;
		}

		/// <summary>
		/// Gets the summary request.
		/// </summary>
		private void GetSummaryRequest()
		{
			Logger.Log("Get summary");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add("is_virtual_payment", mItem.IsVirtualPayment() ? 1 : 0);	
			lParams.Add(String.Format("sku[{0}]", mItem.GetSku()), ItemQuantity);	
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mSummaryUrl, lParams), SummaryRecived, ErrorRecived, false);
		}

		/// <summary>
		/// Summaries the recived.
		/// </summary>
		/// <param name="pNode">P node.</param>
		private void SummaryRecived(JSONNode pNode)
		{
			Logger.Log("Summary recived");
			ProgressBarBuyBtn(false);
			XsollaSummaryRecived lSummary = new XsollaSummaryRecived().Parse(pNode) as XsollaSummaryRecived;
			// Заполнить новые цены
			SetAmountBlock(lSummary.mFinance.mTotal.vcAmount, 
						   lSummary.mFinance.mTotalWithOutDiscont.vcAmount, 
						   lSummary.mFinance.mTotal.amount,
						   lSummary.mFinance.mTotalWithOutDiscont.amount,
				lSummary.mFinance.mTotal.currency);
		}

		/// <summary>
		/// Errors the recived.
		/// </summary>
		/// <param name="pErrors">P errors.</param>
		private void ErrorRecived(XsollaErrorRe pErrors)
		{
			ProgressBarBuyBtn(false);
		}

		/// <summary>
		/// Buies the click.
		/// </summary>
		/// <param name="pItem">P item.</param>
		private void BuyClick(XsollaShopItem pItem)
		{
			Logger.Log("Click buy btn " + pItem.GetId());
			Dictionary<string, object> map = new Dictionary<string, object>();
			map.Add ("sku[" + pItem.GetKey() + "]", mCount);
			gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (map, pItem.IsVirtualPayment());
		}

		public void Update()
		{
			
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

