using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUISetting : MonoBehaviour
{
    public static event Action OnReport;
    public static event Action OnConvene;

    public TextMeshProUGUI rollText;
    public Button killButton;
    public Button ReportButton;
    public Button ConveneButton;

    PhotonView pv;

    private void Awake()
    {
        PlayerInitializer.onSetPlayer += SetRollText;
        MafiaPlayer.OnSetMafia += SetupMafiaUI;
        pv = GetComponent<PhotonView>();

        // ��ü ���� ��ư
        ReportButton.onClick.RemoveAllListeners();
        ReportButton.onClick.AddListener(() =>
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient) MeetingSceneLoad();
            else
            {
                pv.RPC("ReportSceneLoad", RpcTarget.MasterClient);
            }
        });

        // ���ȸ�� ��ư
        ConveneButton.onClick.RemoveAllListeners();
        ConveneButton.onClick.AddListener(()=>
        { 
            if(PhotonNetwork.LocalPlayer.IsMasterClient) MeetingSceneLoad();
            else
            {
                pv.RPC("MeetingSceneLoad", RpcTarget.MasterClient);
            }
        });
    }

    void SetRollText(string t)
    {
        rollText.text = t;
    }

    private void SetupMafiaUI(MafiaPlayer mafiaPlayer)
    {
        killButton.gameObject.SetActive(true);

        killButton.onClick.RemoveAllListeners();
        killButton.onClick.AddListener(() =>
        {
            mafiaPlayer.Kill();
        });
    }

    [PunRPC]
    public void MeetingSceneLoad()
    {
        OnConvene();
    }

    [PunRPC]
    public void ReportSceneLoad()
    {
        OnConvene();
    }
}