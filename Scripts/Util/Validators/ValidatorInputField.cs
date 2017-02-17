using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Xsolla {
	 public class ValidatorInputField : MonoBehaviour {
		
		public InputField _input;
		public GameObject error;
		public Text errorText;
//		private EventTrigger _eventTrigger;
		public Graphic[] imagesToColor;
		public ValidatorFactory.ValidatorType[] types;

		protected int _limit;
		protected bool _isValid = false;
		private bool isActive = false;
		private bool isErrorShowed = false;

		private List<IValidator> validators;
		private string _errorMsg = "Invalid";
		private bool isOn = false;

		public override string ToString ()
	 	{
	 		return string.Format ("[ValidatorInputField: _input={0}, error={1}, errorText={2}, imagesToColor={3}, types={4}, _limit={5}, _isValid={6}, isActive={7}, isErrorShowed={8}, validators={9}, _errorMsg={10}, isOn={11}]", _input, error, errorText, imagesToColor, types, _limit, _isValid, isActive, isErrorShowed, validators, _errorMsg, isOn);
	 	}

		void Awake(){
			if(error != null)
				error.gameObject.SetActive (false);
			
			InitEventTrigger ();
		}

		void Start(){
			if (types != null && types.Length > 0) {
				foreach (var type in types) {
					validators.Add(ValidatorFactory.GetByType(type));
				}
				SetErrorMsg(validators[0].GetErrorMsg());
			}
			_input.onValueChanged.AddListener ((s) => Validate (s));
		}

		private void InitEventTrigger(){
			EventTrigger _eventTrigger = gameObject.AddComponent<EventTrigger>();

			EventTrigger.Entry entryS = new EventTrigger.Entry();
			entryS.eventID =  EventTriggerType.Select;
			entryS.callback = new EventTrigger.TriggerEvent();
			UnityEngine.Events.UnityAction<BaseEventData> callback =
				new UnityEngine.Events.UnityAction<BaseEventData>(OnSelected);
			entryS.callback.AddListener (callback);
			
			EventTrigger.Entry entryD = new EventTrigger.Entry();
			entryD.eventID =  EventTriggerType.Deselect;
			entryD.callback = new EventTrigger.TriggerEvent();
			UnityEngine.Events.UnityAction<BaseEventData> callback1 =
				new UnityEngine.Events.UnityAction<BaseEventData>(OnDeselected);
			entryD.callback.AddListener (callback1);

			List<EventTrigger.Entry> delegates = new List<EventTrigger.Entry>();
			//TODO Problem for unity compability 500 - 510
			delegates.Add (entryS);
			delegates.Add (entryD);
			_eventTrigger.triggers = delegates;
		}

		public ValidatorInputField(){
			validators = new List<IValidator> ();
		}

		public void AddValidator(IValidator validator){
			validators.Add(validator);
		}

		public void SetErrorMsg(string msg)
		{
			_errorMsg = msg;
		}
		
		private void OnSelected(UnityEngine.EventSystems.BaseEventData baseEvent)
		{
			ShowErrorText (!_isValid);
		}
		
		private void OnDeselected(UnityEngine.EventSystems.BaseEventData baseEvent)
		{
			ShowErrorText (false);
			if (!isOn)
				isOn = true;
			UpdateBorderColor ();
		}
		
		protected void ShowErrorText(bool b){			
			if (isOn && error != null && errorText != null) { // && isActive
				errorText.text = _errorMsg;
				error.gameObject.SetActive (b);
				isErrorShowed = b;
			}
		}

		protected void UpdateBorderColor(){
			if (isOn) {
				Color color;
				if (_isValid) {
					color = StyleManager.Instance.GetColor (StyleManager.BaseColor.bg_ok);
				} else {
					color = StyleManager.Instance.GetColor (StyleManager.BaseColor.bg_error);
				}
				foreach(var item in imagesToColor){
					item.color = color;
				}
			}
		}
		
		
		public bool IsValid() {
			if (isOn) {
				return _isValid;
			} else {
				isOn = true;
				return Validate("");
			}
		}

		public bool Validate(string s){
			_isValid = true;
			foreach (IValidator v in validators) {
				if (!v.Validate (s)) {
					SetErrorMsg (v.GetErrorMsg ());
					_isValid = false;
				}
			}
			ShowErrorText (!_isValid);
			UpdateBorderColor ();
			return _isValid;
		}

	}
}
