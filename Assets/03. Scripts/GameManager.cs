using System;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int gameIdx;
    int mafiaNum;

    static Vector3 startPos = Vector3.zero;

    List<GameObject> corpse = new();

    private static GameManager m_instance;
    public static GameManager Instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindAnyObjectByType<GameManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if(Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        GameSetting.OnSetNumber += SetMafiaNumber;
    }

    public void GameStart()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient && CheckReady()) 
        {
            PhotonNetwork.LoadLevel("GameLevel" + gameIdx);
            PlayerSet();
        }
    }

    bool CheckReady()
    {
        foreach(var p in PhotonNetwork.PlayerList)
        {
            bool bReady = (bool)p.CustomProperties[PropertyKeyName.keyIsReady];

            if (!bReady) return false;
        }

        return true;
    }

    void PlayerSet()
    {
        List<GameObject> lists = NetworkManager.Instance.GetPlayerLists();

        int[] selectedMafia = SelectMafia(lists.Count);

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
        int[] mafia = new int[mafiaNum];

        for (int i = 0; i < mafia.Length; i++) 
        {
            int r;
            do
            {
                r = UnityEngine.Random.Range(0, size);

            } while (!Array.Exists(mafia, e => e == r));

            mafia[i] = r;
        }

        return mafia;
    }

    void SetMafiaNumber(int num)
    {
        mafiaNum = num;
        Debug.Log(mafiaNum);
    }

    public void CrewDie(GameObject c)
    {
        corpse.Add(c);
        c.GetComponent<Animator>();
    }
}
