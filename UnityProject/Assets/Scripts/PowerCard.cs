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

	// Use this for initialization
	void Start () {
        myHandHUD = GameMaster.Instance.GetHandHUDFor(owner);
        crown = Crown.Instance;
        crown.onPowerCardStateChanges += UpdateInteractivity;
        GameMaster.Instance.onTurnEnds += UpdateInteractivity;
        UpdateInteractivity();
    }
	
	// Update is called once per frame
	void Update () {
        countText.text = myHandHUD.powerCardsCount.ToString();
    }

    public void OnClick ()
    {
        if ((myHandHUD.powerCardsCount > 0 || (myHandHUD.powerCardsCount == 0 && crown.withPowerCard)) && GameMaster.Instance.turnIndex == (int)owner)
        {
            myHandHUD.powerCardsCount += crown.withPowerCard ? 1 : -1;
            crown.SetPowerCardState(!crown.withPowerCard);
            selectedFeedback.SetActive(crown.withPowerCard);
        }
    }

    void UpdateInteractivity ()
    {
        if ( GameMaster.Instance.turnIndex != (int)owner)
        {
            GetComponent<Button>().interactable = false;
            selectedFeedback.SetActive(false);
        }
        else
        {
            GetComponent<Button>().interactable = true;
            bool targetState = crown.withPowerCard;
            if (selectedFeedback.activeSelf != targetState) selectedFeedback.SetActive(targetState);
        }
        
    }
}
