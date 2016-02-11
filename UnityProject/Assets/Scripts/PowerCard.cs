using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerCard : MonoBehaviour {

    public Owner owner = Owner.PLAYER_0;

    [SerializeField]
    Text countText = null;

    PlayerHandHUD myHandHUD = null;

	// Use this for initialization
	void Start () {
        myHandHUD = GameMaster.Instance.GetHandHUDFor(owner);
        
    }
	
	// Update is called once per frame
	void Update () {
        countText.text = myHandHUD.powerCardsCount.ToString();
    }

    public void OnClick ()
    {
        if (myHandHUD.powerCardsCount > 0)
        {
            Crown.Instance.withPowerCard = !Crown.Instance.withPowerCard;
        }
    }
}
