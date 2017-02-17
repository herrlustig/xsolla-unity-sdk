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

		public override string ToString ()
		{
			return string.Format ("[LinearLayout: objects={0}, totalHeight={1}, containerFinalHeight={2}, parentHeight={3}, parentRectTransform={4}, containerRectTransform={5}]", objects, totalHeight, containerFinalHeight, parentHeight, parentRectTransform, containerRectTransform);
		}
		

		public void ReplaceObject(int position, GameObject gameObject)
		{
			objects[position] = gameObject;
		}

		public void AddObject(GameObject go){
			if(go != null)
				objects.Add (go);
		}

		public void Invalidate(){
			containerRectTransform = GetComponent<RectTransform>();
			parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform> ();
			parentHeight = parentRectTransform.rect.height;
			GetTotalHeight ();
			DrawLayout ();
		}

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