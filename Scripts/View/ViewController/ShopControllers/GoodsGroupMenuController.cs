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
			GameObject baseMenuItem = Resources.Load(PREFAB_MENU_ITEM) as GameObject;
			pGoodsManager.GetItemsList().ForEach((item) =>
				{
					GameObject lMenuItem = Instantiate(baseMenuItem);
					RadioButton lController = lMenuItem.GetComponent<RadioButton>();
					lController.init("", item.GetName(), RadioButton.RadioType.GOODS_ITEM, delegate 
						{ 
							mRadioGroupController.UnselectAll();
							pSelectItem(item); 
						});
					mRadioGroupController.AddButton(lController);
					lMenuItem.transform.SetParent(mMenuContainer.transform);
				});
			mRadioGroupController.radioButtons[0].stateDevider(false);
		}

		public void clickItem(int pIndx)
		{
			mRadioGroupController.radioButtons[pIndx].mBtn.onClick.Invoke();
		}
			
	}
}

