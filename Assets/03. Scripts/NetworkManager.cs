using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using Photon.Pun;
using Photon.Realtime;
using NUnit.Framework;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public static class PropertyKeyName
{
    public static string keySelectedChars = "selectedChars"; // 선택된 모델 배열
    public static string keyIsReady = "isReady"; // 플레이어 준비 상태
    public static string keyNickNameColor = "NickColorIdx"; // 닉네임 색깔
    public static string keyCharIdx = "charIdx"; // 캐릭터 ID
    public static string keySceneSynced = "SceneSynced"; // 동기화 여부
    public static string keyCurrentScene = "CurrentScene"; // 현재 씬
}

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
        SceneManager.sceneLoaded += OnSceneLoadingComplete;
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

    // 방 생성
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;

        PhotonNetwork.CreateRoom(RandomRoomName(), roomOptions, TypedLobby.Default);
    }

    // 방 이름으로 참가
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
            roomProperties[PropertyKeyName.keySelectedChars] = selectedChars;
            roomProperties[PropertyKeyName.keyCurrentScene] = SceneManager.GetActiveScene().name;

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

            StartCoroutine(WaitForPropSet());
        }
        else
        {
            StartCoroutine(CheckSceneSyncWithMaster()) ;
        }
        
        StartCoroutine(WaitRoutine());
    }

    // 캐릭터 초기화
    IEnumerator WaitRoutine()
    {
        // 이벤트 리스너가 없으면 대기
        while (OnJoinRoom == null) 
        {
            yield return null;
        }

        OnJoinRoom?.Invoke();
    }

    // 프로퍼티 세팅 대기한 후 씬 로드
    IEnumerator WaitForPropSet()
    {
        yield return new WaitForSeconds(1f);

        PhotonNetwork.LoadLevel("WaitingRoom");

        StartCoroutine(CheckSceneSyncWithMaster());
    }

    // 씬 동기화 대기
    IEnumerator CheckSceneSyncWithMaster()
    {
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PropertyKeyName.keySceneSynced) ||
           (bool)PhotonNetwork.CurrentRoom.CustomProperties[PropertyKeyName.keySceneSynced] == false)
        {
            yield return null;
        }
    }

    private void OnSceneLoadingComplete(Scene scene, LoadSceneMode mode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 씬 로드 완료
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { PropertyKeyName.keySceneSynced, true } });
        }
    }

    // 알파벳 대소문자, 숫자 중 랜덤 8자리
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
