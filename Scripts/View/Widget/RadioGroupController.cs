using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla {
	public class RadioGroupController : MonoBehaviour {

		public List<RadioButton> radioButtons;
		private int prevSelected = 0;

		public RadioGroupController(){
			radioButtons = new List<RadioButton>();
		}

		public void AddButton(RadioButton rb){
			rb.Deselect();
			radioButtons.Add(rb);
		}

		public void SetButtons(List<GameObject> objects)
		{
			foreach (GameObject go in objects) 
			{
				radioButtons.Add(go.GetComponent<RadioButton>());
			}
		}

		public void UnselectAll()
		{
			radioButtons.ForEach((item) => 
				{
					item.Deselect();
				});
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

		public void SelectItem(int pPosition)
		{
			if (prevSelected >= 0) {
				radioButtons [prevSelected].Deselect ();
			}
			radioButtons [pPosition].Select ();
			prevSelected = pPosition;
		}
	}
}