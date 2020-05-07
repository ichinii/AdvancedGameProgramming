using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    public Text m_levelText;

    void Update()
    {
        m_levelText.text = "Level " + PersistController.Instance.RecentLevel;
    }

    public void ActionPause()
    {
        StateController.Instance.TransitionPause();
    }
}
