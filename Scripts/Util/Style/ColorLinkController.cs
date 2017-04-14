using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class ColorLinkController: MonoBehaviour
	{
		public Button mButton; 

		void Start()
		{
			ColorBlock block = new ColorBlock();
			block.normalColor = StyleManager.Instance.GetColor(StyleManager.BaseColor.link_normal);
			block.highlightedColor = StyleManager.Instance.GetColor(StyleManager.BaseColor.link_hover);
			block.pressedColor = StyleManager.Instance.GetColor(StyleManager.BaseColor.link_hover);
			block.fadeDuration = 0.1f;
			block.colorMultiplier = 1;
			mButton.colors = block;
		}
	}
}

