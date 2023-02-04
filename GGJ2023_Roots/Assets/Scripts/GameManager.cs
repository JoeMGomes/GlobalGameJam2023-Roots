using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public float playerDamageOverTime;

    public float treeLifeTime;

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

    [SerializeField]
    private CinemachineVirtualCamera mainCam, menuCam;
    [SerializeField]
    private RawImage logo;

    [SerializeField]
    private GameObject ToolsObject;

    public TreeController tree;

    private void Awake()
    {
        stateMachine = new StateMachine("stateMachine");

        Menu = new State("Menu");
        Playing = new State("Playing");
        Pause = new State("Pause");

        stateMachine.AddTransition(Menu, Playing, () => true);
        stateMachine.AddTransition(Playing, Menu, () => true);
        stateMachine.AddTransition(Playing, Pause, () => true);
        stateMachine.AddTransition(Pause, Playing, () => true);

        Menu.onEnter = () => MenuOnEnter();
        Menu.onExit = () => MenuOnExit();
        Playing.onEnter = () => PlayOnEnter();
        Playing.onExit = () => PlayOnExit();
        Pause.onEnter = () => PauseOnEnter();
        Pause.onExit = () => PauseOnExit();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //  Change this in final build to state -> Menu
            stateMachine.SetInitialState(Menu);
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
        if (stateMachine.GetState() == Playing) {
            
            if(tree.GetIsTransitioning)
            {
                treeLifeTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (GetGamePausedStatus() == false)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                stateMachine.MakeTransition(Menu);
            }
        }
        else if(stateMachine.GetState() == Menu && Input.GetMouseButtonDown(0))
        {
            stateMachine.MakeTransition(Playing);
        }
    }

    private void PlayOnEnter()
    {
        ToolsObject.SetActive(true);
    }

    private void PlayOnExit()
    {

    }

    private void MenuOnEnter()
    {
        menuCam.enabled = true;
        mainCam.enabled = false;
        StartCoroutine(ToggleLogo(false));
        ToolsObject.SetActive(false);
    }

    private void MenuOnExit()
    {
        mainCam.enabled = true;
        menuCam.enabled = false;
        StartCoroutine(ToggleLogo(true));
    }
    private IEnumerator ToggleLogo(bool fadeOut)
    {
        // fade from opaque to transparent
        if (fadeOut)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                logo.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                logo.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
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