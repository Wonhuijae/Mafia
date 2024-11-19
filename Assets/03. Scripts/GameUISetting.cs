using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUISetting : MonoBehaviour
{
    public Button killBTN;
    public TextMeshProUGUI rollText;

    private void Awake()
    {
        PlayerInitializer.onSetPlayer += SetRollText;
    }

    void SetRollText(string t)
    {
        rollText.text = t;
    }
}
