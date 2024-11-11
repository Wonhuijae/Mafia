using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using Photon.Pun;
using Photon.Realtime;
using NUnit.Framework;
using TMPro;
using System.Collections;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField joinRoomName;
    public static event Action OnJoinRoom;

    List<string> roomNames = new();
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int nameLength = 8;

    private const int totalChars = 8;

    private static NetworkManager m_instance;
    public static NetworkManager Instance
    {
        get
        {
            if (m_instance == null) 
            {
                m_instance = FindAnyObjectByType<NetworkManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(this);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        ServerConnect();
    }

    public void ServerConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        Debug.Log("Connect");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;

        PhotonNetwork.CreateRoom(RandomRoomName(), roomOptions, TypedLobby.Default);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomName.text);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.IsMasterClient)
        { 
            bool[] selectedChars = new bool[totalChars];

            for (int i = 0; i < totalChars; i++)
            {
                selectedChars[i] = false;
            }

            Hashtable roomProperties = new Hashtable();
            roomProperties["selectedChars"] = selectedChars;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

            StartCoroutine(WaitForPropSet());
        }

        StartCoroutine(WaitRoutine());
    }

    IEnumerator WaitRoutine()
    {
        while (OnJoinRoom == null) 
        {
            yield return null;
        }

        OnJoinRoom?.Invoke();
    }

    IEnumerator WaitForPropSet()
    {
        yield return new WaitForSeconds(1f);

        PhotonNetwork.LoadLevel("WaitingRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 생성 실패: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("방 참가 실패: " + message);
    }

    string RandomRoomName()
    {
        string roomNameString = string.Empty;
        char[] roomName = new char[nameLength];

        while (true)
        {
            var random = new System.Random();

            for (int i = 0; i < nameLength; i++) 
            {
                roomName[i] = chars[random.Next(chars.Length)];
            }

            roomNameString = new string(roomName);

            if(!roomNames.Contains(roomNameString)) break;
        }

        return roomNameString;
    }
}
