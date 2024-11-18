using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamePlayer : MonoBehaviour
{
    string playerName;
    static float misson;

    protected bool isDie = false;
    protected GameManager instance;
    protected GameObject model;
    protected PhotonView pv;

    protected PlayerMove playerMove;
    protected CharacterController characterController;

    void Awake()
    {
        instance = GameManager.Instance;
        pv = GetComponent<PhotonView>();
        model = GetComponentInChildren<Animator>().gameObject;
        playerMove = GetComponent<PlayerMove>();
        characterController = GetComponent<CharacterController>();

        SceneManager.activeSceneChanged += RePose;
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

        playerMove.enabled = false;
        characterController.enabled = false;
    }

    void RePose(Scene oldScene, Scene curScene)
    {
        if(curScene.name != "WaitingScene") transform.position = Vector3.zero;
    }
}
