using System.Collections;
using UnityEngine;

namespace Auroraland
{
    /// <summary>
    /// Purpose: Controls the animation sequence of close door, screen fade, particle effects
    /// </summary>
    public class TransitDoor : MonoBehaviour
    {
        public static int DOOR_CLOSE = Animator.StringToHash("Base Layer.CloseDoor");
        public static int DOOR_OPEN = Animator.StringToHash("Base Layer.OpenDoor");
        public ScreenFade ScreenFade;

        [Header("Door")]
        public GameObject[] DoorComponents;
        public Animator DoorAnimator;
        public ParticleSystem[] LightBeams;

        public bool IsAnimating { get; private set; }

        public void OpenDoor()
        {
            //TODO set in front of player
            StartCoroutine(DoOpenDoorAnimation());
        }

        public void CloseDoor()
        {
            //TODO set in the back of player
            StartCoroutine(DoCloseDoorAnimation());
        }

        public void ToggleDoor()
        {
            bool isOn = DoorComponents[0].activeInHierarchy;
            isOn = !isOn;
            SetDoorActive(isOn);
        }

        public void SetupDoor()
        {
            DoorComponents[0].SetActive(true);
            DoorComponents[1].SetActive(false);
        }

        private void SetDoorActive(bool active)
        {
            foreach (GameObject component in DoorComponents)
            {
                component.SetActive(active);
            }
        }
        public int GetCurrentState()
        {
            return DoorAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }

        private IEnumerator DoOpenDoorAnimation()
        {
            IsAnimating = true;
            SetDoorActive(true);
            DoorAnimator.SetBool("IsOpened", true);

            yield return new WaitUntil(() => GetCurrentState() == DOOR_OPEN);
            yield return new WaitWhile(() => DoorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.6f);

            ToggleLights(true);

            if (ScreenFade != null)
            {
                ScreenFade.FadeOut();
                yield return new WaitUntil(() => ScreenFade.State == ScreenFade.FadeState.CLEAR);
            }

            SetDoorActive(false);
            ToggleLights(false);
            IsAnimating = false;
        }

        private IEnumerator DoCloseDoorAnimation()
        {
            IsAnimating = true;
            SetDoorActive(true);
            ToggleLights(false);

            if (ScreenFade != null)
            {
                ScreenFade.FadeIn();
                yield return new WaitUntil(() => ScreenFade.State == ScreenFade.FadeState.APPEAR);
            }

            DoorAnimator.SetBool("IsOpened", false);
            yield return new WaitUntil(() => GetCurrentState() == DOOR_CLOSE);
            Debug.Log(DoorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return new WaitWhile(() => DoorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
            yield return new WaitForSeconds(1f); //the door close animation is too short stay 1 more sec
            SetDoorActive(false);
            IsAnimating = false;
        }

        private void ToggleLights(bool toggle)
        {
            foreach (ParticleSystem light in LightBeams)
            {
                light.gameObject.SetActive(toggle);
            }
        }
    }
}