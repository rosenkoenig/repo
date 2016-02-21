using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class CustomNetworkManager : NetworkManager {

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<NetworkPlayerCreate>().SetOwner();
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
