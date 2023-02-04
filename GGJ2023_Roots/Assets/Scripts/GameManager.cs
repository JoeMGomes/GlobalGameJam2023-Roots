using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float playerDamageOverTime;

    private bool gameIsPaused;

    private static GameManager _gameManager;

    private static GameManager Instance
    {
        get
        {
            if (_gameManager == null)
            {
                _gameManager = FindAnyObjectByType<GameManager>();
            }
            return _gameManager;
        }
    }

    public StateMachine stateMachine;

    public State Menu, Playing, Pause;

    private void Awake()
    {
        stateMachine = new StateMachine("stateMachine");

        Menu = new State("Menu");
        Playing = new State("Playing");
        Pause = new State("Pause");

        stateMachine.AddTransition(Menu, Playing, () => true);
        stateMachine.AddTransition(Playing, Menu, () => true);
        stateMachine.AddTransition(Playing, Pause, () => true);

        Menu.onEnter = () => MenuOnEnter();
        Menu.onExit = () => MenuOnExit();
        Playing.onEnter = () => PlayOnEnter();
        Playing.onExit = () => PlayOnExit();
        Pause.onEnter = () => PauseOnEnter();
        Pause.onExit = () => PauseOnExit();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //  Change this in final build to state -> Menu
            stateMachine.SetInitialState(Playing);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            stateMachine.SetInitialState(Playing);
        }

        InitializeGameManager();
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(GetGamePausedStatus() == false)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    private void PlayOnEnter()
    {
        
    }

    private void PlayOnExit()
    {

    }

    private void MenuOnEnter()
    {

    }

    private void MenuOnExit()
    {

    }

    private void PauseOnEnter()
    {

    }

    private void PauseOnExit()
    {

    }

    private void InitializeGameManager()
    {
        if (_gameManager != null && _gameManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _gameManager = this;
        }
    }

    public void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1;
    }

    public bool GetGamePausedStatus()
    {
        return gameIsPaused;
    }

    public static GameManager GetGameManagerInstance()
    {
        return _gameManager;
    }
}