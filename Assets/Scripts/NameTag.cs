using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Auroraland{
	public class NameTag : MonoBehaviour {

		public Text DisplayName;
		// Use this for initialization

		public void SetNameTagByUserId(string userId, Vector3 position){
            var name = NKController.Instance.GetSelf().Fullname;
			SetNameTag (name, position, NKController.Instance.GetLocalUserId() == userId);
		}

		public void SetNameTag(string displayName, Vector3 position, bool isLocal){
			DisplayName.text = displayName;
			transform.localPosition = position;
			SetGameObjectName (displayName, isLocal);
		}

		private void SetGameObjectName(string name, bool isLocal){
			if (isLocal) {
				transform.parent.parent.gameObject.name = "[Player_" + name + "]"; //change game object name in hiearchy [Player_Space]
			} 
			else {
				transform.parent.gameObject.name = "[Actor_" + name + "]";
			}

		}
		private void SetGameObjectName(string name, string userId){
			bool isLocal = (NKController.Instance.GetLocalUserId() == userId)? true: false;
			if (isLocal) {
				transform.root.gameObject.name = "[Player_" + name + "]";
			} 
			else {
				transform.parent.gameObject.name = "[Actor_" + name + "]";
			}

		}
	}
}
