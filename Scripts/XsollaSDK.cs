using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SimpleJSON;

namespace Xsolla {
	public class XsollaSDK : MonoBehaviour {

		public bool isSandbox = false;
		public CCPayment payment;
		private string token;

		public string getToken()
		{
			return token;
		}

		public string InitPaystation(string token)
		{
			var url = "https://secure.xsolla.com/paystation2/?access_token=" + token;
			Application.OpenURL (url);
			return url;
		}

		public void CreatePaymentForm()
		{
			XsollaPaystationController formController = GetPaystationController ();
			formController.OkHandler += (status) => {Debug.Log("OkHandler 1 " + status);};
			formController.ErrorHandler += (error) => {Debug.Log("ErrorHandler 2 " + error);};


			XsollaJsonGenerator generator = new XsollaJsonGenerator ("user_1", 14004);//test 15764
			generator.user.name = "John Smith";
			generator.user.email = "support@xsolla.com";
			generator.user.country = "US";
			generator.settings.currency = "USD";
			generator.settings.languge = "en";
			string request = generator.GetPrepared (); 

			// Prepare args
			Dictionary<string, object> dataArgs = new Dictionary<string, object>();
			dataArgs.Add("data", request);
			XsollaPaystation.AddHttpRequestObj();
			StartCoroutine(XsollaJsonGenerator.FreshToken ((token) => SetToken(formController, token)));
		}

		private void SetToken(XsollaPaystationController controller, string token){
			if(token != null) { 
				controller.OpenPaystation (token, false);
			}
		}

		public void CreatePaymentForm(InputField inputField)
		{
			XsollaPaystationController formController = GetPaystationController ();
			string accessToken = inputField.text;
			formController.OkHandler += (status) => {Debug.Log("OkHandler 1 " + status);};
			formController.ErrorHandler += (error) => {Debug.Log("ErrorHandler 2 " + error);};
			formController.OpenPaystation (accessToken, isSandbox);
		}

		public void CreatePaymentForm(string data)
		{
			XsollaPaystationController formController = GetPaystationController ();
			formController.OkHandler += (status) => {Debug.Log("OkHandler 1 " + status);};
			formController.ErrorHandler += (error) => {Debug.Log("ErrorHandler 2 " + error);};
			formController.OpenPaystation (data, isSandbox);
		}

		public void CreatePaymentForm(string data, Action<XsollaResult> actionOk, Action<XsollaError> actionError)
		{
			XsollaPaystationController formController = GetPaystationController();
			formController.OkHandler += actionOk;
			formController.ErrorHandler += actionError;
			formController.OpenPaystation (data, isSandbox);
		}

		public void CreatePaymentForm(XsollaJsonGenerator generator, Action<XsollaResult> actionOk, Action<XsollaError> actionError)
		{
			CreatePaymentForm (generator.GetPrepared (), actionOk, actionError);
		}

		public void DirectPayment(CCPayment payment)
		{
			payment.InitPaystation ();
		}


		public void SetToken(string s)
		{
			this.token = s;
		}

		public void SetSandbox(bool b){
			this.isSandbox = b;
		}

		private XsollaPaystationController GetPaystationController() {
			GameObject paystationobject = Instantiate(Resources.Load("Prefabs/XsollaPaystation")) as GameObject;
			XsollaPaystationController formController = paystationobject.GetComponent<XsollaPaystationController> ();
			return formController;
		}
	}
}
