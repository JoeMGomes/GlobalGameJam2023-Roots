using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Notes
// 1. What a finite state machine is
// 2. Examples where you'd use one
//     AI, Animation, Game State
// 3. Parts of a State Machine
//     States & Transitions
// 4. States - 3 Parts
//     Tick - Why it's not Update()
//     OnEnter / OnExit (setup & cleanup)
// 5. Transitions
//     Separated from states so they can be re-used
//     Easy transitions from any state

[Serializable]
public class StateMachine
{
    private State _currentState;

    public List<State> states;

    private Dictionary<string, List<Transition>> _transitions = new Dictionary<string, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>(); // from any source state

    public State[] States => states.ToArray();
    
    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    public string ownerName;
    
    public bool debug;

    /// <summary>
    /// StateMachine constructor.
    /// </summary>
    /// <param name="ownerName">The Object that created the state machine, this is used for debug purposes</param>
    public StateMachine(string ownerName)
    {
        this.ownerName = ownerName;
        debug = false;
        states = new List<State>();
    }

    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        // _currentState?.Tick();
    }

    public void SetInitialState(State state)
    {
        if (_currentState == null)
            SetState(state);
    }

    /// <summary>
    /// Set first state with no onEnter or onExit Callbacks 
    /// </summary>
    /// <param name="state"></param>
    public void SetInitialStateNoCallbacks(State state)
    {
        if (_currentState == null)
        {
            // Set state code, with no onEnters
            _currentState = state;
            _transitions.TryGetValue(_currentState.name, out _currentTransitions);
            if (_currentTransitions == null)
                _currentTransitions = EmptyTransitions;
        }
        else
        {
            Debug.LogWarning("Can't set initial state: a state is already set!");
        }

    }

    /// <summary>
    /// Only set internally by the machine
    /// </summary>
    /// <param name="state"></param>
    private void SetState(State state)
    {
        if (state == _currentState)
            return;

        _currentState?.onExit?.Invoke();
        if (_currentState != null)
        {
            _currentState.isActive = false;
        }
        _currentState = state;
        _currentState.isActive = true;

        _transitions.TryGetValue(_currentState.name, out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        _currentState.onEnter?.Invoke();
    }

    public void MakeTransition(State to)
    {
        if (_currentState.name == to.name)
            return;

        if (_anyTransitions.Find(t => t.To == to) != null || _currentTransitions.Find(t => t.To == to) != null)
        {
            Debug.Log(ownerName + ": " + _currentState.name + " -> TRANSITION to: " + to.name);
            SetState(to);
        }
        else
        {
            Debug.LogWarning(ownerName + ": " + _currentState.name + " -> INVALID TRANSITION to: " + to.name);
        }
    }

    public void AddTransition(State from, State to, Func<bool> predicate, Action onTransition = null)
    {
        if (_transitions.TryGetValue(from.name, out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.name] = transitions;
        }

        transitions.Add(new Transition(to, predicate, onTransition));
    }
    /// <summary>
    /// Adds an always valid transistion, if a predicate is needed use the other overload
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="onTransition"></param>
    public void AddTransition(State from, State to, Action onTransition = null)
    {
        if (_transitions.TryGetValue(from.name, out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.name] = transitions;
        }

        if (!states.Contains(from))
        {
            states.Add(from);
        }

        if (!states.Contains(to))
        {
            states.Add(to);
        }

        transitions.Add(new Transition(to, () => { return true; }, onTransition));
    }

    public void AddAnyTransition(State state, Func<bool> predicate, Action onTransition = null)
    {
        if (!states.Contains(state))
        {
            states.Add(state);
        }
        
        _anyTransitions.Add(new Transition(state, predicate, onTransition));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public State To { get; }
        public Action OnTransition;

        public Transition(State to, Func<bool> condition, Action onTransition = null)
        {
            To = to;
            Condition = condition;
            OnTransition = onTransition;
        }
    }

    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }

    public State GetState()
    {
        return _currentState;
    }
}