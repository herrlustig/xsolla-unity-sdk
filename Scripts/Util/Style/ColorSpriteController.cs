using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class ColorSpriteController: MonoBehaviour
	{
		public Image mImage;
		public StyleManager.BaseSprite mType;

		void Start()
		{
			mImage.sprite = StyleManager.Instance.GetSprite(mType);
		}
	}
}

