using UnityEngine;
using System.Collections;

public class NewTurnFeedback : MonoBehaviour {



    float startTime = 0f;


    [SerializeField]
    float duration = 0.5f;

    [SerializeField]
    GameObject[] visualPerPlayer = null;

	// Use this for initialization
	void Start () {
        GameMaster.Instance.onTurnEnds += StartFeedback;
        ResetVisuals();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartFeedback ()
    {
        int owner = GameMaster.Instance.turnIndex;

        for ( int i = 0; i < visualPerPlayer.Length; i++)
        {
            if (i == owner) visualPerPlayer[i].SetActive(true);
            else visualPerPlayer[i].SetActive(false);
        }

        StartCoroutine("Timer");
    }

    IEnumerator Timer ()
    {
        yield return new WaitForSeconds(duration);
        ResetVisuals();
    }

    void ResetVisuals ()
    {

        for (int i = 0; i < visualPerPlayer.Length; i++)
        {
            visualPerPlayer[i].SetActive(false);
        }
    }
}
