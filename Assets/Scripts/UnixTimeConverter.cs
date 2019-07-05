using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnixTimeConverter{

	public static DateTime UnixTimeStampToDateTime(double unixTimeStamp){
		DateTime epoch = new DateTime (1970,1,1,0,0,0,0, DateTimeKind.Utc);
		DateTime dt = epoch;
		dt = dt.AddMilliseconds (unixTimeStamp).ToLocalTime ();
		return dt;
	}

}
