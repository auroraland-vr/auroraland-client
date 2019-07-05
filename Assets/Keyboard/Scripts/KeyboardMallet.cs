using UnityEngine;
using System.Collections;
using VRTK;

namespace Normal.UI {
    [RequireComponent(typeof(Rigidbody))]
    public class KeyboardMallet : MonoBehaviour {

		  
		public float HapticStrength;
		public GameObject Controller;
		private VRTK_ControllerReference controllerReference;
        [SerializeField]
        private Transform _head;

        [HideInInspector]
        public  Vector3    malletHeadPosition;
        private Vector3   _newMalletHeadPosition;

        // Internal
        [HideInInspector]
        public Keyboard _keyboard;

        void Awake() {
            // Configure the rigidbody
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity  = false;
            rigidbody.isKinematic = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // Mallet collision. Check if we've hit a keyboard key or not.
        void OnTriggerEnter(Collider other) {
            if (_keyboard == null) {
                Debug.LogError("Huh, I, this keyboard mallet, have struck something. However, I am not the child of a keyboard. A lost soul. It pains me to ignore this collision event. What does it mean? Who was it meant for? Unfortunately I am given no choice.");
                return;
            }

			Rigidbody keyRigidbody = other.attachedRigidbody;
            if (keyRigidbody == null)
                return;
            KeyboardKey key = keyRigidbody.GetComponent<KeyboardKey>();
            if (key != null) {
                if (key.IsMalletHeadInFrontOfKey(this)) { // so you can't hit the key from underneath!
                    _keyboard._MalletStruckKeyboardKey(this, key);
					           
                }
            } else {
                //Trigger haptic pulse (originally I wanted to limit this to just key strikes, but I guess haptics make sense if you hit anything...)
				TriggerHapticPulse();
            }
        }

        void Update() {
            // I want the value from the previous frame.
            malletHeadPosition = _newMalletHeadPosition;
			_newMalletHeadPosition = transform.position;


        }
			
		// Haptic pulse
		void TriggerHapticPulse() {
			StartCoroutine(HapticPulse());
		}

		IEnumerator HapticPulse() {

			controllerReference = VRTK_ControllerReference.GetControllerReference(Controller);
			/*
			if (!VRTK_ControllerReference.IsValid(controllerReference)) {
				yield break;
			}*/
			Debug.Log ("called haptic");
			VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, HapticStrength, 2f, 0.1f);
			yield return new WaitForEndOfFrame();

		}

    }
}
