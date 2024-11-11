using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine;

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

    // ��� Ŭ�� �� ���� ����
    [PunRPC]
    public void RPCChangeModel(string _name)
    {
        if (pv.IsMine)
        {
            ChangeModel(_name);
        }
    }

    // �� ���� ��û
    public void RequestModelChange(string _name)
    {
        if (pv.IsMine) pv.RPC("RPCChangeModel", RpcTarget.AllBuffered, _name);
    }
    
    // ���� Ŭ���̾�Ʈ �� ����
    void ChangeModel(string _name)
    {
        if(pv != null)
        {
            GameObject player = pv.gameObject;

            Destroy(pv.GetComponentInChildren<Animator>().gameObject);

            // �� �� �����ؼ� �÷��̾� ������ ��
            GameObject newModel = Instantiate(Resources.Load<GameObject>(_name), transform.position, transform.rotation);
            newModel.transform.SetParent(player.transform, false);
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // �ִϸ�����, ī�޶� �缳��
            SetAnimator();
            SetCameraTarget(newModel);
        }
    }

    void SetAnimator()
    {
        GetComponent<PlayerMove>().SetAnimator();
    }

    void SetCameraTarget(GameObject _model)
    {
        GetComponent<PlayerMove>().SetCameraTarget(_model);
    }
}
