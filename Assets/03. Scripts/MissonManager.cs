using Photon.Pun;
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

    // 미션 1개 완료했을 때 증가할 비율
    float missonGage;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        missonPlayer = GameManager.Instance.GetCrewPlayerNumber();
        missonGage = 1 / (float)(missonPlayer * 5);

        Debug.Log("missonPlayer: " + missonPlayer);
        Debug.Log("missonGage: " + missonGage);
    }

    public void RPC_MissonClear()
    {
        pv.RPC("MissonClear", RpcTarget.All);
    }


    [PunRPC]
    void MissonClear()
    {
        Debug.Log(missonSlider.value);
        missonSlider.value += missonGage;
    }
}
