using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkPlayerCreate : NetworkBehaviour
{
    public bool isReady = false;

    //Use this for initialization
    void Start ()
    {
        //SetOwner();
    }

    public void SetOwner()
    {
        Debug.Log("Set Owner , isLocalPlayer = " + isLocalPlayer, this);
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
        isReady = true;
    }

    public override void OnStartServer ()
    {
        Debug.Log("OnStartServer", this);

    }

    public override void OnStartClient ()
    {
        Debug.Log("OnStartClient : isLocalPlayer = "+ isLocalPlayer+" et isServer = "+isServer, this);
        //SetOwner();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer isLocalPlayer =  "+isLocalPlayer, this);
        //SetOwner();
       // GetComponent<Character>().OnGameStarts();
    }
}
