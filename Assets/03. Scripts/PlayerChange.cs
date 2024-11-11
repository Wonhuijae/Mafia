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

    // 모든 클라에 모델 변경 적용
    [PunRPC]
    public void RPCChangeModel(string _name)
    {
        if (pv.IsMine)
        {
            ChangeModel(_name);
        }
    }

    // 모델 변경 요청
    public void RequestModelChange(string _name)
    {
        if (pv.IsMine) pv.RPC("RPCChangeModel", RpcTarget.AllBuffered, _name);
    }
    
    // 로컬 클라이언트 모델 변경
    void ChangeModel(string _name)
    {
        if(pv != null)
        {
            GameObject player = pv.gameObject;

            Destroy(pv.GetComponentInChildren<Animator>().gameObject);

            // 새 모델 생성해서 플레이어 하위에 둠
            GameObject newModel = Instantiate(Resources.Load<GameObject>(_name), transform.position, transform.rotation);
            newModel.transform.SetParent(player.transform, false);
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // 애니메이터, 카메라 재설정
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
