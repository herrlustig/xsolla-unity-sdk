using System;
using SimpleJSON;
using UnityEngine;

namespace Xsolla
{
	public class XsollaError : IParseble
	{
		public Source errorSource{get; private set;}
		public int errorCode{get; private set;}
		public string elementName{get; private set;}
		public string errorMessage{get; private set;}

		public enum Source {
			XSOLLA_API, APP_API, HTTP, NETWORK, CANCEL, UNEXPECTED
		}

		public XsollaError ()
		{
			this.errorSource = Source.APP_API;
		}

		public XsollaError (int errorCode)
		{	
			this.errorCode = errorCode;
		}

		public XsollaError (string errorMessage)
		{
			this.errorSource = Source.APP_API;
			this.errorMessage = errorMessage;
		}

		public XsollaError(int errorCode, string errorMessage) {
			this.errorSource = Source.UNEXPECTED;
			this.errorCode = errorCode;
			this.errorMessage = errorMessage;
		}

		private XsollaError(Source source) {
			this.errorSource = source;
		}
		
		private XsollaError(Source source, string errorMessage) {
			this.errorSource = source;
			this.errorMessage = errorMessage;
		}

		public IParseble ParseOld(JSONNode errorNode){
			this.errorSource = Source.XSOLLA_API;
			this.errorCode = errorNode ["attributes"]["code"].AsInt;
			this.elementName = errorNode ["name"];
			this.errorMessage = errorNode ["value"];
			return this;
		}

		public IParseble ParseNew(JSONNode errorNode){
			this.errorSource = Source.XSOLLA_API;
			this.errorCode = errorNode ["code"].AsInt;
			this.elementName = errorNode ["element_name"];
			this.errorMessage = errorNode ["message"];
			return this;
		}

		public IParseble Parse (JSONNode errorNode)
		{
			this.errorSource = Source.XSOLLA_API;
			if (errorNode.Count > 2) {
				this.errorCode = errorNode ["code"].AsInt;
				this.elementName = errorNode ["element_name"];
				this.errorMessage = errorNode ["message"];
				return this;
			} else if (errorNode.Count == 0) {
				return null;
			} else {
				this.errorCode = errorNode[XsollaApiConst.ERROR_CODE].AsInt;
				string errorMsg = errorNode["error"].Value;
				if(errorMsg == null || "".Equals(errorMsg))
					errorMsg = errorNode["message"];// NEW ? message
				this.errorMessage = errorMsg;
				return this;
			}
		}

		public static XsollaError GetUnsuportedError() {
			return new XsollaError(Source.XSOLLA_API, "Wrong Payment System");
		}

		public static XsollaError GetCancelError() {
			return new XsollaError(Source.CANCEL, "Canceled");
		}

		public string GetMessage()
		{
			return errorCode + " : " + errorMessage;
		}

		public override string ToString ()
		{
			return string.Format ("[XsollaError]"
			                      + "\n errorSource= " + errorSource 
			                      + "\n errorCode= " + errorCode 
			                      + "\n errorMessage= " + errorMessage);
		}

	}
}

