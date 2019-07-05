using System;
using UnityEngine;
using Nakama;
using Auroraland;
using Newtonsoft.Json;

public class VoiceChatAccountManager
{
	struct AppIdMeta{
		public string app_id;
	}
	struct AccountTypeMeta{
		public string account_type;
	}

    private string bucket;
    private string collection;
    public string AppId;
    //public string AccountType;
	public static VoiceChatAccountManager Instance;

    public VoiceChatAccountManager()
	{
        this.bucket = "Tencent";
        this.collection = "audio";
	}

	public static VoiceChatAccountManager GetInstance()
	{

		if (Instance == null) {
			Instance = new VoiceChatAccountManager ();
		}
		return Instance;
	}

	public void TencentAccountRemove(){
		// setup account info
		string appId = "{\"app_id\": \"1400071799\"}";
		string accountType = "{\"account_type\": \"23286\"}";

		// Write multiple different records in a collection.
		var message = new Nakama.NStorageRemoveMessage.Builder().Remove(this.bucket, this.collection, "appId").Remove(this.bucket, this.collection, "accountType").Build();

		// send write and callback to prove write successfully
		NKController.Instance.Send(message, (bool done) => {
			Debug.Log("remove user record");
			TencentAccountInit();
		}, (INError err) => {
			Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
		});

	}

    public void TencentAccountInit()
    {   
        // setup account info
        string appId = "{\"app_id\": \"1400071799\"}";
        string accountType = "{\"account_type\": \"23286\"}";

        // Write multiple different records in a collection.
        var message = new Nakama.NStorageWriteMessage.Builder()
			.Write(this.bucket, this.collection, "appId", appId, StoragePermissionRead.PublicRead, StoragePermissionWrite.OwnerWrite)
			.Write(this.bucket, this.collection, "accountType", accountType, StoragePermissionRead.PublicRead, StoragePermissionWrite.OwnerWrite)
            .Build();

        // send write and callback to prove write successfully
        NKController.Instance.Send(message, (INResultSet<INStorageKey> list) => {
            foreach (var record in list.Results)
            {
				Debug.LogFormat("Stored record: '{0}'", record.Record);
            }
        }, (INError err) => {
            Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
        });
    }

    public void TencentAccountFetch(string userId) {
        //string userId = session.Id; // an INSession object.
	
        var message = new Nakama.NStorageFetchMessage.Builder()
			.Fetch(this.bucket, this.collection, "appId",  userId)
			.Fetch(this.bucket, this.collection, "accountType",  userId)
            .Build();
		Debug.Log(message.ToString());
        // send fetch and callback to get values successfully
        NKController.Instance.Send(message, (INResultSet<INStorageData> list) => {
            foreach (var record in list.Results)
            {
                Debug.LogFormat("Record value '{0}'", record.Value);
                Debug.LogFormat("Record permissions read '{0}' write '{1}'",
                    record.PermissionRead, record.PermissionWrite);
                //TODO: parse values and give them to spaceJoin
                if (record.Record == "appId") {
					AppIdMeta appId = JsonConvert.DeserializeObject<AppIdMeta>(record.Value);
					AppId = appId.app_id;
					Debug.Log("AppId:"+AppId);
                } /*else {
					AccountTypeMeta accountType = JsonConvert.DeserializeObject<AccountTypeMeta>(record.Value);
					AccountType = accountType.account_type;
					Debug.Log("AccountType:"+ AccountType);
                }*/
            }
        }, (INError err) => {
            Debug.LogErrorFormat("Error: code '{0}' with '{1}'.", err.Code, err.Message);
        });
    }
}
