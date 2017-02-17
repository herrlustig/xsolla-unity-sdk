using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class ShopPaymentBtnController: MonoBehaviour
	{
		public Button _btn;
		public Image  _icon;
		public GameObject _self;

		private XsollaPaymentMethod _method;

		public void setMethod(XsollaPaymentMethod pMethod)
		{
			_method = pMethod;
		}

		public XsollaPaymentMethod getMethod()
		{
			return _method;
		}

		public ShopPaymentBtnController ()
		{
		}

		public void setIcon(ImageLoader pLoader = null)
		{
			switch (_method.id)
			{
			case 1380:
				{
					_icon.sprite = Resources.Load<Sprite> ("Images/ic_cc");
					break;
				}
			case 1738:
				{
					_icon.sprite = Resources.Load<Sprite> ("Images/ic_mobile");
					break;
				}
			case 3012:
				{
					_icon.sprite = Resources.Load<Sprite> ("Images/ic_giftCard");
					break;
				}
			default:
				{
					if (pLoader != null)
						pLoader.LoadImage(_icon, _method.GetImageUrl());
					break;
				}
			}
		}

		public void Hide()
		{
			CanvasGroup canvas = GetComponent<CanvasGroup>();
			if (canvas != null)
			{
				canvas.alpha = 0f;
				canvas.blocksRaycasts = false;
			}
		}
	}
}

