using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;

namespace Xsolla
{
	public class ApiRequest
	{
		private static String DOMAIN = "https://secure.xsolla.com/";

		public static void getApiRequest(MonoBehaviour pCaller, String pUrl, Dictionary<string, object> pParams, Action<JSONNode> pRecivedCallBack, Action<JSONNode> pReciverdError)
		{
			WWWForm lForm = new WWWForm();
			StringBuilder sb = new StringBuilder ();
			foreach(KeyValuePair<string, object> pair in pParams)
			{
				string argValue = pair.Value != null ? pair.Value.ToString() : "";
				sb.Append(pair.Key).Append("=").Append(argValue).Append("&");
				lForm.AddField(pair.Key, argValue);
			}
			WWW lwww = new WWW(DOMAIN + pUrl, lForm);
			pCaller.StartCoroutine(getRequest(lwww, pRecivedCallBack, pReciverdError));
		}

		private static IEnumerator getRequest(WWW pWww, Action<JSONNode> pCallback, Action<JSONNode> pCallbackError)
		{
			yield return pWww;
			if (pWww.error == null)
			{
				JSONNode rootNode = JSON.Parse(pWww.text);
				pCallback(rootNode);
			}
			else
			{
				JSONNode rootNode = JSON.Parse(pWww.error);
				pCallbackError(rootNode);
			}
		}
	}
}

