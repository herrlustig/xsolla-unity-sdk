using System;
using UnityEngine.UI;
using UnityEngine;

namespace Xsolla
{
	public class UserProfileBtnController: MonoBehaviour
	{
		public Text _title;
		public Button _btn;

		public void InitScreen(String pName, Action pAction)
		{
			_title.text = pName;
			_btn.onClick.AddListener(delegate {pAction();});
		}
	}
}

