using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Auroraland{
	public class TabGroup : MonoBehaviour {

        public bool AllowMultipleToggle;
        public bool AllowSwitchOff; //Is it allowed the toggle to be switched off?
        public bool AllowSwitchAllOff; //it is allowed all of the toggles to be switched off?
		private List<TabButton> tabGroup = new List<TabButton>();
        // Use this for initialization

        public List<TabButton> GetTabList() {
            return tabGroup;
        }

		public void AddTab(TabButton tabButton){
			tabGroup.Add (tabButton);
		}

        public bool CheckAllowSwitchingOff(TabButton tab) {
            if (!AllowSwitchOff) return false;
            if (AllowSwitchOff && !AllowSwitchAllOff)
            {
                int toggledCount = 0;
                foreach (TabButton button in tabGroup)
                {
                    if (button.IsButtonToggled && button != tab) 
                    {
                        toggledCount++;
                    }
                }
                if (toggledCount == 0) return false; //can't untoggle the last tab
            }
            return true;

        }

		public void ConstraintOneToggle(TabButton tab)
        { //Toggles that belong to the same group are constrained so that only one of them can switched on at a time - pressing one of them to switch it on, and switches the others off.
            if (AllowMultipleToggle) return;
			foreach (TabButton button in tabGroup) {
				if (button != tab && button.IsButtonToggled) {
					button.ToggledButton (); // turn off
				}
			}
		}
	}
}