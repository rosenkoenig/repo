using UnityEngine;
using System.Collections;

public enum Owner
{
    PLAYER_0,
    PLAYER_1,
    NONE,
}

public class Square : MonoBehaviour {

    public Owner owner = Owner.NONE;

    [HideInInspector]
    public int xIndex = -1;

    [HideInInspector]
    public int yIndex = -1;

    [Tooltip("0 = PLAYER_0,\n1 = PLAYER_1,\n3= NONE")]
    [SerializeField]
    GameObject[] statesVisuals;

    [Tooltip("0 = PLAYER_0,\n1 = PLAYER_1,\n3= CANNOT")]
    [SerializeField]
    GameObject[] targetSquareFeedbacks;

    [Tooltip("0 = PLAYER_0,\n1 = PLAYER_1,\n3= CANNOT")]
    [SerializeField]
    GameObject[] targetAfterSquareFeedbacks;



    // Use this for initialization
    void Start () {
        InitVisuals();

    }
	
	// Update is called once per frame
	void Update () {
    }

    //IEnumerator LetsDanceDiscoMusic()
    //{
    //    while (true) { 
    //        owner = (Owner)Random.Range(0, 3);
    //        InitVisual();

    //        yield return new WaitForSeconds(0.069f);
    //    }
    //}

    void InitVisuals()
    {
        UpdateVisual();
        for (int i = 0; i < targetSquareFeedbacks.Length; i++)
        {
            targetSquareFeedbacks[i].SetActive(false);
        }
        for (int i = 0; i < targetSquareFeedbacks.Length; i++)
        {
            targetAfterSquareFeedbacks[i].SetActive(false);
        }
    }

    void UpdateVisual ()
    {
        for ( int i = 0; i < statesVisuals.Length; i++)
        {
            if (i != (int)owner)
                statesVisuals[i].SetActive(false);
            else if (!statesVisuals[i].activeSelf)
                statesVisuals[i].SetActive(true);
        }
    }

    public void SetOwner ( Owner newOwner )
    {
        owner = newOwner;
        UpdateVisual();
    }

    public void SetTargetSquareFeedbackActive ( bool state, Owner _owner )
    {
        for (int i = 0; i < targetSquareFeedbacks.Length; i++)
        {
            if (i != (int)_owner)
                targetSquareFeedbacks[i].SetActive(false);
            else if (targetSquareFeedbacks[i].activeSelf != state)
                targetSquareFeedbacks[i].SetActive(state);
        }
    }

    public void SetTargetAfterSquareFeedbackActive(bool state, Owner _owner)
    {
        for (int i = 0; i < targetAfterSquareFeedbacks.Length; i++)
        {
            if (i != (int)_owner)
                targetAfterSquareFeedbacks[i].SetActive(false);
            else if (targetAfterSquareFeedbacks[i].activeSelf != state)
                targetAfterSquareFeedbacks[i].SetActive(state);
        }
    }
}
