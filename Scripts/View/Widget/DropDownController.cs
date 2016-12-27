using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Xsolla {
	public class DropDownController : MonoBehaviour {

		public Text dropDownText;
		public Button dropDownButton;
		public GameObject scrollContainer;
		public Transform dropDownList;
		public GameObject dropDownItemPrefab;
		public Action<int, string> OnItemSelected;

		private Transform parentTransform;
		private bool isParentChanged = false;
		private List<string> items;
		private int currentSelected = -1;

		public List<string> getItems()
		{
			return items;
		}

		void Start(){
			if(parentTransform == null)
				parentTransform = gameObject.transform.parent.parent;
			dropDownButton.onClick.AddListener (delegate {
				if (!isParentChanged) {
					scrollContainer.transform.SetParent(parentTransform);
					isParentChanged = true;
				}
			});
		}

		public void SetParentForScroll(Transform newParent){
			parentTransform = newParent;
		}

		public void SetData(List<string> items){
			this.items = items;
			for (int i = 0; i < items.Count; i++) {
				string name = items[i];
				GameObject itemInstance = Instantiate(dropDownItemPrefab) as GameObject;
				Button button = itemInstance.GetComponent<Button>();
				if(button == null)
					button = itemInstance.AddComponent<Button>();
				Text text = itemInstance.GetComponentInChildren<Text>();

				button.onClick.AddListener(delegate {
					SelectItem(items.IndexOf(name), name);
				});
				text.text = name;
				itemInstance.transform.SetParent(dropDownList);
			}

			SelectItem(0, items[0]);
		}

		public void SetData(List<string> items, string title){
			this.items = items;
			for (int i = 0; i < items.Count; i++) {
				string name = items[i];
				GameObject itemInstance = Instantiate(dropDownItemPrefab) as GameObject;
				Button button = itemInstance.GetComponent<Button>();
				if(button == null)
					button = itemInstance.AddComponent<Button>();
				Text text = itemInstance.GetComponentInChildren<Text>();
				
				button.onClick.AddListener(delegate {
					SelectItem(items.IndexOf(name), name);
				});
				text.text = name;
				itemInstance.transform.SetParent(dropDownList);
			}
			
			dropDownText.text = title;
		}

		public void SelectItem(int position, string name){
			currentSelected = position;
			dropDownText.text = name;
			if (OnItemSelected != null)
				OnItemSelected (position, name);
			scrollContainer.SetActive (false);
		}

		public int GetSelected(){
			return currentSelected;
		}
	}
}
