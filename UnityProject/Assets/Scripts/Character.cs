using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour
{

    //[SerializeField]
    public Owner owner = Owner.PLAYER_0;

    PlayerHandHUD myHandHUD = null;

	// Use this for initialization
	void Awake () {
        GameMaster.Instance.onGameStarts += OnGameStarts;
	}

    void OnGameStarts ()
    {
        GameMaster.Instance.onGameStarts -= OnGameStarts;
        
        myHandHUD = GameMaster.Instance.GetHandHUDFor(owner);

        StartCoroutine("DrawAllCards");
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

    IEnumerator DrawAllCards ()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.3f);
        }

        //GameMaster.Instance.SetGameState(GameState.PLAYING);
    }
}
