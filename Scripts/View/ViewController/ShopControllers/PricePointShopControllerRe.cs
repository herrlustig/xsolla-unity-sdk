using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Xsolla
{
	public class PricePointShopControllerRe: MonoBehaviour
	{
		public Text mShopTitle;
		public GameObject mShopContent;
		public GameObject mItemsContentGrid;
		public GameObject mCustomAmountLink;
		public GameObject mShopScreen;
		public GameObject mCustomAmountScreen;
		public Text mEmptyLabel;

		private const String mPricePointsUrl = "paystation2/api/pricepoints";

		private const String PREFAB_SHOP_PRICEPOINT_GRID = "Prefabs/SimpleView/_ScreenShop/ShopSimple/ShopPricePointItemRe";

		private XsollaUtils mUtils;
		private List<PricePointItemController> mListItems;
		private CustomVirtCurrAmountController mCustomController;
		private bool mStateCustomAmount;
		private Action mStartProgressBar;
		private Action mStopProgressBar;

		public bool StateCustomAmount
		{
			get
			{
				return mStateCustomAmount;
			}
			set
			{
				mCustomAmountLink.GetComponent<Text>().text = mUtils.GetTranslations().Get(value? "custom_amount_hide_button" : "custom_amount_show_button");

				// Видимость панелей
				mCustomAmountScreen.SetActive(value);
				mShopScreen.SetActive(!value);

				mStateCustomAmount = value;
			}
		}

		public void SetProgressBarAction(Action pStart, Action pStop)
		{
			mStartProgressBar = pStart;
			mStopProgressBar = pStop;
		}

		public void init(XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mListItems = new List<PricePointItemController>();

			mStartProgressBar();

			mShopTitle.text = (pUtils.GetProject().components ["virtual_currency"].Name != "") ? pUtils.GetProject().components["virtual_currency"].Name : pUtils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_TITLE); 

			// Задаем перевод на отсутствие товаров
			mEmptyLabel.text = mUtils.GetTranslations().Get("virtualitem_no_data");
			mEmptyLabel.gameObject.SetActive(false);

			// Возможность произвольной покупки
			if (mUtils.GetSettings().components.virtualCurreny.customAmount)
			{
				mCustomAmountLink.SetActive(true);
				mCustomController = mCustomAmountScreen.GetComponent<CustomVirtCurrAmountController>();
				mCustomAmountLink.GetComponent<Button>().onClick.AddListener(ChangeStateCusomAmount);
				StateCustomAmount = false;
			}
			else
				mCustomAmountLink.SetActive(false);

			// получить список пакетов
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mPricePointsUrl, lParams), PricePointsRecived, ErrorRecived);
		}

		private void PricePointsRecived(JSONNode pNode)
		{
			Logger.Log("PricePoints recived");
			XsollaPricepointsManager lGoods = new XsollaPricepointsManager().Parse(pNode) as XsollaPricepointsManager;

			// Если группа пустая
			mEmptyLabel.gameObject.SetActive(lGoods.GetCount() == 0);
			if (lGoods.GetCount() == 0)
				mItemsContentGrid.SetActive(false);

			// расчитываем кол-во столбцов
			if ((lGoods.GetCount() % 4) == 0) 
			{
				mItemsContentGrid.GetComponent<GridLayoutGroup>().constraintCount = 4;	
			}
			if ((lGoods.GetCount() % 5) == 0) 
			{
				mItemsContentGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(120, mItemsContentGrid.GetComponent<GridLayoutGroup>().cellSize.y);
				mItemsContentGrid.GetComponent<GridLayoutGroup>().constraintCount = 5;	
			}
				
			// Добавляем кнопки 
			lGoods.GetItemsList().ForEach((item) => 
				{ 
					AddPricePointItem(item); 
				});

			// Инициализируем панель кастомного пополнения
			if (mCustomAmountLink.activeSelf)
			{
				int lCountItems = lGoods.GetItemsList().Count;
				int lAvgIdx = lCountItems / 2 + ((lCountItems % 2) > 0 ? 1 : 0);
				mCustomController.init(mUtils, lGoods.GetItemsList()[lAvgIdx - 1]);
			}

			mStopProgressBar();
		}

		private void AddPricePointItem(XsollaPricepoint pItem)
		{
			GameObject lBaseObj = Resources.Load(PREFAB_SHOP_PRICEPOINT_GRID) as GameObject;
			// создаем экземпляр объекта товара
			GameObject lItemObj = Instantiate(lBaseObj);
			// получаем контроллер
			PricePointItemController itemController = lItemObj.GetComponent<PricePointItemController>();
			// инициализируем контроллер
			itemController.init(pItem, mUtils);
			// добавляем на панель
			lItemObj.transform.SetParent(mItemsContentGrid.transform);
			mListItems.Add(itemController);
		}

		private void ChangeStateCusomAmount()
		{
			StateCustomAmount = !StateCustomAmount;
		}

		private void ErrorRecived(XsollaErrorRe pErrors)
		{
			// описание ошибки
		}

		void Update()
		{
			// Подгоняем размер ячейки 
			float lMaxCellHeight = 0;
			mListItems.ForEach((item) => 
				{
					if (item.mMainBckg.gameObject.GetComponent<ContentSizeFitter>().enabled)
						if (item.mMainBckg.gameObject.GetComponent<RectTransform>().rect.height > lMaxCellHeight)
							lMaxCellHeight = item.mMainBckg.gameObject.GetComponent<RectTransform>().rect.height;
				});

			if (lMaxCellHeight != 0)
			{
				Vector2 lCellSize = mItemsContentGrid.GetComponent<GridLayoutGroup>().cellSize;
				mItemsContentGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(lCellSize.x, lMaxCellHeight);
			}

			mListItems.ForEach((item) => 
				{
					item.mMainBckg.gameObject.GetComponent<ContentSizeFitter>().enabled = false;
					item.mMainBckg.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(item.mMainBckg.gameObject.GetComponent<RectTransform>().offsetMin.x, 0);
				});

		}
	}
}

