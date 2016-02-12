using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour
{

    [SerializeField]
    Owner owner = Owner.PLAYER_0;

    PlayerHandHUD myHandHUD = null;

	// Use this for initialization
	void Start () {
        GameMaster.Instance.onGameStarts += OnGameStarts;
	}

    void OnGameStarts ()
    {
        GameMaster.Instance.onGameStarts -= OnGameStarts;

        myHandHUD = GameMaster.Instance.GetHandHUDFor(owner);

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void DrawCard ()
    {

        CardInfos drawnCard = Pioche.Instance.DrawCard();
        drawnCard.owner = owner;
        myHandHUD.AddCard(drawnCard);


    }

}
