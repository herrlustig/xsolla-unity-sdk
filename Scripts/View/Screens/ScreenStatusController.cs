using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Xsolla 
{
	[Obsolete("class is ScreenStatusController, please use StatusViewController instead.")]
	public class ScreenStatusController : ScreenBaseConroller<XsollaStatus>
	{
		public delegate void OnStatusRecived (XsollaStatusData statusData);

		public event Action<XsollaStatusData> StatusHandler;

		public LinearLayout linerLayout;
		public StatusElementAdapter adapter;

		public void Awake()
		{
			if(linerLayout == null)
				linerLayout = GetComponent<LinearLayout>();
		}

		public override void InitScreen (XsollaTranslations translations, XsollaStatus status)
		{
			DrawScreen (translations, status);
		}

		public void DrawScreen (XsollaTranslations translations, XsollaStatus xsollaStatus)
		{
			ResizeToParent ();
			XsollaStatus.Group group = xsollaStatus.GetGroup ();
			switch (group){
				case XsollaStatus.Group.DONE:
					linerLayout.AddObject (GetOkStatus (xsollaStatus.GetStatusText ().GetState ()));
				break;
				case XsollaStatus.Group.TROUBLED:
					linerLayout.AddObject (GetErrorByString (xsollaStatus.GetStatusText ().GetState ()));
					break;
				case XsollaStatus.Group.INVOICE:
				case XsollaStatus.Group.UNKNOWN:
				default:
					linerLayout.AddObject (GetWaitingStatus (xsollaStatus.GetStatusText ().GetState ()));
					StartCoroutine(TryIt(xsollaStatus.GetInvoice()));
					break;
			}
			linerLayout.AddObject(GetError (null));
			linerLayout.AddObject (GetEmpty ());
			adapter.SetElements (xsollaStatus.GetStatusText().GetStatusTextElements());
			GameObject list = GetList (adapter);
			list.GetComponent<ListView> ().DrawList (GetComponent<RectTransform> ());
			linerLayout.AddObject(list);
			linerLayout.AddObject (GetEmpty ());
			if(group == XsollaStatus.Group.INVOICE || group == XsollaStatus.Group.UNKNOWN){
				linerLayout.AddObject(GetButton ("Start again", delegate{StartAgain();}));
				linerLayout.AddObject (GetEmpty ());
			}
			linerLayout.Invalidate ();
		}

		private IEnumerator TryIt(string invoice)
		{
			yield return new WaitForSeconds(5);
			Dictionary<string, object> map = new Dictionary<string, object> ();
			map.Add ("section", "getstatus");
			map.Add ("action", "getstatus");
			map.Add ("invoice", invoice);
			gameObject.GetComponentInParent<XsollaPaystationController> ().GetStatus (map);//DoPayment (new Dictionary<string, object>());
		}

		
		void StartAgain(){
			gameObject.GetComponentInParent<XsollaPaystationController> ().DoPayment (new Dictionary<string, object>());
		}


		void OnClickButton(XsollaStatus status){
			OnStatusRecivied (status.GetStatusData());
			Destroy (gameObject.GetComponentInParent<XsollaPaystationController> ().gameObject);
		}


		private void OnStatusRecivied(XsollaStatusData xsollaStatusData)
		{
			if (StatusHandler != null)
				StatusHandler (xsollaStatusData);
		}
	}
}
