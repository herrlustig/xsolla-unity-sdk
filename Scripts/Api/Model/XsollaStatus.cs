using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Xsolla
{
	public class XsollaStatus : IParseble
	{

		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
		* CONSTANTS
		* * * * * * * * * * * * * * * * * * * * * * * * * * */
		public static string S_GROUP = "group";
		public static string S_INVOICE = "invoice";
		public static string S_DATA = "statusData";
		public static string S_TEXT = "text";
		
		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
		* PRIVATE FIELDS
		* * * * * * * * * * * * * * * * * * * * * * * * * * */

//		private string marketplace;//		"marketplace":"paystation",
		private XsollaStatusData statusData;//		"statusData":{},
		private string invoice;//		"invoice":"153280115",
//		private string repayment;//		"repayment":[],
//		private string email;//		"email":{},
		private string group;//		"group":"invoice",
		private XsollaStatusText text;//		"text":{},
		private string country;//		"country":"RU",
//		private string feedback;//		"feedback":{},
//		private string change;//		"change":[],
		private string returnRegion;//		"return_region":"top",
		private bool isCancelUser;//		"isCancelUser":false,
		private bool isPreloader;//		"isPreloader":true,
//		private string alternative;//		"alternative":{},
		private bool showEmailRequest;//		"showEmailRequest":true,
//		private string autoRecharge;//		"autoRecharge":{},
		private string titleClass;//		"title_class":"application",
		private bool needToCheck;//		"needToCheck":true,
//		private string ajaxURL;//		"ajaxURL":"https:\/\/secure.xsolla.com\/paystation2\/api\/directpayment?theme=100&project=15070&marketplace=paystation&signparams=email%2Cv1%2Cv2%2Cproject%2Clocal%2Ccurrency&v1=user_1&v2=John+Smith&out=100&email=support%40xsolla.com¤cy=USD&hidden=country%2Cout&country=RU&local=en&out_check_type=equals§ion=getstatus&invoice=153280115"


		public override string ToString ()
		{
			return string.Format ("[XsollaStatus: S_GROUP={0}, S_INVOICE={1}, S_DATA={2}, S_TEXT={3}, statusData={4}, invoice={5}, group={6}, text={7}, country={8}, returnRegion={9}, isCancelUser={10}, isPreloader={11}, showEmailRequest={12}, titleClass={13}, needToCheck={14}]", S_GROUP, S_INVOICE, S_DATA, S_TEXT, statusData, invoice, group, text, country, returnRegion, isCancelUser, isPreloader, showEmailRequest, titleClass, needToCheck);
		}

		public enum Group
		{
			INVOICE, DONE, TROUBLED, DELIVERING, UNKNOWN
		}
		
		public Group GetGroup()
		{
			switch(group)
			{
				case "invoice":
					return Group.INVOICE;
				case "done":
					return Group.DONE;
				case "delivering":
					return Group.DELIVERING;
				case "troubled":
					return Group.TROUBLED;
				default:
					return Group.UNKNOWN;
			}
		}

		public bool IsDone(){
			return "done".Equals (group);
		}

		public string GetInvoice()
		{
			return invoice;
		}

		public XsollaStatus ()
		{
		}
		
		public IParseble Parse (JSONNode rootNode)
		{
			JSONNode statusNode = rootNode[XsollaApiConst.R_STATUS];
			if (statusNode != null && !"null".Equals(statusNode.ToString())) 
			{
				this.group = statusNode[S_GROUP];
				this.invoice = statusNode[S_INVOICE];
				this.statusData = new XsollaStatusData(statusNode[S_DATA]);
				this.text = new XsollaStatusText(statusNode[S_TEXT]);
				this.country = statusNode["country"];
				this.returnRegion = statusNode["return_region"];
				this.isCancelUser = statusNode["isCancelUser"].AsBool;
				this.isPreloader = statusNode["isPreloader"].AsBool;
				this.showEmailRequest = statusNode["showEmailRequest"].AsBool;
				this.titleClass = statusNode["title_class"];
				this.needToCheck = statusNode["needToCheck"].AsBool;
            }
			return this;
		}

		public XsollaStatusText GetStatusText()
		{
			return text;
		}

		public XsollaStatusData GetStatusData()
		{
			return statusData;
		}

//        public override string ToString()
//		{
//			return string.Format ("[XsollaStatus]"
//			                      + "\n group=" + group
//			                      + "\n invoice=" + invoice
//			                      + "\n xsollaStatusData=" + statusData.ToString()
//			                      + "\n xsollaStatusText=" + text).ToString();
//        }
	}

	public class XsollaStatusText {
			
		public static string ST_STATE = "state";
		public static string ST_BACKURL = "backurl";
		public static string ST_BACKURL_CAPTION = "backurl_caption";
		public static string ST_INFO = "info";
		public static string STE_KEY = "key";
		public static string STE_PREF = "pref";
		public static string STE_PARAMETER = "parameter";
		public static string STE_VALUE = "value";
		public static string STE_NAME = "name";

		public string state {get; private set;}
		public string backUrl {get; private set;}
		public string backUrlCaption { get; private set;}
		private List<StatusTextElement> textElements;
		private Dictionary<string ,StatusTextElement> textElementsMap;

		public XsollaStatusText(JSONNode statusTextNode){
			this.state = statusTextNode [ST_STATE];
			this.backUrl = statusTextNode [ST_BACKURL];
			this.backUrlCaption = statusTextNode [ST_BACKURL_CAPTION];
			textElements = new List<StatusTextElement> ();
			textElementsMap = new Dictionary<string ,StatusTextElement> ();
			IEnumerable<JSONNode> enumerable = statusTextNode[ST_INFO].Childs;
			IEnumerator<JSONNode> iterator = enumerable.GetEnumerator ();
			while(iterator.MoveNext())
			{
				JSONNode textElement = iterator.Current;
				string key = textElement[STE_KEY].Value;
				string pref = textElement[STE_PREF].Value;
				string parameter = textElement[STE_PARAMETER].Value;
				string value = textElement[STE_VALUE].Value;
				string name = textElement[STE_NAME].Value;

				if("time".Equals(key)) {
					DateTime dateTime = DateTime.Parse(value);
					string dateString = string.Format("{0:dd/MM/yyyy HH:mm}", dateTime);
					value = dateString;
				} 

				if("recurrentDateNextCharge".Equals(key)) {
					DateTime dateTime = DateTime.Parse(value);
					string dateString = string.Format("{0:dd/MM/yyyy}", dateTime);
					value = dateString;
				}

				var newSte = new StatusTextElement(key, pref, parameter, value, name);
				AddStatusTextElement(newSte);
			}
		}

		public void AddStatusTextElement(StatusTextElement textElement) {
			textElements.Add(textElement);
			textElementsMap.Add (textElement.GetKey(), textElement);
		}

		public StatusTextElement Get(string key){
			if (textElementsMap.ContainsKey (key))
				return textElementsMap [key];
			else
			{
				// TODO: by def return null, but on code above we get object and immediate execute GetValue and have NullReferenceException
				//return null;
				Logger.Log("We get null StatusTextElement by key - " + key);
				return new StatusTextElement("","","","","");
			}
		}

		public string GetPurchsaeValue(){
			if (textElementsMap.ContainsKey ("out")) {
				return textElementsMap ["out"].GetValue ();
			} else if (textElementsMap.ContainsKey ("digital_goods")) {
				return textElementsMap ["digital_goods"].GetValue ();
			} else {
				return "";
			}
		}

		public List<StatusTextElement> GetStatusTextElements()
		{
			return textElements;
		}

		public string GetState()
		{
			return state;
		}

		public string GetProjectString(){
			return textElementsMap ["project"].GetPref () + " - " + textElementsMap ["project"].GetValue();
		}

		public override string ToString() {
			return string.Format ("[XsollaStatusText]"
			                      + "\nstate='" + state
			                      + "\n, backUrl='" + backUrl
			                      + "\n, textElements='" + textElements);
		}

		public class StatusTextElement 
		{
			private string key;
			private string pref;
			private string parameter;
			private string value;
			private string name;

			StatusTextElement() {
			}

			public StatusTextElement(string key, string pref, string parameter, string value, string name) {
				this.key = key;
				this.pref = pref;
				this.parameter = parameter;
				this.value = value;
				this.name = name;
			}

			public string GetKey() {
				return key;
			}

			public string GetPref() {
				return pref;
			}

			public string GetParameter() {
				return parameter;
			}

			public string GetValue() {
				return value;
			}

			public string GetName() {
				return name;
			}

			public override string ToString() {
				return string.Format ("[StatusTextElement]"
										+ "\n key= " + key
										+ "\n pref= " + pref
										+ "\n parameter= " + parameter
										+ "\n value= " + value
										+ "\n name= " + name);
			}

		}
					
    }
    
    public class XsollaStatusData {
		
		public static string SD_EMAIL = "email";
		public static string SD_INVOICE = "invoice";
		public static string SD_USER_ID = "v1";
		public static string SD_STATE = "state";
		public static string SD_STATETEXT = "stateText";
		public static string SD_PID = "pid";
		public static string SD_OUT = "out";
		
		private string email;
		private long invoice;
		private Status status;
		private string userId;
		private string currencyAmount;
		private long paymentSystemId;

		public enum Status {
			CREATED, INVOICE, DONE, CANCELED, UNKNOWN
		}

		public XsollaStatusData(JSONNode statusDataNode){

			this.email = statusDataNode[SD_EMAIL];
			this.invoice = statusDataNode[SD_INVOICE].AsInt;
			this.userId = statusDataNode[SD_USER_ID];
			this.currencyAmount = statusDataNode[SD_OUT];
			switch (statusDataNode [SD_STATE].AsInt) 
			{
				case 0:
					this.status = Status.CREATED;
					break;
				case 1:
					this.status = Status.INVOICE;
					break;
				case 2:
					this.status = Status.CANCELED;
					break;
				case 3:
					this.status = Status.DONE;
					break;
				default:
					this.status = Status.UNKNOWN;
					break;
			}
		}

		public string GetInvoice(){
			return invoice.ToString ();
		}

		public Status GetStatus(){
			return status;
		}

    	
		public override string ToString ()
		{
			return string.Format ("[XsollaStatusData]"
			                      + "\n email= " + email
			                      + "\n invoice= " + invoice
			                      + "\n userId= " + userId
			                      + "\n currencyAmount= " + currencyAmount
			                      + "\n status= " + status
			                      );
		}
	}

}

