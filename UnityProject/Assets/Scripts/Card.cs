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
    public int id = 0;
    public Vector2 direction = Vector2.one;

    public int nb_squares = 0;
    public Owner owner = Owner.NONE;
}

public class Card : MonoBehaviour {

    public Direction _direction = Direction.DOWN;

    public CardInfos cardInfos = new CardInfos();

    public System.Action<int> onCardIsPlayed = null;

    RectTransform rectTransform = null;
    LayoutGroup layoutGroup = null;
    LayoutElement layoutElement = null;
    GameMaster gameMaster = null;

    //visual
    [Header("Visual")]
    [SerializeField]
    Text direction_text = null;

    [SerializeField]
    Text distance_text = null;

    [Tooltip("LEFT,\nUP_LEFT,\nUP,\nUP_RIGHT,\nRIGHT,\nDOWN_RIGHT,\nDOWN,\nDOWN_LEFT, ")]
    [SerializeField]
    Image[] spritePerDirection = null;

    void Awake ()
    {

        gameMaster = GameMaster.Instance;
    }

    void Start ()
    {
        rectTransform = GetComponent<RectTransform>();
        layoutGroup = GetComponentInParent<LayoutGroup>();
        layoutElement = GetComponent<LayoutElement>();
        gameMaster.simpleOnGameStateChanges += UpdateInteractivity;
        GetDirectionAsEnum();
        UpdateDisplay();
        startParent = transform.parent;
        UpdateInteractivity();
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
            case 2:
                return "II";
            case 3:
                return "III";
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
        if (!gameMaster.isItsTurnToPlay(cardInfos.owner)) {
            PlayerTextHUD.Instance.StartFeedback(cardInfos.owner,"", "Not your turn to play!", 1f);
            return false;
        }

        SetTargetSquareFeedbackActive(false);

        bool canMove = Crown.Instance.CanGoTo(cardInfos.direction * cardInfos.nb_squares, cardInfos.owner, Crown.Instance.withPowerCard, true);

        if (canMove)
        {
          GameMaster.Instance.OnCardUsed(cardInfos.id);
            return true;
        }

        return false;
    }

    public void ApllyEffect ()
    {
        if (onCardIsPlayed != null) onCardIsPlayed(cardInfos.id);
        gameMaster.OnPlayerEndTurn();
        Crown.Instance.GoTo(cardInfos.direction * cardInfos.nb_squares, cardInfos.owner);
        Destroy(gameObject);
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

        
        Button button = GetComponent<Button>();

        if (gameMaster.gameState > 0)
        {
            button.interactable = false;
            return;
        }

        button.interactable = CanBePlayed();
    }

    public bool CanBePlayed()
    {
        bool isItsTurnToPlay, canPlayThisCard;

        bool hasPowerCardsRemaining = gameMaster.GetHandHUDFor(cardInfos.owner).powerCardsCount > 0;
        bool canMove = Crown.Instance.CanGoTo(cardInfos.direction * cardInfos.nb_squares, cardInfos.owner, hasPowerCardsRemaining, false);
        isItsTurnToPlay = gameMaster.isItsTurnToPlay(cardInfos.owner);
        canPlayThisCard = canMove && isItsTurnToPlay;

        return canPlayThisCard;
    }

    bool dragging = false;
    Vector3 startPos = Vector3.zero;
    Transform startParent = null;
    // DRAG DROP MANAGEMENT
    public void OnBeginDrag ()
    {
        if (gameMaster.gameState <= 0)
        {
            dragging = true;
            startPos = transform.position;

            OnMouseHover();
        }
    }

    void Update ()
    {
        if (gameMaster.gameState > 0)
        {
            //if ( dragging )
            //{
            //    dragging = false;
            //    ReturnToHand();
            //}
            return;
        }

        if (dragging)
        {
            transform.position = Input.mousePosition;

            if (Mathf.Abs(Input.mousePosition.y - startPos.y) <= (rectTransform.rect.height * GetComponentInParent<Canvas>().scaleFactor))
            {
                layoutElement.ignoreLayout = false;
            }
            else
            {
                layoutElement.ignoreLayout = true;
            }
        }

    }

    public void OnEndDrag ()
    {

        dragging = false;

        //release card if player releases while pausing
        if (gameMaster.gameState > 0)
        {
            ReturnToHand();
            return;
        }
        if ( Mathf.Abs(transform.position.y - startPos.y) >= (rectTransform.rect.height*GetComponentInParent<Canvas>().scaleFactor))
        {
            if ( ValidateInput() )
            {
                OnMouseOut();

            }
            else
            {
                ReturnToHand();
                if (Crown.Instance.withPowerCard) Crown.Instance.SetPowerCardState(false);
            }
        }
        else
        {
            ReturnToHand();
        }
    }

    void ReturnToHand ()
    {
        layoutElement.ignoreLayout = false;
        layoutGroup.SetLayoutVertical();
        layoutGroup.SetLayoutHorizontal();
        OnMouseOut();
    }
    

    void OnDestroy ()
    {
        gameMaster.simpleOnGameStateChanges -= UpdateInteractivity;
    }
}
