using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Auroraland.UI;

namespace Auroraland{
	
	[RequireComponent(typeof(InputfieldGroup))]
	/*This class is for preventing player moves when user is typing*/
	public class KeyInputSwitcher : MonoBehaviour {
		public AvatarController AvatarController;
		private InputfieldGroup inputGroup;
		private bool notTyping = false;
		void Awake(){
			inputGroup = GetComponent<InputfieldGroup> ();
		}
		void Start(){

			foreach (var input in inputGroup.InputFields) {
				input.onEndEdit.AddListener( delegate {
					UpdatePlayerControl();
				});
				input.onValueChanged.AddListener(delegate {
					UpdatePlayerControl();
				});
			}
		}
		void OnDestroy(){
			foreach (var input in inputGroup.InputFields) {
				input.onEndEdit.RemoveListener( delegate {
					UpdatePlayerControl();
				});
				input.onValueChanged.RemoveListener(delegate {
					UpdatePlayerControl();
				});
			}
		}
			
		private void UpdatePlayerControl(){
			bool isTyping = false;
			foreach (var input in inputGroup.InputFields) {
				if (input.isFocused) {
					isTyping = true;
					break;
				}
			}

			//AvatarController.AllowMovement = !isTyping;
		}
	}
}
