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
        player.GetComponent<NetworkPlayerCreate>().SetOwner();
        CheckAllPlayersSpawned();
    }

    void CheckAllPlayersSpawned ()
    {
        if ( playerInstances.Count >= 2)
        {
            foreach ( GameObject go in playerInstances)
            {
                if ( !go.GetComponent<NetworkPlayerCreate>().isReady)
                {
                    return;
                }
            }

            GameMaster.Instance.OnAllPlayersCreated();
        }
    }
}
