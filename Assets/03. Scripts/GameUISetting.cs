using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUISetting : MonoBehaviour
{
    public TextMeshProUGUI rollText;
    public Button killButton;

    private void Awake()
    {
        PlayerInitializer.onSetPlayer += SetRollText;
        MafiaPlayer.OnSetMafia += SetupMafiaUI;
    }

    void SetRollText(string t)
    {
        rollText.text = t;
    }

    private void SetupMafiaUI(MafiaPlayer mafiaPlayer)
    {
        Debug.Log("SetMafiaUI");
        killButton.gameObject.SetActive(true);

        killButton.onClick.RemoveAllListeners();
        killButton.onClick.AddListener(() =>
        {
            mafiaPlayer.Kill();
        });
    }
}
