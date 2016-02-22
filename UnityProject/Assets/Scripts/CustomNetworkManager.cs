using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class CustomNetworkManager : NetworkManager {

    void Start ()
    {
        Connect();
    }

    void Connect ()
    {
        StartMatchMaker();
        matchMaker.ListMatches(0, 10, "", OnMatchList);
    }

    public override void OnMatchList(ListMatchResponse matchList)
    {
        base.OnMatchList(matchList);
        if ( matchList.matches.Count <= 0 )
        {
            //create matchs
            matchMaker.CreateMatch("server", 2u, false, "", OnMatchCreate);
        }
        else
        {
            //connect
            MatchDesc match = matchList.matches[0];
            matchMaker.JoinMatch(match.networkId, "", OnMatchJoined);
        }
        
    }


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


                GameMaster.Instance.OnAllPlayersCreated();
              allAreReady = true;
            }
            
        }
    }

   
}
