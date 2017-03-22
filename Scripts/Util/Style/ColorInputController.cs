using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Xsolla
{
	public class ColorInputController: MonoBehaviour, IPointerClickHandler
	{
		public StyleManager.BaseSprite pType;
		public InputField mInputField;

		void Start()
		{
			mInputField.transition = Selectable.Transition.SpriteSwap;
			SpriteState lSpriteState = new SpriteState();
			lSpriteState.highlightedSprite = StyleManager.Instance.GetSprite(pType, "_hover");// hover
			lSpriteState.pressedSprite = StyleManager.Instance.GetSprite(pType, "_focus"); // focus

			if (mInputField.targetGraphic is Image)
			{
				(mInputField.targetGraphic as Image).sprite = StyleManager.Instance.GetSprite(pType, "_def");
				(mInputField.targetGraphic as Image).color = StyleManager.Instance.GetColor(StyleManager.BaseColor.txt_white);
			}
			mInputField.spriteState = lSpriteState;

			mInputField.onEndEdit.AddListener((eventData) => endEdit(eventData));
 		}

		public void OnPointerClick (PointerEventData eventData)
		{
			SpriteState lSpriteState = new SpriteState();
			lSpriteState.pressedSprite = StyleManager.Instance.GetSprite(pType, "_focus"); // focus
			mInputField.spriteState = lSpriteState;

			if (mInputField.targetGraphic is Image)
			{
				(mInputField.targetGraphic as Image).sprite = StyleManager.Instance.GetSprite(pType, "_focus");
			}
		}

		private void endEdit(String pEvent)
		{
			SpriteState lSpriteState = new SpriteState();
			lSpriteState.highlightedSprite = StyleManager.Instance.GetSprite(pType, "_hover");// focus
			lSpriteState.pressedSprite = StyleManager.Instance.GetSprite(pType, "_focus"); // focus
			mInputField.spriteState = lSpriteState;
			if (mInputField.targetGraphic is Image)
			{
				(mInputField.targetGraphic as Image).sprite = StyleManager.Instance.GetSprite(pType, "_def");
			}
		}
	}
}

