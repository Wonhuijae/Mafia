using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotePlayer
{
    public string nickName;
    public Color nickColor;
    public int votes;

    public VotePlayer(string _name, Color _color, int _votes = 0)
    {
        nickName = _name;
        nickColor = _color;
        votes = _votes;
    }
}

public class VotingItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public GameObject[] elections;
    public Image profile;

    private VotePlayer votePlayer;

    public void SetData(string _name, Color _color, int _votes = 0)
    {
        votePlayer = new VotePlayer(_name, _color, _votes);

        SetItem();
    }

    void SetItem()
    {
        nameText.text = votePlayer.nickName;
        nameText.color = votePlayer.nickColor;
        profile.color = votePlayer.nickColor;
    }
}
