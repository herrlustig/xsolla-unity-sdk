using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;

namespace Xsolla
{
	public class ApiRequest: Singleton<ApiRequest>
	{
		private String DOMAIN = "https://secure.xsolla.com/";
		private const string mVerAPI = "1.0.1";
		private const string SDK_VERSION = "2.0.0";
		protected Dictionary<XsollaRequestPckg, JSONNode> mGlobalCache = null;

		protected ApiRequest() 
		{
			mGlobalCache = new Dictionary<XsollaRequestPckg, JSONNode>();
		}
			
		public void getApiRequest(XsollaRequestPckg pPckg, Action<JSONNode> pRecivedCallBack, Action<JSONNode> pReciverdError)
		{

			if (ChacheExist(pPckg, pRecivedCallBack))
				return;
			
			WWWForm lForm = new WWWForm();
			StringBuilder sb = new StringBuilder ();
			foreach(KeyValuePair<string, object> pair in pPckg.Params())
			{
				string argValue = pair.Value != null ? pair.Value.ToString() : "";
				sb.Append(pair.Key).Append("=").Append(argValue).Append("&");
				lForm.AddField(pair.Key, argValue);
			}
			WWW lwww = new WWW(DOMAIN + pPckg.Url(), lForm);
			StartCoroutine(getRequest(lwww, pPckg, pRecivedCallBack, pReciverdError));
		}

		private IEnumerator getRequest(WWW pWww, XsollaRequestPckg pPckg, Action<JSONNode> pCallback, Action<JSONNode> pCallbackError)
		{
			yield return pWww;

			// логирование ответа
			Logger.Log("WWW respond: Url: " + pWww.url);
			Logger.Log("WWW respond: Time: " + pWww.progress.ToString());
			Logger.Log("WWW respond: Size: " + pWww.size.ToString());
			Logger.Log("WWW respond: Text: " + pWww.text);

			JSONNode jsonRespond = JSON.Parse(pWww.text);

			// проверка на версию API
			if (jsonRespond["api"]["ver"].Value != mVerAPI)
			{
				pCallbackError("Версия SDK не актуальна");
				yield break;
			}

			// Если есть ошибки, выбрасываем эксепшен
			if (pWww.error != null)
			{
				pCallbackError(pWww.error.ToString());
				yield break;
			}
			else
			{
				// логируем запрос
				mGlobalCache.Add(pPckg, jsonRespond);

				// отдаем запрос
				if (!pPckg.isOnlyCache())
					pCallback(jsonRespond); // отдаем данные
			}
		}

		private bool ChacheExist(XsollaRequestPckg pPckg, Action<JSONNode> pReciveAction)
		{
			IEnumerator lEnum = mGlobalCache.Keys.GetEnumerator();
			while(lEnum.MoveNext())
			{
				if (lEnum.Current.Equals(pPckg))
				{
					Logger.Log("Request from Cache!");
					pReciveAction(mGlobalCache[lEnum.Current as XsollaRequestPckg]);
					return true;
				}

			}
			return false;
		}

	}
}

