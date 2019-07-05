using UnityEngine;

namespace Auroraland
{
    public class ScreenFade : MonoBehaviour
    {
        public static ScreenFade Instance;
        public enum FadeState { APPEAR, FADE_IN, FADE_OUT, CLEAR };
        public Color FadeInColor;
        public float FadeInDuration;
        public bool FadeInAtStart;

        public Color FadeOutColor;
        public float FadeOutDuration;

        public FadeState State = FadeState.CLEAR;
        private Material material;
        private Color transparent = new Color(0, 0, 0, 0);
        private float timer = 0;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            material = GetComponent<Renderer>().material;
        }

        void Start()
        {
            material.color = FadeInColor;
            GetComponent<Renderer>().material = material;

            if (FadeInAtStart)
            {
                FadeIn();
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            switch (State)
            {
                case FadeState.FADE_IN:
                    if (timer <= 1)
                    {
                        Fading(((State == FadeState.FADE_IN) ? FadeInColor : FadeOutColor), transparent);
                    }
                    else
                    {
                        material.color = transparent;
                        GetComponent<Renderer>().material = material;
                        State = FadeState.APPEAR;
                        timer = 0;
                    }
                    break;
                case FadeState.FADE_OUT:
                    if (timer <= 1)
                    {
                        Fading(transparent, ((State == FadeState.FADE_IN) ? FadeInColor : FadeOutColor));
                    }
                    else
                    {
                        material.color = ((State == FadeState.FADE_IN) ? FadeInColor : FadeOutColor);
                        GetComponent<Renderer>().material = material;
                        State = FadeState.CLEAR;
                        timer = 0;
                    }
                    break;
            }
        }

        private void Fading(Color a, Color b)
        {
            material.color = Color.Lerp(a, b, timer);
            GetComponent<Renderer>().material = material;
            timer += (Time.deltaTime / ((State == FadeState.FADE_IN) ? FadeInDuration : FadeOutDuration));
        }

        public void FadeIn()
        {
            State = FadeState.FADE_IN;
        }
        public void FadeOut()
        {
            State = FadeState.FADE_OUT;
        }
    }
}
