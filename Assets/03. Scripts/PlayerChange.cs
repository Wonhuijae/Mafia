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
    public void RequestModelChange(string _name)
    {
        if (pv.IsMine) pv.RPC("ChangeModel", RpcTarget.AllBuffered, _name);
    }

    [PunRPC]
    // 로컬 클라이언트 모델 변경
    void ChangeModel(string _name)
    {
        GameObject player = pv.gameObject;

        // 기존 모델 제거
        if (GetComponentInChildren<Animator>() != null)
        {
            GameObject oldModel = GetComponentInChildren<Animator>().gameObject;

            PhotonAnimatorView oldAnimView = oldModel.GetComponent<PhotonAnimatorView>();
            if (oldAnimView != null) Destroy(oldAnimView);

            Destroy(oldModel);
        }

        // 새 모델 생성해서 플레이어 하위에 둠
        GameObject newModel = Instantiate(Resources.Load<GameObject>(_name), transform.position, transform.rotation);
        newModel.transform.SetParent(player.transform, false);
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // PhotonView에 새로운 컴포넌트들 등록
        pv.ObservedComponents.Clear();  // 기존 컴포넌트 제거

        // 필요한 컴포넌트들 추가
        pv.ObservedComponents.Add(newModel.GetComponent<PhotonAnimatorView>());
        pv.ObservedComponents.Add(GetComponent<PhotonTransformView>());

        // 애니메이터 재설정
        pv.RPC("SetAnimator", RpcTarget.AllBuffered);
        // 로컬 카메라 재설정
        if (pv.IsMine) SetCameraTarget(newModel);
    }
    
    [PunRPC]
    void SetAnimator()
    {
        GetComponent<PlayerMove>().SetAnimator();
    }
    
    void SetCameraTarget(GameObject _model)
    {
        GetComponent<PlayerMove>().SetCameraTarget(_model);
    }
}
