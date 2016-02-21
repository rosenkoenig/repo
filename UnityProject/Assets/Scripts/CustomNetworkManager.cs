using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class CustomNetworkManager : NetworkManager {

    public List<GameObject> playerInstances = new List<GameObject>();

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        playerInstances.Add(player);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        if (playerInstances.Count >= 2) GameMaster.Instance.OnAllPlayersCreated();
    }

    IEnumerator CheckAllPlayersSpawned ()
    {
        if ( playerInstances.Count >= 2)
        {
            bool allAreReady = false;
            while (!allAreReady)
            {
                foreach (GameObject go in playerInstances)
                {
                    if (!go.GetComponent<NetworkPlayerCreate>().isReady)
                    {
                        yield return null;
                    }
                }

              
                AllPlayersConnected();
              allAreReady = true;
            }
            
        }
    }

    void AllPlayersConnected ()
    {

        Rpc_ApplySeed();
        
    }

    [ClientRpc]
    void Rpc_ApplySeed ()
    {
        Pioche.Instance.Shuffle(125);
        StartGame();
    }

    [Command]
    void StartGame ()
    {
        GameMaster.Instance.OnAllPlayersCreated();
    }
}
