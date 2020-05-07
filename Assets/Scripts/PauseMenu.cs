using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("PauseMenu instantiated");
    }

    void Update()
    {
        
    }

    public void ActionResumeGame()
    {
        Debug.Log("PauseMenu ActionResumeGame");
        StateController.Instance.TransitionPlaying();
    }

    public void ActionMainMenu()
    {
        Debug.Log("PauseMenu ActionMainMenu");
        StateController.Instance.TransitionMainMenu();
    }
}
