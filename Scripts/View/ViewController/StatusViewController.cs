using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace Xsolla {
	public class StatusViewController : ScreenBaseConroller<XsollaStatus> {
		
		public event Action<XsollaStatus.Group, string, Xsolla.XsollaStatusData.Status, Dictionary<string, object>> StatusHandler;
		public GameObject status;
		public Transform checkListTransform;
		public Button statusViewExitButton;

		// prefabs for checklist linearLayout
		public GameObject rowTitlePrefab;
		public GameObject rowStatusPrefab;
		public GameObject rowLinePrefab;
		public GameObject rowInfoElementPrefab;
		public GameObject rowInfoElementBigPrefab;

		public override void InitScreen (XsollaTranslations translations, XsollaStatus xsollaStatus)
		{
			ResizeToParent ();
			string input = translations.Get (XsollaTranslations.STATUS_PURCHASED_DESCRIPTION);
			XsollaStatusText statusText = xsollaStatus.GetStatusText ();
			XsollaStatus.Group currentStatus = xsollaStatus.GetGroup ();
			if (input != null) {
				string pattern = "{{.*?}}";
				Regex regex = new Regex (pattern);

				if ((statusText.GetPurchsaeValue() != "") && (statusText.Get ("sum").GetValue () != ""))
				{
					input = regex.Replace (input, statusText.GetPurchsaeValue(), 1);
					input = regex.Replace (input, statusText.Get ("sum").GetValue (), 1);	
				}
				else
					input = "";
			}
			else 
			{
				input = "";
			}
			PrepareStatus (currentStatus, xsollaStatus.GetStatusText().GetState (), input, xsollaStatus.GetInvoice());
			AddTitle (statusText.GetProjectString());
			if(currentStatus == XsollaStatus.Group.DONE)
				AddStatus (translations.Get (XsollaTranslations.VIRTUALSTATUS_DONE_DESCRIPTIONS));

			if (statusText.GetPurchsaeValue() != "")
				AddElement (statusText.GetPurchsaeValue(), statusText.Get ("sum").GetValue ());
			
			XsollaStatusText.StatusTextElement element = statusText.Get ("out");
			if(element != null)
				AddElement (statusText.Get("out").GetPref(), statusText.Get("out").GetValue());
			element = statusText.Get ("invoice");
			if(element != null)
				AddElement (statusText.Get("invoice").GetPref(), statusText.Get("invoice").GetValue());
			element = statusText.Get ("details");
			if(element != null)
				AddElement (statusText.Get("details").GetPref(), statusText.Get("details").GetValue());
			element = statusText.Get ("time");
			if(element != null)
				AddElement (statusText.Get("time").GetPref(), statusText.Get("time").GetValue());
			element = statusText.Get ("merchant");
			if(element != null)
				AddElement (statusText.Get("merchant").GetPref(), statusText.Get("merchant").GetValue());
			element = statusText.Get("userWallet");
			if(element != null)
				AddElement (statusText.Get("userWallet").GetPref(), statusText.Get("userWallet").GetValue());
			AddLine ();
			AddBigElement (statusText.Get("sum").GetPref(), statusText.Get("sum").GetValue());

//			if(statusText.backUrlCaption != null && !"".Equals(statusText.backUrlCaption))
//				statusViewExitButton.gameObject.GetComponent<Text> ().text = statusText.backUrlCaption;
//			else
//				statusViewExitButton.gameObject.GetComponent<Text> ().text = translations.Get(XsollaTranslations.BACK_TO_STORE);

			statusViewExitButton.gameObject.GetComponent<Text> ().text = translations.Get(XsollaTranslations.BACK_TO_STORE);
			statusViewExitButton.onClick.AddListener (delegate 
				{
					//OnClickExit(currentStatus, xsollaStatus.GetStatusData().GetInvoice(), xsollaStatus.GetStatusData().GetStatus(), null);
					OnClickBack(currentStatus, xsollaStatus.GetStatusData().GetInvoice(), xsollaStatus.GetStatusData().GetStatus(), null);
				});

		}

		public void DrawVpStatus(XsollaUtils utils, XVPStatus status){
			XsollaTranslations translations = utils.GetTranslations ();
			string input;
			if (status.Status.HeaderDescription == null)
				input = translations.Get (XsollaTranslations.STATUS_PURCHASED_DESCRIPTION);
			else
				input = status.Status.HeaderDescription;
			XsollaStatus.Group currentStatus = status.GetGroup ();

			PrepareStatus (currentStatus, status.Status.Header, input, "");
			AddTitle (utils.GetProject().name);
			AddStatus (status.Status.Description);
			AddElement (translations.Get ("virtualstatus_check_operation"), status.OperationId);
			AddElement (translations.Get ("virtualstatus_check_time"), string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Parse(status.OperationCreated)));
			if (status.Items.Count > 0)
				AddElement (translations.Get ("virtualstatus_check_virtual_items"), status.GetPurchase(0));
			if (status.vCurr.Count > 0)
				AddElement (translations.Get ("virtualstatus_check_virtual_currency"), status.GetVCPurchase(0) + " " + utils.GetProject().virtualCurrencyName);

			if (status.OperationType != "coupon")
			{
				AddLine ();
				AddBigElement (translations.Get ("virtualstatus_check_vc_amount"), status.VcAmount + " " + utils.GetProject().virtualCurrencyName);
			}
			statusViewExitButton.gameObject.GetComponent<Text> ().text = translations.Get(XsollaTranslations.BACK_TO_STORE);
			statusViewExitButton.onClick.AddListener (delegate 
				{
					OnClickBack(currentStatus, status.OperationId, Xsolla.XsollaStatusData.Status.DONE, status.GetPurchaseList());
				});
		}

		public void PrepareStatus(XsollaStatus.Group group, string state, string purchase, string invoice){
			Component[] texts = status.GetComponentsInChildren(typeof(Text),true);// [0] Icon [1] Title [2] purchse
//  			Image bg = status.GetComponent<Image> ();
			ColorController colorController = GetComponent<ColorController> ();
			((Text)texts[1]).text = state;
			if (purchase != null && purchase != "")
			{
				((Text)texts[2]).text = purchase;
				texts[2].gameObject.SetActive(true);
			}
			else
				texts[2].gameObject.SetActive(false);

			switch (group){
				case XsollaStatus.Group.DONE:
				((Text)texts[0]).text = "";
					colorController.ChangeColor(1, StyleManager.BaseColor.bg_ok);
					break;
				case XsollaStatus.Group.TROUBLED:
					((Text)texts[0]).text = "";
					colorController.ChangeColor(1, StyleManager.BaseColor.bg_error);
//					bg.color = new Color (0.980392157f, 0.454901961f, 0.392156863f);
					break;
				case XsollaStatus.Group.INVOICE:
				case XsollaStatus.Group.DELIVERING:
				case XsollaStatus.Group.UNKNOWN:
				default:
					((Text)texts[0]).text = "";
					((Text)texts[0]).gameObject.AddComponent<MyRotation>();
					colorController.ChangeColor(1, StyleManager.BaseColor.selected);
//					bg.color = new Color (0.639215686f, 0.552941176f, 0.847058824f);
					StartCoroutine(UpdateStatus(invoice));
					break;
			}
		}

		private void AddTitle(string s){
			GameObject titleInstance = Instantiate (rowTitlePrefab) as GameObject;
			titleInstance.transform.SetParent (checkListTransform);
			titleInstance.GetComponentInChildren<Text> ().text = s;
		}

		private void AddStatus(string s){
			GameObject statusInstance = Instantiate (rowStatusPrefab) as GameObject;
			statusInstance.transform.SetParent (checkListTransform);
			statusInstance.GetComponentInChildren<Text> ().text = s;
		}

		private void AddLine(){
			GameObject lineInstance = Instantiate (rowLinePrefab) as GameObject;
			lineInstance.transform.SetParent (checkListTransform);
		}

		private void AddElement(string s, string s1){
			GameObject elementInstance = Instantiate (rowInfoElementPrefab) as GameObject;
			elementInstance.transform.SetParent (checkListTransform);
			Text[] texts = elementInstance.GetComponentsInChildren<Text> ();
			texts [0].text = s;
			texts [1].text = s1;
		}

		private void AddBigElement(string s, string s1){
			// if key and value is empty
			if ((s == "") && (s1 == ""))
				return;

			GameObject elementInstance = Instantiate (rowInfoElementBigPrefab) as GameObject;
			elementInstance.transform.SetParent (checkListTransform);
			Text[] texts = elementInstance.GetComponentsInChildren<Text> ();
			texts [0].text = s;
			texts [1].text = s1;
		}
		
		private IEnumerator UpdateStatus(string invoice)
		{
			yield return new WaitForSeconds(5);
			Dictionary<string, object> map = new Dictionary<string, object> ();
			map.Add ("section", "getstatus");
			map.Add ("action", "getstatus");
			map.Add ("invoice", invoice);
			gameObject.GetComponentInParent<XsollaPaystationController> ().GetStatus (map);//DoPayment (new Dictionary<string, object>());
		}

		private void OnClickExit(XsollaStatus.Group group , string invoice, Xsolla.XsollaStatusData.Status status, Dictionary<string, object> pPurchase)
		{
			if (StatusHandler != null)
				StatusHandler (group, invoice, status, pPurchase);
			if (GetComponentInParent<XsollaPaystationController> () != null)
				GetComponentInParent<XsollaPaystationController> ().gameObject.GetComponentInChildren<Selfdestruction> ().DestroyRoot ();
		}

		private void OnClickBack(XsollaStatus.Group group ,string invoice, Xsolla.XsollaStatusData.Status status, Dictionary<string, object> pPurchase)
		{
			if (StatusHandler != null)
				StatusHandler (group, invoice, status, pPurchase);
			if (GetComponentInParent<XsollaPaystationController> () != null)
				GetComponentInParent<XsollaPaystationController> ().LoadGoodsGroups();
		}

	}

}
