using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class CrewPlayer : GamePlayer, ICrew
{
    void Start()
    {
        Invoke("CrewDie", 3f);
    }

    public void Misson()
    {
        throw new System.NotImplementedException();
    }

    public void CrewDie()
    {
        instance.CrewDie(model);

        pv.RPC("SpawnCorpse", RpcTarget.All);
        Die();
        Invoke("Ghost", 3f);
    }

    void Ghost()
    {
        characterController.enabled = true;
        playerMove.enabled = true;

        playerMove.SetCameraTarget();
    }

    [PunRPC]
    public void SpawnCorpse()
    {
        GameObject c = Instantiate(model, transform.position, transform.rotation);
        playerMove.SetCameraTarget(c);
        model.SetActive(false);
        Animator corpseAnim = c.GetComponentInChildren<Animator>();
        corpseAnim.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        c.AddComponent<PhotonView>();
        PhotonView photonView = c.GetComponent<PhotonView>();
        photonView.observableSearch = PhotonView.ObservableSearch.AutoFindAll;

        PhotonAnimatorView animatorView = c.GetComponent<PhotonAnimatorView>();
        Destroy(animatorView);
        c.AddComponent<PhotonAnimatorView>();

        GameObject mesh = c.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
        mesh.AddComponent<BoxCollider>();
        
        Rigidbody r = c.AddComponent<Rigidbody>();
        r.freezeRotation = true;
        r.useGravity = true;

        corpseAnim.SetTrigger("Die");
    }
}