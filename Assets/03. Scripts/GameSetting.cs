using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    public TextMeshProUGUI mafiaNumUI;
    public Button plusBTN;
    public Button minusBTN;

    public static event Action<int> OnSetNumber;

    int mafiaNum = 1;

    private void Start()
    {
        mafiaNumUI.text = mafiaNum.ToString();
    }


    public void PlusMafiaNum()
    {
        mafiaNum++;

        SetButton();
    }

    public void MinusMafiaNum()
    {
        mafiaNum--;

        SetButton();
    }

    void SetButton()
    {
        if (mafiaNum >= 2)
        {
            plusBTN.interactable = false;
        }
        else if (mafiaNum <= 1)
        {
            minusBTN.interactable = false;
        }

        mafiaNumUI.text = mafiaNum.ToString();
        OnSetNumber(mafiaNum);
    }
}
