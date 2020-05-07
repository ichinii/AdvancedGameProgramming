using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StateController.Instance.TransitionMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActionContinueGame()
    {
        Debug.Log("ActionContinueGame");
        StateController.Instance.TransitionPlaying(1);
    }

    public void ActionNewGame()
    {
        Debug.Log("ActionNewGame");
        StateController.Instance.TransitionPlaying(1);
    }

    public void ActionQuit()
    {
        Application.Quit();
    }
}
