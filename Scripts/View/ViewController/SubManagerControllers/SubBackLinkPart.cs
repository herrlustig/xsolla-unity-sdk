using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla
{
	public class SubBackLinkPart: MonoBehaviour
	{
		public Text mBackLinkLabel;
		public Button mBackBtn;
		public Text mConfirmLabel;
		public GameObject mConfirmBtn;

		public void init(String pBackLabel, Action pBackAction, String pConfirmLabel = "", Action pConfirmAction = null)
		{
			mBackLinkLabel.text = pBackLabel;
			mBackBtn.onClick.AddListener(() => {pBackAction();});
			if (pConfirmLabel != "")
			{
				mConfirmBtn.SetActive(true);
				mConfirmLabel.text = pConfirmLabel;
				mConfirmBtn.GetComponent<Button>().onClick.AddListener(() => 
					{
						pConfirmAction();
					});
			}
			else
				mConfirmBtn.SetActive(false);
		}
	}
}

