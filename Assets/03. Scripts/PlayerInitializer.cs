using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInitializer : MonoBehaviourPunCallbacks
{
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
            if(playerObj == null)
            {
                Debug.Log("playerObj: null");
                Debug.Log(targetPlayer.NickName + " : " + targetPlayer.TagObject == null);
            }
            playerObj.GetComponent<PhotonView>().RPC("SetRole", RpcTarget.All, playerRole);
        }
    }

    [PunRPC]
    void SetRole(string role)
    {
        Debug.Log(gameObject.name);
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

        text.text = role;
    }
}
