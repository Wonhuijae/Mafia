using System;
using System.Collections.Generic;
using NUnit.Framework;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int gameIdx;
    int mafiaNum;

    PhotonView pv;
    static Vector3 startPos = Vector3.zero;

    List<GameObject> corpse = new();

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

    public void GameStart()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient && CheckReady())
        {
            PhotonNetwork.LoadLevel("GameLevel" + gameIdx);
            pv.RPC("PlayerSet", RpcTarget.All);
        }
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
    
    [PunRPC]
    public void PlayerSet()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<GameObject> lists = NetworkManager.Instance.GetPlayerLists();
            int[] selectedMafia = SelectMafia(lists.Count);
            pv.RPC("SyncMafiaSelection", RpcTarget.All, selectedMafia); // 모든 클라이언트에 동기화
        }
    }

    [PunRPC]
    public void SyncMafiaSelection(int[] selectedMafia)
    {
        List<GameObject> lists = NetworkManager.Instance.GetPlayerLists();
        Debug.Log(lists.Count > 0);
        AssignRoles(lists, selectedMafia);
    }

    void AssignRoles(List<GameObject> lists, int[] selectedMafia)
    {
        for (int i = 0; i < lists.Count; i++)
        {
            if (Array.Exists(selectedMafia, e => e == i))
            {
                lists[i].AddComponent<MafiaPlayer>();
            }
            else
            {
                lists[i].AddComponent<CrewPlayer>();
            }

            lists[i].transform.position = startPos;
        }
    }

    int[] SelectMafia(int size)
    {
        HashSet<int> selected = new HashSet<int>();

        while (selected.Count < mafiaNum)
        {
            int r = UnityEngine.Random.Range(0, size);
            selected.Add(r);
        }

        return new List<int>(selected).ToArray();
    }

    void SetMafiaNumber(int num)
    {
        mafiaNum = num;
        Debug.Log(mafiaNum);
    }

    public void CrewDie(GameObject c)
    {
        corpse.Add(c);
    }
}
