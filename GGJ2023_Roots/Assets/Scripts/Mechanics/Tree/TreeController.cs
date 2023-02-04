using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

public class TreeController : MonoBehaviour
{
    public Animator animator;

    //Animation Hashes

    //Tree State Animations
    private readonly int seedAnimHash = Animator.StringToHash("Seed");
    private readonly int youngNormalAnimHash = Animator.StringToHash("YoungNormal");
    private readonly int adultNormalAnimHash = Animator.StringToHash("AdultNormal");
    private readonly int oldNormalAnimHash = Animator.StringToHash("OldNormal");

    //Tree Transitions Animations
    private readonly int seedToYoungAnimHash = Animator.StringToHash("SeedToYoung");
    private readonly int youngNormalToDryAnimHash = Animator.StringToHash("YoungNormalToDry");
    private readonly int youngDryToNormalAnimHash = Animator.StringToHash("YoungDryToNormal");
    private readonly int youngNormalToUnhealthyAnimHash = Animator.StringToHash("YoungNormalToUnhealthy");
    private readonly int youngUnhealthyToNormalAnimHash = Animator.StringToHash("YoungUnhealthyToNormal");
    private readonly int youngToAdultAnimHash = Animator.StringToHash("YoungToAdult");    
    private readonly int adultNormalToDryAnimHash = Animator.StringToHash("YoungNormalToDry");
    private readonly int adultDryToNormalAnimHash = Animator.StringToHash("AdultDryToNormal");
    private readonly int adultNormalToUnhealthyAnimHash = Animator.StringToHash("AdultNormalToUnhealthy");
    private readonly int adultUnhealthyToNormalAnimHash = Animator.StringToHash("AdultUnhealthyToNormal");
    private readonly int adultToOldAnimHash = Animator.StringToHash("AdultToOld");
    private readonly int oldNormalToDryAnimHash = Animator.StringToHash("OldNormalToDry");
    private readonly int oldDryToNormalAnimHash = Animator.StringToHash("OldDryToNormal");
    private readonly int oldNormalToAttackedAnimHash = Animator.StringToHash("OldNormalToAttacked");
    private readonly int oldAttackedToNormalAnimHash = Animator.StringToHash("OldAttackedToNormal");

    StateMachine stateMachine;

    State transition;
    State seed;
    State youngNormal, youngDry, youngAttacked;
    State adultNormal, adultDry, adultAttacked;
    State oldNormal, oldDry, oldAttacked;

    private Action growMethod;

    private bool isTransitioning;

    public bool GetIsTransitioning => isTransitioning;

    private void Awake()
    {
        stateMachine = new StateMachine(name);

        transition = new State(nameof(transition));
        seed = new State(nameof(seed));
        youngNormal = new State(nameof(youngNormal));
        youngDry = new State(nameof(youngDry));
        youngAttacked = new State(nameof(youngAttacked));
        adultNormal = new State(nameof(adultNormal));
        adultDry = new State(nameof(adultDry));
        adultAttacked = new State(nameof(adultAttacked));
        oldNormal = new State(nameof(oldNormal));
        oldDry = new State(nameof(oldDry));
        oldAttacked = new State(nameof(oldAttacked));

        seed.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(seedAnimHash);
            growMethod = TransitionToYoung;
        };

        youngNormal.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(youngNormalAnimHash);
            growMethod = TransitionToAdult;
        };

        adultNormal.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(adultNormalAnimHash);
            growMethod = TransitionToOld;
        };

        oldNormal.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(oldNormalAnimHash);
        };

        //Seed To Young
        stateMachine.AddTransition(seed, transition);
        stateMachine.AddTransition(transition, youngNormal);

        //Young Tree
        stateMachine.AddTransition(youngNormal, youngDry);
        stateMachine.AddTransition(youngDry, youngNormal);
        stateMachine.AddTransition(youngNormal, youngAttacked);
        stateMachine.AddTransition(youngAttacked, youngNormal);

        //Young to Adult
        stateMachine.AddTransition(youngNormal, transition);
        stateMachine.AddTransition(transition, adultNormal);

        //Adult Tree
        stateMachine.AddTransition(adultNormal, adultDry);
        stateMachine.AddTransition(adultDry, adultNormal);
        stateMachine.AddTransition(adultNormal, adultAttacked);
        stateMachine.AddTransition(adultAttacked, adultNormal);

        //Adult to Old
        stateMachine.AddTransition(adultNormal, transition);
        stateMachine.AddTransition(transition, oldNormal);

        //Old Tree
        stateMachine.AddTransition(oldNormal, oldDry);
        stateMachine.AddTransition(oldDry, oldNormal);
        stateMachine.AddTransition(oldNormal, oldAttacked);
        stateMachine.AddTransition(oldAttacked, oldNormal);

        //Initial State
        stateMachine.SetInitialState(seed);
    }


    public void SeedStateTick()
    {
        
    }

    public void YoungStateTick()
    {

    }

    public void AdultStateTick()
    {

    }

    public void OldStateTick()
    {

    }

    enum TreeStatus
    {
        Normal,
        Dry,
        Unhealthy
    }


    public void Update()
    {
        //if (stateMachine.GetState() != transition)
        //{
        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        growMethod?.Invoke();
        //        growMethod = null;
        //    }
        //}
    }


    public void Grow()
    {
        growMethod?.Invoke();
        growMethod = null;
    }


    //Transition Animation boilerplate


    [ContextMenu("Transition To Young")]
    public void TransitionToYoung()
    {
        isTransitioning = true;
        transition.onEnter = () => StartCoroutine(TransitionToYoungCoroutine());
        transition.onEnter += () => Debug.Log("Transition State In");
        transition.onExit += () => Debug.Log("Transition State Out");
        stateMachine.MakeTransition(transition);
    }

    IEnumerator TransitionToYoungCoroutine()
    {       
        animator.Play(seedToYoungAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(youngNormal);
    }

    [ContextMenu("Transition To Adult")]
    public void TransitionToAdult()
    {
        isTransitioning = true;
        transition.onEnter = () => StartCoroutine(TransitionToAdultCoroutine());
        transition.onEnter += () => Debug.Log("Transition State In");
        transition.onExit += () => Debug.Log("Transition State Out");
        stateMachine.MakeTransition(transition);
    }

    IEnumerator TransitionToAdultCoroutine()
    {
        animator.Play(youngToAdultAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(adultNormal);
    }

    [ContextMenu("Transition To Old")]
    public void TransitionToOld()
    {
        isTransitioning = true;
        transition.onEnter = () => StartCoroutine(TransitionToOldCoroutine());
        transition.onEnter += () => Debug.Log("Transition State In");
        transition.onExit += () => Debug.Log("Transition State Out");
        stateMachine.MakeTransition(transition);
    }

    IEnumerator TransitionToOldCoroutine()
    {
        animator.Play(adultToOldAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(oldNormal);
    }
}
