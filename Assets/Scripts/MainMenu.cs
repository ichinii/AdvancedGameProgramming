using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button m_continueGameButton;

    void Start()
    {
        m_continueGameButton.enabled = PersistController.Instance.RecentLevel > StateController.Instance.FirstLevelIndex;
    }

    public void ActionContinueGame()
    {
        Debug.Log("ActionContinueGame");
        StateController.Instance.TransitionPlaying(PersistController.Instance.RecentLevel);
    }

    public void ActionNewGame()
    {
        Debug.Log("ActionNewGame");
        StateController.Instance.TransitionPlaying(StateController.Instance.FirstLevelIndex);
    }

    public void ActionQuit()
    {
        Debug.Log("ActionQuit");
        Application.Quit();
    }
}
