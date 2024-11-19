using System;
using UnityEngine;
using UnityEngine.UI;

public class MafiaPlayer : GamePlayer, IMafia
{
    public static event Action<Button> OnSetMafia;

    private void Awake()
    {
        OnSetMafia();
    }

    void KillBtnSet(Button killBtn)
    {
        killBtn.gameObject.SetActive(true);
    }

    public void CloseGate()
    {
        throw new System.NotImplementedException();
    }

    public void Kill()
    {
        Collider[] crew = Physics.OverlapSphere(transform.position, 0.5f);

        if(crew != null)
        {
            ICrew c = crew[0].GetComponent<ICrew>();
            if(c != null)
            {
                c.CrewDie();
            }
        }
    }

    public void Vent()
    {
        throw new System.NotImplementedException();
    }
}
