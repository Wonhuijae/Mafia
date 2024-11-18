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
    void SpawnCorpse()
    {
        GameObject c = Instantiate(model, transform.position, transform.rotation);
        playerMove.SetCameraTarget(c);
        model.SetActive(false);

        c.AddComponent<PhotonView>();
        c.GetComponent<PhotonView>().observableSearch = PhotonView.ObservableSearch.AutoFindAll;

        GameObject mesh = c.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
        mesh.AddComponent<CapsuleCollider>();
        
        Rigidbody r = c.AddComponent<Rigidbody>();
        r.useGravity = true;

        Animator corpseAnim = c.GetComponentInChildren<Animator>();
        corpseAnim.SetTrigger("Die");
    }
}