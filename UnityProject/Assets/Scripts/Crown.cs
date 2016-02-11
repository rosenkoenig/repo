using UnityEngine;
using System.Collections;

public class Crown : MonoBehaviour {
    public static Crown Instance;

    public Square currentSquare = null;

    public bool withPowerCard = false;

    // Use this for initialization
    void Awake ()
    {
        Instance = this;

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
            transform.position = square.transform.position;
            square.SetOwner(owner);
            currentSquare = square;
            if (withPowerCard) GameMaster.Instance.GetHandHUDFor(owner).powerCardsCount--;
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

        }
    }
}
