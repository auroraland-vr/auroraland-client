using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Auroraland
{
    [RequireComponent(typeof(VRTK_Pointer))]
    public class PointerRenderSwitcher : MonoBehaviour
    {
        private VRTK_Pointer pointer;
        private VRTK_InteractGrab interactGrab;
        private VRTK_BezierPointerRenderer beizerRenderer;
        private BeizerPointerGrabController beizerGrabController;
        private VRTK_StraightPointerRenderer straightRenderer;
        private StraightPointerGrabController straightRendererGrabController;
        private Rigidbody attachPoint;
        private void Awake()
        {
            pointer = gameObject.GetComponent<VRTK_Pointer>();
            interactGrab = gameObject.GetComponent<VRTK_InteractGrab>();
            beizerRenderer = gameObject.GetComponent<VRTK_BezierPointerRenderer>();
            straightRenderer = gameObject.GetComponent<VRTK_StraightPointerRenderer>();
            beizerGrabController = gameObject.GetComponent<BeizerPointerGrabController>();
            straightRendererGrabController = gameObject.GetComponent<StraightPointerGrabController>();

            MainMenu.OnEnableMainMenu += SetUpStraightRenderer;
            MainMenu.OnDisableMainMenu += SetUpBeizerRenderer;
        }
        private void OnDestroy()
        {
            MainMenu.OnEnableMainMenu -= SetUpStraightRenderer;
            MainMenu.OnDisableMainMenu -= SetUpBeizerRenderer;
        }

        public void SetUpBeizerRenderer()
        {
            pointer.enabled = false;

            if (beizerRenderer == null)
            {
                beizerRenderer = gameObject.AddComponent<VRTK_BezierPointerRenderer>();
                beizerRenderer.maximumLength.x = 15;
                beizerRenderer.heightLimitAngle = 45;
            }
            else {
                beizerRenderer.enabled = true;
            }

            if (beizerGrabController == null)
            {
                beizerGrabController = gameObject.AddComponent<BeizerPointerGrabController>();
            }
            else
            {
                beizerGrabController.enabled = true;
            }
            

            if (straightRenderer) straightRenderer.enabled = false;
            if (straightRendererGrabController) straightRendererGrabController.enabled = false;

            pointer.pointerRenderer = beizerRenderer;
            pointer.enabled = true;
            /*pointer needs to be reenabled in order to regenerate new rigidbody attach point. 
            * In VRTK_BasePointer.OnEnable it's call AttemptSetController() and then it will call CreateObjectInteractor() to generate attach point*/

        }

        public void SetUpStraightRenderer()
        {
            pointer.enabled = false;
            if (straightRenderer == null)
            {
                straightRenderer = gameObject.AddComponent<VRTK_StraightPointerRenderer>();
            }
            else
            {
                straightRenderer.enabled = true;
            }

            if (straightRendererGrabController == null)
            {
                straightRendererGrabController = gameObject.AddComponent<StraightPointerGrabController>();
            }
            else
            {
                straightRendererGrabController.enabled = true;
            }
          
            if (beizerRenderer) beizerRenderer.enabled = false;
            if (beizerGrabController) beizerGrabController.enabled = false;

            pointer.pointerRenderer = straightRenderer;

            /*pointer needs to be reenabled in order to regenerate new rigidbody attach point. 
             * In VRTK_BasePointer.OnEnable it's call AttemptSetController() and then it will call CreateObjectInteractor() to generate attach point*/
            pointer.enabled = true;
        }
    }

}