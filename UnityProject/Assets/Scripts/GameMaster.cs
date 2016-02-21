using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public enum GameState
{
    PLAYING,
    PAUSED,
    NOT_STARTED,
    FINISHED,
}

public class GameMaster : NetworkBehaviour {
    public static GameMaster Instance;

    [Tooltip("0 = PLAYER_0,\n1 = PLAYER_1")]
    [SerializeField]
    public PlayerHandHUD[] playerHandHuds = new PlayerHandHUD[2];

    [SerializeField]
    AbstractInGameMenu pauseMenu = null;

    [SerializeField]
    float endTurnWaitDuration = 3f;

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
        
    }

    public void DirtyLaunch ()
    {
        Debug.LogWarning("Non non non je suis temporaire");

        StartCoroutine("simulateLoading");
    }

    public override void OnStartServer()
    {
        Debug.Log("OnStartServer GameMaster", this);
        base.OnStartServer();
    }

    public void OnAllPlayersCreated ()
    {
        if ( isServer )
        {
            Debug.Log("OnAllPLayersCreated", this);
            RpcStartGame();
        }
        
    }

    [ClientRpc]
    public void RpcStartGame ()
    {
        Debug.Log("RPC StartGame, launch OnGameStarts", this);

        StartCoroutine(wait());
    }
	

    IEnumerator wait ()
    {

        yield return new WaitForSeconds(2f);

        if (onLoadingIsOver != null) onLoadingIsOver();
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
        if (onTurnEnds != null) onTurnEnds();
        if (!CheckCanPlay((Owner)turnIndex))
        {
            if ( !CheckCanPlay((Owner)(turnIndex == 0 ? 1 : 0)))
            {
                // fin de partie
                OnGameEnds();
            }
            else
            {
                StartCoroutine(PasseLeTour((Owner)turnIndex));
                SetGameState(GameState.PAUSED);
            }
        }
        UpdateAllCardsInteractivity();
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
                if (card) card.UpdateInteractivity();
            }

        }
    
    }

    bool CheckCanPlay ( Owner owner )
    {
        PlayerHandHUD curPlayHand = playerHandHuds[(int)owner];

        if (curPlayHand.powerCardsCount <= 0 && availableTokens <= 0)
        {
            return false;
        }

        if (curPlayHand.cards.Count >= 5)
        {
            bool oneCardCanBePlayed = false;
            foreach ( Card card in curPlayHand.cards)
            {
                if ( card.CanBePlayed())
                {
                    return true;
                }
            }

            if ( !oneCardCanBePlayed )
            {
                return false;
            }

        }
        return true;
    }

    public void OnTokenUsed ()
    {
        availableTokens--;
    }


    IEnumerator PasseLeTour (Owner owner)
    {
        PlayerTextHUD.Instance.StartFeedback(owner, "Turn skipped", "You can't play anything.", endTurnWaitDuration);
        yield return new WaitForSeconds(endTurnWaitDuration);
        SetGameState(GameState.PLAYING);
        OnPlayerEndTurn();
    }
    

    void OnGameEnds ()
    {
        //check winner ^^
        PlayerTextHUD.Instance.StartFeedback((Owner)Random.Range(0,2), "Fin de partie", "On sait pas qui a gagné ^^", 100f);
        SetGameState(GameState.FINISHED);
    }
}
