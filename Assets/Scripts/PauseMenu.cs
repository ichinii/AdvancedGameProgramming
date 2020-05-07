using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void ActionResumeGame()
    {
        StateController.Instance.TransitionPlaying();
    }

    void ActionMainMenu()
    {
        StateController.Instance.TransitionMainMenu();
    }
}
