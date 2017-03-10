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
		public GameObject mItemsContent;

		private const String mGroupsUrl = "paystation2/api/virtualitems/groups";
		private const String mGoodsUrl = "paystation2/api/virtualitems/items";

		private const String PREFAB_SHOP_ITEM = "Prefabs/SimpleView/_ScreenShop/ShopSimple/ShopItemGoodRe";

		private GoodsGroupMenuController mGoodsGroupController;
		private XsollaUtils mUtils;

		public void init(XsollaUtils pUtils)
		{
			mUtils = pUtils;
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
			XsollaGroupsManager lGoodsGrops = new XsollaGroupsManager().Parse(pNode) as XsollaGroupsManager;
			// Строим меню 
			mGoodsGroupController = this.gameObject.GetComponentInChildren<GoodsGroupMenuController>();
			mGoodsGroupController.init(lGoodsGrops, SelectGoodsGroup);
			// Выделяем первый элемент
			mGoodsGroupController.clickItem(0);
		}

		private void SelectGoodsGroup(XsollaGoodsGroup pGroup)
		{
			// выбор товаров по группе
			// Меняем заголовок
			mShopTitle.text = pGroup.GetName();

			// запрос на данные 
			Logger.Log("Load goods from groupId:" + pGroup.id.ToString());
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, mUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, mUtils.GetUser().userBalance.currency);
			lParams.Add("group_id", pGroup.id);
			ApiRequest.Instance.getApiRequest(new XsollaRequestPckg(mGoodsUrl, lParams), GoodsRecived, ErrorRecived);
		}

		private void GoodsRecived(JSONNode pNode)
		{
			XsollaGoodsManager lGoods = new XsollaGoodsManager().Parse(pNode) as XsollaGoodsManager;
			ClearItemsContent();
			lGoods.GetItemsList().ForEach((item) => 
				{ 
					AddShopItem(item); 
				});
		}

		private void ClearItemsContent()
		{
			List<Transform> lListChilds = new List<Transform>();
			for(int i=0; i <= mItemsContent.transform.transform.childCount - 1; i++)
				lListChilds.Add(mItemsContent.transform.GetChild(i));

			lListChilds.ForEach((childs) => { Destroy(childs.gameObject); });
		}

		private void AddShopItem(XsollaShopItem pItem)
		{
			GameObject lBaseObj = Resources.Load(PREFAB_SHOP_ITEM) as GameObject;
			// создаем экземпляр объекта товара
			GameObject lItemObj = Instantiate(lBaseObj);
			// получаем контроллер
			ShopItemController itemController = lItemObj.GetComponent<ShopItemController>();
			// инициализируем контролле
			itemController.init(pItem, mUtils);

			// добавляем на панель
			lItemObj.transform.SetParent(mItemsContent.transform);
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

