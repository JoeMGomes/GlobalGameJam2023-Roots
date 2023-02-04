using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private static GameManager Instance
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

    private void Awake()
    {
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