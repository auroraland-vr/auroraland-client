using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;
using System.IO;

namespace Auroraland
{
    public class IMAXControl : MonoBehaviour {

        // IMAX movie theater
        public VideoPlayer IMAXVideoPlayer;
        public GameObject IMAXScreenGo;
        public MeshRenderer[] movieScreenMeshRenderers;
        // Light
        public Light[] TheatreLights;
        public Material emMatOn;
        public Material emMatOff;
        readonly string keyWord = "_EMISSION";
        public GameObject reflectionProbe;
        // multiple movies
        public List<string> imaxMoviesNameToLocalPath;
        private string filesLocalPath;
        int currentMovieIdx;

        void Awake()
        {
            IMAXScreenGo = GameObject.Find("Screen_03");
            reflectionProbe = GameObject.Find("Reflection Probe");
            // theater room need set the default emission first
            SetDefaultEmission();

            if (IMAXScreenGo != null)
            {
                IMAXVideoPlayer = IMAXScreenGo.GetComponent<VideoPlayer>();
            }
            currentMovieIdx = 0;
            filesLocalPath = @"d:\Auroraland\media\imax";
            imaxMoviesNameToLocalPath = new List<string>();
        }

        void Start()
        {
            IMAXTheaterGetMovieList();
            if (IMAXVideoPlayer != null)
            {
                PlayIMAXIntro(IMAXVideoPlayer);
            }
        }

        private void OnEnable()
        {
            IMAXTheaterVoiceControl.OnPlayIMAXMovie += PlayIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnStopIMAXMovie += StopIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnPauseIMAXMovie += PauseIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnNextIMAXMovie += NextIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnPreviousIMAXMovie += PreviousIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnAdjustIMAXMovieVolume += AdjustIMAXMovieVolumeHandler;
        }

        private void OnDisable()
        {
            IMAXTheaterVoiceControl.OnPlayIMAXMovie -= PlayIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnStopIMAXMovie -= StopIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnPauseIMAXMovie -= PauseIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnNextIMAXMovie -= NextIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnPreviousIMAXMovie -= PreviousIMAXMovieHandler;
            IMAXTheaterVoiceControl.OnAdjustIMAXMovieVolume -= AdjustIMAXMovieVolumeHandler;
        }

        void PlayIMAXMovieHandler() {
            string introVideoPath = Application.dataPath + "/StreamingAssets/imax pre show 4k.mp4";
            string videoURL = imaxMoviesNameToLocalPath[currentMovieIdx];

            if (File.Exists(videoURL) && IMAXVideoPlayer != null)
            {
                // Play
                if (IMAXVideoPlayer.url == introVideoPath)
                {
                    IMAXVideoPlayer.Stop();
                    IMAXVideoPlayer.url = videoURL;
                }
                if (IMAXVideoPlayer.url != videoURL)
                    IMAXVideoPlayer.url = videoURL;
                IMAXVideoPlayer.isLooping = true;
                IMAXVideoPlayer.Play();
                TurnOffLightsSlowly();
                SwitchMaterial(false);
            }
            else if (!File.Exists(videoURL))
            {
                Debug.Log("videoURL: " + videoURL);
            }
            else if (IMAXVideoPlayer == null)
            {
                Debug.Log("video player is null");
            }
        }

        void StopIMAXMovieHandler() {
            if (IMAXVideoPlayer != null)
            {
                IMAXVideoPlayer.Stop();
                PlayIMAXIntro(IMAXVideoPlayer);
            }
            //VideoPlayerControl(true);
            SwitchLights(1.0f, true);
            SwitchMaterial(true);
        }

        void PauseIMAXMovieHandler() {
            if (IMAXVideoPlayer != null)
            {
                IMAXVideoPlayer.Pause();
            }
            SwitchLights(1.0f, true);
            SwitchMaterial(true);
        }

        void NextIMAXMovieHandler() {
            if (currentMovieIdx == imaxMoviesNameToLocalPath.Count - 1)
                currentMovieIdx = 0;
            else currentMovieIdx++;
            PlayIMAXMovieHandler();
        }

        void PreviousIMAXMovieHandler() {
            if (currentMovieIdx == 0)
                currentMovieIdx = imaxMoviesNameToLocalPath.Count - 1;
            else currentMovieIdx--;
            PlayIMAXMovieHandler();
        }

        void AdjustIMAXMovieVolumeHandler(float delta) {
            if (delta > 0.0f)
                SetLouderVolume(IMAXVideoPlayer);
            else
                SetLowerVolume(IMAXVideoPlayer);
        }

        // BELOW are helper methods
        public void IMAXTheaterGetMovieList()
        {
            DirectoryInfo imaxTheaterLocalDir = new DirectoryInfo(filesLocalPath);
            FileInfo[] info = imaxTheaterLocalDir.GetFiles("*.mp4");
            int idx = 0;
            //int infoCounter = info.Length;
            foreach (FileInfo f in info)
            {
                imaxMoviesNameToLocalPath.Add(f.FullName);
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

        public void TurnOffLightsSlowly()
        {
            TheatreLights = FindObjectsOfType<Light>();

            if (reflectionProbe != null)
            {
                ReflectionProbe rp = reflectionProbe.GetComponent<ReflectionProbe>();
                rp.intensity = 0.25f;
            }

            StartCoroutine(DoTurnOffLightsSlowly());
        }

        IEnumerator DoTurnOffLightsSlowly()
        {
            float timeStart = Time.time;
            float timeElapsed = 0;

            while (timeElapsed < 2f)
            {
                foreach (Light TheatreLight in TheatreLights)
                {
                    var lamp = TheatreLight.GetComponent<Light>();
                    lamp.intensity = Mathf.Lerp(1, 0, timeElapsed / 2f);
                }

                timeElapsed = Time.time - timeStart;
                yield return null;
            }
        }

        void PlayIMAXIntro(VideoPlayer vp)
        {
            string introVideoPath = Application.dataPath + "/StreamingAssets/imax pre show 4k.mp4";
            vp.url = introVideoPath;
            vp.Play();
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
            IMAXVideoPlayer.enabled = !isPlaying;
        }
    }
}