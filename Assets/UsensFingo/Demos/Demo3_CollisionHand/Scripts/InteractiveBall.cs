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
using System.Collections.Generic;

public class InteractiveBall : MonoBehaviour
{
    public Material highlightMaterial;
    
    [Tooltip("Highest audio volume if the speed that ball hits wall is greater than this value.")]
    public float fastSpeed = 30f;
    [Tooltip("No audio is played if the speed that ball hits wall is less than this value.")]
    public float slowSpeed = 2f;

    private AudioSource audioSource;
    private float defaultVolume;

    private MeshRenderer targetRenderer;
    private Material defaultMaterial;

    private int numOfHandCollidersInContact = 0;
    private float lastTimeInContactWithHand = -1f;
    private float timeThresh = 0.5f;

	void Awake()
	{
		audioSource = this.GetComponent<AudioSource>();
        if (audioSource != null)
            defaultVolume = audioSource.volume;

        targetRenderer = this.GetComponent<MeshRenderer>();
        if (targetRenderer != null)
            defaultMaterial = targetRenderer.material;

        if (highlightMaterial == null)
            highlightMaterial = defaultMaterial; // use default if it is not specified
    }

	void Update()
	{
		float currentTime = Time.time;
		if ((lastTimeInContactWithHand < 0) ||
			(currentTime - lastTimeInContactWithHand) > timeThresh /* not in contact with hand for a little while */)
        {
            if (targetRenderer != null)
                targetRenderer.material = defaultMaterial;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		Transform parentOfOther = collision.gameObject.transform.parent;
        string parentOfOtherName = (parentOfOther != null) ? parentOfOther.name : "";
	
		if (parentOfOtherName == "CollisionHand_L" ||
            parentOfOtherName == "CollisionHand_R") // collide with a hand
        {
            lastTimeInContactWithHand = Time.time;
            numOfHandCollidersInContact++;

            if (targetRenderer != null)
                targetRenderer.material = highlightMaterial;
        }
        else if (parentOfOtherName == "Room") // collide with a wall
        {
            if (audioSource != null)
            {
                float speed = collision.relativeVelocity.magnitude;
                if (speed > slowSpeed && audioSource.isPlaying == false)
                {
                    audioSource.volume = 
                        (speed > fastSpeed) ? 
                        defaultVolume :
                        defaultVolume * (speed - slowSpeed) / (fastSpeed - slowSpeed);

                    audioSource.Play();
                }
            }
		}
	}

	void OnCollisionStay(Collision collision)
	{
		GameObject parentOfOtherObject = collision.gameObject.transform.parent.gameObject;

		if (parentOfOtherObject.name == "CollisionHand_L" ||
			parentOfOtherObject.name == "CollisionHand_R") // collide with a hand
        {
			lastTimeInContactWithHand = Time.time;
		}
	}

	void OnCollisionExit(Collision collision)
	{
		GameObject parentOfOtherObject = collision.gameObject.transform.parent.gameObject;

		if (parentOfOtherObject.name == "CollisionHand_L" ||
			parentOfOtherObject.name == "CollisionHand_R") // no longer in contact with a hand
        {
            numOfHandCollidersInContact--;

            if (numOfHandCollidersInContact == 0)
                lastTimeInContactWithHand = -1;
        }
	}
}