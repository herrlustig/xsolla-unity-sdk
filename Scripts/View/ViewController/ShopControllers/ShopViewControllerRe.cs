using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Xsolla
{
	public class ShopViewControllerRe: MonoBehaviour
	{
		public Text mShopTitle;
		public GameObject mShopContent;
		public GameObject mItemsContentGrid;
		public GameObject mItemsContentList;
		public Text mEmptyLabel;
		public ScrollRect mScrollRectItemsContainer;
		public MyRotation  mProgressBar;

		private const String mGroupsUrl = "paystation2/api/virtualitems/groups";
		private const String mGoodsUrl = "paystation2/api/virtualitems/items";
		private const String mSalesUrl = "paystation2/api/virtualitems/sales";
		private const String mFavItemsUrl = "paystation2/api/virtualitems/favorite";

		private const String PREFAB_SHOP_ITEM_GRID = "Prefabs/SimpleView/_ScreenShop/ShopSimple/ShopItemGoodRe";
		private const String PREFAB_SHOP_ITEM_LIST = "Prefabs/SimpleView/_ScreenShop/ShopSimple/ShopItemGoodListRe";
		private bool mIsListLayout = false;

		private GoodsGroupMenuController mGoodsGroupController;
		private XsollaUtils mUtils;
		private List<ShopItemController> mListItems;
		private int mCurrGroupId;
		private Dictionary<int, bool> mGroupUseCached;

		public GameObject GetItemContainer
		{
			get
			{
				return (mUtils.GetSettings().mDesktop.pVirtItems.isListLayout()) ? mItemsContentList : mItemsContentGrid;
			}
		}

		public void init(XsollaUtils pUtils)
		{
			mUtils = pUtils;
			mListItems = new List<ShopItemController>();
			mGroupUseCached = new Dictionary<int, bool>();
			// Задаем настройки на лэндинг
			mIsListLayout = pUtils.GetSettings().mDesktop.pVirtItems.isListLayout();
			SetLanding();
				
			// Задаем перевод на отсутствие товаров
			mEmptyLabel.text = mUtils.GetTranslations().Get("virtualitem_no_data");
			mEmptyLabel.gameObject.SetActive(false);

			// строим навигационное меню магазина 
			// получить список групп
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mGroupsUrl, lParams), GoodsGroupRecived, ErrorRecived);
		}

		private void SetLanding()
		{
			if (mIsListLayout)
			{
				mItemsContentGrid.SetActive(false);
				mItemsContentList.SetActive(true);

				// Задаем новый компонент на скролл
				mItemsContentList.transform.parent.gameObject.GetComponent<ScrollRect>().content = mItemsContentList.GetComponent<RectTransform>();
			}
			else
			{
				mItemsContentGrid.SetActive(true);
				mItemsContentList.SetActive(false);
			}
		}

		private void GoodsGroupRecived(JSONNode pNode)
		{
			// получили список групп товаров. 
			XsollaGroupsManager lGoodsGroups = new XsollaGroupsManager().Parse(pNode["groups"]) as XsollaGroupsManager;
			// Строим меню 
			mGoodsGroupController = this.gameObject.GetComponentInChildren<GoodsGroupMenuController>();
			if (mUtils.mBonus.mHasSales)
			{
				XsollaGoodsGroup salesGroup = new XsollaGoodsGroup();
				salesGroup.id = -1;
				salesGroup.mChildren = new XsollaGroupsManager();
				salesGroup.name = "%" + " " + mUtils.GetTranslations().Get("sales_page_title");
				lGoodsGroups.InsertItem(0, salesGroup);
			}
				
			mGoodsGroupController.init(lGoodsGroups, SelectGoodsGroup);
			// Выделяем первый активный элемент
			mGoodsGroupController.clickFirstActiveItem();
			//mGoodsGroupController.clickItem(0);
		}

		private void SelectGoodsGroup(XsollaGoodsGroup pGroup)
		{
			// Зачищаем панель с товарами
			ClearItemsContent();
			// Запускаем прелоадер
			mProgressBar.SetLoading(true);
			// выбор товаров по группе
			// Меняем заголовок
			mShopTitle.text = pGroup.GetName();
			mCurrGroupId = pGroup.id;

			// запрос на данные 
			Logger.Log("Load goods from groupId:" + pGroup.id.ToString());
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			lParams.Add("group_id", pGroup.id);
			// Если id = -1 то это распродажа и делаем запрос по другому адресу
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg((pGroup.id == -1) ? mSalesUrl : mGoodsUrl, lParams), GoodsRecived, ErrorRecived, mGroupUseCached.ContainsKey(mCurrGroupId) ? mGroupUseCached[mCurrGroupId] : true);

			SetCachedStateOnGroupId(mCurrGroupId, true);
		}

		private void GoodsRecived(JSONNode pNode)
		{
			// Позиция для скролла
			mScrollRectItemsContainer.verticalNormalizedPosition = 1;

			XsollaGoodsManager lGoods = new XsollaGoodsManager().Parse(pNode) as XsollaGoodsManager;

			// Если группа пустая
			mEmptyLabel.gameObject.SetActive(lGoods.GetCount() == 0);
			if (lGoods.GetCount() == 0)
			{
				mItemsContentGrid.SetActive(false);
				mItemsContentList.SetActive(false);
			}
			else
				SetLanding();

			lGoods.GetItemsList().ForEach((item) => 
				{ 
					AddShopItem(item); 
				});

			mProgressBar.SetLoading(false);
		}

		private void UpdateLayout()
		{
			// Подгоняем размер ячейки 
			float lMaxCellHeight = 0;
			mListItems.ForEach((item) => 
				{
					if (item.mMainBckg.gameObject.GetComponent<ContentSizeFitter>().enabled)
					if (item.mMainBckg.gameObject.GetComponent<RectTransform>().rect.height > lMaxCellHeight)
						lMaxCellHeight = item.mMainBckg.gameObject.GetComponent<RectTransform>().rect.height;
				});

			Logger.Log("Max height " + lMaxCellHeight);

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
			
		private void AddShopItem(XsollaShopItem pItem)
		{
			GameObject lBaseObj = Resources.Load(mIsListLayout ? PREFAB_SHOP_ITEM_LIST : PREFAB_SHOP_ITEM_GRID) as GameObject;
			// создаем экземпляр объекта товара
			GameObject lItemObj = Instantiate(lBaseObj);
			// получаем контроллер
			ShopItemController itemController = lItemObj.GetComponent<ShopItemController>();
			// инициализируем контроллер
			itemController.init(pItem, mUtils, mCurrGroupId, SetCachedStateOnGroupId, mIsListLayout);
			itemController.mCollapseAnotherDesc = CollapseAllDesc;
			// добавляем на панель
			lItemObj.transform.SetParent(GetItemContainer.transform);
			// масштабирование
			lItemObj.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			// Добавляем в лист кэша магазина
			mListItems.Add(itemController);
		}

		private void SetCachedStateOnGroupId(int pGroupid, bool pState)
		{
			if (!mGroupUseCached.ContainsKey(pGroupid))
				mGroupUseCached.Add(pGroupid, pState);
			else
				mGroupUseCached[pGroupid] = pState;
		}

		public void ShowFavItems()
		{
			Logger.Log("Get request on favorite items");
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mFavItemsUrl, lParams), FavItemsRecived, ErrorRecived, false);
		}

		private void FavItemsRecived(JSONNode pNode)
		{
			// Задем заголовок
			mShopTitle.text = mUtils.GetTranslations().Get(XsollaTranslations.VIRTUALITEMS_TITLE_FAVORITE);
			// Снимаем отметку с бокового меню
			mGoodsGroupController.unselectAll();
			GoodsRecived(pNode);
		}

		public void CollapseAllDesc()
		{
			mListItems.ForEach((item) => 
				{
					if (item.LongDescState)
						item.LongDescState = false;
				});
		}

		private void ClearItemsContent()
		{
			List<Transform> lListChilds = new List<Transform>();
			for(int i=0; i <= GetItemContainer.transform.transform.childCount - 1; i++)
				lListChilds.Add(GetItemContainer.transform.GetChild(i));

			lListChilds.ForEach((childs) => { Destroy(childs.gameObject); });
			mListItems.Clear();
		}

		private void ErrorRecived(XsollaErrorRe pErrors)
		{
			// описание ошибки
		}

		void Update()
		{
			
		}

	}
}

