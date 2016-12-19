using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Xsolla 
{
	public class ListView : MonoBehaviour
	{

		private IBaseAdapter adapter;

		public void ResizeToParent()
		{
            RectTransform containerRectTransform = GetComponent<RectTransform>();
            RectTransform parentRectTransform = transform.root.gameObject.GetComponent<RectTransform> ();
			float parentHeight = parentRectTransform.rect.height;
            float parentWidth = parentRectTransform.rect.width;
            //float parentRatio = parentWidth/parentHeight;// > 1 horizontal
            //float width = containerRectTransform.rect.width;
			containerRectTransform.offsetMin = new Vector2 (-parentWidth/2, -parentHeight/2);
			containerRectTransform.offsetMax = new Vector2 (parentWidth/2, parentHeight/2);
		}

		void Clear()
		{
			List<GameObject> children = new List<GameObject>();
			foreach (Transform child in transform) children.Add(child.gameObject);
			children.ForEach(child => Destroy(child));
		}

		public void SetAdapter(IBaseAdapter adapter){
			this.adapter = adapter;
		}

		public void DrawList(){
			Clear ();
//			ResizeToParent ();
            GameObject itemPrefab = adapter.GetPrefab ();
			int itemCount = adapter.GetCount ();
			if(itemCount > 0){
				int columnCount = 1;

	            RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform>();
	            RectTransform containerRectTransform = gameObject.GetComponent<RectTransform>();

	            //calculate the width and height of each child item.
	            float width = containerRectTransform.rect.width / columnCount;
	            float ratio = width / rowRectTransform.rect.width;
	            float height = rowRectTransform.rect.height * ratio;
	            int rowCount = itemCount / columnCount;
	            if (itemCount % rowCount > 0)
	                rowCount++;

	            //adjust the height of the container so that it will just barely fit all its children
	            float scrollHeight = height * rowCount;
	            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
	            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

	            int j = 0;
	            for (int i = 0; i < itemCount; i++)
	            {
	                //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
	                if (i % columnCount == 0)
	                    j++;

	                GameObject newItem = adapter.GetView(i);
	                newItem.transform.SetParent(gameObject.transform);

	                //move and size the new item
	                RectTransform rectTransform = newItem.GetComponent<RectTransform>();

	                float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
	                float y = containerRectTransform.rect.height / 2 - height * j;
	                rectTransform.offsetMin = new Vector2(x, y);

	                x = rectTransform.offsetMin.x + width;
	                y = rectTransform.offsetMin.y + height;
	                rectTransform.offsetMax = new Vector2(x, y);
	            }
			}
        }

		public void DrawList(RectTransform parentRectTransform){
			Clear ();
			//			ResizeToParent ();
			LayoutElement elem = GetComponent<LayoutElement>();
			GameObject itemPrefab = adapter.GetPrefab ();
			int itemCount = adapter.GetCount ();
			if (itemCount > 0) {
				int columnCount = 1;
			
				RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform> ();
				RectTransform containerRectTransform = gameObject.GetComponent<RectTransform> ();
			
				//calculate the width and height of each child item.
				float width = parentRectTransform.rect.width / columnCount;
				float ratio = width / rowRectTransform.rect.width;
				float height = rowRectTransform.rect.height * ratio;
				int rowCount = itemCount / columnCount;
				if (itemCount % rowCount > 0)
					rowCount++;
			
				//adjust the height of the container so that it will just barely fit all its children
				float scrollHeight = height * rowCount;
				containerRectTransform.offsetMin = new Vector2 (parentRectTransform.offsetMin.x, -scrollHeight / 2);
				containerRectTransform.offsetMax = new Vector2 (parentRectTransform.offsetMax.x, scrollHeight / 2);
			
				int j = 0;
				for (int i = 0; i < itemCount; i++) {
					//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
					if (i % columnCount == 0)
						j++;
				
					GameObject newItem = adapter.GetView (i);
					newItem.transform.SetParent (gameObject.transform);
					// set height for all listView
					elem.minHeight += newItem.GetComponent<RectTransform>().rect.height;
				
					//move and size the new item
					RectTransform rectTransform = newItem.GetComponent<RectTransform> ();
				
					float x = -parentRectTransform.rect.width / 2 + width * (i % columnCount);
					float y = containerRectTransform.rect.height / 2 - height * j;
					rectTransform.offsetMin = new Vector2 (x, y);
				
					x = rectTransform.offsetMin.x + width;
					y = rectTransform.offsetMin.y + height;
					rectTransform.offsetMax = new Vector2 (x, y);
				}
			}
		}
		
	}
}
