using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Xsolla {
	public class RadioButton : MonoBehaviour, IRadioButton {

		public Graphic image;
		public Graphic text;
		public StyleManager.BaseColor activeImage;
		public StyleManager.BaseColor normalImage;
		public StyleManager.BaseColor activeText;
		public StyleManager.BaseColor normalText;

		private bool isSelected = false;
		private bool isStarted = false;
		private RadioType _typeRadioBtn;

		public enum RadioType
		{
			SCREEN_GOODS, SCREEN_PRICEPOINT, SCREEN_SUBSCRIPTION, SCREEN_REDEEMCOUPON, SCREEN_FAVOURITE
		};

		public void setType(RadioType pType)
		{
			_typeRadioBtn = pType;
		}

		public RadioType getType()
		{
			return _typeRadioBtn;
		}

		void Start() {
			isStarted = true;
		}

		public void Select()
		{
			if (!isSelected) {
				//if(image != null && activeImage != null)
				if(image != null)
					image.color = StyleManager.Instance.GetColor (activeImage);
				//if(text != null && activeText != null)
				if(text != null)
					text.color = StyleManager.Instance.GetColor (activeText);
				if(isStarted)
					isSelected = true;
				else
					Invoke("Select", 1);
			}
		}

		public void Deselect()
		{
			if (isSelected) {
				//if(image != null && normalImage != null)
				if(image != null)	
					image.color = StyleManager.Instance.GetColor (normalImage);
				//if(text != null && normalText != null)
				if(text != null)	
					text.color = StyleManager.Instance.GetColor (normalText);
				isSelected = false;
			}
		}

		void Update() {
			if (isSelected) {
				//if(image != null && activeImage != null)
				if(image != null)
					image.color = StyleManager.Instance.GetColor (activeImage);
				//if(text != null && activeText != null)
				if(text != null)
					text.color = StyleManager.Instance.GetColor (activeText);
			} else {
				//if(image != null && normalImage != null)
				if(image != null)
					image.color = StyleManager.Instance.GetColor (normalImage);
				//if(text != null && normalText != null)
				if(text != null)
					text.color = StyleManager.Instance.GetColor (normalText);
			}
		}

	}
}
