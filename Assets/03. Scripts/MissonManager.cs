using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MissonManager : MonoBehaviour
{
    PhotonView pv;

    [SerializeField]
    private Slider missonSlider;

    // 플레이어당 주어지는 미션 개수
    const int missonCount = 5;
    int missonPlayer;

    // 총 미션 완료 개수
    int missonGage = 0;

    private static MissonManager m_instance;
    public static MissonManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindAnyObjectByType<MissonManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (Instance != this) 
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        pv = GetComponent<PhotonView>();

        missonPlayer = GameManager.Instance.GetCrewPlayerNumber();

        if (pv.IsMine) CrewPlayer.OnMissonStarted += MissonStart;
    }

    public void RPC_MissonClear()
    {
        pv.RPC("MissonClear", RpcTarget.All);
    }


    [PunRPC]
    void MissonClear()
    {
        missonGage++;
        missonSlider.value = (float)missonGage / (missonCount * missonPlayer);
    }

    void MissonStart()
    {
        // 미션 골라서 부여
    }
}
