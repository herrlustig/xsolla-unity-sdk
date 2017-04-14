using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

namespace Xsolla
{
	public class GoodsGroupMenuController: MonoBehaviour
	{
		public GameObject mMenuContainer;

		private RadioGroupController mRadioGroupController;

		private const String PREFAB_MENU_ITEM = "Prefabs/SimpleView/_ScreenShop/GoodsMenuItem";

		public void init(XsollaGroupsManager pGoodsManager, Action<XsollaGoodsGroup> pSelectItem)
		{
			Logger.Log("Create goods menu");
			mRadioGroupController = this.gameObject.AddComponent<RadioGroupController>();
			pGoodsManager.GetItemsList().ForEach((item) =>
				{
					addMenuItem(item, pSelectItem, mMenuContainer);
				});
			mRadioGroupController.radioButtons[0].stateDevider(false);
		}

		private void addMenuItem(XsollaGoodsGroup pGoodsGroup, Action<XsollaGoodsGroup> pSelectItem, GameObject pParent)
		{
			GameObject baseMenuItem = Resources.Load(PREFAB_MENU_ITEM) as GameObject;
			GameObject lMenuItem = Instantiate(baseMenuItem);
			RadioButton lController = lMenuItem.GetComponent<RadioButton>();
			lController.init("", pGoodsGroup.GetName(), RadioButton.RadioType.GOODS_ITEM, delegate 
				{ 
					mRadioGroupController.UnselectAll();
					pSelectItem(pGoodsGroup); 
				}, pGoodsGroup.mLevel);

			mRadioGroupController.AddButton(lController);

			// Задаем является ли родителем
			lController.setParentState(pGoodsGroup.mChildren.Count > 0);

			//  Заносим детей
			if (pGoodsGroup.mChildren.Count > 0)
			{
				pGoodsGroup.mChildren.GetItemsList().ForEach((item) => 
					{
						addMenuItem(item, pSelectItem, lController.mChildrenContainer);
					});
			}
				
			lMenuItem.transform.SetParent(pParent.transform);
			Resizer.SetDefScale(lMenuItem);
		}
			
		public void clickItem(int pIndx)
		{
			mRadioGroupController.radioButtons[pIndx].mBtn.onClick.Invoke();
		}

		public void clickFirstActiveItem()
		{
			foreach(RadioButton item in mRadioGroupController.radioButtons)
			{
				if (item.hasChildren())
					item.Expand = true;
				else
				{
					item.mBtn.onClick.Invoke();
					return;
				}
			}
		}

		public void unselectAll()
		{
			mRadioGroupController.UnselectAll();
		}
			
	}
}

