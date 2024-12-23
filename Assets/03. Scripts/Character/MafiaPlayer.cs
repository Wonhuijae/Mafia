using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MafiaPlayer : GamePlayer, IMafia
{
    public static event Action<MafiaPlayer> OnSetMafia;

    void Start()
    {
        if (pv.IsMine) OnSetMafia(this);
    }

    public void CloseGate()
    {
        throw new System.NotImplementedException();
    }

    public void Kill()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, 1f);
        foreach (var coll in colls)
        {
            ICrew crew = coll.GetComponent<ICrew>();
            if(crew != null)
            {
                crew.CrewDie();
            }
        }
    }

    public void Vent()
    {
        throw new System.NotImplementedException();
    }
}
