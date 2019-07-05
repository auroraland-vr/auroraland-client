using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using System.IO;

namespace Auroraland {

    public class TherapyVideoControl : MonoBehaviour {

        public VideoPlayer therapyVideoPlayer;
        public GameObject therapyScreenGo;
        public GameObject therapyScreenMeshGo;
        public MeshRenderer[] therapyScreenMeshRenderers;
        public GameObject directLightGo;
        public Light directLight;

        private string filesLocalPath;

        void Awake()
        {
            therapyScreenGo = GameObject.Find("TherapyRoom_Video Player");
            therapyScreenMeshGo = GameObject.Find("TherapyRoom_MovieScreen");
            directLightGo = GameObject.Find("Directional Light");
            directLight = directLightGo.GetComponent<Light>();
            if (therapyScreenGo != null)
            {
                therapyVideoPlayer = therapyScreenGo.GetComponent<VideoPlayer>();
            }
            filesLocalPath = @"d:\Auroraland\media\treatment\video";
        }

        private void OnEnable()
        {
            TherapyVoiceControl.OnPlayVideo += PlayVideoHandler;
            TherapyVoiceControl.OnStopVideo += StopVideoHandler;
        }

        private void OnDisable()
        {
            TherapyVoiceControl.OnPlayVideo -= PlayVideoHandler;
            TherapyVoiceControl.OnStopVideo -= StopVideoHandler;
        }

        void PlayVideoHandler(string videoName) {
            Debug.Log("On Play Video: " + videoName);
            if (videoName == "night")
            {
                FindVideoAndPlay("night");
                directLight.enabled = false;
                return;
            }
            if (videoName == "flowers")
            {
                FindVideoAndPlay("flowers");
                directLight.enabled = true;
                return;
            }
            if (videoName == "sunset") {
                FindVideoAndPlay("sunset");
                directLight.enabled = true;
                return;
            }
            FindVideoAndPlay("waves");
        }

        void FindVideoAndPlay(string videoName)
        {
            StopVideoHandler();
            string videoURL = filesLocalPath + "\\" + "savedVideoFile_" + videoName + ".mp4";
            if (File.Exists(videoURL) && therapyVideoPlayer != null)
            {
                therapyVideoPlayer.url = videoURL;
                therapyVideoPlayer.isLooping = true;
                TherapyScreenMesh();
                TherapyVideoPlayerControl();
                therapyVideoPlayer.Play();
            } else if (!File.Exists(videoURL)) {
                Debug.Log("videoURL: " + videoURL);
            } else if (therapyVideoPlayer == null) {
                Debug.Log("video player is null");
            }
        }

        void StopVideoHandler() {
            Debug.Log("On stop video");
            if (therapyVideoPlayer != null && therapyVideoPlayer.isPlaying)
            {
                therapyVideoPlayer.Stop();
                TherapyVideoPlayerControl();
                if (directLight.enabled == false) {
                    directLight.enabled = true;
                }
                TherapyScreenMesh();
            }
        }

        // MESH
        // therapy room Video Player control
        void TherapyVideoPlayerControl()
        {
            therapyVideoPlayer.enabled = !therapyVideoPlayer.enabled;
        }

        // therapy room mesh control
        void TherapyScreenMesh()
        {
            therapyScreenMeshRenderers = therapyScreenMeshGo.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer smr in therapyScreenMeshRenderers)
                smr.enabled = !smr.enabled;
        }
    }
}
