using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInitializer : MonoBehaviourPunCallbacks
{
    public static event Action<string> onSetPlayer;

    TextMeshProUGUI text;
    PhotonView pv;

    private void Awake()
    {
        text = FindAnyObjectByType<TextMeshProUGUI>();
        pv = GetComponent<PhotonView>();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Role"))
        {
            string playerRole = changedProps["Role"].ToString();
            Debug.Log(targetPlayer.NickName + " : " + targetPlayer.CustomProperties["Role"]);

            GameObject playerObj = targetPlayer.TagObject as GameObject;

            playerObj.GetComponent<PhotonView>().RPC("SetRole", RpcTarget.All, playerRole);

            if (playerObj.GetComponent<PhotonView>().IsMine) onSetPlayer(playerRole);
        }
    }

    [PunRPC]
    void SetRole(string role)
    {
        if (role == "Mafia")
        {
            if (GetComponent<CrewPlayer>() != null)
            {
                Destroy(GetComponent<CrewPlayer>());
            }

            if (GetComponent<MafiaPlayer>() == null)
            {
                gameObject.AddComponent<MafiaPlayer>();
            }
        }
        else
        {
            if (GetComponent<MafiaPlayer>() != null)
            {
                Destroy(GetComponent<MafiaPlayer>());
            }

            if (GetComponent<CrewPlayer>() == null)
            {
                gameObject.AddComponent<CrewPlayer>();
            }
        }
    }
}
