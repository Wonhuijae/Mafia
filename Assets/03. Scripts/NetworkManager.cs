using UnityEngine;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;
using NUnit.Framework;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    List<string> roomNames = new();

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
        
    }
}
