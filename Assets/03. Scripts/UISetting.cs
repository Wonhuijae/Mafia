using Photon.Pun;
using TMPro;
using UnityEngine;

public class UISetting : MonoBehaviour
{
    public TextMeshProUGUI roomName;

    private void Start()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void CopyRoomName()
    {
        GUIUtility.systemCopyBuffer = roomName.text;
    }
}
