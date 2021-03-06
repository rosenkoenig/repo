﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerTextHUD : MonoBehaviour {
    public static PlayerTextHUD Instance = null;


    float startTime = 0f;


    float duration = 0.5f;

    [SerializeField]
    float defaultDuration = 3f;

    [SerializeField]
    float newTurnDuration = 0.5f;

    [SerializeField]
    GameObject[] visualPerPlayer = null;

    [SerializeField]
    Text[] titleTextsPerPlayer = null;

    [SerializeField]
    Text[] textsPerPlayer = null;

    Coroutine timerCoroutineInstance = null;

	// Use this for initialization
    void Awake ()
    {
        Instance = this;
    }

	void Start () {
        GameMaster.Instance.onTurnEnds += StartTurnFeedback;
        ResetVisuals();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartTurnFeedback ()
    {
        int owner = GameMaster.Instance.turnIndex;

        for ( int i = 0; i < visualPerPlayer.Length; i++)
        {
            if (i == owner) visualPerPlayer[i].SetActive(true);
            else visualPerPlayer[i].SetActive(false);
        }

        for (int i = 0; i < textsPerPlayer.Length; i++)
        {
            titleTextsPerPlayer[i].text = "Your turn!";
            textsPerPlayer[i].text = GameMaster.Instance.availableTokens.ToString()+" tokens left.";
        }

        duration = newTurnDuration;
        if (timerCoroutineInstance != null) StopCoroutine(timerCoroutineInstance);
        timerCoroutineInstance = StartCoroutine("Timer");
    }
    
    public void StartFeedback(Owner _owner, string titleText, string text, float _duration)
    {
        for (int i = 0; i < visualPerPlayer.Length; i++)
        {
            if (i == (int)_owner) visualPerPlayer[i].SetActive(true);
            else visualPerPlayer[i].SetActive(false);
        }

        for (int i = 0; i < textsPerPlayer.Length; i++)
        {
            textsPerPlayer[i].text = text;
            titleTextsPerPlayer[i].text = titleText;
        }

        if ( _duration > 0 )
        {
            duration = _duration;
            if (timerCoroutineInstance != null) StopCoroutine(timerCoroutineInstance);
            timerCoroutineInstance = StartCoroutine("Timer");
        }

    }
    public void StartFeedback(Owner _owner, string titleText, string text)
    {
        StartFeedback(_owner, titleText, text, defaultDuration);
    }

    IEnumerator Timer ()
    {
        yield return new WaitForSeconds(duration);
        ResetVisuals();
    }

    public void ResetVisuals ()
    {

        for (int i = 0; i < visualPerPlayer.Length; i++)
        {
            visualPerPlayer[i].SetActive(false);
        }

        if (timerCoroutineInstance != null) StopCoroutine(timerCoroutineInstance);
    }
}
