using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VoiceManager : MonoBehaviour
{
    PunVoiceClient voiceClient;
    Recorder recorder;

    public GameObject detectMicInput;
    private void Awake()
    {
        voiceClient = GetComponent<PunVoiceClient>();
        recorder = GetComponent<Recorder>();

        recorder.TransmitEnabled = true;
        recorder.VoiceDetection = true;

        PlayerSelecter.OnCharacterInit += SetSpeacker;

        // 음성 감지 시 UI에 표시
        recorder.VoiceDetector.OnDetected += DetectVoice;

        SceneManager.activeSceneChanged += MicSetting;
    }

    private void OnDisable()
    {
        PlayerSelecter.OnCharacterInit -= SetSpeacker;
    }

    public void MicSetting(Scene oldScene, Scene curScene)
    {
        if (curScene.name.Contains("GameLevel")) MicMute();
        else MicUnMute();
    }

    public void MicMute()
    {
        if (recorder != null)
        {
            recorder.TransmitEnabled = false;
        }
    }

    public void MicUnMute()
    {
        if (recorder != null)
        {
            recorder.TransmitEnabled = true;
        }
    }

    void SetSpeacker(GameObject player)
    {
        PhotonView playePv = player.GetComponent<PhotonView>();
        
        if (playePv != null)
        {
            // 스피커 설정
            if (playePv.IsMine)
            {
                voiceClient.SpeakerPrefab = player;
            }
        }
    }

    void DetectVoice()
    {
        detectMicInput.SetActive(!detectMicInput.activeInHierarchy);
    }
}
