using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Auroraland{
	[RequireComponent(typeof(Button))]
	public class TabButton : MonoBehaviour, IPointerClickHandler,  IPointerEnterHandler, IPointerExitHandler {

        [Header("Custom Setting")]
		public TabGroup TabGroup;
        public Button Button;
        public string Name;

        [Header("Selected Color Theme")]
        public Color SelectedNormalColor;
        public Color SelectedHighlightedColor;
        public Color SelectedPressedColor;
        [Header("Deselected Color Theme")]
        public Color DeselectedNormalColor;
        public Color DeselectedHighlightedColor;
        public Color DeselectedPressedColor;
        public TabClickEvent OnTabClick;
        public bool IsButtonToggled { get { return isButtonToggled; }
            set {
                isButtonToggled = value;
                ResetTabColor();
            }
        }
		protected bool hasHovered = false;
        private bool isButtonToggled = false;

        void Awake() {
           if(!Button) Button = GetComponent<Button>();
        }

        void Start(){
			if(TabGroup != null) TabGroup.AddTab(this);
		}

        public void ToggledButton()
        {
            bool allowToggle = true;
            if (TabGroup != null)
            {
                if (IsButtonToggled) allowToggle = TabGroup.CheckAllowSwitchingOff(this);
            }
            if(allowToggle) IsButtonToggled = !IsButtonToggled;
        }

        public void OnPointerClick(PointerEventData eventData){
            ToggledButton();
            OnTabClick.Invoke(this);
        }

        public void OnPointerEnter(PointerEventData eventData){
			hasHovered = true;
		}

        public void OnPointerExit(PointerEventData eventData)
        {
            hasHovered = false;

        }

        private void ResetTabColor() {
            if (isButtonToggled)
            {
                var colors = Button.colors;
                colors.normalColor = SelectedNormalColor;
                colors.highlightedColor = SelectedHighlightedColor;
                colors.pressedColor = SelectedPressedColor;
                Button.colors = colors;
            }
            else
            {
                var colors = Button.colors;
                colors.normalColor = DeselectedNormalColor;
                colors.highlightedColor = DeselectedHighlightedColor;
                colors.pressedColor = DeselectedPressedColor;
                Button.colors = colors;
            }
        }

        [System.Serializable]
        public sealed class TabClickEvent : UnityEvent<TabButton>
        {

        }
    }
}
