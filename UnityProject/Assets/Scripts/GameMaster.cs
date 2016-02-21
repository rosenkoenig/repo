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

    [SerializeField]
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
        turnIndex = isServer ? 0 : 1;
    }

    public void OnAllPlayersCreated ()
    {
        if ( isServer )
        {
            Debug.Log("OnAllPLayersCreated", this);

            int seed = System.Environment.TickCount;
            Rpc_ApplySeed(seed);
        }
        
    }

    [ClientRpc]
    void Rpc_ApplySeed( int seed )
    {
        Pioche.Instance.Shuffle(seed);

        if (isServer)
        {
            RpcStartGame();
        }
    }

    [ClientRpc]
    public void RpcStartGame ()
    {
        Debug.Log("RPC StartGame, launch StartGame", this);

        StartCoroutine(CheckPlayers());
    }

    IEnumerator CheckPlayers ()
    {
        NetworkPlayerCreate[] npcs = FindObjectsOfType<NetworkPlayerCreate>();

        bool allAreReady = false;
        while (!allAreReady)
        {
            foreach (NetworkPlayerCreate npc in npcs)
            {
                if (!npc.isReady)
                {
                    yield return null;
                }
            }

            StartGame();
            allAreReady = true;
        }
    }

    void StartGame ()
    {


        if (onLoadingIsOver != null) onLoadingIsOver();
        if (onGameStarts != null) onGameStarts();

        SetGameState(GameState.PLAYING);
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

    
    public void OnCardUsed ( int id )
    {
        foreach (PlayerHandHUD player in playerHandHuds)
        {
            foreach (Card card in player.cards)
            {
                if (card.cardInfos.id == id)
                {
                    player.UseCard(id);
                    return;
                }
            }
        }

    }

    public void OnDrawButton ()
    {
        playerHandHuds[turnIndex].Cmd_OnDrawButton();
    }

    [ClientRpc]
    public void Rpc_ApplyDraw ()
    {

        playerHandHuds[turnIndex].AddCard();
    }
   

    [ClientRpc]
    public void Rpc_ApplyCardEffect (int id)
    {
        foreach (PlayerHandHUD player in playerHandHuds)
        {
            foreach (Card card in player.cards)
            {
                if (card.cardInfos.id == id)
                {
                    card.ApllyEffect();
                    return;
                }
            }
        }
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
