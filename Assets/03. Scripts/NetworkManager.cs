using System;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using NUnit.Framework;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField joinRoomName;

    List<string> roomNames = new();
    private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int nameLength = 8;

    private NetworkManager m_instance;
    public NetworkManager Instance
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
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비 접속");
    }

    public void CreateRoom()
    {
        PhotonNetwork.LoadLevel("WaitingRoom");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;

        PhotonNetwork.CreateRoom(RandomRoomName(), roomOptions, TypedLobby.Default);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomName.text);
        PhotonNetwork.LoadLevel("WaitingRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
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
