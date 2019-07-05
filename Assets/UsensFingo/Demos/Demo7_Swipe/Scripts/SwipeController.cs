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

[RequireComponent(typeof(SwipeableObject))]
public class SwipeController : MonoBehaviour
{
    public bool enableSwipeOnStart = true;

    public GameObject leftmostCube;
	public GameObject rightmostCube;
    public int numOfCubes = 9;

    public float horiAnimDuration = 0.1f;
    public float vertAnimDuration = 0.2f;
    public float alphaFadeDuration = 0.1f; // animate alpha after vertical animation is complete

    public float moveDamping = 0.3f;

    // The corresponding SwipeableObject component. Must not be null.
    private SwipeableObject swipeableObject;

    private Vector3 defaultPosition;
    private float xmin, xmax, ymin, ymax;
    
    private float disabledAlpha = 0.2f;
    private float highlightColorScale = 1.5f;

    private float delayBeforeEnablingSwipe = 0.5f;

    private float EPS = 1e-3f;


    void Awake()
    {
        swipeableObject = this.GetComponent<SwipeableObject>();
        defaultPosition = this.transform.position;
    }

    void Start()
    {
        // Calculate x boundaries
        if (leftmostCube != null)
            xmin = leftmostCube.transform.position.x;
        if (rightmostCube != null)
            xmax = rightmostCube.transform.position.x;

        // Calculate y boundaries.
        // Note that y dimensions are determined by all the swipeable lines.
        ymin = 1000f;
        ymax = -1f;
        foreach (SwipeableObject line in SwipeableObject.instances)
        {
            float y = line.transform.position.y;
            if (y < ymin)
                ymin = y;
            if (y > ymax)
                ymax = y;
        }
        
        // Enable/disable the corresponding swipeable object properly
        if (swipeableObject != null)
        {
            swipeableObject.SetTransparency(enableSwipeOnStart ? 1f : disabledAlpha);
            swipeableObject.IsAvailableForSwipe = enableSwipeOnStart;
        }
    }

    public void Activate()
    {
        if (swipeableObject != null)
            swipeableObject.SetHighlightColor(highlightColorScale);
    }

    public void Deactivate()
    {
        if (swipeableObject != null)
            swipeableObject.SetDefaultColor();
    }

    public void SwipeResponse(MoveDir moveDir)
    {
        if (moveDir == MoveDir.Left || moveDir == MoveDir.Right)
        {
            SwipeResponseHorizontal(moveDir);
        }
        else if (moveDir == MoveDir.Up || moveDir == MoveDir.Down)
        {
            SwipeResponseVertical(moveDir);
        }
    }

    void SwipeResponseHorizontal(MoveDir moveDir)
    {
        // distance between adjacent cubes
        float dx = (xmax - xmin) / (numOfCubes - 1);
        Vector3 destination = this.transform.position;

        switch (moveDir)
        {
            case MoveDir.Left:
                destination.x = Mathf.Max(destination.x - dx, defaultPosition.x - (xmax-xmin)/2); // move until boundary
                break;
            case MoveDir.Right:
                destination.x = Mathf.Min(destination.x + dx, defaultPosition.x + (xmax-xmin)/2); // move until boundary
                break;
        }
        StartCoroutine(DoAnimationHorizontal(destination));
    }

    void SwipeResponseVertical(MoveDir moveDir)
    {
        if (SwipeableObject.instances.Count <= 1)
            return;

        // distance between adjacent lines
        float dy = (ymax - ymin) / (SwipeableObject.instances.Count - 1);

        // Note, when swiping vertically, we are moving all the lines.
        foreach (SwipeableObject line in SwipeableObject.instances)
        {
            line.IsAvailableForSwipe = false;

            Vector3 destination = line.transform.position;
            switch (moveDir)
            {
                case MoveDir.Down:
                    destination.y = (destination.y - dy < ymin) ? ymax : (destination.y - dy); // wrap around
                    break;
                case MoveDir.Up:
                    destination.y = (destination.y + dy > ymax) ? ymin : (destination.y + dy); // wrap around
                    break;
            }
            StartCoroutine(DoAnimationVertical(line, destination));
        }
    }

    IEnumerator DoAnimationHorizontal(Vector3 destination)
    {
        if (swipeableObject == null || swipeableObject.IsAnimating)
            yield break;

        swipeableObject.IsAnimating = true;
        swipeableObject.OnSwipeAnimStarted.Invoke();

        StartCoroutine(AnimateToDestination(swipeableObject, destination, horiAnimDuration));
        
        // Add a short delay after horizontal animation completes
        yield return new WaitForSeconds(horiAnimDuration + delayBeforeEnablingSwipe);

        swipeableObject.IsAnimating = false;
        swipeableObject.OnSwipeAnimCompleted.Invoke();
    }

    IEnumerator DoAnimationVertical(SwipeableObject line, Vector3 destination)
    {
        if (line == null || line.IsAnimating)
            yield break;

        line.IsAnimating = true;
        line.OnSwipeAnimStarted.Invoke();

        StartCoroutine(AnimateYThenResetX(line, destination));
        
        yield return new WaitForSeconds(vertAnimDuration);
        
        bool inMiddle = (Mathf.Abs(destination.y - (ymax + ymin) / 2) < EPS);
        StartCoroutine(AnimateAlpha(line, inMiddle, alphaFadeDuration));
    }

    IEnumerator AnimateToDestination(SwipeableObject line, Vector3 destination, float duration)
    {
        Vector3 pos = transform.position;
        int steps = Mathf.Max(1, (int)(duration / 0.01f));
        for (int i = 0; i < steps; i++)
        {
            line.transform.position = Vector3.Lerp(pos, destination, (float)(i + 1) / (float)(steps));
            if (i < steps - 1)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator AnimateYThenResetX(SwipeableObject line, Vector3 destination)
    {
        // First animate to destination
        Vector3 pos = line.transform.position;
        int steps = Mathf.Max(1, (int)(vertAnimDuration / 0.01f));
        for (int i = 0; i < steps; i++)
        {
            line.transform.position = Vector3.Lerp(pos, destination, (float)(i + 1) / (float)(steps));
            if (i < steps - 1)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
        
        // Then reset X position as necessary
        if (Mathf.Abs(line.transform.position.x - defaultPosition.x) > EPS) // need to reset
        {
            destination = line.transform.position;
            destination.x = defaultPosition.x; // reset x

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(AnimateToDestination(line, destination, horiAnimDuration));
        }
    }

    IEnumerator AnimateAlpha(SwipeableObject line, bool inMiddle, float duration)
    {
        Color color = line.GetColor();
        float alpha = color.a;
        float targetAlpha = inMiddle ? 1f : disabledAlpha;

        int steps = Mathf.Max(1, (int)(duration / 0.01f));
        for (int i = 0; i < steps; i++)
        {
            float t = (float)(i + 1) / (float)(steps);
            line.SetTransparency(alpha * (1 - t) + targetAlpha * t);
            if (i < steps-1)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }

        line.IsAnimating = false;
        line.OnSwipeAnimCompleted.Invoke();

        // Add a short delay after alpha animation completes
        yield return new WaitForSeconds(delayBeforeEnablingSwipe);

        // Enable the line if it is in middle; otherwise disable it.
        line.IsAvailableForSwipe = inMiddle;
    }

    public void MoveResponse(float dist, MoveDir dir)
    {
        Vector3 displacement = Vector3.zero;
        switch (dir)
        {
            // Unlike fast swipe, in this sample, we only allow to move (slowly) in horizontal directions.
            case MoveDir.Left:
                displacement.x = -dist * moveDamping;
                break;
            case MoveDir.Right:
                displacement.x = dist * moveDamping;
                break;
        }
        transform.Translate(displacement);
	}
}
