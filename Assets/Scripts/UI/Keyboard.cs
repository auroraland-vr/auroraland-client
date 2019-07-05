using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Auroraland{
	public class Keyboard : MonoBehaviour {

		private InputField displayInput;
		private InputField inputfield;
		// Use this for initialization
		void Start () {
			inputfield = GetComponentInChildren<InputField> ();
		}

		// Update is called once per frame
		void Update () {
			
		}
		public void SetDisplayInputField(InputField display){
			displayInput = display;
			inputfield.text = displayInput.text;
			inputfield.ActivateInputField ();
			displayInput.ActivateInputField ();
		}
		public void ClickKey(string character)
		{
			inputfield.text += character;
			Debug.Log (character);
		}

		public void Backspace()
		{
			if (inputfield.text.Length > 0)
			{
				inputfield.text = inputfield.text.Substring(0, inputfield.text.Length - 1);
			}
		}

		public void Enter()
		{
			Debug.Log (inputfield.text);
			inputfield.text = "";
			inputfield.DeactivateInputField ();
			displayInput.DeactivateInputField ();
		}

	}
}
