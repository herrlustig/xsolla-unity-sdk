using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubManagerNotifyPartController: MonoBehaviour
	{
		public Text mMainLabel;
		public GameObject mLink;

		public Button getLinkBtn()
		{
			return mLink.GetComponent<Button>();
		}

		public void init(String pMainLabel, String pBtnLabel, XsollaManagerSubDetails pSubDetail, Action<XsollaManagerSubDetails> pActionLink = null)
		{
			mMainLabel.text = pMainLabel;
			if (pBtnLabel != "")
				mLink.GetComponent<Text>().text = pBtnLabel;

			if (pActionLink != null)
				getLinkBtn().onClick.AddListener(() => pActionLink(pSubDetail));
		}
	}
}

