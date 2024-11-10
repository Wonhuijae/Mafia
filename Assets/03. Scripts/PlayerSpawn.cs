using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    private NetworkManager instance;

    private void Awake()
    {
        instance = NetworkManager.Instance;
    }

   
}
