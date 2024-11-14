using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine;
using static Photon.Pun.PhotonAnimatorView;

public class PlayerChange : MonoBehaviourPunCallbacks
{
    private NetworkManager instance;
    PhotonView pv;

    private void Awake()
    {
        instance = NetworkManager.Instance;
        pv = PhotonView.Get(this);

        PlayerSelecter.OnChangeCharacter += RequestModelChange;
    }

    // 모델 변경 요청
    public void RequestModelChange(string _name, int _curIdx, int _newIdx)
    {
        if (pv.IsMine)
        {
            pv.RPC("ChangeModel", RpcTarget.AllBuffered, _name);
            UpdateSelect(_curIdx, _newIdx);
        }
    }

    void UpdateSelect(int _curIdx, int _newIdx)
    {
        Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        if(roomProperties.TryGetValue("selectedChars", out object selects))
        {
            bool[] selectChars = (bool[])selects;
            selectChars[_newIdx] = true;
            selectChars[_curIdx] = false;

            roomProperties["selectedChars"] = selectChars;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
    }

    [PunRPC]
    // 로컬 클라이언트 모델 변경
    void ChangeModel(string _name)
    {
        GameObject player = pv.gameObject;

        SkinnedMeshRenderer mesh = player.GetComponentInChildren<SkinnedMeshRenderer>();
        if (mesh != null)
        {
            string matName = $"Astronault_{_name}_Mat";
            Material mat = Resources.Load<Material>("Materials/" + matName);
            if (mat != null)
            {
                mesh.material = mat;
            }
            else
            {
                Debug.LogError($"Failed to load material: {matName}");
            }
        }
    }
}