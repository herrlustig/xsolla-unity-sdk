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
			
		public void getApiRequest(XsollaRequestPckg pPckg, Action<JSONNode> pRecivedCallBack, Action<XsollaErrorRe> pReciverdError, bool pUseCached = true)
		{

			if (pUseCached && ChacheExist(pPckg, pRecivedCallBack))
				return;
			
			WWWForm lForm = new WWWForm();
			//StringBuilder sb = new StringBuilder ();
			foreach(KeyValuePair<string, object> pair in pPckg.Params())
			{
				string argValue = pair.Value != null ? pair.Value.ToString() : "";
				//sb.Append(pair.Key).Append("=").Append(argValue).Append("&");
				lForm.AddField(pair.Key, argValue);
			}
			WWW lwww = new WWW(DOMAIN + pPckg.Url(), lForm);
			StartCoroutine(getRequest(lwww, pPckg, pRecivedCallBack, pReciverdError));
		}

		private IEnumerator getRequest(WWW pWww, XsollaRequestPckg pPckg, Action<JSONNode> pCallback, Action<XsollaErrorRe> pCallbackError)
		{
			yield return pWww;

			Logger.Log("WWW respond: Url: " + pWww.url);
			JSONNode jsonRespond = JSON.Parse(pWww.text);

			if (pWww.error != null)
			{
				Logger.Log("WWW respond: Error: " + pWww.error);

				pCallbackError(new XsollaErrorRe().Parse(jsonRespond["errors"]) as XsollaErrorRe);
				yield break;
			}
			else
			{
				// проверка на версию API
				if (jsonRespond["api"]["ver"].Value != mVerAPI)
				{
					Logger.LogError("Invalid API version");
					yield break;
				}
					
				Logger.Log("WWW respond: Time: " + pWww.progress.ToString());
				Logger.Log("WWW respond: Size: " + pWww.size.ToString());
				Logger.Log("WWW respond: Text: " + pWww.text);

				// логируем запрос
				bool lContaint = false;
				IEnumerator lEnum = mGlobalCache.Keys.GetEnumerator();
				while(lEnum.MoveNext())
				{
					if (lEnum.Current.Equals(pPckg))
					{
						mGlobalCache[lEnum.Current as XsollaRequestPckg] = jsonRespond;
						lContaint = true;
						break;
					}
				}

				if (!lContaint)
				{	
					mGlobalCache.Add(pPckg, jsonRespond);
				}

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

	public class XsollaErrorRe: IParseble
	{
		public List<Error> mErrorList;

		public IParseble Parse (JSONNode rootNode)
		{
			mErrorList = new List<Error>();
			IEnumerator lEnum = rootNode.AsArray.GetEnumerator();

			while(lEnum.MoveNext())
				mErrorList.Add(new Error((lEnum.Current as JSONNode)["message"],(lEnum.Current as JSONNode)["support_code"]));
				
			return this;
		}

		public struct Error
		{
			public String mMessage;
			public String mSupportCode;

			public Error(String pMsg, String pSupportCode)
			{
				mMessage = pMsg;
				mSupportCode = pSupportCode;
			}

			public override string ToString ()
			{
				return string.Format ("{0} \n {1}", mMessage, mSupportCode);
			}
			
		}
	}


}

