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
    [SerializeField] private bool m_isLevelLoaded = false;
    [SerializeField] private int m_currentLevel;
    [SerializeField] private const int m_firstLevelIndex = 1;
    [SerializeField] private const int m_levelCount = 1;
    public GameObject m_pauseMenuPrefab;
    public GameObject m_gameHudPrefab;
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private GameObject m_gameHud;

    void Awake()
    {
        Debug.Log("StateController instantiated");

        // preload pause menu prefab
        m_pauseMenuPrefab = Resources.Load("PauseMenuPrefab") as GameObject;
        Assert.IsNotNull(m_pauseMenuPrefab);

        // preload game hud prefab
        m_gameHudPrefab = Resources.Load("GameHudPrefab") as GameObject;
        Assert.IsNotNull(m_gameHudPrefab);

        // initial state is MainMenu
        m_state = State.MainMenu;
    }

    void Update()
    {

    }

    public void TransitionMainMenu()
    {
        // go to main menu in any case
        Debug.Log("TransitionMainMenu state = " + m_state);

        switch (m_state) {
        case State.MainMenu:
            Assert.IsTrue(false);
            break;
        case State.PauseMenu:
            Assert.IsNotNull(m_pauseMenu);
            Destroy(m_pauseMenu);
            m_pauseMenu = null;
            m_state = State.MainMenu;
            SceneManager.LoadScene("MainMenuScene");
            break;
        case State.Playing:
            // destroy game hud
            Assert.IsNotNull(m_gameHud);
            Destroy(m_gameHud);
            m_gameHud = null;

            m_state = State.MainMenu;
            SceneManager.LoadScene("MainMenuScene");
            break;
        }
    }

    public void TransitionPause()
    {
        Debug.Log("TransitionPause state = " + m_state);

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
            // create pause menu
            Assert.IsNull(m_pauseMenu);
            m_pauseMenu = Instantiate(m_pauseMenuPrefab, transform);
            Assert.IsNotNull(m_pauseMenu);

            // destroy game hud
            Assert.IsNotNull(m_gameHud);
            Destroy(m_gameHud);
            m_gameHud = null;

            // pause by time scale
            Time.timeScale = 0.0f;

            m_state = State.PauseMenu;
            break;
        }
    }

    public void TransitionPlaying()
    {
        // transition to playing state, but level must be loaded already, thus no level is specified in the parameter list
        Assert.IsTrue(m_isLevelLoaded);
        TransitionPlaying(m_currentLevel);
    }

    public void TransitionPlaying(int level)
    {
        Debug.Log("TransitionPlaying (prev level = " + m_currentLevel + ", new level = " + level + ")");
        Debug.Log("TransitionPlaying state = " + m_state);

        switch (m_state) {
        case State.MainMenu:
            m_state = State.Playing;

            // create pause menu
            Assert.IsNull(m_gameHud);
            m_gameHud = Instantiate(m_gameHudPrefab, transform);
            Assert.IsNotNull(m_gameHud);

            LoadLevelScene(level);
            break;
        case State.PauseMenu:
            // crash shortcut for debug purposes, should never happen
            Assert.AreEqual(level, m_currentLevel);
            // level must be loaded
            Assert.IsTrue(m_isLevelLoaded);

            // destroy pause menu
            Assert.IsNotNull(m_pauseMenu);
            Destroy(m_pauseMenu);
            m_pauseMenu = null;

            // create game hud
            Assert.IsNull(m_gameHud);
            m_gameHud = Instantiate(m_gameHudPrefab, transform);
            Assert.IsNotNull(m_gameHud);

            // unpause timescale
            Time.timeScale = 1.0f;
            m_state = State.Playing;
            break;
        case State.Playing:
            // crash shortcut for debug purposes, should never happen
            Assert.AreNotEqual(level, m_currentLevel);
            LoadLevelScene(level);
            break;
        }
    }

    public void RotateLevel()
    {
        //PersistController.Instance.Persist(m_currentLevel);

        // increment level
        int level = (m_currentLevel + 1) % (m_levelCount + m_firstLevelIndex);
        // after full rotation, go to the first level
        // TODO: dont go to the first level. use an end screen instead
        level = Mathf.Max(level, m_firstLevelIndex);
        TransitionPlaying(level);
    }

    private void LoadLevelScene(int level)
    {
        m_isLevelLoaded = true;
        m_currentLevel = level;
        SceneManager.LoadScene("Level" + level + "Scene");
    }
}
