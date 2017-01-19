using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla
{
	public class GridView : MonoBehaviour {

		//private GameObject itemPrefab;
		public int itemCount, columnCount;
		public IBaseAdapter adapter; 

		public void SetAdapter(IBaseAdapter adapter, int columnCount)
		{
			if (this == null)
				return;
			this.adapter = adapter;
			this.itemCount = adapter.GetCount ();
			this.columnCount = columnCount;
			Draw ();
		}

		public void Draw()
		{
			if (this == null)
				return;
			Clear ();
			RectTransform itemRectTransform = adapter.GetPrefab().GetComponent<RectTransform>();//itemPrefab.GetComponent<RectTransform>();
			RectTransform containerRectTransform = gameObject.GetComponent<RectTransform>();
			
			//calculate the width and height of each child item.
			float width = containerRectTransform.rect.width / columnCount;
			float ratio = width / itemRectTransform.rect.width;
			float height = itemRectTransform.rect.height * ratio;
			int rowCount = itemCount / columnCount;
			if (rowCount == 0) {
				rowCount = 1;
			} else {
				if (itemCount % rowCount > 0) {
					rowCount++;
				} 
//				else if(itemCount > columnCount){
//					rowCount++;
//				}
			}
			
			//adjust the height of the container so that it will just barely fit all its children
			float scrollHeight = height * rowCount;
			containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, containerRectTransform.offsetMax.y - scrollHeight);//-scrollHeight / 2);
			containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, containerRectTransform.offsetMax.y);//scrollHeight / 2);
			
			int j = 0;
			for (int i = 0; i < itemCount; i++)
			{
				//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
				if (i % columnCount == 0)
					j++;
				
				//create a new item, name it, and set the parent
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

		void Clear()
		{
			List<GameObject> children = new List<GameObject>();
			if (this == null)
				return;
			for(int i = 0; i < transform.childCount; i++)
				children.Add(transform.GetChild(i).gameObject);

			foreach (GameObject obj in children)
			{
				if (obj != null)
					Destroy(obj);
			}

			//children.ForEach(child => Destroy(child));
		}
	}
}
	