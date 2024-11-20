using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Realtime;

public class VoteManager : MonoBehaviour
{
    Dictionary<string, bool> IsDeads = new();
    Dictionary<string, Color> colors = new();

    GameManager gameManager;
    PhotonView pv;

    public GameObject[] voteItems;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        pv= GetComponent<PhotonView>();

        SetVoteList();
    }

    void SetVoteList()
    {
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

        for(; idx< 8; idx++)
        {
            voteItems[idx].SetActive(false);
        }
    }
}
