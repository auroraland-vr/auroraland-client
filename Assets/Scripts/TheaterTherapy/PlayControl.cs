using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Auroraland
{
    public class PlayControl : MonoBehaviour
    {
        public static PlayControl Instance;
        
        public Light[] TheatreLights;
        //public Material emMat;
        public Material emMatOn;
        public Material emMatOff;
        readonly string keyWord = "_EMISSION";
        //public MeshRenderer[] WallEmissives;

        // standard movie theater
        public VideoPlayer standardVideoPlayer;
        public GameObject standardScreenGo;

        // dome movie theater
        public GameObject domeScreenGo;
        public GameObject domeSphereGo;
        public VideoPlayer domeVideoPlayer;
        public GameObject reflectionProbe;
        public MeshRenderer[] domeScreenMeshRenderers;
        public MeshCollider[] domeScreenMeshColliders;

        // IMAX movie theater
        public VideoPlayer IMAXVideoPlayer;
        public GameObject IMAXScreenGo;
        public MeshRenderer[] movieScreenMeshRenderers;
        
        // therapy room
        public VideoPlayer therapyVideoPlayer;
        public GameObject therapyScreenGo;
        public GameObject therapyScreenMeshGo;
        public MeshRenderer[] therapyScreenMeshRenderers;

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

        // for video streaming
        public string serverURL;
        readonly string clipNamePrefix = "savedVideoFile_";
        public string clipName;

        // loading bar without animation
        public Slider slider; // already assigned in inspector

        // multiple video play
        public string therapyVideoPath;
        Dictionary<string, string> therapyNameToRmtPath = new Dictionary<string, string>();
        Dictionary<string, string> therapyNameToLocalPath = new Dictionary<string, string>();
        public int videoIdx;
        public string[] serverURLs;
        public bool serverURLsFlag;

        void Awake()
        {
            Instance = this;
            // standard movie theater
            standardScreenGo = GameObject.Find("Movie_Screen");
            // dome movie theater
            domeSphereGo = GameObject.Find("Dome_Sphere");
            domeScreenGo = GameObject.Find("Screen");
            reflectionProbe = GameObject.Find("Reflection Probe");
            // IMAX movie theater
            IMAXScreenGo = GameObject.Find("Screen_03");
            // therapy room
            therapyScreenGo = GameObject.Find("TherapyRoom_Video Player");
            therapyScreenMeshGo = GameObject.Find("TherapyRoom_MovieScreen");
            // 360 movie theater
            T360VideoPlayerGo = GameObject.Find("360Video_Video Player");
            T360RoomHolderGo = GameObject.Find("Room_Holder");
            T360LightBeamGo = GameObject.Find("lightbeam");
            needChangeStatus = true;

            // theater room need set the default emission first
            SetDefaultEmission();

            // multiple video for IMAX and Therapy room
            videoIdx = 0;
            serverURLsFlag = false;

            if (domeSphereGo != null) {
                domeVideoPlayer = domeScreenGo.GetComponent<VideoPlayer>();
            } 
            if (standardScreenGo != null) {
                standardVideoPlayer = standardScreenGo.GetComponent<VideoPlayer>();
            }
            if (IMAXScreenGo != null) {
                IMAXVideoPlayer = IMAXScreenGo.GetComponent<VideoPlayer>();
            }
            if (therapyScreenGo != null) {
                therapyVideoPlayer = therapyScreenGo.GetComponent<VideoPlayer>();
            }
            if (T360VideoPlayerGo != null && T360LightBeamGo != null) {
                T360VideoPlayer = T360VideoPlayerGo.GetComponent<VideoPlayer>();
            }
            if (slider != null) {
                slider.gameObject.SetActive(false);
            }
        }

        void Start()
        {
            if (IMAXVideoPlayer != null)
            {
                PlayIMAXIntro(IMAXVideoPlayer);
            }
            if (therapyVideoPlayer != null)
            {
                TherapyGetVideoListFrmLocal();
            }
        }

        void PlayIMAXIntro(VideoPlayer vp) {
            string introVideoPath = Application.dataPath + "/StreamingAssets/imax pre show 4k.mp4";
            vp.url = introVideoPath;
            vp.Play();
        }

        // Given existing server URL, used for play ONLY one video in the room
        void PlayVideo(string filePath, VideoPlayer vp)
        {
            string introVideoPath = Application.dataPath + "/StreamingAssets/imax pre show 4k.mp4";

            if (vp == IMAXVideoPlayer && vp.url == introVideoPath)
            {
                vp.Stop();
                vp.url = filePath;
            }
            if (vp.url.Length == 0)
                vp.url = filePath;

            vp.Play();
        }

        // Getting URL lists, used for play multiple videos in the room
        void PlayMultiVideo(VideoPlayer vp, int idx)
        {
            string introVideoPath = Application.dataPath + "/StreamingAssets/imax pre show 4k.mp4";
            if (vp == IMAXVideoPlayer && vp.url == introVideoPath)
            {
                vp.Stop();
                vp.url = serverURLs[idx];
            }

            if (vp.url.Length == 0)
                // always play the first one, which is the default one
                vp.url = serverURLs[idx];

            vp.isLooping = true;
            vp.Play();
        }

        // Given existing server URL, used for play ONLY one video in the room
        void VideoDownload(VideoPlayer vp, string subFolderName)
        {
            // 0. preparation (dynamically get the clipname and use it to create the path)
            string path = @"d:/Auroraland/media" + "/" + subFolderName + "/" + clipNamePrefix + subFolderName + ".mp4";

            // 0.5 loading bar without animation
            if (slider != null) {
                slider.value = 0;
                slider.gameObject.SetActive(true);
            }

            // 4. set slider
            if (slider != null) {
                slider.value = 1;
                slider.gameObject.SetActive(false);
            }

            // 5. play the video
            PlayVideo(path, vp);
        }

        void MultiVideoDownloadHelper(VideoPlayer vp, Dictionary<string, string> nameToLocalPath)
        {
            int idx = 0;

            foreach (KeyValuePair<string, string> pair in nameToLocalPath)
            {
                // 0.5 loading bar without animation
                if (slider != null)
                {
                    slider.value = 0;
                    slider.gameObject.SetActive(true);
                }

                // 4. set slider
                if (slider != null)
                {
                    slider.value = 1;
                    slider.gameObject.SetActive(false);
                }

                // serverURLs[] saves all the local path of the saved video
                serverURLs[idx] = pair.Value;
                Debug.Log("MultiVideoDownloadHelper - Got the local path that saved video: " + pair.Value);
                idx++;
                Debug.Log("accumated counter of video saved: " + idx);
            }
            serverURLsFlag = true;
            PlayMultiVideo(vp, videoIdx);
        }

        // Getting URL lists, used for play multiple videos in the room
        public void MultiVideoDownload(VideoPlayer vp, string roomName)
        {
            if (roomName == "therapy")
            {
                MultiVideoDownloadHelper(vp, therapyNameToLocalPath);
            }
        }

        void PlayNextHelper(VideoPlayer vp)
        {
            if (videoIdx >= 0 && videoIdx < serverURLs.Length - 1)
            {
                videoIdx++;
                vp.url = serverURLs[videoIdx];
            }
            else if (videoIdx == serverURLs.Length - 1)
            {
                videoIdx = 0;
                vp.url = serverURLs[videoIdx];
            }
            vp.isLooping = true;
            vp.Play();
        }

        public void PlayNext()
        {
            if (therapyVideoPlayer != null)
            {
                if (serverURLs[0] == null)
                {
                    Debug.Log("Next from just play");
                    MultiVideoDownload(therapyVideoPlayer, "therapy"); // just play it since no play before!
                }
                else
                {
                    PlayStop();
                    Debug.Log("Next from next");
                    PlayNextHelper(therapyVideoPlayer);
                }
            }
        }

        void PlayPreviousHelper(VideoPlayer vp)
        {
            if (videoIdx >= 1 && videoIdx < serverURLs.Length)
            {
                videoIdx--;
                vp.url = serverURLs[videoIdx];
            }
            else if (videoIdx == 0)
            {
                videoIdx = serverURLs.Length - 1;
                vp.url = serverURLs[videoIdx];
            }
            vp.isLooping = true;
            vp.Play();
        }

        public void PlayPrevious()
        {
            if (therapyVideoPlayer != null)
            {
                if (serverURLs[0] == null)
                {
                    MultiVideoDownload(therapyVideoPlayer, "therapy"); // just play it since no play before!
                }
                else
                {
                    PlayStop();
                    PlayPreviousHelper(therapyVideoPlayer);
                }
            }
        }

        public void PlayPause()
        {
            if (domeVideoPlayer != null)
            {
                domeVideoPlayer.Pause();
            }
            if (standardVideoPlayer != null) {
                standardVideoPlayer.Pause();
            }
            if (IMAXVideoPlayer != null) {
                IMAXVideoPlayer.Pause();
            }
            if (therapyVideoPlayer != null && therapyVideoPlayer.isPlaying) {
                therapyVideoPlayer.Pause();
            }
            if (T360VideoPlayer != null)
            {
                T360VideoPlayer.Pause();
            }

        }

        public void PlayStop() {
            if (domeVideoPlayer != null)
            {
                domeVideoPlayer.Stop();
            }
            if (standardVideoPlayer != null)
            {
                standardVideoPlayer.Stop();
            }
            if (IMAXVideoPlayer != null)
            {
                IMAXVideoPlayer.Stop();
                PlayIMAXIntro(IMAXVideoPlayer);
            }
            if (therapyVideoPlayer != null && therapyVideoPlayer.isPlaying)
            {
                therapyVideoPlayer.Stop();
            }
            if (T360VideoPlayer != null)
            {
                T360VideoPlayer.Stop();
            }
        }

        public void PlayContinue()
        {
            if (domeVideoPlayer != null && !domeVideoPlayer.isPlaying)
            {
                if (serverURL != null)
                {
                    VideoDownload(domeVideoPlayer, "dome");
                }
            }
            
            if (IMAXVideoPlayer != null) {
                if (serverURL != null)
                {
                    VideoDownload(IMAXVideoPlayer, "imax");
                }
            }
            if (therapyVideoPlayer != null && !therapyVideoPlayer.isPlaying)
            {
                if (serverURL != null)
                {
                    MultiVideoDownload(therapyVideoPlayer, "therapy");
                }
            }
            if (T360VideoPlayer != null && !T360VideoPlayer.isPlaying) {
                if (serverURL != null) {
                    VideoDownload(T360VideoPlayer, "360");
                }
            }
        }

        public void SetLouderVolume(VideoPlayer vp) {
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

        public void LouderVolume() {
            SetLouderVolume(domeVideoPlayer);
            SetLouderVolume(standardVideoPlayer);
            SetLouderVolume(IMAXVideoPlayer);
            SetLouderVolume(therapyVideoPlayer);
            SetLouderVolume(T360VideoPlayer);

        }

        public void LowerVolume()
        {
            SetLowerVolume(domeVideoPlayer);
            SetLowerVolume(standardVideoPlayer);
            SetLowerVolume(IMAXVideoPlayer);
            SetLowerVolume(therapyVideoPlayer);
            SetLowerVolume(T360VideoPlayer);
        }

        public void TurnOnLights()
        {
            TheatreLights = FindObjectsOfType<Light>();
            
            if (reflectionProbe != null && therapyScreenGo == null)
            {
                ReflectionProbe rp = reflectionProbe.GetComponent<ReflectionProbe>() as ReflectionProbe;
                rp.intensity = 1.0f;
            }

            foreach (Light TheatreLight in TheatreLights)
            {
                var lamp = TheatreLight.GetComponent<Light>();
                lamp.intensity = 1;
                lamp.enabled = true;
            }
        }

        public void TurnOffLights()
        {
            TheatreLights = FindObjectsOfType<Light>();
            
            if (reflectionProbe != null) {
                ReflectionProbe rp = reflectionProbe.GetComponent<ReflectionProbe>();
                rp.intensity = 0.25f;
            }

            foreach (Light TheatreLight in TheatreLights)
            {
                var lamp = TheatreLight.GetComponent<Light>();
                lamp.enabled = false;
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

        // dome mesh control
        public void DomeScreenMesh() {
            domeScreenMeshRenderers = domeSphereGo.GetComponentsInChildren<MeshRenderer>();
            domeScreenMeshColliders = domeSphereGo.GetComponentsInChildren<MeshCollider>();
            foreach (MeshRenderer smr in domeScreenMeshRenderers)
                smr.enabled = !smr.enabled;
            foreach (MeshCollider smc in domeScreenMeshColliders)
                smc.enabled = !smc.enabled;
        }

        // dome Video Player control
        public void VideoPlayerControl(bool isPlaying) {

            domeVideoPlayer.enabled = !isPlaying;
        }

        // IMAX mesh control
        public void MovieTheaterScreenMesh() {
            movieScreenMeshRenderers = IMAXScreenGo.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer smr in movieScreenMeshRenderers)
                smr.enabled = !smr.enabled;
        }

        // therapy room Video Player control
        public void TherapyVideoPlayerControl() {
            therapyVideoPlayer.enabled = !therapyVideoPlayer.enabled;
        }

        // therapy room mesh control
        public void TherapyScreenMesh() {
            therapyScreenMeshRenderers = therapyScreenMeshGo.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer smr in therapyScreenMeshRenderers)
                smr.enabled = !smr.enabled;
        }

        // 360 Video Player control
        void T360VideoPlayerControl()
        {
            T360VideoPlayer.enabled = !T360VideoPlayer.enabled;
        }

        // 360 light beam (mesh) control helper
        void T360LightBeamControlHelper(bool lightBeamOn) {
            T360LightBeamMeshRenderers = T360LightBeamGo.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer lbr in T360LightBeamMeshRenderers)
                lbr.enabled = lightBeamOn;

            if (lightBeamOn)
            {
                FinishedLightBeamOpen = true;
                FinishedLightBeamClose = false;
            }
            else {
                FinishedLightBeamClose = true;
                FinishedLightBeamOpen = false;
            }
        }

        // 360 room component control
        void T360RoomControl(bool needActive) {
            SetActivateChildren(T360RoomHolderGo, needActive);
        }

        // 360 room component control helper
        void SetActivateChildren(GameObject g, bool needActive)
        {
            if (g.name == "Room") {
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
            else {
                FinishedRoomClose = false;
                FinishedRoomOpen = true;
            }
        }

        public void PlayT360()
        {
            StartCoroutine(PlayT360Helper());
        }

        public void StopT360()
        {
            StartCoroutine(StopT360Helper());
        }

        IEnumerator PlayT360Helper()
        {
            if (needChangeStatus) {
                T360LightBeamControlHelper(true);
                T360RoomControl(false);
                T360VideoPlayerControl();

                yield return new WaitForSeconds(1f);

                T360LightBeamControlHelper(false);
                needChangeStatus = false;
            }
            
            PlayContinue();
        }

        IEnumerator StopT360Helper()
        {
            PlayStop();
            T360VideoPlayerControl();
            T360LightBeamControlHelper(true);
            T360RoomControl(true);

            yield return new WaitForSeconds(1f);

            T360LightBeamControlHelper(false);
            needChangeStatus = true;

        }

        public void TherapyGetVideoListFrmLocal()
        {
            string path = @"d:\Auroraland\media\treatment\video";
            DirectoryInfo therapyLocalDir = new DirectoryInfo(path);
            FileInfo[] info = therapyLocalDir.GetFiles("*.mp4");
            foreach (FileInfo f in info)
            {
                therapyNameToLocalPath.Add(f.Name, f.FullName);
            }
            int nameToLocalPathCounter = therapyNameToLocalPath.Count;
            serverURLs = new string[nameToLocalPathCounter];
        }
    }
}
