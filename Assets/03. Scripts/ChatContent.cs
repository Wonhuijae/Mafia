using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatContent : MonoBehaviour
{
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textContent;
    public Image profile;

    public void SetMessage(string name, string content, Color color)
    {
        textName.text = name;
        textName.color = color;
        textContent.text = content;
        profile.color = color;
    }
}
