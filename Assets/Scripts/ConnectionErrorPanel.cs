using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Auroraland{
	public class ConnectionErrorPanel : MonoBehaviour {
		
		private static ConnectionErrorPanel  instance;
		public static ConnectionErrorPanel Instance{
			get{ return instance;}
		}
		void Awake(){
			// singleton pattern
			if (instance != null && instance != this) {
				Destroy (this.gameObject);
			} else {
				instance = this;
			}
		}

		public void SetPanelActive(bool isActive){
			gameObject.SetActive (isActive);
		
		}

		public void ToHomeScene(){
			SceneManager.LoadScene ("Home");
			gameObject.SetActive (false);
		
		} 

	}

}