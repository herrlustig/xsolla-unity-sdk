using UnityEngine;
using System.Collections;

namespace Xsolla 
{
	public class OpenUrlHelper : MonoBehaviour {

		public void OpenUrl(string urlToOpen)
		{
			Application.OpenURL (urlToOpen);
			return;
		}

	}
}
