using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland.UI{
	public class VRKeyboard : MonoBehaviour {
		public delegate void KeyPressedDelegate(VRKeyboard keyboard, string keyPress);
		public event KeyPressedDelegate keyPressed;

		[SerializeField]
		private GameObject Letters;

		[SerializeField]
		private GameObject Numbers;

		[SerializeField]
		private VRKeyboardKey LayoutSwapKey;

		private VRKeyboardKey[] Keys;

		private bool shift = false;
		public  bool  Shift { get { return shift; } set { SetShift(value); } }

		public enum KeyLayout {
			Letters,
			Numbers
		};

		private KeyLayout layout = KeyLayout.Letters;
		public  KeyLayout  Layout { get { return layout; } set { SetLayout(value); } }

		void Awake() {
			Keys = GetComponentsInChildren<VRKeyboardKey>(true);


			foreach (VRKeyboardKey key in Keys) {
				key.keyboard = this;
			}
		}
			
		public void UseKeyboardKey(VRKeyboardKey key) {
			// Did we hit the key for another keyboard?
			if (key.keyboard != this)
				return;

			// Trigger key press animation
			key.KeyPressed();

			// Fire key press event
			if (keyPressed != null) {
				string keyPress = key.GetCharacter();

				bool shouldFireKeyPressEvent = true;
				//Debug.Log ("key press"+ keyPress);
				if (keyPress == "\\s") {
					// Shift
					Shift = !Shift;
					shouldFireKeyPressEvent = false;
				} else if (keyPress == "\\l") {
					// Layout swap
					if (Layout == KeyLayout.Letters)
						Layout = KeyLayout.Numbers;
					else if (Layout == KeyLayout.Numbers)
						Layout = KeyLayout.Letters;

					shouldFireKeyPressEvent = false;
				} else if (keyPress == "\\b") {
					// Backspace
					keyPress = "\b";
				} else {
					// Turn off shift after typing a letter
					/*
                    if (shift && layout == Layout.Letters)
                        shift = false;*/
				}

				if (shouldFireKeyPressEvent)
					keyPressed(this, keyPress);
			}
		}

		void SetShift(bool shift) {
			if (shift == this.shift)
				return;

			foreach (VRKeyboardKey key in Keys)
				key.Shift = shift;

			this.shift = shift;
		}

		void SetLayout(KeyLayout layout) {
			if (layout == this.layout)
				return;

			Shift = false;

			if (layout == KeyLayout.Letters) {
				// Swap layouts
				Letters.SetActive(true);
				Numbers.SetActive(false);

				// Update layout swap key
				LayoutSwapKey.DisplayCharacter      = "123";
				LayoutSwapKey.ShiftDisplayCharacter = "123";
				LayoutSwapKey.RefreshDisplayCharacter();
			} else if (layout == KeyLayout.Numbers) {
				// Swap layouts
				Letters.SetActive(false);
				Numbers.SetActive(true);

				// Update layout swap key
				LayoutSwapKey.DisplayCharacter      = "abc";
				LayoutSwapKey.ShiftDisplayCharacter = "abc";
				LayoutSwapKey.RefreshDisplayCharacter();
			}

			this.layout = layout;
		}
	}
}
