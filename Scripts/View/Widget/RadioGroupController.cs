using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla {
	public class RadioGroupController : MonoBehaviour {

		public List<RadioButton> radioButtons;
		private int prevSelected = 0;
		private bool isUpdated = false;

		public RadioGroupController(){
			radioButtons = new List<RadioButton>();
		}

		public void AddButton(RadioButton rb){
			radioButtons.Add(rb);
		}

		public void SetButtons(List<GameObject> objects)
		{
			foreach (GameObject go in objects) 
			{
				radioButtons.Add(go.GetComponent<RadioButton>());
			}
		}

		public void SelectItem(RadioButton.RadioType pType)
		{
			foreach(RadioButton btn in radioButtons)
			{
				if (btn.getType() != pType)
					btn.Deselect();
				else
					btn.Select();
			}
		}

		void Update() {
			if (!isUpdated) {
				foreach (var rb in radioButtons) {
					rb.Deselect ();
				}
				radioButtons [prevSelected].Select ();
				isUpdated = true;
			}
		}
	}
}