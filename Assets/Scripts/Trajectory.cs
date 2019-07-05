using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland
{
	public class Trajectory: MonoBehaviour
	{
		public LineRenderer Line;
		public int Resolution;
		public float Angle;
		public float Velocity;
		public float Height;

		private float gravity;
		private float radianAngle;
		private float maxDistance;

		void Awake () {
			gravity = Mathf.Abs(Physics.gravity.y);	
		}

		public void RenderTrajectory(Vector3 force, float height){
			Velocity = Mathf.Sqrt (force.y * force.y + force.z * force.z);
			Angle = Mathf.Atan2 (force.y, force.z)* Mathf.Rad2Deg;
			Height = height;
			RenderArc ();

		}
		void RenderArc(){
			Line.positionCount = Resolution + 1;
			Vector3[] array = CalculateArcArray ();
			for (int i = 0; i <= Resolution; i++) {
				array [i] = new Vector3 (array[i].z, array[i].y, array[i].x);
			}
			Line.SetPositions(array);
		}

		Vector3[] CalculateArcArray(){
			Vector3[] arcArray = new Vector3[Resolution + 1];
			radianAngle = Mathf.Deg2Rad * Angle;
			maxDistance = Velocity * Mathf.Cos (radianAngle) / gravity * (Velocity * Mathf.Sin (radianAngle) + Mathf.Sqrt (Velocity * Velocity * Mathf.Sin (radianAngle) * Mathf.Sin (radianAngle) + 2 * gravity * Height));
			//(Velocity * Velocity * Mathf.Sin (2 * radianAngle)) / gravity;
			for (int i = 0; i <= Resolution; i++) {
				float t = (float)i / (float)Resolution;
				arcArray [i] = CalculateArcPoint (t, maxDistance);
			}
			return arcArray;
		}

		Vector3 CalculateArcPoint(float t , float maxDistance){
			float x = t * maxDistance;
			float y =  Height + x * Mathf.Tan(radianAngle) - ((gravity* x * x) / (2 * Velocity* Mathf.Cos(radianAngle)*Velocity* Mathf.Cos(radianAngle)));
			return new Vector3 (x, y);
		}
		
	}
}

