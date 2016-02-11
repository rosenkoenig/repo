using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerCard : MonoBehaviour {
    Crown crown = null;

    public Owner owner = Owner.PLAYER_0;

    [SerializeField]
    Text countText = null;

    PlayerHandHUD myHandHUD = null;

	// Use this for initialization
	void Start () {
        myHandHUD = GameMaster.Instance.GetHandHUDFor(owner);
        crown = Crown.Instance;
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
        }
    }
}
