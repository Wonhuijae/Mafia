using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine;

public class PlayerChange : MonoBehaviourPunCallbacks
{
    private NetworkManager instance;

    private void Awake()
    {
        instance = NetworkManager.Instance;

        PlayerSelecter.OnChangeCharacter += ChangeModel;
    }

    void ChangeModel(CharacterInfo _info)
    {
        PhotonView pv = PhotonView.Get(this);

        if(pv != null && pv.IsMine)
        {
            GameObject player = pv.gameObject;

            Destroy(pv.GetComponentInChildren<Animator>().gameObject);

            GameObject newModel = Instantiate(_info.charModel);
            newModel.transform.SetParent(player.transform, false);

            SetAnimator();
        }
    }

    void SetAnimator()
    {
        GetComponent<PlayerMove>().SetAnimator();
    }
}
