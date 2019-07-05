using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using Normal.UI;

namespace Auroraland.UI{
	public class VRKeyboardKey : VRTK_InteractableObject{

		[Header("Key Setting")]
		public string Character = "a";

		// These are overrides in case you need something different.
		public string DisplayCharacter = null;
		public string ShiftCharacter = null;
		public string ShiftDisplayCharacter = null;

		private bool shift = false;
		public  bool  Shift { get { return shift; } set { SetShift(value); } }

		[SerializeField]
		private Text text;

		private float position       = 0.0f;
		private float targetPosition = 0.0f;

		[SerializeField]
		private AudioSource audioSource;

		// Internal
		[HideInInspector]
		public VRKeyboard keyboard;


		public override void StartUsing(VRTK_InteractUse usingObject)
		{
			base.StartUsing (usingObject);
			keyboard.UseKeyboardKey (this);
			//Debug.Log ("use key" + Character);
			//TriggerHapticPulse ();

		}

		public override void StopUsing(VRTK_InteractUse usingObject)
		{
			base.StopUsing(usingObject);
		}
			
		/*
		// Haptic pulse
		private void TriggerHapticPulse() {
			StartCoroutine(HapticPulse());
		}

		IEnumerator HapticPulse() {

			controllerReference = VRTK_ControllerReference.GetControllerReference(Controller);

				if (!VRTK_ControllerReference.IsValid(controllerReference)) {
					yield break;
				}
			Debug.Log ("called haptic");
			VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, HapticStrength, 2f, 0.1f);
			yield return new WaitForEndOfFrame();

		}*/


		public void KeyPressed() {
			
			position = -0.1f;

			if (audioSource != null) {
				if (audioSource.isPlaying)
					audioSource.Stop();

				float scalePitch = 1.0f/(keyboard.transform.lossyScale.x + 0.2f);
				float pitchVariance = Random.Range(0.95f, 1.05f);
				audioSource.pitch = scalePitch * pitchVariance;
				audioSource.Play();
			}
		}

		void SetShift(bool shift) {
			if (shift == this.shift)
				return;

			this.shift = shift;

			RefreshDisplayCharacter();
		}

		// Key animation
		protected override void Update() {

			base.Update ();
			// Animate bounce
			position = Mathf.Lerp(position, targetPosition, Time.deltaTime * 20.0f);

			// Set position
			Vector3 localPosition = transform.localPosition;
			localPosition.y = position;
			transform.localPosition = localPosition;
		}

		public void RefreshDisplayCharacter() {
			text.text = GetDisplayCharacter();
		}

		// Helper functions
		string GetDisplayCharacter() {
			// Start with the character
			string dc = Character;
			if (dc == null)
				dc = "";

			// If we've got a display character, swap for that.
			if (DisplayCharacter != null && DisplayCharacter != "")
				dc = DisplayCharacter;

			// If we're in shift mode, check our shift overrides.
			if (shift) {
				if (ShiftDisplayCharacter != null && ShiftDisplayCharacter != "")
					dc = ShiftDisplayCharacter;
				else if (ShiftCharacter != null && ShiftCharacter != "")
					dc = ShiftCharacter;
				else
					dc = dc.ToUpper();
			}

			return dc;
		}

		public string GetCharacter() {
			if (Shift) {
				if (ShiftCharacter != null && ShiftCharacter != "")
					return ShiftCharacter;
				else
					return Character.ToUpper();
			}

			return Character;
		}
	}
}
