using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public float playerDamageOverTime;
    public float treeDamageOverTime;
    public float treeHealth;
    public float treeSeedGrowTime;
    public float treeYoungGrowTime;
    public float treeAdultGrowTime;

    private float treeLifetime;
    public TreeController tree;
    public GameEvents gameEvents;
    public AudioSource audioSource;
    public Event currentHoveredEvent;

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

    public bool instanced;

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

        gameEvents = gameObject.GetComponent<GameEvents>();

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
                if(tree.stateMachine.GetState() == tree.seed)
                {
                    treeLifetime += Time.deltaTime;
                    if(treeLifetime > treeSeedGrowTime)
                    {
                        treeLifetime = 0;
                        Debug.Log("Grew");
                        tree.Grow();
                    }
                }

                if(tree.stateMachine.GetState() == tree.youngNormal || tree.stateMachine.GetState() == tree.adultNormal || tree.stateMachine.GetState() == tree.oldNormal)
                {
                    if(instanced == false)
                    {
                        gameEvents.InstantiateEvent();
                    }
                    treeLifetime += Time.deltaTime;
                }

                if(treeHealth < 0)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10000, LayerMask.NameToLayer("Tools")))
                {
                    if(hit.collider.GetComponent<Event>())
                    {
                        if (hit.collider.GetComponent<Event>() != currentHoveredEvent)
                        {
                            currentHoveredEvent = hit.collider.GetComponent<Event>();
                        }
                    }
                    else
                    {
                        currentHoveredEvent = null;
                    }
                }

                if(Input.GetMouseButton(0))
                {
                    if (currentHoveredEvent != null)
                    {
                        currentHoveredEvent.TakeDamage();
                    }
                }

                if (tree.stateMachine.GetState() == tree.youngNormal)
                {
                    if (treeLifetime >= treeYoungGrowTime)
                    {
                        treeLifetime = 0;
                        gameEvents.InstantiateEvent();
                        Debug.Log("Grew");
                        tree.Grow();
                    }
                }
                else if (tree.stateMachine.GetState() == tree.adultNormal)
                {
                    if (treeLifetime >= treeAdultGrowTime)
                    {
                        treeLifetime = 0;
                        gameEvents.InstantiateEvent();
                        Debug.Log("Grew");
                        tree.Grow();
                    }
                }

                else if (tree.stateMachine.GetState() == tree.youngPlague || tree.stateMachine.GetState() == tree.adultPlague || tree.stateMachine.GetState() == tree.oldPlague)
                {
                    treeHealth -= (Time.deltaTime * treeDamageOverTime);
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
        audioSource.Play();
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
        tree.Init();
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