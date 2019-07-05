using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Auroraland.UI{
	public class VRKeyboardDisplay : MonoBehaviour {
		
			public InputfieldGroup inputGroup;
			private Text text;

			[SerializeField]
			private VRKeyboard keyboard;
			public  VRKeyboard  Keyboard { get { return keyboard; } set { SetKeyboard(value); } }

			void Awake() {
				StartObservingKeyboard(keyboard);
			}

			void OnDestroy() {
				StopObservingKeyboard(keyboard);
			}

			void SetKeyboard(VRKeyboard keyboard) {
				if (keyboard == this.keyboard)
					return;

				StopObservingKeyboard(keyboard);
				StartObservingKeyboard(keyboard);

				this.keyboard = keyboard;
			}

			void StartObservingKeyboard(VRKeyboard keyboard) {
				if (keyboard == null)
					return;

				keyboard.keyPressed += KeyPressed;
			}

			void StopObservingKeyboard(VRKeyboard keyboard) {
				if (keyboard == null)
					return;

				keyboard.keyPressed -= KeyPressed;
			}

			void KeyPressed(VRKeyboard keyboard, string keyPress) {
			
				string input = (inputGroup) ? inputGroup.Current.text : text.text;

				if (keyPress == "\b") {
					// Backspace
					if (input.Length > 0)
						input = input.Remove (input.Length - 1);
				} else {
					// Regular key press
					input += keyPress;
				}

				if (inputGroup == null) {
					text.text = input;
				} else {
					inputGroup.Current.text = input;
					inputGroup.Current.MoveTextEnd (false); //always set caret at the end, doesn't work properly if you inset text 
				     //TODO modify caret position 
				    //caretPosition = input.Length;// reset caret position
				}
			}

	}
}
