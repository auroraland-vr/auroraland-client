/*************************************************************************\
*                           USENS CONFIDENTIAL                            *
* _______________________________________________________________________ *
*                                                                         *
* [2014] - [2017] USENS Incorporated                                      *
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fingo;

public enum SwipeState
{
    Idle,
    Activated,
    Moving,
    Swiping
}

// 4-way move direction
public enum MoveDir
{
    Right,
    Left,
    Up,
    Down,
    Stay    // not moving
}

// rectangle position relative to another rectangle
// x dimension
public enum RectRelativeToRect_X
{
    Overlap,
    Right,
    Left,
    Unknown
}
// y dimension
public enum RectRelativeToRect_Y
{
    Overlap,
    Above,
    Below,
    Unknown
}

public class SwipeHand : MonoBehaviour
{
    /// <summary>
    /// The hand type of the tracked hand.
    /// </summary>
    public HandType handType;

    private Hand hand;

    public UnityEvent OnSwipeSucceeded;

    [Tooltip("The minimum distance between hand and camera before you can swipe. In meters.")]
    public float swipeEnableDistanceFromCam = 0.3f;

    // Threshold values to dertermine if the user is swiping the object fast (v.s. moving it slowly)
    const string help = "To swipe the object fast, the speed of the hand in the swiping direction must be greater than speedThresh (m/s)" +
        " AND the distance the hand moved continously in the swiping direction must be greater than distThresh (m).";
    [Tooltip(help)]
    public float speedThresh = 0.1f;    // 10 cm / second
    [Tooltip(help)]
    public float distThresh = 0.08f;    // 8 cm

    public float halfHandWidth = 0.03f;
	public float halfHandHeight = 0.03f;


    private SwipeableObject activeObject = null;

    private SwipeState swipeState = SwipeState.Idle;

    private MoveDir moveDir = MoveDir.Stay;
    private MoveDir lastMoveDir = MoveDir.Stay;

    private float distCurFrame = 0;
    private float durationInOneDirection = 0;
    private float distInOneDirection = 0;

    // Track when object becomes out of reach
    private float lastTimeObjectInReach = -1;
    private float timeThreshForReset = 0.5f;

    // Track how long hand touches the object and is not moving
    private float timeDurationBeforeMoving = 0;
    private float timeThreshForSettingStartPos = 0.2f;

    // Delegate for calculating swiping velocity
    private SwipeVelocityCalculator velocityCalculator;

    // Delegate for tracking hand detetion lost and recover time, as well as if hand is currently inside camera view.
    private HandVisibilityTracker handVisibilityTracker;
    private float handLostTime = -1;
    private float handRecoverTime = -1;
    private bool handOnscreen = true;

    private bool handIsLost = false;
    private bool handIsLostLastFrame = false;

    // The render manager for updating hand material at run-time according to current state
    private MeshHandRenderManager meshHandRenderMgr;


    void Awake()
    {
        velocityCalculator = this.GetComponent<SwipeVelocityCalculator>();
        meshHandRenderMgr = this.GetComponent<MeshHandRenderManager>();
        handVisibilityTracker = this.GetComponent<HandVisibilityTracker>();
    }

    void OnEnable()
    {
        swipeState = SwipeState.Idle;

        if (OnSwipeSucceeded == null)
        {
            OnSwipeSucceeded = new UnityEvent();
        }
    }
    
    void Update()
    {
        hand = FingoMain.Instance.GetHand(handType);

        // Track hand lost and recover time, as well as if palm is currently inside camera view.
        if (handVisibilityTracker != null)
        {
            handLostTime = handVisibilityTracker.HandLostTime;
            handRecoverTime = handVisibilityTracker.HandRecoverTime;
            handIsLost = handVisibilityTracker.HandIsLost;
            handOnscreen = handVisibilityTracker.PalmOnScreen();
        }

        if (hand.IsDetected())
        {
            // Use index tip position for calculating swipe direction
            this.transform.localPosition = hand.GetTipPosition(TipIndex.IndexTip);

            TrackSwipeStartPosition();
            UpdateSwipeStates();
        }

        if (handIsLost && !handIsLostLastFrame) // lost hand detection this frame
        {
            ResetAll();
        }
        handIsLostLastFrame = handIsLost;
    }

    void TrackSwipeStartPosition()
    {
        // Track how long hand touches the object and is not moving
        if (activeObject != null && CanStartSwiping(activeObject) &&
            moveDir == MoveDir.Stay)
        {
            timeDurationBeforeMoving += Time.deltaTime;
        }
        else
        {
            timeDurationBeforeMoving = 0; //reset
        }

        // Set swipe start position
        if (timeDurationBeforeMoving > timeThreshForSettingStartPos && 
            velocityCalculator != null)
        {
            velocityCalculator.SetSwipeStartPos(this.transform.position);
        }
    }

    void UpdateSwipeStates()
    {
        if (swipeState == SwipeState.Idle || swipeState == SwipeState.Activated)
        {
            SwipeableObject nextActiveObject = null;
            // Iterate all swipeable objects
            foreach (SwipeableObject obj in SwipeableObject.instances)
            {
                // find one that is currently available and in reach.
                if (obj.IsAvailableForSwipe && !obj.IsAnimating && 
                    CanStartSwiping(obj))
                {
                    nextActiveObject = obj;
                    break;
                }
            }

            if (activeObject != nextActiveObject)
            {
                if (nextActiveObject != null)
                {
                    swipeState = SwipeState.Activated;
                    Activate();
                }
                else
                {
                    swipeState = SwipeState.Idle;
                    Deactivate();
                }
                activeObject = nextActiveObject;
            }
        }

        if (swipeState != SwipeState.Idle)
        {
            UpdateSwipeData();

            // Calculate hand move speed
            float speed = (durationInOneDirection > 0) ? (distInOneDirection / durationInOneDirection) : 0;

            // It is a "fast" swipe, if (1) the speed of the hand in the swiping direction is greater than speedThresh (m/s),
            // AND (2) the distance the hand moved continously in the swiping direction is greater than distThresh (m).
            bool fastSwipe = (speed > speedThresh && distInOneDirection > distThresh);

            if (fastSwipe)
            {
                if (activeObject != null)
                    activeObject.OnSwipe.Invoke(moveDir);

                swipeState = SwipeState.Swiping;
                OnSwipeSucceeded.Invoke();
            }
            else if (speed > 0) // user is moving "slowly"
            {
                Vector3 objPos = activeObject.transform.position;
                Vector3 handPos = this.transform.position;
                // scale that projects hand dimensions to object depth
                float s = objPos.z / handPos.z;

                if (activeObject != null)
                    activeObject.OnMove.Invoke(distCurFrame * s, moveDir);

                swipeState = SwipeState.Moving;
            }
            else
            {
                swipeState = SwipeState.Activated;
            }
        }

        // Treat SwipeState.Moving and SwipeState.Swiping states differently to exit to default state properly.
        if (swipeState == SwipeState.Moving || swipeState == SwipeState.Activated)
        {
            // Track if the current active object is in reach
            if (activeObject != null && CanStartSwiping(activeObject))
            {
                lastTimeObjectInReach = Time.time;
            }
            // Reset when the current active object is out of reach for a short period of time
            if (lastTimeObjectInReach > 0 && Time.time - lastTimeObjectInReach > timeThreshForReset) 
            {
                ResetAll();
            }
        }
        // Note, we handle the case of SwipeState.Swiping in Unity.
        // When SwipeableObject.OnSwipeAnimStarted event is invoked, ResetAll() will be called.


        //Debug.Log("swipeState = " + swipeState);
    }


    /// <summary>
    /// Update swipe relevant data, including moving direction, time duration user has moved in that direction,
    /// and distance (both current frame and accumulated) user has moved in that direction.
    /// </summary>
    void UpdateSwipeData()
    {
        Vector3 velocity = Vector3.zero;
        if (velocityCalculator != null)
        {
            // Calculate hand moving velocity based on palm position.
            velocityCalculator.UpdatePositionData(this.transform.position, handLostTime, handRecoverTime);
            velocity = velocityCalculator.CalculateVelocity();
        }

        // Calculate moving direction and distance (current frame) from velocity.
        CalculateMoveDirectionAndDistance(velocity);

        // Accumulate time duration and distance if user was moving in the same direction last frame.
        if (moveDir == lastMoveDir && moveDir != MoveDir.Stay)
        {
            durationInOneDirection += Time.deltaTime;
            distInOneDirection += distCurFrame;
        }
        else // otherwise, reset data.
        {
            durationInOneDirection = 0;
            distInOneDirection = distCurFrame;
        }

        lastMoveDir = moveDir;
    }

    /// <summary>
    /// Given a 3D velocity vector, calculate moving direction (left, right, up, down, or not moving) 
    /// as well as moving distance in that direction this frame.
    /// </summary>
    void CalculateMoveDirectionAndDistance(Vector3 velocity)
    {
        if (Vector3.Magnitude(velocity) < 1e-3)
        {
            moveDir = MoveDir.Stay;
            distCurFrame = 0;
        }
        else
        {
            float absx = Mathf.Abs(velocity.x);
            float absy = Mathf.Abs(velocity.y);
            if (absx > absy)
            {
                moveDir = (velocity.x > 0) ? MoveDir.Right : MoveDir.Left;
                distCurFrame = absx * Time.deltaTime;
            }
            else
            {
                moveDir = (velocity.y > 0) ? MoveDir.Up : MoveDir.Down;
                distCurFrame = absy * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Reset swipe relevant data, including reseting moving direction to MoveDir.Stay, time duration user has moved 
    /// in same direction to 0, and also accumulated distance user has moved in same direction to 0.
    /// </summary>
    void ResetSwipeData()
    {
        durationInOneDirection = 0;
        distInOneDirection = 0;
        moveDir = MoveDir.Stay;
        lastMoveDir = MoveDir.Stay;
    }

    /// <summary>
    /// Reset swipe state as well as swipe relevant data.
    /// </summary>
    public void ResetAll()
    {
        swipeState = SwipeState.Idle;

        ResetSwipeData();

        Deactivate();
    }

    void Activate()
    {
        if (activeObject != null)
            activeObject.OnActivate.Invoke();

        // To give user visual feedbacks, make the hand transparent.
        if (meshHandRenderMgr != null)
            meshHandRenderMgr.FadeOut();
    }

    void Deactivate()
    {
        if (activeObject != null)
            activeObject.OnDeactivate.Invoke();

        // To give user visual feedbacks, make the hand opaque as default.
        if (meshHandRenderMgr != null)
            meshHandRenderMgr.FadeIn();
    }

    ///////////////////////////////////////////////////////////////////////////////////////
    // helper functions

    /// <summary>
    /// Helper function to dertermine if hand is in position to start swiping.
    /// </summary>
    bool CanStartSwiping(SwipeableObject obj)
    {
        if (obj != null)
        {
            return (handOnscreen && InRange() && InBounds(obj));
        }
        return false;
    }

    bool InRange()
    {
        return (Vector3.Distance(this.transform.position, Camera.main.transform.position) > swipeEnableDistanceFromCam);
    }

    bool InBounds(SwipeableObject obj)
    {
        RectRelativeToRect_X xdim;
        RectRelativeToRect_Y ydim;
        if (HandOverlapsCollider(obj.boundingBox, out xdim, out ydim))
        {
            return true;
        }
        else
        {
            if (xdim == RectRelativeToRect_X.Right || xdim == RectRelativeToRect_X.Left)
            {
                if (moveDir == MoveDir.Right || moveDir == MoveDir.Left) // move horizontally
                    return (ydim == RectRelativeToRect_Y.Overlap);
                else
                    return false;
            }

            if (ydim == RectRelativeToRect_Y.Above || ydim == RectRelativeToRect_Y.Below)
            {
                if (moveDir == MoveDir.Up || moveDir == MoveDir.Down) // move vertically
                    return (xdim == RectRelativeToRect_X.Overlap);
                else
                    return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Utility function to determine if current hand bounds (defined by palm position, hand width and height)
    /// overlaps the bounds of the input collider.
    /// </summary>
    /// <returns> 
    /// Returns true if hand bounds overlaps collider bounds. 
    /// Also outputs hand position relative to the collider bounds. Outputs x and y dimensions respectively. 
    /// </returns>
    bool HandOverlapsCollider(Collider collider, out RectRelativeToRect_X xdim, out RectRelativeToRect_Y ydim)
    {
        if (collider == null || collider.bounds.size.x <= 0 || collider.bounds.size.y <= 0)
        {
            xdim = RectRelativeToRect_X.Unknown;
            ydim = RectRelativeToRect_Y.Unknown;
            return false;
        }

        // object dimensions in world space
        float objLeft = collider.bounds.min.x;
        float objRight = collider.bounds.max.x;
        float objBottom = collider.bounds.min.y;
        float objTop = collider.bounds.max.y;

        // hand dimensions in local hand space
        Vector3 palmPos = hand.GetPalmPosition();
        Vector3 handBoundsMin = new Vector3(palmPos.x - halfHandWidth, palmPos.y - halfHandHeight, palmPos.z);
        Vector3 handBoundsMax = new Vector3(palmPos.x + halfHandWidth, palmPos.y + halfHandHeight, palmPos.z);

        // hand dimensions in world space
        palmPos = transform.parent.TransformPoint(palmPos);
        handBoundsMin = transform.parent.TransformPoint(handBoundsMin);
        handBoundsMax = transform.parent.TransformPoint(handBoundsMax);

        // project hand dimensions to object depth
        float s = collider.bounds.center.z / palmPos.z;

        float handLeft = handBoundsMin.x * s;
        float handRight = handBoundsMax.x * s;
        float handBottom = handBoundsMin.y * s;
        float handTop = handBoundsMax.y * s;

        xdim = (handRight < objLeft) ? RectRelativeToRect_X.Left : ((handLeft > objRight) ? RectRelativeToRect_X.Right : RectRelativeToRect_X.Overlap);
        ydim = (handTop < objBottom) ? RectRelativeToRect_Y.Below : ((handBottom > objTop) ? RectRelativeToRect_Y.Above : RectRelativeToRect_Y.Overlap);

        return (xdim == RectRelativeToRect_X.Overlap && ydim == RectRelativeToRect_Y.Overlap);
    }
}