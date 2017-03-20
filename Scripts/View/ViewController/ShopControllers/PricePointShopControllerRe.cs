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

		private const String mPricePoints = "paystation2/api/pricepoints";

		private const String PREFAB_SHOP_PRICEPOINT_GRID = "Prefabs/SimpleView/_ScreenShop/ShopSimple/ShopPricePointItemRe";

		private XsollaUtils mUtils;
		private List<PricePointItemController> mListItems;
		private bool mStateCustomAmount;

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

		public void init(XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mListItems = new List<PricePointItemController>();

			mShopTitle.text = mUtils.GetTranslations().Get("pricepoint_page_title");

			// Возможность произвольной покупки
			if (mUtils.GetSettings().components.virtualCurreny.customAmount)
			{
				mCustomAmountLink.SetActive(true);

				// Инициализируем саму панель
				CustomVirtCurrAmountController customController = mCustomAmountScreen.GetComponent<CustomVirtCurrAmountController>();
				//customController.initScreen();

				mCustomAmountLink.GetComponent<Button>().onClick.AddListener(ChangeStateCusomAmount);
				StateCustomAmount = false;
			}
			else
				mCustomAmountLink.SetActive(false);

			// получить список пакетов
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mPricePoints, lParams), PricePointsRecived, ErrorRecived);
		}

		private void PricePointsRecived(JSONNode pNode)
		{
			Logger.Log("PricePoints recived");
			XsollaPricepointsManager lGoods = new XsollaPricepointsManager().Parse(pNode) as XsollaPricepointsManager;
			// Добавляем кнопки 
			lGoods.GetItemsList().ForEach((item) => 
				{ 
					AddPricePointItem(item); 
				});
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

		private void ErrorRecived(JSONNode pNode)
		{
			// описание ошибки
		}

		void Update()
		{
			// Подгоняем размер ячейки 
			float lMaxCellHeight = 0;
			mListItems.ForEach((item) => 
				{
					if (item.mMainBckg.gameObject.GetComponent<RectTransform>().rect.height > lMaxCellHeight)
						lMaxCellHeight = item.mMainBckg.gameObject.GetComponent<RectTransform>().rect.height;
				});

			Vector2 lCellSize = mItemsContentGrid.GetComponent<GridLayoutGroup>().cellSize;
			mItemsContentGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(lCellSize.x, lMaxCellHeight);
			//Logger.Log("Max cell size - " + lMaxCellHeight.ToString());
		}
	}
}

