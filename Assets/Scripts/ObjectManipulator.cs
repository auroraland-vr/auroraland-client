using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace Auroraland
{
    /// <summary>
    /// Enables the ability to rotate and scale objects;
    /// This script will exist on the AttachPoint gameobject
    /// </summary>
    public class ObjectManipulator : MonoBehaviour
    {
        public enum Direction : int { Up, Down, Left, Right, Clockwise, CounterClockwise, Backward, Forward };   // Used to interface with external scripts

        private static float rotateBy = 15.0f;   // Degrees to increment rotation by
        private static float scaleBy = 0.05f;   // Amount to increment scale by
        private static float depthBy = 0.2f;    // Amount to increment depth
        private static float minScale = 0.1f;   // Minimum scale value
        private static float minDepth = 0.025f;   // Minimum depth value
        private static float maxDepth = 10f;   // Maximum depth value

        private GameObject rotator;     // Used to apply rotations to the object
        private Transform rotateRef;    // Point of reference to apply rotations from
        private float yRotation;        // Input to rotate object left/right
        private float xRotation;        // Input to rotate object clockwise/counterclockwise
        private float zRotation;        // Input to rotate object forward/backwards
        private float scale;            // Input to scale object larger/smaller
        private float depth;            // The distance between object and the user  
        /// <summary>
        /// Performs a rotation in the given direction in 2D screen space
        /// </summary>
        /// <param name="direction">The direction to rotate the object</param>
        public void DoRotate(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    yRotation += rotateBy;
                    break;
                case Direction.Right:
                    yRotation -= rotateBy;
                    break;
                case Direction.CounterClockwise:
                    zRotation -= rotateBy;
                    break;
                case Direction.Clockwise:
                    zRotation += rotateBy;
                    break;
                case Direction.Forward:
                    xRotation += rotateBy;
                    break;
                case Direction.Backward:
                    xRotation -= rotateBy;
                    break;
            }
            ApplyRotation();
            ResetInput();
        }

        /// <summary>
        /// Performs a uniform scaling up or down on the object
        /// </summary>
        /// <param name="direction">The up or down direction to scale the object</param>
        public void DoScale(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    scale += scaleBy;
                    break;
                case Direction.Down:
                    scale -= scaleBy;
                    break;
            }
            ApplyScale();
            ResetInput();
        }
        /// <summary>
        /// Moves the object in the z direction (depth) to push this object away or bring this object to the user
        /// </summary>
        /// <param name="direction">The Backward or Forward direction to push or bring the object</param>
        public void DoMove(Direction direction)
        {
            switch (direction)
            {
                case Direction.Backward:
                    depth += depthBy;
                    break;
                case Direction.Forward:
                    depth -= depthBy;
                    break;
            }
            ApplyMove();
            ResetInput();
        }

        private void Start()
        {
            // Initialize rotator object as a child of the attach point
            rotator = new GameObject("Rotator");
            rotator.transform.SetParent(transform);
            ResetInput();
        }

        private void Update()
        {
            if (rotateRef == null) {    // Ensure that point of reference is initialized
                if (VRTK_DeviceFinder.HeadsetCamera())
                    rotateRef = VRTK_DeviceFinder.HeadsetCamera().transform;
                else return;
            }

            // The rotator will always face the rotation reference point
            rotator.transform.LookAt(rotateRef);
            rotator.transform.localPosition = Vector3.zero;

            TestInput();
            ResetInput();
        }

        /// <summary>
        /// Accepts keyboard input for testing the object manipulation system
        /// </summary>
        private void TestInput()
        {
            // Input for rotation forwards/backwards
            if (Input.GetKey(KeyCode.Keypad5))
                DoRotate(Direction.Backward);
            if (Input.GetKey(KeyCode.Keypad2))
                DoRotate(Direction.Forward);

            // Input for rotation left/right
            if (Input.GetKey(KeyCode.Keypad1))
                DoRotate(Direction.Left);
            if (Input.GetKey(KeyCode.Keypad3))
                DoRotate(Direction.Right);


            // Input for rotation clockwise/counterclockwise
            if (Input.GetKey(KeyCode.Keypad4))
                DoRotate(Direction.CounterClockwise);
            if (Input.GetKey(KeyCode.Keypad6))
                DoRotate(Direction.Clockwise);

            // Input for scaling larger/smaller
            if (Input.GetKey(KeyCode.Keypad7))
                DoScale(Direction.Down);
            if (Input.GetKey(KeyCode.Keypad8))
                DoScale(Direction.Up);


            // Input for moving the object closer/further
            if (Input.GetKey(KeyCode.KeypadDivide))
                DoMove(Direction.Forward);
            if (Input.GetKey(KeyCode.KeypadMultiply))
                DoMove(Direction.Backward);
        }

        /// <summary>
        /// Internal function used to apply the rotation along the X/Y/Z axes
        /// </summary>
        private void ApplyRotation()
        {
            // Swap the parent-child relationship of the rotator and attach point
            rotator.transform.parent = transform.parent;
            transform.parent = rotator.transform;

            // Apply the rotation to the rotator object along the X/Y/Z axes
            rotator.transform.Rotate(xRotation, yRotation, zRotation);
            //rotator.transform.Rotate(yRotation, 0f, xRotation); //rotation along the X/Y axes

            // Swap the parent-child relationship back to original
            transform.parent = rotator.transform.parent;
            rotator.transform.parent = transform;
        }

        /// <summary>
        /// Internal function used to apply the scale operation on the object
        /// </summary>
        private void ApplyScale()
        {
            // Apply the scale to the actual object
            GameObject obj = transform.parent.gameObject.GetComponent<VRTK_InteractGrab>().GetGrabbedObject();
            if (!obj) return;
            Vector3 scaleVec = obj.transform.localScale;
            scaleVec.x += scale;
            scaleVec.y += scale;
            scaleVec.z += scale;
            if (scaleVec.x < minScale) // Clamp small scale values
                scaleVec = new Vector3(minScale, minScale, minScale);
            obj.transform.localScale = scaleVec;
        }

        /// <summary>
        /// Internal function used to apply the move operation on the object
        /// </summary>
        private void ApplyMove()
        {
            // Apply the scale to the actual object
            GameObject obj = transform.parent.gameObject.GetComponent<VRTK_InteractGrab>().GetGrabbedObject();
            if (!obj) return;
            float currentDepth = transform.localPosition.z; //get the attach point's z position
            float currentScale = transform.lossyScale.z; //get the attach point's z scale
            depth = Mathf.Clamp(currentDepth + depth, minDepth/currentScale, maxDepth/currentScale);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, depth);
        }

        /// <summary>
        /// Internal function to reset the input values to zero
        /// </summary>
        private void ResetInput()
        {
            xRotation = 0.0f;
            yRotation = 0.0f;
            zRotation = 0.0f;
            scale = 0.0f;
            depth =0.0f;
        }
    }
}


