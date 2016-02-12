using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
    PLAYING,
    PAUSED,
    NOT_STARTED,
    FINISHED,
}

public class GameMaster : MonoBehaviour {
    public static GameMaster Instance;

    [Tooltip("0 = PLAYER_0,\n1 = PLAYER_1")]
    [SerializeField]
    PlayerHandHUD[] playerHandHuds = new PlayerHandHUD[2];

    [SerializeField]
    AbstractInGameMenu pauseMenu = null;

    public int turnIndex = 0;

    public System.Action onTurnEnds = null;

    public int availableTokens = 54;

    GameState _gameState = GameState.NOT_STARTED;
    public GameState gameState
    {
        get { return _gameState;  }
    }
    public System.Action<GameState, GameState> onGameStateChanges = null;
    public System.Action simpleOnGameStateChanges = null;
    public System.Action onGameStarts = null;
    public System.Action onLoadingIsOver = null;

	// Use this for initialization
    void Awake ()
    {
        Instance = this;
        pauseMenu.gameObject.SetActive(false);
    }

	void Start ()
    {
        StartCoroutine("simulateLoading");
    }

    IEnumerator simulateLoading ()
    {
        yield return new WaitForSeconds(0.3f);
        StartGame();
    }

    void StartGame ()
    {
        if (onLoadingIsOver != null) onLoadingIsOver();
        SetGameState(GameState.PLAYING);
        if (onGameStarts != null) onGameStarts();
    }
	
	// Update is called once per frame
	void Update () {
        CheckPause();

    }

    void CheckPause ()
    {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (pauseMenu.gameObject.activeInHierarchy)
            {
                pauseMenu.LeaveMenu();
            }
            else
            {
                pauseMenu.gameObject.SetActive(true);
            }

        }
    }

    public void SetGameState (GameState value)
    {
        if (_gameState != value)
        {
            if (onGameStateChanges != null) onGameStateChanges(_gameState, value);
            _gameState = value;
            if (simpleOnGameStateChanges != null) simpleOnGameStateChanges();
        }
    }

    public PlayerHandHUD GetHandHUDFor ( Owner owner )
    {
        if (owner == playerHandHuds[0].owner) return playerHandHuds[0];
        else return playerHandHuds[1];
    }

    public void OnPlayerEndTurn ( )
    {
        turnIndex = turnIndex == 0 ? 1 : 0;
        Crown.Instance.SetPowerCardState(false);
        UpdateAllCardsInteractivity();
        if (onTurnEnds != null) onTurnEnds();
    }

    public bool isItsTurnToPlay (Owner owner)
    {
        if (turnIndex == (int)owner) return true;
        else {
            return false;
        }
    }

    void UpdateAllCardsInteractivity()
    {
        foreach (PlayerHandHUD playerHand in playerHandHuds)
        {
            foreach (Card card in playerHand.cards)
            {
                card.UpdateInteractivity();
            }

        }
    
    }

    public void OnTokenUsed ()
    {
        availableTokens--;
    }
    
}
