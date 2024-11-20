using Photon.Pun;
using UnityEngine;

public class MeetingChat : ChatManager
{
    public GameObject ChatMe;
    public GameObject ChatOther;

    public override void SendChat(string input)
    {
        string chat = chatInput.text;
        if (string.IsNullOrEmpty(chat)) return;

        float[] colors =
            (float[])PhotonNetwork.LocalPlayer.CustomProperties[PropertyKeyName.keyNickNameColor];

        pv.RPC("UpdateChat", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chat, colors);
        chatInput.text = "";
    }

    [PunRPC]
    protected override void UpdateChat(string name, string str, float[] colors)
    {
        GameObject msgObj;

        if (name == PhotonNetwork.LocalPlayer.NickName) 
        {
            msgObj = Instantiate(ChatMe);
        }
        else
        {
            msgObj = Instantiate(ChatOther);
        }

        msgObj.transform.SetParent(chatScroll.transform, false);

        Color msgColor = new Color(colors[0], colors[1], colors[2], colors[3]);

        msgObj.GetComponent<ChatContent>().SetMessage(name, str, msgColor);
    }
}
