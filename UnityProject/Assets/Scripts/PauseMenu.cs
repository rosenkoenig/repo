using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : AbstractInGameMenu {

    [SerializeField]
    AbstractInGameMenu optionsMenu = null;
    
    void OnEnable ()
    {
        if (GameMaster.Instance) GameMaster.Instance.SetGameState(GameState.PAUSED);
    }

    public override void LeaveMenu()
    {
        base.LeaveMenu();
        GameMaster.Instance.SetGameState(GameState.PLAYING);
    }

    public void OnQuit()
    {
        SceneManager.LoadScene(0);
    }

    public void OnOptionsButton()
    {
        if (optionsMenu) optionsMenu.gameObject.SetActive(true);
        else Debug.LogError("Please do this option popup for chist sake!");
    }
}
