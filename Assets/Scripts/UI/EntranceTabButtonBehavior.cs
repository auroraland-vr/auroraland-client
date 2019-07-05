using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Auroraland.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class EntranceTabButtonBehavior : MonoBehaviour
    {
        Button _button;
        Image _buttonImage;

        [SerializeField]
        Image _selectedStateArrowIndicator;
        [SerializeField]
        GameObject _objectToToggle;
        [SerializeField]
        EntranceTabButtonBehavior _otherTabButtonInGroup;

        [Header("Unselected State Behavior")]
        [SerializeField]
        Color _unselectedStatePanelColor;
        [SerializeField]
        int _unselectedStateDepression = 5;

        [Header("Selected State Behavior")]
        [SerializeField]
        Color _selectedStatePanelColor;
        [SerializeField]
        int _selectedStateDepression = 0;
        

        private void Start()
        {
            _button = GetComponent<Button>();
            _buttonImage = GetComponent<Image>();
            _button.onClick.AddListener(DoCustomSelectBehavior);

            Assert.IsNotNull(_selectedStateArrowIndicator);
            Assert.AreNotEqual(_selectedStatePanelColor, _unselectedStatePanelColor);
            Assert.AreNotEqual(_selectedStateDepression, _unselectedStateDepression);
        }

        void DoCustomSelectBehavior()
        {
            _buttonImage.color = _selectedStatePanelColor;

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _selectedStateDepression);
            _selectedStateArrowIndicator.enabled = true;
            _objectToToggle.SetActive(true);
            _otherTabButtonInGroup.DoCustomDeselectBehavior();
        }

        void DoCustomDeselectBehavior()
        {
            _buttonImage.color = _unselectedStatePanelColor;

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, _unselectedStateDepression);
            _selectedStateArrowIndicator.enabled = false;
            _objectToToggle.SetActive(false);
        }
    }
}
