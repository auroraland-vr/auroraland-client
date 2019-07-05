using UnityEngine.PostProcessing;
using UnityEngine;
using VRTK;

namespace Auroraland
{
    /// <summary>
    /// The dynamic vignette system serves to combat motion sickness in VR
    /// by increasing/decreasing the vignette intensity based on how far
    /// the headset was rotated or positioned.
    /// </summary>
    public class DynamicVignette : MonoBehaviour
    {
        float shrinkSpeed = 1f;    // Speed at which vignette will shrink
        float growSpeed = 0.5f;      // Speed at which vignette will grow

        float minIntensity = 0.4f;  // Minimum intensity for vignette
        float maxIntensity = 1f;   // Maximum intensity for vignette
        float rotationThreshold = 0.2f;    // Amount of difference in rotations/positions to invoke intensity change
        float positionThreshold = 0.01f;    // Amount of difference in rotations/positions to invoke intensity change
        float lingerTime = 0.1f; // Amount of time to stay in that vignette intensity
        private float lingerAccumulator = 0;

        private AvatarController avatarController;
        private float intensity;    // Current intensity for vignett
        private Quaternion lastRotation;    // Value of last headset rotation
        private Vector3 lastPosition;       // Value of last headset position

        private bool isInitialized; // Determines whether variables have been initialized

        private Transform cameraTransform;                  // The headset camera's transform
        private PostProcessingProfile postProcess;          // The post processing profile used by the camera
        private VignetteModel.Settings originalSettings;    // The original vignette settings for post processing profile

        private void Start()
        {
            isInitialized = false;
            avatarController = GetComponent<AvatarController>();
        }

        void Update()
        {
            if (string.Equals(SDK_SetupMode.Instance.CurrentSDKName, "NonVR"))
                return; // Do not run this script on NonVR

            
            if (!isInitialized)
            {
                Initialize();
                return;
            }

            /*
            float rotationDifference = ComputeDifference(cameraTransform.rotation);
            float positionDifference = ComputeDifference(cameraTransform.position);

            if (positionDifference > positionThreshold || avatarController.LazyTurn != 0)
            {
                lingerAccumulator = 0;
                ChangeIntensityBy(growSpeed * Time.deltaTime);
                SetIntensity();
            }
            else if (lingerAccumulator >= lingerTime)
            {
                ChangeIntensityBy(-(shrinkSpeed * Time.deltaTime));
                SetIntensity();
            }
            else
                lingerAccumulator += Time.deltaTime;
                */
        }

        private void OnDestroy()
        {
            if (isInitialized)  // Reset original settings for post processing profile
                postProcess.vignette.settings = originalSettings;
        }

        /// <summary>
        /// Initializes the dynamic vignette system for the first time
        /// </summary>
        private void Initialize()
        {
            // Set headset camera transform
            cameraTransform = VRTK_DeviceFinder.HeadsetCamera();
            if (!cameraTransform) return;

            // Set initial position and rotation values
            lastPosition = cameraTransform.position;
            lastRotation = cameraTransform.rotation;

            PostProcessingBehaviour behaviour = cameraTransform.gameObject.GetComponent<PostProcessingBehaviour>();

            if (behaviour != null)
            {
                if (behaviour.profile != null)  // Set the post processing profile
                    postProcess = behaviour.profile;
                else
                {
                    postProcess = new PostProcessingProfile();
                    behaviour.profile = postProcess;
                }
            }
            else
            {  // Post Processing behaviour does not exist, so create one
                behaviour = cameraTransform.gameObject.AddComponent<PostProcessingBehaviour>();
                postProcess = new PostProcessingProfile();
                behaviour.profile = postProcess;
            }

            // Set custom values for vignette settings
            // TODO Restore vignetting?
            /*
            VignetteModel.Settings settings = postProcess.vignette.settings;
            originalSettings = settings;    // Save original settings
            settings.color = Color.black;
            settings.rounded = true;
            settings.smoothness = 1.0f;
            postProcess.vignette.settings = settings;
            postProcess.vignette.enabled = true;
            
            */

            AntialiasingModel.Settings aaSettings = postProcess.antialiasing.settings;
            aaSettings.method = AntialiasingModel.Method.Fxaa;
            aaSettings.fxaaSettings.preset = AntialiasingModel.FxaaPreset.ExtremeQuality;
            postProcess.antialiasing.settings = aaSettings;
            postProcess.antialiasing.enabled = true;
            postProcess.eyeAdaptation.enabled = true;
            postProcess.colorGrading.enabled = true;

            // SetIntensity(minIntensity);

            isInitialized = true;   // Initialization complete
        }

        /// <summary>
        /// Sets the intensity value for the vignette settings 
        /// </summary>
        /// <param name="newIntensity">Intensity value to set, if provided</param>
        private void SetIntensity(float newIntensity = -1.0f)
        {
            VignetteModel.Settings settings = postProcess.vignette.settings;
            if (newIntensity > 0.0f) intensity = newIntensity;
            settings.intensity = intensity;
            postProcess.vignette.settings = settings;
        }

        /// <summary>
        /// Changes the class intensity value by difference
        /// </summary>
        /// <param name="difference"></param>
        private void ChangeIntensityBy(float difference)
        {
            intensity += difference;
            intensity = Mathf.Clamp(intensity, minIntensity, maxIntensity);
        }

        /// <summary>
        /// Computes a difference between the previous rotation and
        /// the provided one
        /// </summary>
        /// <param name="rotation">Rotation value to test against</param>
        /// <returns>Difference heuristic value</returns>
        private float ComputeDifference(Quaternion rotation)
        {
            float difference = 0.0f;

            if (!isInitialized) return difference;

            difference = Quaternion.Angle(rotation, lastRotation);
            lastRotation = rotation;

            return difference;
        }

        /// <summary>
        /// Computes a difference between the previous position and
        /// the provided one
        /// </summary>
        /// <param name="position">Position value to test against</param>
        /// <returns>Difference heuristic value</returns>
        private float ComputeDifference(Vector3 position)
        {
            float difference = 0.0f;
            if (!isInitialized) return difference;

            difference = Vector3.Distance(position, lastPosition);
            lastPosition = position;

            return difference;
        }
    }
}
