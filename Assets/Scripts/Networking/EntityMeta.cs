using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Auroraland{

	public class EntityMeta{
		public bool Static;
		public ColorData Color;
	}

	public struct SpaceLeaveMessage
	{
		public string user_id;
		public string entity_id;

	}
	public struct ColorData{
		public float r;
		public float g;
		public float b;
		public float a;
	}

	public struct AvatarModelData{
		public Vector3 eyePosition;
	}

	public struct AvatarMetaData{
		public AnimatorParameters animatorParameters;
		public IKData ik;
	}

	public struct AnimatorParameters{
		public bool isRunKeyPressed;
		public bool wasSitKeyPressed;
		public bool wasKickKeyPressed;
		public float userInputMove;
		public float userInputTurn;
		public float userInputStrafe;
		public bool isFrontAgainstWall;
		public bool isBackAgainstWall;
		public bool isGrounded;
		public bool isMinimumTurnReached;
		public int intensityOfHeadTurn;
		public bool isLeftHandCasting;
		public bool isRightHandCasting;
		public bool isLeftHandClosing;
		public bool isRightHandClosing;

	}

	public struct IKData{
		public Vector3 lookAtPosition;
		public Vector3 leftHandPosition;
		public Quaternion leftHandRotation;
		public Vector3 rightHandPosition;
		public Quaternion rightHandRotation;
	}
}
