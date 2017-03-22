﻿using System;
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

			// строим навигационное меню магазина 
			// получить список групп
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mGroupsUrl, lParams), GoodsGroupRecived, ErrorRecived);
		}

		public void init(XsollaUtils pUtils, GameObject pContent)
		{
		}

		private void GoodsGroupRecived(JSONNode pNode)
		{
			// получили список групп товаров. 
			XsollaGroupsManager lGoodsGroups = new XsollaGroupsManager().Parse(pNode) as XsollaGroupsManager;
			// Строим меню 
			mGoodsGroupController = this.gameObject.GetComponentInChildren<GoodsGroupMenuController>();
			if (mUtils.mBonus.mHasSales)
			{
				XsollaGoodsGroup salesGroup = new XsollaGoodsGroup();
				salesGroup.id = -1;
				salesGroup.name = "%" + " " + mUtils.GetTranslations().Get("sales_page_title");
				lGoodsGroups.InsertItem(0, salesGroup);
			}
				
			mGoodsGroupController.init(lGoodsGroups, SelectGoodsGroup);
			// Выделяем первый элемент
			mGoodsGroupController.clickItem(0);
		}

		private void SelectGoodsGroup(XsollaGoodsGroup pGroup)
		{
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
			// Зачищаем панель с товарами
			ClearItemsContent();
			XsollaGoodsManager lGoods = new XsollaGoodsManager().Parse(pNode) as XsollaGoodsManager;
			lGoods.GetItemsList().ForEach((item) => 
				{ 
					AddShopItem(item); 
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
			itemController.init(pItem, mUtils, mCurrGroupId, SetCachedStateOnGroupId);
			itemController.mCollapseAnotherDesc = CollapseAllDesc;
			// добавляем на панель
			lItemObj.transform.SetParent(GetItemContainer.transform);
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

		private void ErrorRecived(JSONNode pNode)
		{
			// описание ошибки
		}

		void Update()
		{
			// TODO обновление кол-ва столбцов в контейнере 	
		}

	}
}

