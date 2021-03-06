﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Xsolla {
	public class RadioButton : MonoBehaviour, IRadioButton {

		public Text mIcon;
		public Text mName;
		public Button mBtn;
		public Image mDevider;
		public Text mExpandState;
		public GameObject mChildrenContainer;
		public VerticalLayoutGroup mGroupOnLevel;

		public StyleManager.BaseColor activeText;
		public StyleManager.BaseColor normalText;

		private Action mClickAction;

		private bool isSelected = false;
		private RadioType _typeRadioBtn;
		private bool mExpand = false;
		private bool mHover = false;

		public bool Expand 
		{
			get 
			{
				return mExpand;
			}
			set 
			{
				if (mExpandState != null)
				{
					Logger.Log("Expand is " + (value).ToString());
					mExpandState.text = value ? "" : "";
					mChildrenContainer.SetActive(value);
				}
				mExpand = value;
				
			}
		}

		public bool hasChildren()
		{
			return mExpandState.enabled;
		}

		public enum RadioType
		{
			SCREEN_GOODS, 
			SCREEN_PRICEPOINT, 
			SCREEN_SUBSCRIPTION, 
			SCREEN_REDEEMCOUPON, 
			SCREEN_FAVOURITE,
			GOODS_ITEM,
			SCREEN_HISTORY
		};

		public void init(string pIcon, string pName, RadioType pType, Action pActionClick, int pLevel, bool pOnlyAction = false)
		{
			if (mIcon != null)
				mIcon.text = pIcon;
			if (mName != null)
				mName.text = pName;
			
			setType(pType);

			mClickAction = pActionClick;

			if (mBtn != null)
			{
				mBtn.onClick.AddListener(delegate 
					{
						mClickAction();
						if (!pOnlyAction)
							Select();
					});
			}

			// Задаем смещение для уровня
			if (mGroupOnLevel != null)
			{
				mGroupOnLevel.padding.left = mGroupOnLevel.padding.left + (pLevel * 20);
				if (pLevel != 0)
				{
					mName.fontSize = 12;
					stateDevider(false);
				}
			}
		}

		public void stateDevider(bool pState)
		{
			mDevider.gameObject.SetActive(pState);
		}

		private void setType(RadioType pType)
		{
			_typeRadioBtn = pType;
		}

		public RadioType getType()
		{
			return _typeRadioBtn;
		}

		public void Select()
		{
			if (!isSelected) 				
				isSelected = true;
		}

		public void Deselect()
		{
			if (isSelected) 
				isSelected = false;
		}

		void Update() {

			switch (getType()) {
			case RadioType.GOODS_ITEM:
				{
					if (mIcon != null)
						mIcon.color = isSelected ? StyleManager.Instance.GetColor (activeText) : StyleManager.Instance.GetColor (normalText);
					if (mName != null)
						mName.color = isSelected ? StyleManager.Instance.GetColor (activeText) : StyleManager.Instance.GetColor (normalText);
					if (mDevider != null)
						mDevider.color = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.selected) : StyleManager.Instance.GetColor (StyleManager.BaseColor.divider_1);

					if (isSelected)
						(mBtn.targetGraphic as Image).color = StyleManager.Instance.GetColor(StyleManager.BaseColor.txt_white);

					ColorBlock lBlock = new ColorBlock();
					lBlock.normalColor = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.bg_item_btn) : StyleManager.Instance.GetColor (StyleManager.BaseColor.bg_left_menu);
					lBlock.highlightedColor = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.bg_item_btn) : StyleManager.Instance.GetColor (StyleManager.BaseColor.divider_1);
					lBlock.pressedColor = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.bg_item_btn) : StyleManager.Instance.GetColor (StyleManager.BaseColor.divider_1);
					lBlock.fadeDuration = 0.1f;
					lBlock.colorMultiplier = 1;
					mBtn.colors = lBlock;
					break;
				}
			default:
				if (mHover)
				{
					if (mIcon != null)
						mIcon.color = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.nav_menu_item_select_hover) : StyleManager.Instance.GetColor (StyleManager.BaseColor.nav_menu_item_not_select_hover);
					if (mName != null)
						mName.color = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.nav_menu_item_select_hover) : StyleManager.Instance.GetColor (StyleManager.BaseColor.nav_menu_item_not_select_hover);
					if (mDevider != null)
						mDevider.color = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.nav_menu_item_select_hover) : StyleManager.Instance.GetColor (StyleManager.BaseColor.nav_menu_item_not_select_hover);
				}
				else
				{
					if (mIcon != null)
						mIcon.color = isSelected ? StyleManager.Instance.GetColor (activeText) : StyleManager.Instance.GetColor (normalText);
					if (mName != null)
						mName.color = isSelected ? StyleManager.Instance.GetColor (activeText) : StyleManager.Instance.GetColor (normalText);
					if (mDevider != null)
						mDevider.color = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.selected) : StyleManager.Instance.GetColor (StyleManager.BaseColor.divider_1);
				}
				break;
			}

		}

		public void visibleBtn(bool pState)
		{
			this.gameObject.SetActive(pState);
		}

		public void setParentState(bool pState)
		{
			mExpandState.enabled = pState;
			// если есть дети
			if (pState)
			{
				if (mBtn != null)
				{
					mBtn.onClick.RemoveAllListeners();
					mBtn.onClick.AddListener(delegate 
						{
							ChangeExpand();
						});
				}
			}
		}

		public void setHoverImg()
		{
			mHover = true;
		}

		public void onExitPoint()
		{
			mHover = false;
		}

		private void ChangeExpand()
		{
			Expand = !Expand;
		}
	}
}
