using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Xsolla {
	public class RadioButton : MonoBehaviour, IRadioButton {

		public Text mIcon;
		public Text mName;
		public Button mBtn;
		public Image mDevider;

		public StyleManager.BaseColor activeText;
		public StyleManager.BaseColor normalText;

		private Action mClickAction;

		private bool isSelected = false;
		private RadioType _typeRadioBtn;

		public enum RadioType
		{
			SCREEN_GOODS, 
			SCREEN_PRICEPOINT, 
			SCREEN_SUBSCRIPTION, 
			SCREEN_REDEEMCOUPON, 
			SCREEN_FAVOURITE,
			GOODS_ITEM
		};

		public void init(string pIcon, string pName, RadioType pType, Action pActionClick)
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
						Select();
					});
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
			if (mIcon != null)
				mIcon.color = isSelected ? StyleManager.Instance.GetColor (activeText) : StyleManager.Instance.GetColor (normalText);
			if (mName != null)
				mName.color = isSelected ? StyleManager.Instance.GetColor (activeText) : StyleManager.Instance.GetColor (normalText);
			if (mDevider != null)
				mDevider.color = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.selected) : StyleManager.Instance.GetColor (StyleManager.BaseColor.divider_1);

			if (getType() == RadioType.GOODS_ITEM)
			{
				ColorBlock lBlock = new ColorBlock();
				lBlock.normalColor = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.selected) : StyleManager.Instance.GetColor (StyleManager.BaseColor.bg_main);
				lBlock.highlightedColor = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.selected) : StyleManager.Instance.GetColor (StyleManager.BaseColor.divider_1);
				lBlock.pressedColor = isSelected ? StyleManager.Instance.GetColor (StyleManager.BaseColor.selected) : StyleManager.Instance.GetColor (StyleManager.BaseColor.divider_1);
				lBlock.fadeDuration = 0.1f;
				lBlock.colorMultiplier = 1;
				mBtn.colors = lBlock;
			}
		}

		public void visibleBtn(bool pState)
		{
			this.gameObject.SetActive(pState);
		}
	}
}
