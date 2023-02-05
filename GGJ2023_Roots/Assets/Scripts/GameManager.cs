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
    public float treeYoungGrowTime;
    public float treeAdultGrowTime;
    public float treeOldGrowTime;

    private float treeLifetime;

    public float treeHealth;
    public TreeController tree;

    private static GameManager _gameManager;
    public static GameManager GetGameManagerInstance
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
        if(stateMachine.GetState() == Menu)
        {
            if (Input.GetMouseButtonDown(0))
            {
                stateMachine.MakeTransition(Playing);
            }
        }

        else if(stateMachine.GetState() == Playing)
        {
            if (tree.GetIsTransitioning == false)
            {
                treeYoungGrowTime += Time.deltaTime;

                if (tree.stateMachine.GetState() == tree.youngNormal)
                {
                    if (treeLifetime >= treeYoungGrowTime)
                    {
                        treeLifetime = 0;
                        tree.Grow();
                    }
                }
                else if (tree.stateMachine.GetState() == tree.adultNormal)
                {
                    if (treeLifetime >= treeAdultGrowTime)
                    {
                        treeLifetime = 0;
                        tree.Grow();
                    }
                }
                else if (tree.stateMachine.GetState() == tree.oldNormal)
                {
                    if (treeLifetime >= treeOldGrowTime)
                    {
                        treeLifetime = 0;
                        tree.Grow();
                    }
                }
            }
                
            if (Input.GetKeyDown(KeyCode.P))
            {
                stateMachine.MakeTransition(Pause);
                PauseGame();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                stateMachine.MakeTransition(Menu);
            }
        }

        else if(stateMachine.GetState() == Pause)
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                stateMachine.MakeTransition(Playing);
                ResumeGame();
            }
        }
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
            logo.enabled = false;
        }
        // fade from transparent to opaque
        else
        {
            logo.enabled = true;
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                logo.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }

    private void PlayOnEnter()
    {
        ToolsObject.SetActive(true);
    }

    private void PlayOnExit()
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
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}