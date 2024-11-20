using Photon.Pun;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CrewPlayer : GamePlayer, ICrew
{
    public void Misson()
    {
        throw new System.NotImplementedException();
    }
    
    public void CrewDie()
    {
        pv.RPC("Die", RpcTarget.All);
    }

    [PunRPC]
    public override void Die()
    {
        base.Die();
        instance.CrewDie(model);
        StartCoroutine(WaitForGhost());
        RPC_SpawnCorpse();
    }


    IEnumerator WaitForGhost()
    {
        yield return new WaitForSeconds(3f);
        Ghost();
    }

    // 관전(유령 모드)
    void Ghost()
    {
        characterController.enabled = true;
        playerMove.enabled = true;

        if (pv.IsMine) playerMove.SetCameraTarget();
    }

    // 시체 생성
    public void SpawnCorpsewn()
    {
        // 시체 오브젝트 생성
        GameObject c = Instantiate(model, transform.position, transform.rotation);

        // 타겟을 시체로 설정해 넘어지는 효과
        playerMove.SetCameraTarget(c);
        model.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        model.SetActive(false);

        // 시체 애니메이터 준비
        Animator corpseAnim = c.GetComponentInChildren<Animator>();
        corpseAnim.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        // 네트워크 동기화 위한 포톤뷰
        c.AddComponent<PhotonView>();
        PhotonView photonView = c.GetComponent<PhotonView>();
        photonView.observableSearch = PhotonView.ObservableSearch.AutoFindAll;

        // 포톤 애니메이터 뷰
        PhotonAnimatorView animatorView = c.GetComponent<PhotonAnimatorView>();
        Destroy(animatorView);
        c.AddComponent<PhotonAnimatorView>();

        // Report 기능을 위해 콜라이더 추가
        GameObject mesh = c.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
        mesh.AddComponent<BoxCollider>();

        // 바닥에 떨어지도록 함
        Rigidbody r = c.AddComponent<Rigidbody>();
        r.freezeRotation = true;
        r.useGravity = true;

        corpseAnim.SetTrigger("Die");
    }
}