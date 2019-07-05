using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland{
	public class InstructionMenu : MonoBehaviour {
		public GameObject ViveInstruction;
		public GameObject OculusInstruction;
		// Use this for initialization
		void OnEnable () {
            SDK_SetupMode.SwitchToOculus += SwitchOculusInstruction;
            SDK_SetupMode.SwitchToSteamVR += SwitchSteamInstruction;
		}

		void OnDisable(){
            SDK_SetupMode.SwitchToOculus -= SwitchOculusInstruction;
            SDK_SetupMode.SwitchToSteamVR -= SwitchSteamInstruction;
        }

        void SwitchOculusInstruction() {
            ViveInstruction.SetActive(false);
            OculusInstruction.SetActive(true);
        }
		void SwitchSteamInstruction(){
			ViveInstruction.SetActive (true);
			OculusInstruction.SetActive (false);
		}

	}
}
