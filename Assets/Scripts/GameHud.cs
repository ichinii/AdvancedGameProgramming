using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHud : MonoBehaviour
{
    public void ActionPause()
    {
        StateController.Instance.TransitionPause();
    }
}
