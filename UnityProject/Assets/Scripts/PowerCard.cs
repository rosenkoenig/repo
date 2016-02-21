using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerCard : MonoBehaviour {
    Crown crown = null;

    public Owner owner = Owner.PLAYER_0;

    [SerializeField]
    Text countText = null;

    [SerializeField]
    GameObject selectedFeedback = null;

    PlayerHandHUD myHandHUD = null;
    GameMaster gameMaster = null;

	// Use this for initialization
	void Start () {
        crown = Crown.Instance;
        gameMaster = GameMaster.Instance;
        gameMaster.onGameStarts += OnGameStarts;
        crown.onPowerCardStateChanges += UpdatePowerCardInteractivity;
        gameMaster.onTurnEnds += UpdatePowerCardInteractivity;
        gameMaster.simpleOnGameStateChanges += UpdatePowerCardInteractivity;
        UpdatePowerCardInteractivity();
    }

    void OnGameStarts ()
    {
        myHandHUD = GameMaster.Instance.GetHandHUDFor(owner);

    }
	
	// Update is called once per frame
	void Update () {
        if (gameMaster.gameState > 0) return;

        countText.text = "("+myHandHUD.powerCardsCount.ToString()+")";
    }

    public void OnClick ()
    {
        if (gameMaster.gameState > 0) return;

        if ((myHandHUD.powerCardsCount > 0 || (myHandHUD.powerCardsCount == 0 && crown.withPowerCard)) && GameMaster.Instance.turnIndex == (int)owner)
        {
            myHandHUD.Cmd_AddPowerCardsCount(crown.withPowerCard ? 1 : -1);
            crown.SetPowerCardState(!crown.withPowerCard);
            selectedFeedback.SetActive(crown.withPowerCard);
        }
    }

    void UpdatePowerCardInteractivity ()
    {
        Button button = GetComponent<Button>();
        if ( gameMaster.gameState > 0 )
        {
            button.interactable = false;
            selectedFeedback.SetActive(false);
            return;
        }

        if ( GameMaster.Instance.turnIndex != (int)owner)
        {
            button.interactable = false;
            selectedFeedback.SetActive(false);
        }
        else
        {
            button.interactable = true;
            bool targetState = crown.withPowerCard;
            if (selectedFeedback.activeSelf != targetState) selectedFeedback.SetActive(targetState);
        }
        
    }
}
