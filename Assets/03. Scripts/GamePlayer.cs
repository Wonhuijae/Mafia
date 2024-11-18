using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayer : MonoBehaviour
{
    string playerName;
    static float misson;

    protected bool isDie = false;
    protected GameManager instance;
    protected GameObject model;
    protected PhotonView pv;

    void Awake()
    {
        instance = GameManager.Instance;
        pv = GetComponent<PhotonView>();
        model = GetComponentInChildren<Animator>().gameObject;
        Debug.Log(model.name);
    }

    // 시체 신고
    public void Report()
    {

    }

    // 회의 소집
    public void convene()
    {

    }

    public virtual void Die()
    {
        if (isDie) return;

        GetComponent<PlayerMove>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        
        model.SetActive(false);
    }
}
