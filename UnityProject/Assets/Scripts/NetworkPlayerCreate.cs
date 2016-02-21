using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkPlayerCreate : NetworkBehaviour
{
    //Use this for initialization
    void Start ()
    {
        
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
        else
        {
            GetComponent<PlayerHandHUD>().owner = Owner.PLAYER_0;
            GetComponent<Character>().owner = Owner.PLAYER_0;
            GetComponent<PlayerHandHUD>().cardParent = GameObject.Find("Player_0_HandHUD").transform;
            GameMaster.Instance.playerHandHuds[0] = GetComponent<PlayerHandHUD>();
        }
    }

    public override void OnStartServer ()
    {
        Debug.Log("OnStartServer");

    }

    public override void OnStartClient ()
    {
        Debug.Log("OnStartClient "+ isLocalPlayer+" et isServer :"+isServer, this);
        SetOwner();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer "+isLocalPlayer, this);
        SetOwner();
       // GetComponent<Character>().OnGameStarts();
    }
}
