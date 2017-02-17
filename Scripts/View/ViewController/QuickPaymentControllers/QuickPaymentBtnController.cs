using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Xsolla
{
	public class QuickPaymentBtnController: MonoBehaviour
	{
		public Image _iconMethod;
		public Text  _labelMethod;
		public Button _btnMethod;
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

		public void setLable(String pName)
		{
			_labelMethod.text = pName;
		}

		public void Hide()
		{
			CanvasGroup canvas = GetComponent<CanvasGroup>();
			canvas.alpha = 0f;
			canvas.blocksRaycasts = false;
		}

		public void Show()
		{
			CanvasGroup canvas = GetComponent<CanvasGroup>();
			canvas.alpha = 1f;
			canvas.blocksRaycasts = true;
		}

		public void setIcon(long pMethodID, ImageLoader pLoader = null)
		{
			switch (pMethodID)
			{
			case 1380:
				{
					_iconMethod.sprite = Resources.Load<Sprite> ("Images/ic_cc");
					break;
				}
			case 1738:
				{
					_iconMethod.sprite = Resources.Load<Sprite> ("Images/ic_mobile");
					break;
				}
			case 3012:
				{
					_iconMethod.sprite = Resources.Load<Sprite> ("Images/ic_giftCard");
					break;
				}
			default:
				{
					if (pLoader != null)
						pLoader.LoadImage(_iconMethod, _method.GetImageUrl());
					break;
				}
			}
		}
	}
}

