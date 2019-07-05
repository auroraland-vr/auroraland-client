using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AOT;

namespace TencentMobileGaming
{
    public class QAVSDKInit
    {
        public static void InitSDK()
        {
            if (inited == false)
            {
                lock (sLock) {
                    if (inited == false) {
#if UNITY_ANDROID && !UNITY_EDITOR
                        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

                        AndroidJavaObject curActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject javaObj = new AndroidJavaObject("com.tencent.av.wrapper.OpensdkGameWrapper", curActivity);
                        bool isSuc = javaObj.Call<bool>("initOpensdk");
                        Debug.Log("initOpensdk Complete." + isSuc);
#endif
                        inited = true;
                    }
                }
            }
            return;
        }
        
        private static Boolean inited = false;
        private static readonly System.Object sLock = new System.Object();
    }
}
