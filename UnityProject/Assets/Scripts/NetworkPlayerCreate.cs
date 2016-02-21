using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkPlayerCreate : NetworkBehaviour
{
    //Use this for initialization
    void Start ()
    {
        SetOwner();
    }

    void SetOwner()
    {
        if (!isLocalPlayer)
        {
            GetComponent<PlayerHandHUD>().owner = Owner.PLAYER_1;
            GetComponent<Character>().owner = Owner.PLAYER_1;
            GetComponent<PlayerHandHUD>().cardParent = GameObject.Find("Player_1_HandHUD").transform;
            GameMaster.Instance.playerHandHuds[1] = GetComponent<PlayerHandHUD>();
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<PlayerHandHUD>().owner = Owner.PLAYER_0;
        GetComponent<Character>().owner = Owner.PLAYER_0;
        GetComponent<PlayerHandHUD>().cardParent = GameObject.Find("Player_0_HandHUD").transform;
        GameMaster.Instance.playerHandHuds[0] = GetComponent<PlayerHandHUD>();
        GameMaster.Instance.StartGame();
    }
}
