using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;

namespace Xsolla {
	public class HttpTlsRequest: MonoBehaviour {
				
		private static String outputFileName = "requestResult.x";
		public static String loaderGameObjName = "HttpRequestLoader";

		// Method to get http tls request
		public IEnumerator Request(string pUrl, Dictionary<string, object> pDataDic, Action<RequestClass> onReturnRes)
		{
			Logger.isLogRequired = true;
			RequestClass res = new RequestClass();
			string urldata = "";
			string args = "";

			// Get data paremetrs
			foreach(KeyValuePair<string, object> dicItem in pDataDic)
			{
				urldata += dicItem.Key + "=" + Uri.EscapeDataString((dicItem.Value==null)?"":dicItem.Value.ToString()) + "&";
			}
			urldata = "?" + urldata.Substring(0, urldata.Length - 1);
			args = pUrl + urldata;
			Logger.Log(this.GetType().Name + " -> " + args);


			// Platform switcher
			Logger.Log("ApplicationPlatform -> " + Application.platform);
			Logger.Log("ApplicationDataPath -> " + Application.dataPath);
			switch (Application.platform)
			{
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:	
			//case RuntimePlatform.OSXWebPlayer:	
			case RuntimePlatform.LinuxPlayer:
				{
					CaptureConsoleCmdOutput("curl", args , out res);
					break;
				}
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.WindowsPlayer:
			//case RuntimePlatform.WindowsWebPlayer:
				{
					CaptureConsoleCmdOutputWin(@"" + Application.dataPath +"/Plugins/" + "ExecConnectWin.dll", args, out res);
					break;
				}
			case RuntimePlatform.Android:
				//{
					//CaptureConsoleCmdOutput(@"ExecConnect.dll", args, out exitcode, out res);
					//break;
				//}
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.IPhonePlayer:
				{
					Logger.Log("StartCoroutine");
					// Coroutine request only for Iphone
					StartCoroutine(GetWWWFormRequest(pUrl, pDataDic, (value) => onReturnRes(value)));
					break;
				}
			default:
				{
					break;
				}
			}

			yield return res;
			onReturnRes(res);
		}

		private static IEnumerator GetWWWFormRequest(string pUrl, Dictionary<string, object> pDataDic, Action<RequestClass> onComplite)
		{
			WWWForm form = new WWWForm ();
			foreach(KeyValuePair<string, object> itemDic in pDataDic)
			{
				form.AddField(itemDic.Key, itemDic.Value.ToString());
			}

			WWW www = new WWW(pUrl, form);
			yield return www;	
			if (www.error == null) 
			{
				if (onComplite != null)
				{
					Logger.Log("www.text -> " + www.text);
					onComplite(new RequestClass(www.text, pUrl));
				}
			} else {
				if (onComplite != null)
				{
					Logger.Log("www.text.error -> " + www.text);
					onComplite(new RequestClass(www.text,pUrl,true,www.error));
				}
			}
		}

		private static void CaptureConsoleCmdOutputWin(string pExeName, string pArgs, out RequestClass pRes)
		{
			string result = "";
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = pExeName;
			start.Arguments = pArgs;
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			start.CreateNoWindow = true;
			try
			{
				Process process = Process.Start(start);
				using (StreamReader sr = process.StandardOutput)
				{
					// Read the stream to a string, and write the string to the console.
					result = sr.ReadToEnd();
				}

				process.WaitForExit();
				pRes = new RequestClass(result, pArgs, false, "");
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
				pRes = new RequestClass("",pArgs,true,e.Message.ToString());
			}
		}

		private static void CaptureConsoleCmdOutput(string pExeName, string pArgs, out RequestClass pRes)
		{
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = pExeName;
			start.Arguments = "--globoff --output " + outputFileName + " " + pArgs;
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			start.CreateNoWindow = true;
			try
			{
				string result = "";
				Process process = Process.Start(start);
				process.WaitForExit();
				StreamReader reader = process.StandardOutput;
				//result = reader.ReadToEnd();
			
				try
				{    // Open the text file using a stream reader.
					using (StreamReader sr = new StreamReader(outputFileName))
					{
						// Read the stream to a string, and write the string to the console.
						result = sr.ReadToEnd();
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("The file could not be read:");
					Console.WriteLine(e.Message);
				}


				pRes = new RequestClass(result, pArgs, false, "");
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
				pRes = new RequestClass("",pArgs,true,e.Message.ToString());
			}
		}
	}
}
