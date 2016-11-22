using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Xsolla
{
	public class SavedMethodBtnController: MonoBehaviour
	{
		public Image _iconMethod;
		public Text _nameMethod;
		public Text _nameType;
		public Button _btnMethod;
		public GameObject _btnDelete;
		public Text _btnDeleteName;
		public GameObject _self;

		private XsollaSavedPaymentMethod _method;

		public void setMethod (XsollaSavedPaymentMethod pMethod)
		{
			_method = pMethod;
		}

		public XsollaSavedPaymentMethod getMethod()
		{
			return _method;
		}

		public void setNameMethod(String pNameMethod)
		{
			_nameMethod.text = pNameMethod;
		}

		public void setNameType(String pNametype)
		{
			_nameType.text = pNametype;
		}

		public void setDeleteBtn(bool pState)
		{
			_btnDelete.SetActive(pState);
			_btnMethod.enabled = !pState;
		}

		public void setDeleteBtnName(String pName)
		{
			_btnDeleteName.text = pName;
		}

		public Button getBtnDelete()
		{
			return _btnDelete.GetComponent<Button>();
		}
	}
}

