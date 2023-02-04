using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float playerDamageOverTime;

    public static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            return instance;
        }
    }

    public enum Gamestate
    {
        Menu,
        Playing,
        Pause
    };

    public Gamestate gamestate;

    public Gamestate GetCurrentGamestate()
    {
        Gamestate state = gamestate;
        return state;
    }

    public void SetCurrentGamestate(Gamestate state)
    {
        gamestate = state;
    }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SetCurrentGamestate(Gamestate.Menu);
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            SetCurrentGamestate(Gamestate.Playing);
        }

        Debug.Log("Current Gamestate: " + GetCurrentGamestate());

        InitializeGameManager();
    }

    private void Start()
    {
        this.gamestate = Gamestate.Playing;

        DontDestroyOnLoad(this.gameObject);
    }

    private void InitializeGameManager()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
}