using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla 
{
	public class UserProfileController: MonoBehaviour
	{

		private List<UserProfileBtnController> listBtn;

		public UserProfileController ()
		{
			listBtn = new List<UserProfileBtnController>();
		}

		public void AddBtn(UserProfileBtnController pBtn)
		{
			listBtn.Add(pBtn);
		}
	}
}

