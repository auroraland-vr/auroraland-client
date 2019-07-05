using UnityEngine;

namespace Auroraland
{
    public class DomeDissolver : MonoBehaviour
    {
        public static DomeDissolver Instance;

        public MeshRenderer domeGlass;
        public MeshRenderer[] domeDissolveRenderers;
        public MeshRenderer videoScreen;

        public Material domeDissolveMat;
        public Material domeNormalMat;

        public Material videoScreenNormalMat;
        public Material videoScreenDissolveMat;

        public MeshRenderer sunBlocker;

        public float startTime;
        public float dissolveTime = 4;

        [Range(0, 1)] public float dissolveAmount;
        [Range(0, 1)] public float appearAmount;

        public enum DissolveState
        {
            DissolveDome,
            AppearVideo,
            DissolveVideo,
            AppearDome,
            None
        }

        public DissolveState dissolveState;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            SetDefaults();
        }


        void SetDefaults()
        {
            dissolveState = DissolveState.None;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Dissolve();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Appear();
            }

            switch (dissolveState)
            {
                case DissolveState.DissolveDome:
                    DissolveDome();
                    break;
                case DissolveState.AppearVideo:
                    AppearVideo();
                    break;
                case DissolveState.DissolveVideo:
                    DissolveVideo();
                    break;
                case DissolveState.AppearDome:
                    AppearDome();
                    break;
                case DissolveState.None:
                    break;
            }
        }

        // Dome screen show up/appear
        public void Appear()
        {
            startTime = 0;
            dissolveAmount = 1;
            appearAmount = 0;

            domeGlass.enabled = false;
            videoScreen.enabled = true;
            sunBlocker.enabled = false;

            videoScreen.material = videoScreenDissolveMat;
            videoScreenDissolveMat.SetFloat("_DissolveAmount", 0);
            domeDissolveMat.SetFloat("_DissolveAmount", 1);

            foreach (MeshRenderer ren in domeDissolveRenderers)
            {
                ren.enabled = false;
                ren.material = domeDissolveMat;
            }

            dissolveState = DissolveState.DissolveVideo;
        }

        public void Dissolve()
        {
            //set the default dome states
            startTime = 0;
            dissolveAmount = 0;
            appearAmount = 1;

            videoScreen.enabled = false;
            sunBlocker.enabled = false;
            dissolveState = DissolveState.DissolveDome;
            videoScreen.material = videoScreenDissolveMat;
            videoScreenDissolveMat.SetFloat("_DissolveAmount", 1);
            domeDissolveMat.SetFloat("_DissolveAmount", 0);

            foreach (MeshRenderer ren in domeDissolveRenderers)
            {
                ren.enabled = true;
                ren.material = domeDissolveMat;
            }
        }

        void DissolveVideo()
        {
            startTime += Time.deltaTime; //.02  0-1
            appearAmount += Time.deltaTime / dissolveTime;
            appearAmount = Mathf.Clamp01(appearAmount);
            videoScreenDissolveMat.SetFloat("_DissolveAmount", appearAmount);

            //after video has disappeared
            if (startTime >= dissolveTime)
            {
                videoScreen.enabled = false;
                startTime = 0;

                foreach (MeshRenderer ren in domeDissolveRenderers)
                {
                    ren.enabled = true;
                }

                dissolveState = DissolveState.AppearDome;
            }
        }

        void DissolveDome()
        {
            startTime += Time.deltaTime; //.02  0-1
            dissolveAmount += Time.deltaTime / dissolveTime;
            dissolveAmount = Mathf.Clamp01(dissolveAmount);
            domeDissolveMat.SetFloat("_DissolveAmount", dissolveAmount);

            if (startTime >= dissolveTime)
            {
                startTime = 0;
                videoScreen.enabled = true;
                dissolveState = DissolveState.AppearVideo;
            }
        }

        void AppearDome()
        {
            startTime += Time.deltaTime; //.02  0-1
            dissolveAmount -= Time.deltaTime / dissolveTime;
            dissolveAmount = Mathf.Clamp01(dissolveAmount);
            domeDissolveMat.SetFloat("_DissolveAmount", dissolveAmount);

            if (!(startTime >= dissolveTime))
            {
                return;
            }

            startTime = 0;
            dissolveState = DissolveState.None;

            domeGlass.enabled = true;

            foreach (MeshRenderer ren in domeDissolveRenderers)
            {
                ren.material = domeNormalMat;
            }
        }

        void AppearVideo()
        {
            startTime += Time.deltaTime; //.02  0-1
            appearAmount -= Time.deltaTime / dissolveTime;
            appearAmount = Mathf.Clamp01(appearAmount);
            videoScreenDissolveMat.SetFloat("_DissolveAmount", appearAmount);

            if (startTime >= dissolveTime)
            {
                sunBlocker.enabled = true;
                videoScreen.material = videoScreenNormalMat;
                domeGlass.enabled = false;
                dissolveState = DissolveState.None;

                foreach (MeshRenderer ren in domeDissolveRenderers)
                {
                    ren.enabled = false;
                }
            }
        }
    }
}
