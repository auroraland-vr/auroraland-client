using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using VRTK;

namespace Auroraland{
    public class InteractableAsset : VRTK_InteractableObject
    {
        public Color NormalHighlightColor = new Color(0f,0.99f,0f,0.054f);//new Color(0.65f, 0.97f, 0.08f);
        public Color DeleteHighlightColor = new Color(0.83f, 0.19f, 0.19f, 0.054f);
        public float scaleDelta = 0.01f;
        [SerializeField] private bool isDeleteMode = false;

        public void Initialize()
        {
            disableWhenIdle = false;
            isUsable = true;
            pointerActivatesUseAction = true;
            holdButtonToUse = false;
            useOnlyIfGrabbed = false;

            isGrabbable = true;
            holdButtonToGrab = true;
            stayGrabbedOnTeleport = true;
            validDrop = ValidDropTypes.DropAnywhere;
            grabOverrideButton = VRTK_ControllerEvents.ButtonAlias.Undefined;
            allowedGrabControllers = AllowedController.Both;
            grabAttachMechanicScript = transform.GetComponent<VRTK.GrabAttachMechanics.VRTK_FixedJointGrabAttach>();
            secondaryGrabActionScript = transform.GetComponent<VRTK.SecondaryControllerGrabActions.VRTK_SwapControllerGrabAction>();
        }

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            if(isDeleteMode)DeleteAsset();
        }

        public void EnableDeleteMode(bool isOn)
        {
            isDeleteMode = isOn;
            if (isDeleteMode)
            {
                touchHighlightColor = DeleteHighlightColor;
            }
            else {
                touchHighlightColor = NormalHighlightColor;
            }
        }

        public void DeleteAsset()
        {
            INEntity entity = GetComponent<NetObject>().GetNEntity();
            NKController.Instance.DeleteEntity(entity);
            gameObject.SetActive(false);
        }
        /*
        Bounds CalculateBounds()
        {

            Vector2 x = new Vector2(int.MaxValue, int.MinValue); //x.x: min of x, x.y: min of x
            Vector2 y = new Vector2(int.MaxValue, int.MinValue); //y.x: min of y, y.y: min of y
            Vector2 z = new Vector2(int.MaxValue, int.MinValue); //z.x: min of z, z.y: min of z

            foreach (var collider in Target.GetComponentsInChildren<Collider>(true))
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("UI")) continue; // skip UI layer components
                if (collider.transform.position.x + collider.bounds.max.x > x.y)
                { // max value of x
                    x.y = collider.transform.position.x + collider.bounds.max.x;
                }
                if (collider.transform.position.x + collider.bounds.min.x < x.x) // min value of x
                {
                    x.x = collider.transform.position.x + collider.bounds.min.x;
                }
                if (collider.transform.position.y + collider.bounds.max.y > y.y) // min value of y
                {
                    y.y = collider.transform.position.y + collider.bounds.max.y;
                }
                if (collider.transform.position.y + collider.bounds.min.y < y.x) // min value of y
                {
                    y.x = collider.transform.position.y + collider.bounds.min.y;
                }
                if (collider.transform.position.z + collider.bounds.max.z > z.y) // max value of z
                {
                    z.y = collider.transform.position.z + collider.bounds.max.z;
                }
                if (collider.transform.position.z + collider.bounds.min.z < z.x) // min value of z
                {
                    z.x = collider.transform.position.z + collider.bounds.min.z;
                }
            }
            Vector3 center = Vector3.zero;
            Vector3 size = Vector3.zero;
            size.x = Mathf.Abs(x.y - x.x);
            size.y = Mathf.Abs(y.y - y.x);
            size.z = Mathf.Abs(z.y - z.x);
            center.x = (x.y + x.x) / 2 - Target.transform.position.x;
            center.y = (y.y + y.x) / 2 - Target.transform.position.y;
            center.z = (z.y + z.x) / 2 - Target.transform.position.z;
            Bounds bounds = new Bounds(center, size);
            //Debug.Log(Target.name + " bounds:"+ bounds);
            return bounds;
        }

        public void SetUIPosition(){
            bounds = CalculateBounds();
            height = bounds.max.y + HeightOffset;
            transform.position = new Vector3 ( Target.transform.position.x, height , Target.transform.position.z);
		}
        */
    }

}