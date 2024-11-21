using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotePlayer
{
    public string nickName;
    public Color nickColor;
    public int votes;
    public bool isDead;

    public VotePlayer(string _name, Color _color, bool _isDead)
    {
        nickName = _name;
        nickColor = _color;
        isDead = _isDead;
    }
}

public class VotingItem : MonoBehaviour
{
    static GameObject userSelect;

    public TextMeshProUGUI nameText;

    public GameObject[] elections;
    public Image profile;
    public GameObject select;
    public GameObject XImage;

    private VotePlayer votePlayer;

    GameManager gameManager;

    int votes = 0;
    bool isDead = false;

    private void Awake()
    {
        gameManager = GameManager.Instance;

        VoteManager.OnVoteTimeOut += TimeOut;
    }

    public void SetData(VotePlayer v)
    {
        votePlayer = v;
        SetItem();
    }

    void SetItem()
    {
        nameText.text = votePlayer.nickName;
        nameText.color = votePlayer.nickColor;
        profile.color = votePlayer.nickColor;
        isDead = votePlayer.isDead;
    }

    public void Elect()
    {
        // 이전 선택 취소
        if (userSelect != null) userSelect.SetActive(false);

        select.SetActive(true);
        userSelect = select;
    }

    void TimeOut()
    {
        // 이벤트 발생 시 투표 결과를 매니저에 전송
        if (userSelect == select)
        {
            VoteManager.GetVoteData(votePlayer.nickName);
        }
    }

    public void DisplayVoteResult(string name, int votes)
    {
        Debug.Log(votes);
        for (int i = 0; i < votes; i++) 
        {
            elections[i].SetActive(true);
        }
    }

    public string GetPlayerName()
    {
        Debug.Log(votePlayer == null);
        return votePlayer.nickName;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
