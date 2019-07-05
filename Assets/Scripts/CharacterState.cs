using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
    public class CharacterState : MonoBehaviour
    {
        public GameObject[] mockMeshes;
        public GameObject[] vrcullMeshes;

        public static int WALK_START = Animator.StringToHash("BaseLayer.walk_start");
        public static int WALK_LEFT_START = Animator.StringToHash("BaseLayer.walk_left_start");
        public static int WALK_LEFT_PASS = Animator.StringToHash("BaseLayer.walk_left_pass");
        public static int WALK_LEFT_END = Animator.StringToHash("BaseLayer.walk_left_end");
        public static int WALK_RIGHT_START = Animator.StringToHash("BaseLayer.walk_right_start");
        public static int WALK_RIGHT_PASS = Animator.StringToHash("BaseLayer.walk_right_pass");
        public static int WALK_RIGHT_END = Animator.StringToHash("BaseLayer.walk_right_end");

        public static int WALKBACK_START = Animator.StringToHash("BaseLayer.walkback_start");
        public static int WALKBACK_LEFT_START = Animator.StringToHash("BaseLayer.walkback_left_start");
        public static int WALKBACK_LEFT_PASS = Animator.StringToHash("BaseLayer.walkback_left_pass");
        public static int WALKBACK_LEFT_END = Animator.StringToHash("BaseLayer.walkback_left_end");
        public static int WALKBACK_RIGHT_START = Animator.StringToHash("BaseLayer.walkback_right_start");
        public static int WALKBACK_RIGHT_PASS = Animator.StringToHash("BaseLayer.walkback_right_pass");
        public static int WALKBACK_RIGHT_END = Animator.StringToHash("BaseLayer.walkback_right_end");

        public static int STRAFELEFT_START = Animator.StringToHash("BaseLayer.strafeleft_start");
        public static int STRAFELEFT_LEFT_START = Animator.StringToHash("BaseLayer.strafeleft_left_start");
        public static int STRAFELEFT_LEFT_PASS = Animator.StringToHash("BaseLayer.strafeleft_left_pass");
        public static int STRAFELEFT_RIGHT_START = Animator.StringToHash("BaseLayer.strafeleft_right_start");
        public static int STRAFELEFT_RIGHT_PASS = Animator.StringToHash("BaseLayer.strafeleft_right_pass");
        public static int STRAFELEFT_LEFT_END = Animator.StringToHash("BaseLayer.strafeleft_left_end");
        public static int STRAFELEFT_RIGHT_END = Animator.StringToHash("BaseLayer.strafeleft_right_end");

        public static int STRAFERIGHT_START = Animator.StringToHash("BaseLayer.straferight_start");
        public static int STRAFERIGHT_LEFT_START = Animator.StringToHash("BaseLayer.straferight_left_start");
        public static int STRAFERIGHT_LEFT_PASS = Animator.StringToHash("BaseLayer.straferight_left_pass");
        public static int STRAFERIGHT_RIGHT_START = Animator.StringToHash("BaseLayer.straferight_right_start");
        public static int STRAFERIGHT_RIGHT_PASS = Animator.StringToHash("BaseLayer.straferight_right_pass");
        public static int STRAFERIGHT_RIGHT_END = Animator.StringToHash("BaseLayer.straferight_right_end");
        public static int STRAFERIGHT_LEFT_END = Animator.StringToHash("BaseLayer.straferight_left_end");

        public static int JOG_START = Animator.StringToHash("BaseLayer.jog_start");
        public static int JOG_LEFT_START = Animator.StringToHash("BaseLayer.jog_left_start");
        public static int JOG_LEFT_PASS = Animator.StringToHash("BaseLayer.jog_left_pass");
        public static int JOG_LEFT_END = Animator.StringToHash("BaseLayer.jog_left_end");
        public static int JOG_RIGHT_START = Animator.StringToHash("BaseLayer.jog_right_start");
        public static int JOG_RIGHT_PASS = Animator.StringToHash("BaseLayer.jog_right_pass");
        public static int JOG_RIGHT_END = Animator.StringToHash("BaseLayer.jog_right_end");

        public static int STRAFE_RIGHT_START = Animator.StringToHash("BaseLayer.strafe_right_start");
        public static int STRAFE_RIGHT_END = Animator.StringToHash("BaseLayer.strafe_right_end");
        public static int STRAFE_LEFT_START = Animator.StringToHash("BaseLayer.strafe_left_start");
        public static int STRAFE_LEFT_END = Animator.StringToHash("BaseLayer.strafe_left_end");

        public static int SIT_DOWN = Animator.StringToHash("BaseLayer.sit_down");
        public static int SIT_UP = Animator.StringToHash("BaseLayer.sit_up");
        public static int SIT_IDLE = Animator.StringToHash("BaseLayer.sit_idle");

        public static int IDLE = Animator.StringToHash("BaseLayer.idle");
        public static int IDLE_KICK = Animator.StringToHash("BaseLayer.idle_kick");
        public static int JOG_KICK = Animator.StringToHash("BaseLayer.jog_kick");

        public static int TURN_RIGHT = Animator.StringToHash("BaseLayer.turn_right");
        public static int TURN_LEFT = Animator.StringToHash("BaseLayer.turn_left");

        void OnEnable()
        {
            if (transform.parent != null)
            {
                for (int i = 0; i < mockMeshes.Length; i++)
                {
                    if (transform.parent.tag == "Actor")
                    {
                        if (mockMeshes[i].GetComponent<SkinnedMeshRenderer>())
                            mockMeshes[i].GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        else
                            mockMeshes[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

                        vrcullMeshes[i].SetActive(false);
                    }
                    else if (transform.parent.tag == "Avatar")
                    {
                        if (mockMeshes[i].GetComponent<SkinnedMeshRenderer>())
                            mockMeshes[i].GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                        else
                            mockMeshes[i].GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                        vrcullMeshes[i].SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("CharacterState: Parent needs to be tagged as Actor or Avatar! (occuring in index: " + i + ")");
                    }
                }
            }
        }

        public void SetAnimationSpeed(float inputSpeed)
        {
            GetComponent<Animator>().speed = inputSpeed;
        }

        public void PauseAnimation()
        {
            SetAnimationSpeed(0);
        }

        public void UnpauseAnimation()
        {
            SetAnimationSpeed(1f);
        }

        public int GetCurrentState(int inputLayer)
        {
            return GetComponent<Animator>().GetCurrentAnimatorStateInfo(inputLayer).fullPathHash;
        }

        public void PlaySound(AudioClip audioFile)
        {
            GetComponent<AudioSource>().PlayOneShot(audioFile, 1f);
        }
    }
}
