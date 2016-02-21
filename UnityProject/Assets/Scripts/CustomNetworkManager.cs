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
        player.GetComponent<NetworkPlayerCreate>().SetOwner();
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        CheckAllPlayersSpawned();
    }

    void CheckAllPlayersSpawned ()
    {
        if ( playerInstances.Count >= 2)
        {
            GameMaster.Instance.OnAllPlayersCreated();
        }
    }
}
