using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class ColorBtnController: MonoBehaviour
	{
		public StyleManager.BaseSprite pType;
		public Button mButton;

		void Start()
		{
			mButton.transition = Selectable.Transition.SpriteSwap;
			SpriteState lSpriteState = new SpriteState();
			lSpriteState.highlightedSprite = StyleManager.Instance.GetSprite(pType, "_hover");
			lSpriteState.pressedSprite = StyleManager.Instance.GetSprite(pType, "_hover");

			if (mButton.targetGraphic is Image)
			{
				(mButton.targetGraphic as Image).sprite = StyleManager.Instance.GetSprite(pType, "_normal");
				(mButton.targetGraphic as Image).color = StyleManager.Instance.GetColor(StyleManager.BaseColor.txt_white);
			}
				
			mButton.spriteState = lSpriteState;
		}
	}
}

