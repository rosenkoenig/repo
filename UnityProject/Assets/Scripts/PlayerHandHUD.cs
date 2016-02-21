using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandHUD : MonoBehaviour {
    

    [SerializeField]
    GameObject cardPrefab = null;

    public List<Card> cards = new List<Card>();

    public int powerCardsCount = 0;

    public Owner owner = Owner.NONE;

    //[SerializeField]
    public Transform cardParent = null;

    // Use this for initialization
    void Start () {
        powerCardsCount = 4;
        if (cardParent == null) cardParent = transform;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddCard ( CardInfos cardInfos )
    {
        GameObject Host = GameObject.Instantiate(cardPrefab);
        Host.transform.SetParent(cardParent);

        Card visualCard = Host.GetComponent<Card>();

        visualCard.cardInfos = cardInfos;
        visualCard.onCardIsPlayed += RemoveCard;

        Host.transform.localScale = Vector3.one;

        cards.Add(visualCard);
    }

    public void RemoveCard ( Card card )
    {
        if ( cards.Contains(card))
        {
            card.onCardIsPlayed -= RemoveCard;
            cards.Remove(card);
            Pioche.Instance.AddCardInPile(card.cardInfos);
        }

    }
}
