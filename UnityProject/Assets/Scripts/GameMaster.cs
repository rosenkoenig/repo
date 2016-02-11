using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {
    public static GameMaster Instance;

    [Tooltip("0 = PLAYER_0,\n1 = PLAYER_1")]
    [SerializeField]
    PlayerHandHUD[] playerHandHuds = new PlayerHandHUD[2];

    public int turnIndex = 0;

	// Use this for initialization
    void Awake ()
    {
        Instance = this;

    }

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public PlayerHandHUD GetHandHUDFor ( Owner owner )
    {
        if (owner == playerHandHuds[0].owner) return playerHandHuds[0];
        else return playerHandHuds[1];
    }

    public void OnPlayerEndTurn ( )
    {
        turnIndex = turnIndex == 0 ? 1 : 0;
        Crown.Instance.SetPowerCardState(false);
        UpdateAllCardsInteractivity();
        Debug.Log(turnIndex);
    }

    public bool isItsTurnToPlay (Owner owner)
    {
        if (turnIndex == (int)owner) return true;
        else {
            Debug.Log("Not your turn to play.");
            return false;
        }
    }

    void UpdateAllCardsInteractivity()
    {
        foreach (PlayerHandHUD playerHand in playerHandHuds)
        {
            foreach (Card card in playerHand.cards)
            {
                card.UpdateInteractivity();
            }

        }
    
    }
}
