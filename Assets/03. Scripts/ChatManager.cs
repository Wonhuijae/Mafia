using Photon.Pun;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatManager : MonoBehaviour
{
    public GameObject chat;
    public GameObject chatScroll;
    public TMP_InputField chatInput;

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    // 채팅 전송
    public void SendChat(string input)
    {
        string chat = chatInput.text;
        if (string.IsNullOrEmpty(chat)) return;
        pv.RPC("UpdateChat", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chat);
        chatInput.text = "";
    }

    // 채팅창 업데이트
    [PunRPC]
    void UpdateChat(string name, string str)
    {
        GameObject msgObj = Instantiate(chat);
        msgObj.transform.SetParent(chatScroll.transform, false);

        TextMeshProUGUI msg = msgObj.GetComponent<TextMeshProUGUI>();

        if (PhotonNetwork.LocalPlayer.NickName == name)
        {
            msg.alignment = TextAlignmentOptions.Right;
        }

        msg.text = name + " " + str;
    }
}
