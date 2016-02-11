using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crown : MonoBehaviour {
    public static Crown Instance;

    public Square currentSquare = null;

    public bool withPowerCard = false;

    [SerializeField]
    GameObject powerCardFeedback = null;

    [SerializeField]
    float movementDamping = 0.3f;

    Vector3 targetPosition = Vector3.zero;

    // Use this for initialization
    void Awake ()
    {
        Instance = this;
        powerCardFeedback.SetActive(false);
    }

    void Start () {
        GoTo(Board.Instance.GetSquare(4, 4), Owner.NONE);
	}

    void Update ()
    {



    }

    public bool GoTo ( Square square, Owner owner )
    {
        bool canGo = CanGoTo(square, owner, false);
        if (canGo)
        {
            if (withPowerCard && square.owner == Owner.NONE) GameMaster.Instance.GetHandHUDFor(owner).powerCardsCount++;

            targetPosition = square.transform.position;
            if (displacementCoroutine != null ) StopCoroutine(displacementCoroutine);
            displacementCoroutine = StartCoroutine(Move());
            square.SetOwner(owner);
            currentSquare = square;

        }

        return canGo;

    }

    public bool GoTo ( Vector2 delta, Owner owner )
    {
        Square target = Board.Instance.GetSquare(currentSquare.xIndex + (int)delta.x, currentSquare.yIndex +(int) delta.y);

       return  GoTo(target, owner);
    }

    public bool CanGoTo (Square square, Owner owner, bool simulatePowerCard)
    {
        if ((simulatePowerCard || withPowerCard) && square.owner != owner) return true;

        bool squareIsAvailable = square.owner == Owner.NONE;

        if (!squareIsAvailable )
        {
            Debug.Log("Cannot move to square because already occupied");
        }


        return squareIsAvailable;
    }

    public bool CanGoTo(Vector2 delta, Owner owner, bool simulatePowerCard)
    {
        Square target = Board.Instance.GetSquare(currentSquare.xIndex + (int)delta.x, currentSquare.yIndex + (int)delta.y);

        if (target == null)
        {
            Debug.Log("Cannot move because out of board.");
            return false;
        }

        return CanGoTo(target, owner, simulatePowerCard);
    }

    public void DisplayTargetSquareFeedback (bool state, Vector2 delta, Owner owner)
    {
        Square target = Board.Instance.GetSquare(currentSquare.xIndex + (int)delta.x, currentSquare.yIndex + (int)delta.y);

        if (target == null)
        {
            Debug.Log("No target square!");
            
        }
        else
        {
            target.SetTargetSquareFeedbackActive(state, owner);

            if ( true )
            {
                List<Card> cards = GameMaster.Instance.GetHandHUDFor(owner == Owner.PLAYER_0 ? Owner.PLAYER_1 : Owner.PLAYER_0).cards;
                foreach (Card card in cards)
                {
                    Square targetAfter = Board.Instance.GetSquare(currentSquare.xIndex + (int)delta.x + (int)card.cardInfos.direction.x*card.cardInfos.nb_squares, currentSquare.yIndex + (int)delta.y + (int)card.cardInfos.direction.y * card.cardInfos.nb_squares);
                    if (targetAfter != null)
                    {
                        targetAfter.SetTargetAfterSquareFeedbackActive(state, owner == Owner.PLAYER_0 ? Owner.PLAYER_1 : Owner.PLAYER_0);
                    }
                }
            }
        }
    }

    public void SetPowerCardState ( bool state)
    {
        if ( state != withPowerCard )
        {
            withPowerCard = state;
            powerCardFeedback.SetActive(state);
            if (onPowerCardStateChanges != null) onPowerCardStateChanges();
        }
        
    }
    public System.Action onPowerCardStateChanges = null;

    Coroutine displacementCoroutine = null;
    IEnumerator Move ()
    {
        while ((targetPosition - transform.position).magnitude > 0.02f )
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, movementDamping);
            yield return new WaitForEndOfFrame();
        }

    }
}
