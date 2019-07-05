using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Auroraland
{
    public class T360Control : MonoBehaviour
    {
        // 360 movie theater
        public GameObject T360RoomHolderGo;
        public GameObject T360VideoPlayerGo;
        public VideoPlayer T360VideoPlayer;
        public GameObject T360LightBeamGo;
        public MeshRenderer[] T360LightBeamMeshRenderers;
        public bool FinishedLightBeamOpen = false;
        public bool FinishedLightBeamClose = false;
        public bool FinishedRoomOpen = false;
        public bool FinishedRoomClose = false;
        public bool needChangeStatus;

        // play multiple movies
        public List<string> T360MoviesNameToLocalPath;
        private string filesLocalPath;
        int currentMovieIdx;

        void Awake()
        {
            T360VideoPlayerGo = GameObject.Find("360Video_Video Player");
            T360RoomHolderGo = GameObject.Find("Room_Holder");
            T360LightBeamGo = GameObject.Find("lightbeam");
            needChangeStatus = true;

            if (T360VideoPlayerGo != null && T360LightBeamGo != null)
            {
                T360VideoPlayer = T360VideoPlayerGo.GetComponent<VideoPlayer>();
            }
            currentMovieIdx = 0;
            filesLocalPath = @"d:\Auroraland\media\360";
            T360MoviesNameToLocalPath = new List<string>();
        }

        void Start()
        {
            T360TheaterGetMovieList();
        }

        private void OnEnable()
        {
            T360TheaterVoiceControl.OnPlayT360Movie += PlayT360MovieHandler;
            T360TheaterVoiceControl.OnStopT360Movie += StopT360MovieHandler;
            T360TheaterVoiceControl.OnPauseT360Movie += PauseT360MovieHandler;
            T360TheaterVoiceControl.OnNextT360Movie += NextT360MovieHandler;
            T360TheaterVoiceControl.OnPreviousT360Movie += PreviousT360MovieHandler;
            T360TheaterVoiceControl.OnAdjustT360MovieVolume += AdjustT360MovieVolumeHandler;
        }

        private void OnDisable()
        {
            T360TheaterVoiceControl.OnPlayT360Movie -= PlayT360MovieHandler;
            T360TheaterVoiceControl.OnStopT360Movie -= StopT360MovieHandler;
            T360TheaterVoiceControl.OnPauseT360Movie -= PauseT360MovieHandler;
            T360TheaterVoiceControl.OnNextT360Movie -= NextT360MovieHandler;
            T360TheaterVoiceControl.OnPreviousT360Movie -= PreviousT360MovieHandler;
            T360TheaterVoiceControl.OnAdjustT360MovieVolume -= AdjustT360MovieVolumeHandler;
        }

        void PlayT360MovieHandler() {
            
            StartCoroutine(PlayT360Helper());
        }

        void StopT360MovieHandler() {
            StartCoroutine(StopT360Helper());
        }

        void PauseT360MovieHandler() {
            T360VideoPlayer.Pause();
        }

        void NextT360MovieHandler() {
            //needChangeStatus = false;
            if (currentMovieIdx == T360MoviesNameToLocalPath.Count - 1)
                currentMovieIdx = 0;
            else currentMovieIdx++;
            PlayT360MovieHandler();
        }

        void PreviousT360MovieHandler() {
            //needChangeStatus = false;
            if (currentMovieIdx == 0)
                currentMovieIdx = T360MoviesNameToLocalPath.Count - 1;
            else currentMovieIdx--;
            PlayT360MovieHandler();
        }

        void AdjustT360MovieVolumeHandler(float delta) {
            if (delta > 0.0f)
                SetLouderVolume(T360VideoPlayer);
            else
                SetLowerVolume(T360VideoPlayer);
        }

        // BELOW are helper methods
        IEnumerator PlayT360Helper()
        {
            string videoURL = T360MoviesNameToLocalPath[currentMovieIdx];
            if (File.Exists(videoURL) && T360VideoPlayer != null) {
                if (needChangeStatus)
                {
                    T360LightBeamControlHelper(true);
                    T360RoomControl(false);
                    T360VideoPlayerControl();

                    yield return new WaitForSeconds(1f);

                    T360LightBeamControlHelper(false);
                    needChangeStatus = false;
                }
                if (T360VideoPlayer.url != videoURL)
                    T360VideoPlayer.url = videoURL;
                T360VideoPlayer.isLooping = true;
                T360VideoPlayer.Play();
            }
            else if (!File.Exists(videoURL))
            {
                Debug.Log("videoURL: " + videoURL);
            }
            else if (T360VideoPlayer == null)
            {
                Debug.Log("video player is null");
            }
        }

        IEnumerator StopT360Helper()
        {
            T360VideoPlayer.Stop();
            T360VideoPlayerControl();
            T360LightBeamControlHelper(true);
            T360RoomControl(true);

            yield return new WaitForSeconds(1f);

            T360LightBeamControlHelper(false);
            needChangeStatus = true;
        }

        // 360 Video Player control
        void T360VideoPlayerControl()
        {
            T360VideoPlayer.enabled = !T360VideoPlayer.enabled;
        }

        // 360 light beam (mesh) control helper
        void T360LightBeamControlHelper(bool lightBeamOn)
        {
            T360LightBeamMeshRenderers = T360LightBeamGo.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer lbr in T360LightBeamMeshRenderers)
                lbr.enabled = lightBeamOn;

            if (lightBeamOn)
            {
                FinishedLightBeamOpen = true;
                FinishedLightBeamClose = false;
            }
            else
            {
                FinishedLightBeamClose = true;
                FinishedLightBeamOpen = false;
            }
        }

        // 360 room component control
        void T360RoomControl(bool needActive)
        {
            SetActivateChildren(T360RoomHolderGo, needActive);
        }

        // 360 room component control helper
        void SetActivateChildren(GameObject g, bool needActive)
        {
            if (g.name == "Room")
            {
                g.SetActive(needActive);
            }
            foreach (Transform child in g.transform)
            {
                child.gameObject.SetActive(needActive);
            }
            if (!needActive)
            {
                FinishedRoomClose = true;
                FinishedRoomOpen = false;
            }
            else
            {
                FinishedRoomClose = false;
                FinishedRoomOpen = true;
            }
        }

        void T360TheaterGetMovieList()
        {
            DirectoryInfo t360TheaterLocalDir = new DirectoryInfo(filesLocalPath);
            FileInfo[] info = t360TheaterLocalDir.GetFiles("*.mp4");
            int idx = 0;
            foreach (FileInfo f in info)
            {
                T360MoviesNameToLocalPath.Add(f.FullName);
                idx++;
            }
        }

        public void SetLouderVolume(VideoPlayer vp)
        {
            if (vp != null && vp.canSetDirectAudioVolume == true)
            {
                var curVolume = vp.GetDirectAudioVolume(0);
                if (curVolume <= 0.8f)
                {
                    curVolume += 0.1f;
                }
                else
                {
                    curVolume = 1.0f;
                }
                vp.SetDirectAudioVolume(0, curVolume);
            }
        }

        public void SetLowerVolume(VideoPlayer vp)
        {
            if (vp != null && vp.canSetDirectAudioVolume == true)
            {
                var curVolume = vp.GetDirectAudioVolume(0);
                if (curVolume >= 0.2f)
                {
                    curVolume -= 0.1f;
                }
                else
                {
                    curVolume = 0.0f;
                }
                vp.SetDirectAudioVolume(0, curVolume);
            }
        }

    }
}