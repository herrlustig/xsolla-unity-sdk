using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public static class Resizer {

		public static void ResizeToParrent(GameObject go)
		{
			var containerRectTransform = go.GetComponent<RectTransform>();
			var parentRectTransform = go.transform.parent.gameObject.GetComponent<RectTransform> ();
			var parentHeight = parentRectTransform.rect.height;
			var parentWidth = parentRectTransform.rect.width;
			var parentRatio = parentWidth/parentHeight;// > 1 horizontal
			float parentScale = parentRectTransform.localScale.x;
    		var width = containerRectTransform.rect.width;
			if (parentRatio < 1) {
				containerRectTransform.offsetMin = new Vector2 (-parentWidth/2, -parentHeight/2);
				containerRectTransform.offsetMax = new Vector2 (parentWidth/2, parentHeight/2);
			} else {
				var newWidth = parentWidth/3;
				if(width < newWidth){
					containerRectTransform.offsetMin = new Vector2 (-newWidth/2, -parentHeight/2);
					containerRectTransform.offsetMax = new Vector2 (newWidth/2, parentHeight/2);
				} else {
					containerRectTransform.offsetMin = new Vector2 (-width/2, -parentHeight/2/parentScale);
					containerRectTransform.offsetMax = new Vector2 (width/2, parentHeight/2/parentScale);
				}
			}
		}

		public static void DestroyChilds(Transform parentTransform)
		{
			List<GameObject> children = new List<GameObject>();
			foreach (Transform child in parentTransform) children.Add(child.gameObject);
			children.ForEach(child => Object.Destroy(child));
		}

		public static void SetParentToFullScreen(GameObject pObj, GameObject pParent)
		{
			pObj.transform.SetParent(pParent.transform);
			RectTransform objRect = pObj.GetComponent<RectTransform>();

			objRect.anchorMin = new Vector2(0, 0);
			objRect.anchorMax = new Vector2(1, 1);
			objRect.localScale = new Vector3(1, 1, 1);
			objRect.localPosition = new Vector3(0, 0, 0);
			objRect.offsetMin = new Vector2(0, 0);
			objRect.offsetMax = new Vector2(0, 0);
		}

		public static void ResizeToParrentRe(GameObject pObj)
		{
			RectTransform objRect = pObj.GetComponent<RectTransform>();

			objRect.anchorMin = new Vector2(0, 0);
			objRect.anchorMax = new Vector2(1, 1);
			objRect.localScale = new Vector3(1, 1, 1);
			objRect.localPosition = new Vector3(0, 0, 0);
			objRect.offsetMin = new Vector2(0, 0);
			objRect.offsetMax = new Vector2(0, 0);
		}
	}

}
