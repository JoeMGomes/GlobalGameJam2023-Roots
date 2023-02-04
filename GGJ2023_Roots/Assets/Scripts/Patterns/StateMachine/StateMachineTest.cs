using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StateMachineTest : MonoBehaviour
{
    public StateMachine stateMachine;
    private State state1, state2, state3, state4;

    public void Awake()
    {
        stateMachine = new StateMachine("Test");
        state1 = new State("State 1");
        state2 = new State("State 2");
        state3 = new State("State 3");
        state4 = new State("State 4");

        state1.onEnter += () =>
        {
            Debug.Log("Entering " + state1.name);
        };

        state1.onExit += () =>
        {
            Debug.Log("Exiting " + state1.name);
        };
        
        state2.onEnter += () =>
        {
            Debug.Log("Entering " + state2.name);
        };

        state2.onExit += () =>
        {
            Debug.Log("Exiting " + state2.name);
        };
        
        state3.onEnter += () =>
        {
            Debug.Log("Entering " + state3.name);
        };

        state3.onExit += () =>
        {
            Debug.Log("Exiting " + state3.name);
        };
        
        state4.onEnter += () =>
        {
            Debug.Log("Entering " + state4.name);
        };

        state4.onExit += () =>
        {
            Debug.Log("Exiting " + state4.name);
        };
        
        stateMachine.AddTransition(state1, state2);
        stateMachine.AddTransition(state2, state3);
        stateMachine.AddTransition(state3, state4);
        stateMachine.AddTransition(state4, state1);
        
        stateMachine.SetInitialState(state1);
    }

    private void Start()
    {
        StartCoroutine(TransitionStatesTest());
    }


    private IEnumerator TransitionStatesTest()
    {
        yield return new WaitForSeconds(2f);
        stateMachine.MakeTransition(state2);
        yield return new WaitForSeconds(2f);
        stateMachine.MakeTransition(state3);
        yield return new WaitForSeconds(2f);
        stateMachine.MakeTransition(state4);
        yield return new WaitForSeconds(2f);
        stateMachine.MakeTransition(state1);
    }
}
