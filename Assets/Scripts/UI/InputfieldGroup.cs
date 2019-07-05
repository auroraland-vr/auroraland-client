using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Auroraland.UI
{
    public class InputfieldGroup : MonoBehaviour
    {
        public List<InputField> InputFields = new List<InputField>();
        public InputField Current { private set; get; }

        void Start()
        {
            InputFields.AddRange(transform.GetComponentsInChildren<InputField>(true));

            foreach (InputField input in InputFields)
            {
                EventTrigger evtTrigger = input.gameObject.GetComponent<EventTrigger>();

                if (evtTrigger == null)
                    evtTrigger = input.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.Select
                };

                entry.callback.AddListener((data) =>
                {
                    Current = input;
                    Current.ActivateInputField();
                });

                evtTrigger.triggers.Add(entry);
            }

            if (InputFields.Count > 0)
            {
                Current = InputFields[0]; // set default
                Current.ActivateInputField();
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Current == null)
                {
                    InputFields[0].ActivateInputField();
                    Current = InputFields[0];
                }
                else
                {
                    int indexToUse = InputFields.IndexOf(Current);

                    if (indexToUse + 1 < InputFields.Count)
                        indexToUse++;
                    else
                        indexToUse = 0;

                    InputFields[indexToUse].ActivateInputField();
                }
            }
        }

        public void ClearAllFields() {
            foreach (InputField inputfield in InputFields) {
                inputfield.text = "";
                inputfield.DeactivateInputField();
            }
        }
    }
}
