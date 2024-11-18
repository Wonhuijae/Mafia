using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using System;

public class UISetting : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI roomName;
    public TMP_InputField inputPlayerNickname;
    public TMP_InputField updatePlayerNickname;

    public TextMeshProUGUI mafiaNumUI;
    public Button plusBTN;
    public Button minusBTN;

    public TextMeshProUGUI[] playerName;
    public Toggle[] playerReady;
    public GameObject roomSettingButton;

    public static event Action OnInputTextStart;
    public static event Action OnInputTextEnd;
    public static event Action<int> OnSetNumber;

    private string keyNameIsReady = PropertyKeyName.keyIsReady;
    private string keyNameNickNameColor = PropertyKeyName.keyNickNameColor;

    int mafiaNum = 1;

    private void Awake()
    {
        NetworkManager.OnJoinRoom += UpdatePlayerList;  
        NetworkManager.OnJoinRoom += RoomSettingShow;
    }

    private void OnDisable()
    {
        NetworkManager.OnJoinRoom -= UpdatePlayerList;
        NetworkManager.OnJoinRoom -= RoomSettingShow;
    }

    private void Start()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        updatePlayerNickname.onEndEdit.AddListener(ChangeNickName);

        mafiaNumUI.text = mafiaNum.ToString();
    }

    public void CopyRoomName()
    {
        GUIUtility.systemCopyBuffer = roomName.text;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey(keyNameIsReady) || changedProps.ContainsKey(keyNameNickNameColor) || changedProps.ContainsKey(255))
        {
            UpdatePlayerList();
        }
        else
        {
            Debug.Log("키가 없음");
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < playerName.Length; i++) 
        {
            if (i < players.Length)
            {
                if (players[i].CustomProperties.ContainsKey(keyNameIsReady))
                {
                    bool isReady = (bool)players[i].CustomProperties[keyNameIsReady];
                    playerReady[i].isOn = isReady;
                }

                if (players[i].CustomProperties.ContainsKey(keyNameNickNameColor))
                {
                    float[] color = (float[])players[i].CustomProperties[keyNameNickNameColor];
                    Color nickColor = new Color(color[0], color[1], color[2], color[3]);

                    playerName[i].color = nickColor;
                }

                string nick = players[i].NickName;
                playerName[i].text = nick;
            }
            else
            {
                // 접속되어 있는 플레이어가 더 없을 경우
                playerName[i].text = "";
                playerReady[i].isOn = false;
            }
        }
    }

    public void SetNickName()
    {
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;

        PhotonNetwork.LocalPlayer.NickName = inputPlayerNickname.text;
        playerProperties[keyNameIsReady] = false;

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        updatePlayerNickname.text = PhotonNetwork.LocalPlayer.NickName;
        UpdatePlayerList();
    }

    public void SetReady(bool _ready)
    {
        // 게임 준비 상태 변경
        Hashtable playerReady = PhotonNetwork.LocalPlayer.CustomProperties;
        string keyName = PropertyKeyName.keyIsReady;

        if (playerReady.ContainsKey(keyName))
        {
            playerReady[keyName] = _ready;
        }

        // 동기화
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerReady);
        Debug.Log(playerReady[keyName]);
    }

    public void ChangeNickName(string name)
    {
        PhotonNetwork.LocalPlayer.NickName = name;
    }

    public void InputStart(string _textInput)
    {
        if (OnInputTextStart != null) OnInputTextStart();
    }

    public void InputEnd(string _textInput)
    {
        if (OnInputTextEnd != null) OnInputTextEnd();
    }

    void RoomSettingShow()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) roomSettingButton.SetActive(true);
    }

    public void PlusMafiaNum()
    {
        mafiaNum++;

        SetButton();
    }

    public void MinusMafiaNum()
    {
        mafiaNum--;

        SetButton();
    }

    void SetButton()
    {
        if (mafiaNum >= 2)
        {
            plusBTN.interactable = false;
        }
        else if (mafiaNum <= 1)
        {
            minusBTN.interactable = false;
        }

        mafiaNumUI.text = mafiaNum.ToString();
        OnSetNumber(mafiaNum);
    }
}
