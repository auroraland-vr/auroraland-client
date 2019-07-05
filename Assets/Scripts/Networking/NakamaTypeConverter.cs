using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;

namespace Auroraland{
	/*
	 * use for type casting 
	*/
	public static class NakamaTypeConverter{

		public static NVector3 Vector3ToNVector3(Vector3 vector){
			return new NVector3(){X = vector.x, Y = vector.y, Z = vector.z};
		}

		public static Vector3 INVector3ToVector3(INVector3 vector){
			return new Vector3 (vector.X, vector.Y, vector.Z);
		}

		public static NVector4 QuaternionToNVector4(Quaternion quaternion){
			return new NVector4(){ X = quaternion.x, Y = quaternion.y, Z = quaternion.z,  W = quaternion.w};
		}

		public static Quaternion INVector4ToQuaternion(INVector4 vector){
			return new Quaternion (vector.X, vector.Y, vector.Z, vector.W);
		}
			


		/*
		public static NSpaceUser INUserToNSpaceUser(INUser user){
			return new NSpaceUser(new SpaceUser(){ Id = user.Id, Handle = user.Handle, Fullname = user.Fullname, 
				                  Lang = user.Lang, Location = user.Location, Timezone = user.Timezone, Metadata = user.Metadata});
			
		}*/

	}
}
