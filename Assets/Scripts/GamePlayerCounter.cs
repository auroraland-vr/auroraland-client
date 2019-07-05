using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland{


	public class GamePlayerCounter : MonoBehaviour {

		public int PlayerCount { private set; get;} // the number of players enter the GameArea
		private NetObject game;
		private Object playerCountLocker;
		// Use this for initialization
		void Start () {
			PlayerCount = 0;
			game = GetComponent<NetObject> ();
		}

		void OnTriggerEnter(Collider collider){
			if (collider.gameObject.tag == "Actor" || collider.gameObject.tag == "Player") {
				lock (playerCountLocker) {
					PlayerCount++;
				}
			}

		}

		void OnTriggerExit(Collider collider){
			if (collider.gameObject.tag == "Actor" || collider.gameObject.tag == "Player") {
				lock (playerCountLocker) {
					PlayerCount--;
				}
			}
		}
		/*
		IEnumerator IncreasePlayerCount(){
			
		
		}

		IEnumerator DecreasePlayerCount(){
		}*/

	}


}