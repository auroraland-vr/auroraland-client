using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Nakama;

namespace Auroraland{

	public struct SelfMetaData{
		public float height;
		public string heightUnit;
	}
	public class UserProfileMenu : MonoBehaviour {

		public bool IsEditor;
		[Header ("Display UI")]
		public Text UserEmail;
		public RawImage ProfilePicture;
		public Texture DefaultProfile;
		public Text DisplayName;
		public Text UserHeight;
		public Button EditButton;
		[Header ("Editor UI")]
		public InputField DisplayNameInput;
		public InputField HeightInput;
		public InputField ProfileUrlInput;
		public Toggle FeetToggle;
		public Toggle CmToggle;
		public Button SaveButton;

		private string profilePictureUrl;
		private string displayName;
		private float height;
		private string heightUnit;
		// Use this for initialization
		void OnEnable () {
            NKController.Instance.LoadSelfInfoSuccess += OnLoadSelfInfoSuccess;
            NKController.Instance.LoadSelfInfo();
		}

		void OnDisable(){
            NKController.Instance.LoadSelfInfoSuccess -= OnLoadSelfInfoSuccess;
		}

		public void OnLoadSelfInfoSuccess(object sender, NKSingleArg<INSelf> selfArg){
            INSelf self = selfArg.value;
			displayName = self.Fullname;
			profilePictureUrl = self.AvatarUrl;
			SelfMetaData meta = JsonUtility.FromJson<SelfMetaData> (self.Metadata);
			height = meta.height;
			heightUnit = meta.heightUnit;

			//set ui
			UserEmail.text = self.Email;

			if (IsEditor) {
				DisplayNameInput.text =  (string.IsNullOrEmpty(displayName))?"Guest "+ self.Id: displayName;
				ProfileUrlInput.text = profilePictureUrl;
				HeightInput.text = (height == 0) ? "170" : height.ToString ();
				FeetToggle.isOn = (heightUnit == "feet");
				CmToggle.isOn = (heightUnit == "cm");
			} 
			else {
				DisplayName.text = (string.IsNullOrEmpty(displayName))?"Guest "+ self.Id: displayName;
				UserHeight.text = (height == 0) ? "170 cm" : height.ToString () + heightUnit;
			}

			StartCoroutine (ImageLoader.LoadImage(ProfilePicture, DefaultProfile, profilePictureUrl));

		}

		public void SaveUserProfile(){
			displayName = DisplayNameInput.text;
			height = float.Parse(HeightInput.text);
			heightUnit = (FeetToggle.isOn) ? "feet" : "cm";
			profilePictureUrl = ProfileUrlInput.text;

			NKController.Instance.UpdateSelfInfo (displayName, profilePictureUrl, height, heightUnit);
		}
		/*
		private void SetAvatarEyePosition(){
			float playerHeight = (heightUnit == "feet")?FeetToMeter(height/100f):height/100f;
			PlayerCameraRoot.localPosition = new Vector3 (0f,  playerHeight , 0.114f);
		}*/

		private float FeetToMeter(float feet){
			float meter = feet / 3.2808399f;
			return meter;
		}
	}
}