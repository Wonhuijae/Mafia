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
    }

    [PunRPC]
    void SpawnCorpse()
    {
        GameObject c = Instantiate(model, transform.position, transform.rotation);

        c.AddComponent<PhotonView>();
        c.GetComponent<PhotonView>().observableSearch = PhotonView.ObservableSearch.AutoFindAll;

        CapsuleCollider coll = c.AddComponent<CapsuleCollider>();
        coll.center = new Vector3(0, 0.5f, 0);
        coll.radius = 0.4f;
        coll.height = 2.06f;
        
        Rigidbody r = c.AddComponent<Rigidbody>();
        // r.useGravity = true;

        Animator corpseAnim = c.GetComponentInChildren<Animator>();
        corpseAnim.SetTrigger("Die");
    }
}