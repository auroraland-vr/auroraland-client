using Auroraland;
using Nakama;
using System.Collections;
using System.Collections.Generic;
using TencentMobileGaming;
using UnityEngine;

public class VoiceChatSpatialSoundManager : MonoBehaviour
{
    private Dictionary<string, INEntity> entityDict = new Dictionary<string, INEntity>();

    private string identifier = "";
    private int spaceIdentifier = -1;
    private string selfUserId = "";
    private bool isCoroutineStarted = false;
    private Vector3 selfPlayerPosition;
    private Vector3 selfPlayerRotation;

    //private static VoiceChatSpatialSoundManager instance;
    public static VoiceChatSpatialSoundManager Instance;
    //public static VoiceChatSpatialSoundManager Instance { get { return instance; } private set{ Instance = instance;}}

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ITMGContext.GetInstance().Poll();

        // update for spatial sound
        if (!isCoroutineStarted)
        {
            StartCoroutine(GetSelfPlayerInfo());
        }

        // update the most recent entire user <-> entity dictionary
        entityDict = SpaceManager.Instance.GetAvatarEntityDict();

        // get self player's real time position and rotation
        //string selfUserId = GetUserIdFromUserSeq(Identifier);
        if (!string.IsNullOrEmpty(selfUserId) && isCoroutineStarted)
        {
            selfPlayerPosition = INVector3ToVector3(entityDict[selfUserId].Position);
            selfPlayerRotation = INVector3ToVector3(entityDict[selfUserId].Rotation);
            //Debug.Log(string.Format("selfPlayerRotation, x:{0}, y:{1}, z:{2}", selfPlayerRotation.x, selfPlayerRotation.y, selfPlayerRotation.z));
        }

        if (isCoroutineStarted &&
            VoiceChatManager.Instance.IsEnabledSpatializer /*&&
            (!string.IsNullOrEmpty(VoiceChatManager.Instance.SpaceId))*/)
        {
            // calculate position
            updateSpatializer("50");
        }
    }

    public IEnumerator GetSelfPlayerInfo()
    {
        yield return new WaitUntil(() => VoiceChatManager.Instance.FinishedAudioSetup);
        identifier = VoiceChatManager.Instance.IdentifierId;
        spaceIdentifier = VoiceChatManager.Instance.RoomId;
        yield return new WaitUntil(() => (!string.IsNullOrEmpty(identifier) && spaceIdentifier > 0));
        yield return new WaitUntil(() => (!string.IsNullOrEmpty(VoiceChatManager.Instance.UserId)));
        selfUserId = VoiceChatManager.Instance.UserId;
        yield return new WaitUntil(() => (!string.IsNullOrEmpty(VoiceChatManager.Instance.SpaceId)));
        yield return new WaitUntil(() => VoiceChatManager.Instance.AddSelf);
        isCoroutineStarted = true;
    }


    public static Vector3 INVector3ToVector3(INVector3 vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }

    public void updateSpatializer(string strRange)
    {
        int range;
        if (int.TryParse(strRange, out range))
        {
            if (ITMGContext.GetInstance().GetRoom().UpdateAudioRecvRange(range) == 0)
            {
                //Debug.Log(string.Format("range {0}", range));
            }

        }

        Quaternion eulerToQuaternion = Quaternion.Euler(selfPlayerRotation);
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, eulerToQuaternion, Vector3.one);
        int[] position = new int[3] { (int)selfPlayerPosition.z, (int)selfPlayerPosition.x, (int)selfPlayerPosition.y };
        float[] axisForward = new float[3] { matrix.m22, matrix.m02, matrix.m12 };
        float[] axisRight = new float[3] { matrix.m20, matrix.m00, matrix.m10 };
        float[] axisUp = new float[3] { matrix.m21, matrix.m01, matrix.m11 };
        ITMGContext.GetInstance().GetRoom().UpdateSelfPosition(position, axisForward, axisRight, axisUp);
        //Debug.Log("update self position");
    }
}