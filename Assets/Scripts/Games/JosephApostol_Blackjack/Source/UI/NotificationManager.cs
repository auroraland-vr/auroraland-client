using UnityEngine;
using UnityEngine.UI;

namespace ProjectRenaissance.UI
{
    public sealed class NotificationManager : MonoBehaviour
    {
        Color _initialColor;
        Color _targetColor = new Color(1, 1, 1, 0);
        float _accumulator = 0;
        float _stayTime = 1.5f;
        float _fadeTime = 1f;
        bool _didNotify;

        [SerializeField]
        Text _text;

        void Start()
        {
            _initialColor = _text.color;
            _targetColor = new Color(_initialColor.r, _initialColor.g, _initialColor.b, 0);
        }

        void Update()
        {
            if (_didNotify)
            {
                _accumulator += Time.deltaTime;

                if (_accumulator > _stayTime)
                {
                    float percentCompleted = (_accumulator - _stayTime) / _fadeTime;
                    _text.color = Color.Lerp(_initialColor, _targetColor, percentCompleted);
                }

                if (_accumulator > _stayTime + _fadeTime)
                {
                    _accumulator = 0;
                    _text.text = "";
                    _text.transform.parent.gameObject.SetActive(false);
                    _didNotify = false;
                }
            }
        }

        public void Notify(string notification)
        {
            _text.transform.parent.gameObject.SetActive(true);
            _text.text = notification + "\n" + _text.text;
            _text.color = _initialColor;
            _accumulator = 0;
            _didNotify = true;
        }
    }
}