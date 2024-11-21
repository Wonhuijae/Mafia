using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Realtime;
using System;

public class VoteManager : MonoBehaviour
{
    Dictionary<string, bool> IsDeads = new();
    Dictionary<string, Color> colors = new();
    Dictionary<string, int> voteResult = new();

    GameManager gameManager;
    PhotonView pv;

    static string votePlayer;

    public GameObject[] voteItems;

    public static event Action OnVoteTimeOut;

    float time = 0f;
    bool isTimeOut = false;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        pv= GetComponent<PhotonView>();

        SetVoteList();
    }

    void Update()
    {
        if (isTimeOut) return;

        time += Time.deltaTime;

        // 투표 시간 종료되면 이벤트 호출
        if (time >= 5f) 
        {
            OnVoteTimeOut();
            TimeOut();
            time = 0;
            isTimeOut = true;
        }
    }

    // 투표 후보자 리스트 설정
    // 스크롤뷰 버튼
    void SetVoteList()
    {
        // 마스터 클라이언트에서만 설정하고 이외 클라이언트는 마스터에 동기화
        if(PhotonNetwork.IsMasterClient)
        {
            IsDeads = gameManager.SetVoteList();
            string dictToJson = JsonConvert.SerializeObject(IsDeads);
            pv.RPC("SyncVoteList", RpcTarget.Others, dictToJson);
            SetVoteView();
        }
    }

    [PunRPC]
    void SyncVoteList(string dictToJson)
    {
        IsDeads = JsonConvert.DeserializeObject<Dictionary<string, bool>>(dictToJson);
        SetVoteView();
    }

    void SetVoteView()
    {
        VotePlayer[] vs = new VotePlayer[8];
        int idx = 0;

        // 투표 리스트 구성
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            string nickName = p.NickName;

            float[] pc = (float[])p.CustomProperties[PropertyKeyName.keyNickNameColor];
            Color c = new Color(pc[0], pc[1], pc[2], pc[3]);

            VotePlayer v = new(nickName, c, IsDeads[nickName]);
            vs[idx] = v;
            voteItems[idx].GetComponent<VotingItem>().SetData(v);

            colors.Add(nickName, c);
            idx++;
        }

        for (; idx < 8; idx++) 
        {
            voteItems[idx].SetActive(false);
        }
    }

    // 투표 결과 저장
    public static void GetVoteData(string name)
    {
        votePlayer = name;
    }

    // 투표 결과 취합(마스터)
    void TimeOut()
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ResultAdd(votePlayer);

            Invoke("DisplayResults", 3f);
        }
        else
        {
            pv.RPC("ResultAdd", RpcTarget.MasterClient, votePlayer);
        }
    }

    [PunRPC]
    // 딕셔너리에 투표 결과 저장
    void ResultAdd(string name)
    {
        if(voteResult.ContainsKey(name))
        {
            voteResult[name]++;
        }
        else
        {
            voteResult.Add(name, 1);
        }
    }

    // 투표 결과
    void DisplayResults()
    {
        List<string> playerNames = new List<string>();
        List<int> playerVotes = new List<int>();

        foreach (GameObject o in voteItems)
        {
            if (!o.activeInHierarchy) continue;

            VotingItem v = o.GetComponent<VotingItem>();
            if (v.IsDead()) continue;

            string itemName = v.GetPlayerName();

            if (voteResult.ContainsKey(itemName))
            {
                int itemVote = voteResult[itemName];
                v.DisplayVoteResult(itemName, itemVote);

                playerNames.Add(itemName);
                playerVotes.Add(voteResult[itemName]);
            }
        }

        pv.RPC("SyncVoteResults", RpcTarget.Others, playerNames.ToArray(), playerVotes.ToArray());
    }

    // 마스터와 동기화
    [PunRPC]
    void SyncVoteResults(string[] playerNames, int[] playerVotes)
    {
        for (int i = 0; i < playerNames.Length; i++)
        {
            foreach (GameObject o in voteItems)
            {
                if (!o.activeInHierarchy) continue;

                VotingItem v = o.GetComponent<VotingItem>();
                if (v.IsDead()) continue;

                if (v.GetPlayerName() == playerNames[i])
                {
                    v.DisplayVoteResult(playerNames[i], playerVotes[i]);
                }
            }
        }
    }
}
