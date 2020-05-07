using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class StateController : Singleton<StateController>
{
    enum State {
        MainMenu,
        PauseMenu,
        Playing,
    }

    [SerializeField] private State m_state = State.MainMenu;
    [SerializeField] private bool m_levelLoaded = false;
    [SerializeField] private int m_currentLevel;
    [SerializeField] private GameObject m_pauseMenu;

    void Start()
    {
        Debug.Log("StateController instiated");
    }

    void Update()
    {

    }

    public void TransitionMainMenu()
    {
        // go to main menu in any case
        SceneManager.LoadScene("MainMenu");
    }

    public void TransitionPause()
    {
        Debug.Log("TransitionPause");

        switch (m_state) {
        case State.MainMenu:
            // crash shortcut for debug purposes, should never happen
            Assert.IsTrue(false);
            break;
        case State.PauseMenu:
            // crash shortcut for debug purposes, should never happen. maybe do nothing here
            Assert.IsTrue(false);
            break;
        case State.Playing:
            // TODO: pause the game. display pause menu
            break;
        }
    }

    public void TransitionPlaying()
    {
        Assert.IsTrue(m_levelLoaded);
        TransitionPlaying(m_currentLevel);
    }

    public void TransitionPlaying(int level)
    {
        Debug.Log("TransitionPlaying (current level = " + m_currentLevel + ", new level = " + level + ")");

        switch (m_state) {
        case State.MainMenu:
            m_levelLoaded = true;
            m_currentLevel = level;
            LoadLevelScene(level);
            break;
        case State.PauseMenu:
            // crash shortcut for debug purposes, should never happen
            Assert.AreEqual(level, m_currentLevel);
            // level must be loaded
            Assert.IsTrue(m_levelLoaded);
            m_currentLevel = level;
            // TODO: hide pause menu
            break;
        case State.Playing:
            // crash shortcut for debug purposes, should never happen
            Assert.AreNotEqual(level, m_currentLevel);
            LoadLevelScene(m_currentLevel);
            break;
        }
    }

    public void RotateLevel()
    {
        //PersistController.Instance.Persist(m_currentLevel);
        TransitionPlaying(m_currentLevel + 1);
    }

    private void LoadLevelScene(int level)
    {
        m_currentLevel = level;
        SceneManager.LoadScene("Level" + level + "Scene");
    }
}
