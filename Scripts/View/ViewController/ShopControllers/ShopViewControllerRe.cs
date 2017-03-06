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

		private const String mGroupsUrl = "paystation2/api/virtualitems/groups";

		public void init(XsollaUtils pUtils)
		{
			// строим навигационное меню магазина 
			// получить список групп
			Dictionary<String, object> lParams = new Dictionary<string, object>();
			lParams.Add(XsollaApiConst.ACCESS_TOKEN, pUtils.GetAcceessToken());
			lParams.Add(XsollaApiConst.USER_INITIAL_CURRENCY, pUtils.GetUser().userBalance.currency);
			ApiRequest.getApiRequest(this, mGroupsUrl, lParams, GoodsRecived, ErrorRecived);

		}


		private void GoodsRecived(JSONNode pNode)
		{
			// получили список групп товаров. 
			XsollaGroupsManager lGoodsGrops = new XsollaGroupsManager().Parse(pNode) as XsollaGroupsManager;
			// Строим меню 
			this.gameObject.GetComponentInChildren<GoodsGroupMenuController>().init(lGoodsGrops, SelectGoodsGroup);
		}

		private void SelectGoodsGroup(int pId)
		{
			
		}

		private void ErrorRecived(JSONNode pNode)
		{
			// описание ошибки
		}

		public void init(XsollaUtils pUtils, GameObject pContent)
		{
		}
	}
}

