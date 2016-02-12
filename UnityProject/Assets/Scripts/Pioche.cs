using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Pioche : MonoBehaviour {
    public static Pioche Instance;


    List<CardInfos> cards = new List<CardInfos>();

    GameMaster gameMaster = null;

	// Use this for initialization
    void Awake ()
    {
        Instance = this;
    }

	void Start () {
        gameMaster = GameMaster.Instance;
        gameMaster.onLoadingIsOver += GenerateAllCards;
        gameMaster.simpleOnGameStateChanges += UpdateInteractivity;
        UpdateInteractivity();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void GenerateAllCards ()
    {


        for ( int i = 0; i < 8; i++)
        {
            Vector2 finalDir = Vector2.zero;

            switch ( i )
            {
                case 0:
                    finalDir = new Vector2(-1, 0);
                    break;
                case 1:
                    finalDir = new Vector2(-1, 1);
                    break;
                case 2:
                    finalDir = new Vector2(0, 1);
                    break;
                case 3:
                    finalDir = new Vector2(1, 1);
                    break;
                case 4:
                    finalDir = new Vector2(1, 0);
                    break;
                case 5:
                    finalDir = new Vector2(1, -1);
                    break;
                case 6:
                    finalDir = new Vector2(0, -1);
                    break;
                case 7:
                    finalDir = new Vector2(-1, -1);
                    break;

            }

            for (int j = 1; j < 4; j++)
            {
                CardInfos card = new CardInfos();
                card.direction = finalDir;
                card.nb_squares = j;
                cards.Add(card);
            }

        }

        Shuffle();
    }

    public CardInfos DrawCard ()
    {
        CardInfos toReturn = cards[0];

        cards.RemoveAt(0);

        return toReturn;
    }

    public void OnDrawButton()
    {
        Owner newOwner = (Owner)GameMaster.Instance.turnIndex;

        if ( GameMaster.Instance.GetHandHUDFor(newOwner).cards.Count >= 5 )
        {
            Debug.Log("Cant draw because " + newOwner.ToString() + "'s hand is full.");
            return;
        }

        CardInfos newCard = DrawCard();
        newCard.owner = newOwner;
        GameMaster.Instance.GetHandHUDFor(newOwner).AddCard(newCard);

        GameMaster.Instance.OnPlayerEndTurn();
    }

    public void AddCardInPile (CardInfos cardInfo)
    {
        cards.Add(cardInfo);

    }

    void Shuffle ()
    {
        List<CardInfos> deck = cards;
        List<CardInfos> shuffledDeck = new List<CardInfos>();
        int deckCount = deck.Count;
        for ( int i = 0; i < 24; i++)
        {

            int idx = Random.Range(0, deck.Count);

            shuffledDeck.Add(deck[idx]);
            deck.RemoveAt(idx);
            
        }

        cards = shuffledDeck;
    }

    void UpdateInteractivity ()
    {
        Button[] buttons = GetComponentsInChildren<Button>();

        bool mustBeInteractable = GameMaster.Instance.gameState == GameState.PLAYING;

        foreach ( Button button in buttons )
        {
            button.interactable = mustBeInteractable;
        }
    }
}
