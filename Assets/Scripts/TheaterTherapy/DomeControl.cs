using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using System.IO;

namespace Auroraland
{
    public class DomeControl : MonoBehaviour {
        // Dome Screen
        public GameObject domeScreenGo;
        public GameObject domeSphereGo;
        public VideoPlayer domeVideoPlayer;
        public GameObject reflectionProbe;
        public MeshRenderer[] domeScreenMeshRenderers;
        public MeshCollider[] domeScreenMeshColliders;
        // Light
        public Light[] TheatreLights;
        // Material emMat;
        public Material emMatOn;
        public Material emMatOff;
        readonly string keyWord = "_EMISSION";

        // loading bar without animation
        //public Slider slider; // already assigned in inspector

        // play multiple movies
        //Dictionary<string, string> domeMoviesNameToLocalPath = new Dictionary<string, string>();
        public List<string> domeMoviesNameToLocalPath;
        private string filesLocalPath;
        int currentMovieIdx;

        bool needChangeMeshStatus;

        void Awake() {
            domeSphereGo = GameObject.Find("Dome_Sphere");
            domeScreenGo = GameObject.Find("Screen");
            reflectionProbe = GameObject.Find("Reflection Probe");

            // theater room need set the default emission first
            SetDefaultEmission();

            if (domeSphereGo != null)
            {
                domeVideoPlayer = domeScreenGo.GetComponent<VideoPlayer>();
            }
            currentMovieIdx = 0;
            filesLocalPath = @"d:\Auroraland\media\dome";
            domeMoviesNameToLocalPath = new List<string>();
            needChangeMeshStatus = true;
        }

        void Start()
        {
            DomeTheaterGetMovieList();
        }

        private void OnEnable()
        {
            DomeTheaterVoiceControl.OnPlayDomeMovie += PlayDomeMovieHandler;
            DomeTheaterVoiceControl.OnStopDomeMovie += StopDomeMovieHandler;
            DomeTheaterVoiceControl.OnPauseDomeMovie += PauseDomeMovieHandler;
            DomeTheaterVoiceControl.OnNextDomeMovie += NextDomeMovieHandler;
            DomeTheaterVoiceControl.OnPreviousDomeMovie += PreviousDomeMovieHandler;
            DomeTheaterVoiceControl.OnAdjustDomeMovieVolume += AdjustDomeMovieVolumeHandler;
        }

        private void OnDisable()
        {
            DomeTheaterVoiceControl.OnPlayDomeMovie -= PlayDomeMovieHandler;
            DomeTheaterVoiceControl.OnStopDomeMovie -= StopDomeMovieHandler;
            DomeTheaterVoiceControl.OnPauseDomeMovie -= PauseDomeMovieHandler;
            DomeTheaterVoiceControl.OnNextDomeMovie -= NextDomeMovieHandler;
            DomeTheaterVoiceControl.OnPreviousDomeMovie -= PreviousDomeMovieHandler;
            DomeTheaterVoiceControl.OnAdjustDomeMovieVolume -= AdjustDomeMovieVolumeHandler;
        }

        void PlayDomeMovieHandler() {

            string videoURL = domeMoviesNameToLocalPath[currentMovieIdx];
            if (File.Exists(videoURL) && domeVideoPlayer != null)
            {
                // Dissolve
                if (needChangeMeshStatus)
                {
                    DomeDissolver.Instance.Dissolve();
                    needChangeMeshStatus = false;
                }
                // Lights & VideoPlayer preparation
                SwitchLights(0.25f, false);
                SwitchMaterial(false);
                VideoPlayerControl(false);
                // Play
                if (domeVideoPlayer.url != videoURL)
                    domeVideoPlayer.url = videoURL;
                domeVideoPlayer.isLooping = true;
                domeVideoPlayer.Play();
            }
            else if (!File.Exists(videoURL))
            {
                Debug.Log("videoURL: " + videoURL);
            }
            else if (domeVideoPlayer == null)
            {
                Debug.Log("video player is null");
            }
        }

        void StopDomeMovie()
        {
            if (domeVideoPlayer != null)
            {
                domeVideoPlayer.Stop();
            }
            VideoPlayerControl(true);
            SwitchLights(1.0f, true);
            SwitchMaterial(true);
            needChangeMeshStatus = true;
        }

        void StopDomeMovieHandler()
        {
            StopDomeMovie();
            DomeDissolver.Instance.Appear();
        }

        void PauseDomeMovieHandler() {
            if (domeVideoPlayer != null)
            {
                domeVideoPlayer.Pause();
            }
            SwitchLights(1.0f, true);
            SwitchMaterial(true);
            needChangeMeshStatus = false;
            Debug.Log("needChangeMeshStatus" + needChangeMeshStatus);
        }

        void NextDomeMovieHandler() {
            if (currentMovieIdx == domeMoviesNameToLocalPath.Count - 1)
                currentMovieIdx = 0;
            else currentMovieIdx++;
            PlayDomeMovieHandler();
        }

        void PreviousDomeMovieHandler()
        {
            if (currentMovieIdx == 0)
                currentMovieIdx = domeMoviesNameToLocalPath.Count - 1;
            else currentMovieIdx--;
            PlayDomeMovieHandler();
        }

        void AdjustDomeMovieVolumeHandler(float delta) {
            if (delta > 0.0f)
                SetLouderVolume(domeVideoPlayer);
            else
                SetLowerVolume(domeVideoPlayer);
        }

        // Below are HELPER methods
        public void DomeTheaterGetMovieList()
        {
            DirectoryInfo domeTheaterLocalDir = new DirectoryInfo(filesLocalPath);
            FileInfo[] info = domeTheaterLocalDir.GetFiles("*.mp4");
            int idx = 0;
            foreach (FileInfo f in info)
            {
                domeMoviesNameToLocalPath.Add(f.FullName);
                idx++;
            }
        }

        public void SwitchLights(float intensityValue, bool shouldEnabled)
        {
            TheatreLights = FindObjectsOfType<Light>();

            if (reflectionProbe != null)
            {
                ReflectionProbe rp = reflectionProbe.GetComponent<ReflectionProbe>() as ReflectionProbe;
                rp.intensity = intensityValue;
            }

            foreach (Light TheatreLight in TheatreLights)
            {
                var lamp = TheatreLight.GetComponent<Light>();
                if (shouldEnabled)
                    lamp.intensity = 1;
                lamp.enabled = shouldEnabled;
            }
        }

        public void SetDefaultEmission()
        {
            if (emMatOn != null) emMatOn.EnableKeyword(keyWord);
            if (emMatOff != null) emMatOff.DisableKeyword(keyWord);
        }

        public void SwitchMaterial(bool enabled)
        {
            if (enabled)
            {
                emMatOff.DisableKeyword(keyWord);
                emMatOn.EnableKeyword(keyWord);
            }
            else
            {
                emMatOff.EnableKeyword(keyWord);
                emMatOn.DisableKeyword(keyWord);
            }
        }

        public void VideoPlayerControl(bool isPlaying)
        {
            domeVideoPlayer.enabled = !isPlaying;
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