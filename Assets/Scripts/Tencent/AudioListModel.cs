
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TencentMobileGaming;

class AudioListModel
{
	public static AudioListModel GetInstance()
	{
		if (sInstance == null) {
			lock (sLock) {
				if (sInstance == null) {
					sInstance = new AudioListModel ();
				}
			}
		}
		return sInstance;
	}

	private AudioListModel()
	{
		ITMGContext.GetInstance().OnEnterRoomCompleteEvent += new QAVEnterRoomComplete(OnEnterRoomComplete);
		ITMGContext.GetInstance().OnExitRoomCompleteEvent += new QAVExitRoomComplete(OnExitRoomComplete);
		ITMGContext.GetInstance().OnRoomDisconnectEvent += new QAVRoomDisconnect(OnRoomDisconnect);
		ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent += new QAVEndpointsUpdateInfo(OnEndpointsUpdateInfo);
	}

	~AudioListModel()
	{
		ITMGContext.GetInstance().OnEnterRoomCompleteEvent -= new QAVEnterRoomComplete(OnEnterRoomComplete);
		ITMGContext.GetInstance().OnExitRoomCompleteEvent -= new QAVExitRoomComplete(OnExitRoomComplete);
		ITMGContext.GetInstance().OnRoomDisconnectEvent -= new QAVRoomDisconnect(OnRoomDisconnect);
		ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent -= new QAVEndpointsUpdateInfo(OnEndpointsUpdateInfo);
	}

	public void OnEnterRoomComplete(int result, string error_info)
	{

	}

	public void OnExitRoomComplete()
	{
		mForbiddedMembers.Clear();
		mSpeakingMembers.Clear();
	}

	public void OnRoomDisconnect(int result, string error_info)
	{
		mForbiddedMembers.Clear();
		mSpeakingMembers.Clear();
	}

	public void OnEndpointsUpdateInfo(int eventID, int count, string[] identifierList)
	{
		if (eventID == QAVContext.EVENT_ID_ENDPOINT_HAS_AUDIO) {
			foreach (string identifier in identifierList) {
				mSpeakingMembers.Add (identifier);
			}
		} else if (eventID == QAVContext.EVENT_ID_ENDPOINT_NO_AUDIO) {
			foreach (string identifier in identifierList) {
				mSpeakingMembers.Remove (identifier);
			}
		}
	}

	/*public int ForbidMemberVoice (string member, bool bEnable)
	{
		if (bEnable)
		{
			mForbiddedMembers.Add(member);
		}
		else
		{
			mForbiddedMembers.Remove(member);
		}
		
		string[] identifierList = new string[mForbiddedMembers.Count];
		mForbiddedMembers.CopyTo (identifierList);
		return ITMGContext.GetInstance().GetRoom ().UnrequestAudioList (identifierList);
	}*/

	public bool IsMemberVoiceForbidded (string member)
	{
		return mForbiddedMembers.Contains (member);
	}

	public string[] GetSpeakingMembers ()
	{
		string[] result = new string[mSpeakingMembers.Count];
		mSpeakingMembers.CopyTo (result);
		return result;
	}

	private HashSet<string> mSpeakingMembers = new HashSet<string> ();
	private HashSet<string> mForbiddedMembers = new HashSet<string> ();
	private static AudioListModel sInstance;
	private static readonly System.Object sLock = new System.Object ();

}
