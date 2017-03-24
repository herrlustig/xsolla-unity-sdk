using UnityEngine;
using System.Collections;
using Xsolla;

public class Selfdestruction : MonoBehaviour {

	public void DestroyRoot(){
		XsollaPaystationController controller = gameObject.GetComponentInParent<XsollaPaystationController> ();
		Destroy (controller.gameObject);

		StyleManager[] listObjStyles = (StyleManager[])FindObjectsOfType(typeof(StyleManager));
		foreach(StyleManager item in listObjStyles)
			Destroy(item.gameObject);

		ApiRequest[] listObjRequest = (ApiRequest[])FindObjectsOfType(typeof(ApiRequest));
		foreach(ApiRequest item in listObjRequest)
			Destroy(item.gameObject);

		TransactionHelper.Clear ();
	}

	public void Selfdestroy(){
		Destroy (gameObject);
	}

	public void DestroyObject(GameObject go){
		Destroy (go);
	}
}
