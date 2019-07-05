/*************************************************************************\
*                           USENS CONFIDENTIAL                            *
* _______________________________________________________________________ *
*                                                                         *
* [2015] - [2017] USENS Incorporated                                      *
* All Rights Reserved.                                                    *
*                                                                         *
* NOTICE:  All information contained herein is, and remains               *
* the property of uSens Incorporated and its suppliers,                   *
* if any.  The intellectual and technical concepts contained              *
* herein are proprietary to uSens Incorporated                            *
* and its suppliers and may be covered by U.S. and Foreign Patents,       *
* patents in process, and are protected by trade secret or copyright law. *
* Dissemination of this information or reproduction of this material      *
* is strictly forbidden unless prior written permission is obtained       *
* from uSens Incorporated.                                                *
*                                                                         *
\*************************************************************************/

using UnityEngine;
using System.Collections;


namespace Fingo
{
    /// <summary>
    /// FingoImage is a script updating the transform and material of the 
    /// gameobject which contain a quad Mesh Renderer. The quad will show 
    /// the image got from Fingo Device.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class FingoImage : MonoBehaviour
    {
        [Tooltip("The eye type of the image, left right or invalid.")]
        public EyeType EyeType; //!< The eye type of the image, left right or invalid.
        [Tooltip("The type of the Fingo which the image come from, Mono or RGB.")]
        public FingoDeviceType FingoType; //!< The type of the Fingo which the image come from, Mono or RGB.
        [Tooltip("The distance of the canvas which render the image.")]
        public float canvasDistance = 2.0f; //!< The distance of the canvas which render the image.

        private Material imageMaterial;

        private Image image;

        private bool isSetDistance = false;

        void Awake()
        {
			image = new Image();
            Shader imageShader = Shader.Find("Standard");
            if (FingoType == FingoDeviceType.Mono)
            {
                imageShader = Shader.Find("Fingo/FingoMONOImage_Transparent");
            }
            else
            {
                imageShader = Shader.Find("Fingo/FingoRGBImage");
            }
            imageMaterial = new Material(imageShader);
            GetComponent<Renderer>().material = imageMaterial;
            
        }

        void Update()
        {
            if(FingoType == FingoDeviceType.Mono)
            {
                image = FingoMain.Instance.GetInfraredImage(EyeType);
            }
            else if(FingoType == FingoDeviceType.RGB)
            {
                image = FingoMain.Instance.GetRGBImage(EyeType);
            }
            if (image != null)
            {
                if (!isSetDistance)
                {
                    image.SetDistance(canvasDistance);
                    isSetDistance = true;
                }
                imageMaterial.mainTexture = image.GetTexture();
                transform.localPosition = image.position;
                transform.localScale = image.scale;
            }
        }
    }
}
