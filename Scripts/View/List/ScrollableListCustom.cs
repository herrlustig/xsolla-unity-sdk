using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;

namespace Xsolla 
{
	public class ScrollableListCustom : MonoBehaviour
	{
	    public GameObject itemPrefab;
		private List<GameObject> items;
	//    public int itemCount = 10, columnCount = 1;

//		void Start()
//		{
//			Dictionary<string, string> dict = new Dictionary<string, string> ();
//			dict.Add ("1", "assdsdasd");
//			dict.Add ("2", "bytyter");
//			dict.Add ("3", "trwegvbtrw");
//			dict.Add ("4", "45123412");
//			dict.Add ("5", "tgtv52345");
//			dict.Add ("6", "4321fff43214");
//			dict.Add ("7", "123412f43");
//			SetData (null, dict);
//		}

		public void SetData(Action<string> onItemClickAction, Dictionary<string, string> objects)
	    {
			int itemCount = objects.Count;
			int columnCount = 1;
			items = new List<GameObject> (columnCount);
			RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform> ();
			RectTransform containerRectTransform = gameObject.GetComponent<RectTransform> ();

			//calculate the width and height of each child item.
			float width = containerRectTransform.rect.width / columnCount;
			float ratio = width / rowRectTransform.rect.width;
			float height = rowRectTransform.rect.height * ratio;
			int rowCount = itemCount / columnCount;
			if (itemCount % rowCount > 0)
				rowCount++;

			//adjust the height of the container so that it will just barely fit all its children
			float scrollHeight = height * rowCount;
			containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, -scrollHeight / 2);
			containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, scrollHeight / 2);

			int j = 0;
			for (int i = 0; i < itemCount; i++) {
				//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
				if (i % columnCount == 0)
					j++;

				//create a new item, name it, and set the parent
				GameObject newItem = Instantiate (itemPrefab) as GameObject;
				newItem.name = gameObject.name + " item at (" + i + "," + j + ")";
				newItem.transform.SetParent(gameObject.transform);
				Resizer.SetDefScale(newItem);
				Text textField = newItem.GetComponentsInChildren<Text> (true)[0];
				KeyValuePair<string, string> o = objects.ElementAt(i);//.Keys.ElementAt2(i);
				textField.text = o.Value;
				if (onItemClickAction != null) {
					newItem.GetComponents<Button>()[0].onClick.AddListener (() => {onItemClickAction(o.Key);});
				}
				items.Add(newItem);
				//move and size the new item
				RectTransform rectTransform = newItem.GetComponent<RectTransform> ();

				float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
				float y = containerRectTransform.rect.height / 2 - height * j;
				rectTransform.offsetMin = new Vector2 (x, y);

				x = rectTransform.offsetMin.x + width;
				y = rectTransform.offsetMin.y + height;
				rectTransform.offsetMax = new Vector2 (x, y);
			}
		}

		public List<GameObject> GetItems(){
			return items;
		}

	}
}
