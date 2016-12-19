using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla 
{
	public class LinearLayout : MonoBehaviour 
		{

		public List<GameObject> objects;

		private float totalHeight = 0;
		private float containerFinalHeight = 0;
		private float parentHeight;
		private RectTransform parentRectTransform;
		private RectTransform containerRectTransform;

//		void Start()
//		{
//			for (int i = 0; i < objects.Count; i++) {
//				objects[i] = Instantiate(objects[i]);
//			}
//			Invalidate ();
//		}

		public void ReplaceObject(int position, GameObject gameObject)
		{
			objects[position] = gameObject;
		}

		public void AddObject(GameObject go){
			if(go != null)
				objects.Add (go);
		}

		public void Invalidate(){
//			parentHeight = transform.parent.gameObject.GetComponent<RectTransform> ().rect.height;
//			containerRectTransform = GetComponent<RectTransform>();
			containerRectTransform = GetComponent<RectTransform>();
			parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform> ();
			parentHeight = parentRectTransform.rect.height;
//			ResizeToParent ();
			GetTotalHeight ();
			DrawLayout ();
		}

//		public void ResizeToParent()
//		{
//
//			float parentWidth = parentRectTransform.rect.width;
//			float parentRatio = parentWidth/parentHeight;// > 1 horizontal
//			float width = containerRectTransform.rect.width;
//			if (parentRatio < 1) {
//				containerRectTransform.offsetMin = new Vector2 (-parentWidth/2, -parentHeight/2);
//				containerRectTransform.offsetMax = new Vector2 (parentWidth/2, parentHeight/2);
//			} else {
//				float newWidth = parentWidth/3;
//				if(width < newWidth){
//					containerRectTransform.offsetMin = new Vector2 (-newWidth/2, -parentHeight/2);
//					containerRectTransform.offsetMax = new Vector2 (newWidth/2, parentHeight/2);
//				}
//			}
//		}

		float GetTotalHeight()
		{
			foreach (GameObject go in objects) 
			{
				RectTransform goRectTransform = go.GetComponent<RectTransform>();
				
				//calculate the width and height of each child item.
				float width = containerRectTransform.rect.width;
				float ratio = width / goRectTransform.rect.width;
				float height = goRectTransform.rect.height * ratio;
				
				//adjust the height of the container so that it will just barely fit all its children
				containerFinalHeight += height;
			}
			return containerFinalHeight;
		}

		public void DrawLayout(){
			float height = 0;

			foreach (GameObject go in objects) 
			{
				DrawObject(go);
				height += go.transform.localScale.y;
			}
			// offsetMin = Lower Left Corner
			//containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, (-containerFinalHeight + parentHeight / 2));
			// offsetMax = Upper Right Corner
			//containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, parentHeight / 2);//totalHeight / 2
//			RectTransform rect = this.GetComponent<RectTransform>();
//			rect.position = new Vector3(0,0);
		}

		void DrawObject(GameObject go){
			RectTransform goRectTransform = go.GetComponent<RectTransform>();
			
			//calculate the width and height of each child item.
			float width = containerRectTransform.rect.width;
			float ratio = width / goRectTransform.rect.width;
			float height = goRectTransform.rect.height * ratio;
			
			//adjust the height of the container so that it will just barely fit all its children
			totalHeight += height;
			go.transform.SetParent (gameObject.transform);
			//move and size the new item
			RectTransform rectTransform = go.GetComponent<RectTransform>();
			
			float x = -containerRectTransform.rect.width / 2;
			float y = containerFinalHeight / 2 - totalHeight;
			rectTransform.offsetMin = new Vector2(x, y);

			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2(x, y);
		}
	}
}