using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Direction
{
    LEFT,
    UP_LEFT,
    UP,
    UP_RIGHT,
    RIGHT,
    DOWN_RIGHT,
    DOWN,
    DOWN_LEFT,
}

[System.Serializable]
public class CardInfos
{

    public Vector2 direction = Vector2.one;

    public int nb_squares = 0;
    public Owner owner = Owner.NONE;
}

public class Card : MonoBehaviour {

    public Direction _direction = Direction.DOWN;

    public CardInfos cardInfos = new CardInfos();

    public System.Action<Card> onCardIsPlayed = null;

    RectTransform rectTransform = null;
    LayoutGroup layoutGroup = null;

    //visual
    [Header("Visual")]
    [SerializeField]
    Text direction_text = null;

    [SerializeField]
    Text distance_text = null;

    [Tooltip("LEFT,\nUP_LEFT,\nUP,\nUP_RIGHT,\nRIGHT,\nDOWN_RIGHT,\nDOWN,\nDOWN_LEFT, ")]
    [SerializeField]
    Image[] spritePerDirection = null;


    void Start ()
    {
        rectTransform = GetComponent<RectTransform>();
        layoutGroup = GetComponentInParent<LayoutGroup>();
        GetDirectionAsEnum();
        UpdateDisplay();
        startParent = transform.parent;
    }

    void UpdateDisplay ()
    {
        direction_text.text = cardInfos.direction.x.ToString()+","+ cardInfos.direction.y.ToString();
        distance_text.text = TransformTheNumberIntoARomanNumber();

        for ( int i = 0; i < spritePerDirection.Length; i++)
        {
            if (i == (int)_direction )
            {
                if (spritePerDirection[i].enabled == false) spritePerDirection[i].enabled = true;
            }
            else
            {
                spritePerDirection[i].enabled = false;
            }
        }
    }

    string TransformTheNumberIntoARomanNumber (  )
    {
        switch (cardInfos.nb_squares)
        {
            case 1:
                return "I";
                break;
            case 2:
                return "II";
                break;
            case 3:
                return "III";
                break;
        }
        return "suce";
    }

    void GetDirectionAsEnum ()
    {
        Direction finalDirection = Direction.DOWN;


        if (cardInfos.direction == new Vector2(-1, 0))
        {
            finalDirection = Direction.LEFT;
        }
        else if (cardInfos.direction == new Vector2(-1, 1))
        {
            finalDirection = Direction.UP_LEFT;
        }
        else if (cardInfos.direction == new Vector2(0, 1))
        {
            finalDirection = Direction.UP;
        }
        else if (cardInfos.direction == new Vector2(1, 1))
        {
            finalDirection = Direction.UP_RIGHT;
        }
        else if (cardInfos.direction == new Vector2(1, 0))
        {
            finalDirection = Direction.RIGHT;
        }
        else if (cardInfos.direction == new Vector2(1, -1))
        {
            finalDirection = Direction.DOWN_RIGHT;
        }
        else if (cardInfos.direction == new Vector2(0, -1))
        {
            finalDirection = Direction.DOWN;
        }
        else if (cardInfos.direction == new Vector2(-1, -1))
        {
            finalDirection = Direction.DOWN_LEFT;
        }

        _direction = finalDirection;

    
    }
   

    #region Inputs
    bool ValidateInput ()
    {
        if (!GameMaster.Instance.isItsTurnToPlay(cardInfos.owner)) {
            PlayerTextHUD.Instance.StartFeedback(cardInfos.owner, "Not your turn to play!", 3f);
            return false;
        }

        SetTargetSquareFeedbackActive(false);

        bool canMove = Crown.Instance.CanGoTo(cardInfos.direction * cardInfos.nb_squares, cardInfos.owner, Crown.Instance.withPowerCard, true);

        if (canMove && Crown.Instance.GoTo(cardInfos.direction * cardInfos.nb_squares, cardInfos.owner))
        {
            if (onCardIsPlayed != null) onCardIsPlayed(this);
            GameMaster.Instance.OnPlayerEndTurn();
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    public void OnClick ()
    {
        //ValidateInput();

    }

    public void OnMouseHover ()
    {
        SetTargetSquareFeedbackActive(true);
    }

    public void OnMouseOut ()
    {
        SetTargetSquareFeedbackActive(false);
    }

    #endregion

    void SetTargetSquareFeedbackActive(bool state)
    {
        // if (!GameMaster.Instance.isItsTurnToPlay(cardInfos.owner)) return;
        bool hasPowerCardsRemaining = GameMaster.Instance.GetHandHUDFor(cardInfos.owner).powerCardsCount > 0;
        bool canMove = Crown.Instance.CanGoTo(cardInfos.direction * cardInfos.nb_squares, cardInfos.owner, hasPowerCardsRemaining, false);

        if (!canMove)
        {
            return;
        }

        Crown.Instance.DisplayTargetSquareFeedback(state, cardInfos.direction * cardInfos.nb_squares, cardInfos.owner);
    }

    public void UpdateInteractivity ()
    {
        bool hasPowerCardsRemaining = GameMaster.Instance.GetHandHUDFor(cardInfos.owner).powerCardsCount > 0;
        bool canMove = Crown.Instance.CanGoTo(cardInfos.direction * cardInfos.nb_squares, cardInfos.owner, hasPowerCardsRemaining, false);
        bool isItsTurnToPlay = GameMaster.Instance.isItsTurnToPlay(cardInfos.owner);
        bool canPlayThisCard = canMove && isItsTurnToPlay;

        GetComponent<Button>().interactable = canPlayThisCard;
        

    }

    bool dragging = false;
    Vector3 startPos = Vector3.zero;
    Transform startParent = null;
    // DRAG DROP MANAGEMENT
    public void OnBeginDrag ()
    {


        
            dragging = true;
            startPos = transform.position;

            OnMouseHover();
        
    }

    void Update ()
    {
        if (dragging)
        {
            transform.position = Input.mousePosition;

            if (Mathf.Abs(Input.mousePosition.y - startPos.y) <= (rectTransform.rect.height * GetComponentInParent<Canvas>().scaleFactor))
            {
                GetComponent<LayoutElement>().ignoreLayout = false;
            }
            else
            {
                GetComponent<LayoutElement>().ignoreLayout = true;
            }
        }

    }

    public void OnEndDrag ()
    {
        dragging = false;

        if ( Mathf.Abs(transform.position.y - startPos.y) >= (rectTransform.rect.height*GetComponentInParent<Canvas>().scaleFactor))
        {
            if ( ValidateInput() )
            {

            }
            else
            {
                ReturnToHand();
            }
        }
        else
        {
            ReturnToHand();
        }
        OnMouseOut();
    }

    void ReturnToHand ()
    {
        GetComponent<LayoutElement>().ignoreLayout = false;
        layoutGroup.SetLayoutVertical();
        layoutGroup.SetLayoutHorizontal();
    }
    
}
