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

		public void SelectItem(int position)
		{
			if (prevSelected >= 0) {
				radioButtons [prevSelected].Deselect ();
			}
			radioButtons [position].Select ();
			prevSelected = position;
			isUpdated = false;
		}

		public void SelectItem(RadioButton pBtn)
		{
			foreach(RadioButton btn in radioButtons)
			{
				btn.Deselect();
			}
			pBtn.Select();
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

//		public void SelectItem(RadioButton radioButton)
//		{
//			if(prevSelected != null)
//				radioButton.Deselect ();
//			radioButton.Select ();
//			prevSelected = radioButton;
//		}

	}
}