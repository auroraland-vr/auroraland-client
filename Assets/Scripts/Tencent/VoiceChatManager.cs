using Auroraland;
using Nakama;
using System.Collections;
using TencentMobileGaming;
using UnityEngine;

public class VoiceChatManager : MonoBehaviour
{
    private readonly string appId = "1400099700";
    private readonly string appKey = "zPH2xWHEgjIRcEBh";
    //private string accountType = "28773";
    public bool FinishedAudioSetup = false;
    public bool AudioClosed = false;
    public bool IsEnabledSpatializer = false;
    public bool AddSelf = false;

    public string IdentifierId = "";
    public int RoomId = -1;
    public string UserId = "";
    public string SpaceId = "";

    // private Hashtable mMemberCellMap = new Hashtable();

    public static VoiceChatManager Instance;

    private readonly string appVersion = "voice_chat_1_3_0";

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

    void Start()
    {
        NKController.Instance.JoinSpaceSuccess += OnJoinSpaceSuccess;
        NKController.Instance.LeaveSpaceSuccess += OnLeaveSpaceSuccess;
        NKController.Instance.PlayerJoin += OnPlayerJoin;

        ITMGContext.GetInstance().OnEnterRoomCompleteEvent += OnEnterRoomComplete;
        ITMGContext.GetInstance().OnExitRoomCompleteEvent += OnExitRoomComplete;
        ITMGContext.GetInstance().OnRoomDisconnectEvent += OnRoomDisconnect;
    }

    void Update()
    {
        ITMGContext.GetInstance().Poll();
    }

    private void OnDisable()
    {
        OnQuitRoom();

        NKController.Instance.JoinSpaceSuccess -= OnJoinSpaceSuccess;
        NKController.Instance.LeaveSpaceSuccess -= OnLeaveSpaceSuccess;
        NKController.Instance.PlayerJoin -= OnPlayerJoin;
    }

    private void OnJoinSpaceSuccess(object sender, NKListArgs<INEntity> e)
    {
        // get current player seqence and space id info
        IdentifierId = NKController.Instance.GetUserSeq().ToString();
        RoomId = (int)NKController.Instance.GetSpaceSeq();
        SpaceId = NKController.Instance.GetSpaceId();

        // if everything is OK then let player enter chatting room
        StartCoroutine(EnterRoomCoroutine());
    }

    private void OnLeaveSpaceSuccess(object sender, NKSingleArg<bool> e)
    {
        OnQuitRoom();
    }

    private IEnumerator EnterRoomCoroutine()
    {
        yield return new WaitUntil(() => (IdentifierId.Length != 0) && (RoomId > 0));
        OnEnterRoom(appId, IdentifierId, RoomId, false);
        yield return null;
    }

    public void OnEnterRoom(string appId, string identifierId, int roomId, bool isListenOnly)
    {
        // check if all the inputs are valid or not
        // TODO: add a ShowWarning method to give users typing hint
        if (appId.Equals("") || identifierId.Equals(""))
        {
            return;
        }

        if (roomId < 100000)
        {
            return;
        }

        // init SDK
        ITMGContext.GetInstance().SetAppVersion(appVersion);
        int ret = ITMGContext.GetInstance().Init(appId, identifierId);
        if (ret != QAVError.OK)
        {
            return;
        }

        // TODO: add a loading view when join room
        // setup for join room
        byte[] authBuffer = this.GetAuthBuffer(appId, identifierId, roomId);

        // join room
        ITMGContext.GetInstance().EnterRoom(roomId.ToString(), ITMGRoomType.ITMG_ROOM_TYPE_HIGHQUALITY, authBuffer);
    }

    public void OnQuitRoom()
    {
        ITMGContext.GetInstance().ExitRoom();
        ITMGContext.GetInstance().Uninit();
    }

    private byte[] GetAuthBuffer(string appId, string userId, int roomId)
    {
        return QAVAuthBuffer.GenAuthBuffer(int.Parse(appId), roomId.ToString(), userId, appKey);
    }

    // current player join for getting userId
    private void OnPlayerJoin(object sender, NKSingleArg<INEntity> entityArg)
    {
        var entity = entityArg.value;

        if (string.IsNullOrEmpty(entity.UserId) || entity.AssetType != "avatar")
        {
            return;
        }

        UserId = entity.UserId;
        AddSelf = true;
    }

    private void OnEnterRoomComplete(int err, string errInfo)
    {
        if (ITMGContext.GetInstance().IsRoomEntered())
        {
            ITMGContext.GetInstance().GetAudioCtrl().EnableMic(true);
            ITMGContext.GetInstance().GetAudioCtrl().EnableSpeaker(true);
            StartCoroutine(IsMicAndSpeakerOn());
        }
    }

    private void OnExitRoomComplete()
    {
        ITMGContext.GetInstance().GetAudioCtrl().EnableMic(false);
        ITMGContext.GetInstance().GetAudioCtrl().EnableSpeaker(false);
        StartCoroutine(IsMicAndSpeakerOff());
        VoiceChatSpatialSoundManager.Instance.enabled = false;
    }

    IEnumerator IsMicAndSpeakerOn()
    {
        while (ITMGContext.GetInstance().GetAudioCtrl().GetMicState() != 1 ||
            ITMGContext.GetInstance().GetAudioCtrl().GetSpeakerState() != 1)
        {
            yield return null;
        }
        // init and enable spatial sound
        VoiceChatSpatialSoundManager.Instance.enabled = true;
        ITMGContext.GetInstance().GetAudioCtrl().InitSpatializer();
        ITMGContext.GetInstance().GetAudioCtrl().EnableSpatializer(true, false);
        if (ITMGContext.GetInstance().GetAudioCtrl().IsEnableSpatializer())
        {
            IsEnabledSpatializer = true;
        }
        FinishedAudioSetup = true;
        yield return new WaitUntil(() => IsEnabledSpatializer);
        yield return new WaitUntil(() => FinishedAudioSetup);
    }

    IEnumerator IsMicAndSpeakerOff()
    {
        while (ITMGContext.GetInstance().GetAudioCtrl().GetMicState() != 0 ||
            ITMGContext.GetInstance().GetAudioCtrl().GetSpeakerState() != 0)
        {
            yield return null;
        }
        AudioClosed = true;
    }

    private void OnDestroy()
    {
        ITMGContext.GetInstance().OnEnterRoomCompleteEvent -= new QAVEnterRoomComplete(OnEnterRoomComplete);
        ITMGContext.GetInstance().OnExitRoomCompleteEvent -= new QAVExitRoomComplete(OnExitRoomComplete);
        ITMGContext.GetInstance().OnRoomDisconnectEvent -= new QAVRoomDisconnect(OnRoomDisconnect);
    }

    private void OnRoomDisconnect(int err, string errInfo)
    {
    }
}
