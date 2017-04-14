using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla {
	public class Logger {

		public static bool isLogRequired;

		public static void Log(string message) {
			if(isLogRequired)
				Debug.Log (message);
		}

		public static void LogError(string message) {
			if(isLogRequired)
				Debug.LogError (message);
		}

		public static void Log(string elemName, Dictionary<string, object> dictToLog){
			if (dictToLog == null)
			{
				Log ("Empty dict");
				return;
			}
			
			if (dictToLog.Count > 0) {
				foreach (KeyValuePair<string, object> i in dictToLog) {
					Log(elemName + " key =" + i.Key + " value = " + i.Value);
				}
			} else {
				Log ("Empty dict");
			}
		}
	}
}
