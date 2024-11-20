using Photon.Pun;
using TMPro;
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
        if (pv.IsMine)
        {
            GameUISetting.OnReport += Report;
            GameUISetting.OnConvene += Convene;

            Debug.Log("Listener Set");
        }
    }

    private void OnDisable()
    {
        if (pv.IsMine)
        {
            GameUISetting.OnReport -= Report;
            GameUISetting.OnConvene -= Convene;
        }
    }

    // 시체 신고
    public void Report()
    {
        PhotonNetwork.LoadLevel("MeetingRoom");
    }

    // 회의 소집
    public void Convene()
    {
        PhotonNetwork.LoadLevel("MeetingRoom");
    }

    public virtual void Die()
    {
        if (isDie) return;

        playerMove.enabled = false;
        characterController.enabled = false;
        isDie = true;
    }

    void RePose(Scene oldScene, Scene curScene)
    {
        if(curScene.name != "WaitingScene") transform.position = Vector3.zero;
    }

    // 시체 스폰
    public void RPC_SpawnCorpse()
    {
        if(this is CrewPlayer crew)
        {
            crew.SpawnCorpsewn();
        }
    }
}