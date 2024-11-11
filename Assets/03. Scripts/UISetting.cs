using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UISetting : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI roomName;

    public TextMeshProUGUI[] playerName;

    private void Awake()
    {
        NetworkManager.OnJoinRoom += UpdatePlayerList;  
    }

    private void Start()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void CopyRoomName()
    {
        GUIUtility.systemCopyBuffer = roomName.text;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        UpdatePlayerList();
    }

    void UpdatePlayerList()
    {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++) 
        {
            string nick = players[i].NickName;
            if (nick == "") nick = "player" + i;

            playerName[i].text = nick;
            Debug.Log(nick);
        }
    }
}
