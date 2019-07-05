using UnityEngine;
using UnityEngine.UI;
using UMP;

public class UMPTextureUpdator : MonoBehaviour
{
    public RawImage _image;
    public UniversalMediaPlayer _player;
    private Texture2D _texture;

    void Start () {
        _player.AddPreparedEvent(OnPrepared);
        _player.AddStoppedEvent(OnStop);
	}
	
	void Update () {
        if (_texture != null)
        {
            _texture.LoadRawTextureData(_player.FramePixels);
            _texture.Apply();
        }
    }

    void OnDestroy()
    {
        _player.RemoveStoppedEvent(OnStop);
    }

    void OnPrepared(Texture texture)
    {
        //Video size != Video buffer size (FramePixels has video buffer size), so we will use
        //previously created playback texture size that based on video buffer size
        _texture = MediaPlayerHelper.GenVideoTexture(texture.width, texture.height);
        _image.texture = _texture;
    }

    void OnStop()
    {
        if (_texture != null)
            Destroy(_texture);
        _texture = null;
    }
}
