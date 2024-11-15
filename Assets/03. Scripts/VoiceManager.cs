using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceManager : MonoBehaviour
{
    PunVoiceClient voiceClient;
    Recorder recorder;

    private void Awake()
    {
        voiceClient = GetComponent<PunVoiceClient>();
        recorder = GetComponent<Recorder>();

        recorder.TransmitEnabled = true;
        recorder.VoiceDetection = true;

        PlayerSelecter.OnCharacterInit += SetSpeacker;
    }

    private void OnDisable()
    {
        PlayerSelecter.OnCharacterInit -= SetSpeacker;
    }

    public void MicMute()
    {
        if (recorder != null)
        {
            recorder.TransmitEnabled = !recorder.TransmitEnabled;
        }
    }

    void SetSpeacker(GameObject player)
    {
        PhotonView playePv = player.GetComponent<PhotonView>();
        
        if (playePv != null)
        {
            Speaker speaker = player.GetComponentInChildren<Speaker>();

            // 자신의 캐릭터의 스피커만 살려둠
            if (playePv.IsMine)
            {
                voiceClient.SpeakerPrefab = player;
            }
            else
            {
                Destroy(speaker);
            }
        }
    }
}
