using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    int gameIdx;
    int mafiaNum = 1;

    PhotonView pv;
    static Vector3 startPos = Vector3.zero;

    List<GameObject> corpse = new();

    List<string> players = new();
    List<string> livePlayers = new();
    List<string> deadPlayers = new();

    bool bIsGameRun = false;

    private static GameManager m_instance;
    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindAnyObjectByType<GameManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        pv = GetComponent<PhotonView>();

        GameSetting.OnSetNumber += SetMafiaNumber;
    }
    bool CheckReady()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            bool bReady = (bool)p.CustomProperties[PropertyKeyName.keyIsReady];

            if (!bReady) return false;
        }

        return true;
    }

    public void GameStart()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient && CheckReady())
        {
            PhotonNetwork.LoadLevel("GameLevel" + gameIdx);
            AssignMafiaRoles();
            bIsGameRun = true;
            ListInitialize();
        }
    }

    public void AssignMafiaRoles()
    {
        // 현재 접속된 플레이어 목록 가져오기
        List<Player> players = PhotonNetwork.PlayerList.ToList();
        int mafiaCount = mafiaNum;

        // 마피아를 뽑을 인덱스를 무작위로 선택
        HashSet<int> mafiaIndexes = new HashSet<int>();
        while (mafiaIndexes.Count < mafiaCount)
        {
            int index = UnityEngine.Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
            mafiaIndexes.Add(index);  // 중복 없이 마피아 인덱스를 추가
        }

        // 각 플레이어에게 역할을 부여 (커스텀 프로퍼티 사용)
        foreach (var player in players)
        {
            Hashtable roleSet = player.CustomProperties;

            // 마피아인지 크루인지 역할 설정
            if (mafiaIndexes.Contains(players.IndexOf(player)))
            {
                // 마피아일 경우 커스텀 프로퍼티 설정
                roleSet["Role"] = "Mafia";
            }
            else
            {
                // 크루일 경우 커스텀 프로퍼티 설정
                roleSet["Role"] = "Crew";
            }

            player.SetCustomProperties(roleSet);  // 프로퍼티 설정
        }
    }

    void SetMafiaNumber(int num)
    {
        mafiaNum = num;
    }

    public void CrewDie(GameObject c)
    {
        corpse.Add(c);
    }

    void ListInitialize()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            players.Clear();
            livePlayers.Clear();
            foreach (var p in PhotonNetwork.PlayerList)
            {
                players.Add(p.NickName);
            }

            livePlayers = players;
            
            // 동기화
            pv.RPC("UpdateLivePlayers", RpcTarget.Others, livePlayers.ToArray());
        }
    }

    [PunRPC]
    void UpdateLivePlayers(string[] playerNames)
    {
        players = new List<string>(playerNames);
        livePlayers = players;
    }

    public void PlayerDie(string nickName)
    {
        deadPlayers.Add(nickName);
        livePlayers.Remove(nickName);
    }

    public bool IsDead(string nickName)
    {
        return deadPlayers.Contains(nickName);
    }

    public Dictionary<string, bool> SetVoteList()
    {
        Dictionary<string, bool> dicts = new();
        foreach(var p in players)
        {
            if (deadPlayers.Contains(p)) dicts.Add(p, true);
            else dicts.Add(p, false);
        }

        return dicts;
    }

    public int GetCrewPlayerNumber()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount - mafiaNum;
    }
}
