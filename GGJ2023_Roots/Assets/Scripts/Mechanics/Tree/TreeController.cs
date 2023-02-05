using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

public class TreeController : MonoBehaviour
{
    public Animator animator;
    public SkinnedMeshRenderer treeSkinnedMeshRenderer;
    private MeshCollider treeMeshcollider;
    private Mesh treeMesh;
    public SkinnedMeshRenderer leavesSkinnedMeshRenderer;
    private MeshCollider leavesMeshCollider;
    private Mesh leavesMesh;
    public ParticleSystem plagueParticleSystem;

    //Animation Hashes

    //Tree State Animations
    private readonly int seedAnimHash = Animator.StringToHash("Seed");
    private readonly int youngNormalAnimHash = Animator.StringToHash("YoungNormal");
    private readonly int adultNormalAnimHash = Animator.StringToHash("AdultNormal");
    private readonly int oldNormalAnimHash = Animator.StringToHash("OldNormal");

    //Tree Transitions Animations
    private readonly int seedToYoungAnimHash = Animator.StringToHash("SeedToYoung");

    //Young Dry
    private readonly int youngNormalToDryAnimHash = Animator.StringToHash("YoungNormalToDry");
    private readonly int youngDryAnimHash = Animator.StringToHash("YoungDry");
    private readonly int youngDryToNormalAnimHash = Animator.StringToHash("YoungDryToNormal");

    //Young Plague
    private readonly int youngNormalToPlagueAnimHash = Animator.StringToHash("YoungNormalToPlague");
    private readonly int youngPlagueAnimHash = Animator.StringToHash("YoungPlague");
    private readonly int youngPlagueToNormalAnimHash = Animator.StringToHash("YoungPlagueToNormal");

    //Young To Adult
    private readonly int youngToAdultAnimHash = Animator.StringToHash("YoungToAdult");

    //Adult Dry
    private readonly int adultNormalToDryAnimHash = Animator.StringToHash("AdultNormalToDry");
    private readonly int adultDryAnimHash = Animator.StringToHash("AdultDry");
    private readonly int adultDryToNormalAnimHash = Animator.StringToHash("AdultDryToNormal");

    //Adult Plague
    private readonly int adultNormalToPlagueAnimHash = Animator.StringToHash("AdultNormalToPlague");
    private readonly int adultPlagueAnimHash = Animator.StringToHash("AdultPlague");
    private readonly int adultPlagueToNormalAnimHash = Animator.StringToHash("AdultPlagueToNormal");

    //Adult To Old
    private readonly int adultToOldAnimHash = Animator.StringToHash("AdultToOld");

    //Adult Dry
    private readonly int oldNormalToDryAnimHash = Animator.StringToHash("OldNormalToDry");
    private readonly int oldDryAnimHash = Animator.StringToHash("OldDry");
    private readonly int oldDryToNormalAnimHash = Animator.StringToHash("OldDryToNormal");

    //Adult Plague
    private readonly int oldNormalToPlagueAnimHash = Animator.StringToHash("OldNormalToPlague");
    private readonly int oldPlagueAnimHash = Animator.StringToHash("OldPlague");
    private readonly int oldPlagueToNormalAnimHash = Animator.StringToHash("OldPlagueToNormal");

    public StateMachine stateMachine;

    public State transition;
    public State init;
    public State seed, youngNormal, youngDry, youngPlague;
    public State adultNormal, adultDry, adultPlague;
    public State oldNormal, oldDry, oldPlague;

    private Action growTransitionMethod;
    private Action dryTransitionMethod;
    private Action plagueTransitionMethod;
    private Action normalTransitionMethod;



    private bool isTransitioning;

    public bool GetIsTransitioning => isTransitioning;

    private void Awake()
    {
        treeMeshcollider = treeSkinnedMeshRenderer.GetComponent<MeshCollider>();
        leavesMeshCollider = leavesSkinnedMeshRenderer.GetComponent<MeshCollider>();

        stateMachine = new StateMachine(name);

        transition = new State(nameof(transition));
        init = new State(nameof(init));
        seed = new State(nameof(seed));
        youngNormal = new State(nameof(youngNormal));
        youngDry = new State(nameof(youngDry));
        youngPlague = new State(nameof(youngPlague));
        adultNormal = new State(nameof(adultNormal));
        adultDry = new State(nameof(adultDry));
        adultPlague = new State(nameof(adultPlague));
        oldNormal = new State(nameof(oldNormal));
        oldDry = new State(nameof(oldDry));
        oldPlague = new State(nameof(oldPlague));

        init.onEnter = () =>
        {
            StartCoroutine(Init());
            growTransitionMethod = null;
        };

        seed.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(seedAnimHash);
            
            growTransitionMethod = TransitionToYoung;
        };

        youngNormal.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(youngNormalAnimHash);
            UpdateMesh();
            growTransitionMethod = TransitionToAdult;
            dryTransitionMethod = () => StartCoroutine(YoungDryCoroutine());           
            plagueTransitionMethod = () => StartCoroutine(YoungPlagueCoroutine());

        };

        adultNormal.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(adultNormalAnimHash);
            UpdateMesh();
            growTransitionMethod = TransitionToOld;
            dryTransitionMethod = () => StartCoroutine(AdultDryCoroutine());
            plagueTransitionMethod = () => StartCoroutine(AdultPlagueCoroutine());
        };

        oldNormal.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(oldNormalAnimHash);
            UpdateMesh();
            dryTransitionMethod = () => StartCoroutine(OldDryCoroutine());
            plagueTransitionMethod = () => StartCoroutine(OldPlagueCoroutine());
        };

        youngDry.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(youngDryAnimHash);
            UpdateMesh();
            normalTransitionMethod = () => StartCoroutine(YoungDryToNormalCoroutine());
        };

        adultDry.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(adultDryAnimHash);
            UpdateMesh();
            normalTransitionMethod = () => StartCoroutine(AdultDryToNormalCoroutine());
        };

        oldDry.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(oldDryAnimHash);
            UpdateMesh();
            normalTransitionMethod = () => StartCoroutine(OldDryToNormalCoroutine());
        };

        youngPlague.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(youngPlagueAnimHash);
            UpdateMesh();
            normalTransitionMethod = () => StartCoroutine(YoungPlagueToNormalCoroutine());
        };

        adultPlague.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(adultPlagueAnimHash);
            UpdateMesh();
            normalTransitionMethod = () => StartCoroutine(AdultPlagueToNormalCoroutine());
        };

        oldPlague.onEnter = () =>
        {
            isTransitioning = false;
            animator.Play(oldPlagueAnimHash);
            UpdateMesh();
            normalTransitionMethod = () => StartCoroutine(OldPlagueToNormalCoroutine());
        };

        stateMachine.AddTransition(init, seed);

        //Seed To Young
        stateMachine.AddTransition(seed, transition);
        stateMachine.AddTransition(transition, youngNormal);

        //Young Tree
        stateMachine.AddTransition(youngNormal, youngDry);
        stateMachine.AddTransition(youngDry, youngNormal);
        stateMachine.AddTransition(youngNormal, youngPlague);
        stateMachine.AddTransition(youngPlague, youngNormal);

        //Young to Adult
        stateMachine.AddTransition(youngNormal, transition);
        stateMachine.AddTransition(transition, adultNormal);

        //Adult Tree
        stateMachine.AddTransition(adultNormal, adultDry);
        stateMachine.AddTransition(adultDry, adultNormal);
        stateMachine.AddTransition(adultNormal, adultPlague);
        stateMachine.AddTransition(adultPlague, adultNormal);

        //Adult to Old
        stateMachine.AddTransition(adultNormal, transition);
        stateMachine.AddTransition(transition, oldNormal);

        //Old Tree
        stateMachine.AddTransition(oldNormal, oldDry);
        stateMachine.AddTransition(oldDry, oldNormal);
        stateMachine.AddTransition(oldNormal, oldPlague);
        stateMachine.AddTransition(oldPlague, oldNormal);

        //Initial State
        stateMachine.SetInitialState(init);
    }

    private void UpdateMesh()
    {
        treeMesh = new Mesh();       
        treeSkinnedMeshRenderer.BakeMesh(treeMesh);
        treeMeshcollider.sharedMesh = treeMesh;
        leavesMesh = new Mesh();
        leavesSkinnedMeshRenderer.BakeMesh(leavesMesh);
        leavesMeshCollider.sharedMesh = leavesMesh;
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

    public void SetDefaultState()
    {
        
    }

    public void SetDryState()
    {

    }

    public void SetSicknessState()
    {

    }


    public void Update()
    {
        if (stateMachine.GetState() != transition)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                growTransitionMethod?.Invoke();
                growTransitionMethod = null;
            }
        }
    }


    public void Grow()
    {
        growTransitionMethod?.Invoke();
        growTransitionMethod = null;
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
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
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
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(oldNormal);
    }

    IEnumerator Init()
    {
        animator.Play(seedAnimHash);
        yield return new WaitForSeconds(0.1f);
        UpdateMesh();
        stateMachine.MakeTransition(seed);
    }


    public void TransitionToDry()
    {
        isTransitioning = true;
        transition.onEnter = () => dryTransitionMethod?.Invoke();
        transition.onEnter += () => Debug.Log("Transition State In");
        transition.onExit += () => Debug.Log("Transition State Out");
        stateMachine.MakeTransition(transition);
    }

    public void TransitionToPlague()
    {
        isTransitioning = true;
        transition.onEnter = () => plagueTransitionMethod?.Invoke();
        transition.onEnter += () => Debug.Log("Transition State In");
        transition.onExit += () => Debug.Log("Transition State Out");
        stateMachine.MakeTransition(transition);
    }

    public void TransitionToNormal()
    {
        isTransitioning = true;
        transition.onEnter = () => normalTransitionMethod?.Invoke();
        transition.onEnter += () => Debug.Log("Transition State In");
        transition.onExit += () => Debug.Log("Transition State Out");
        stateMachine.MakeTransition(transition);
    }

    IEnumerator YoungDryCoroutine()
    {
        animator.Play(youngNormalToDryAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(youngDry);
    }

    IEnumerator YoungDryToNormalCoroutine()
    {
        animator.Play(youngDryToNormalAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(youngNormal);
    }

    IEnumerator YoungPlagueCoroutine()
    {
        animator.Play(youngNormalToPlagueAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(youngPlague);
    }

    IEnumerator YoungPlagueToNormalCoroutine()
    {
        animator.Play(youngPlagueToNormalAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(youngNormal);
    }

    IEnumerator AdultDryCoroutine()
    {
        animator.Play(adultNormalToDryAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(adultDry);
    }

    IEnumerator AdultDryToNormalCoroutine()
    {
        animator.Play(adultDryToNormalAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(adultNormal);
    }

    IEnumerator AdultPlagueCoroutine()
    {
        animator.Play(adultNormalToPlagueAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(adultPlague);
    }

    IEnumerator AdultPlagueToNormalCoroutine()
    {
        animator.Play(adultPlagueToNormalAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(adultNormal);
    }

    IEnumerator OldDryCoroutine()
    {
        animator.Play(oldNormalToDryAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(oldDry);
    }

    IEnumerator OldDryToNormalCoroutine()
    {
        animator.Play(oldDryToNormalAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(oldNormal);
    }

    IEnumerator OldPlagueCoroutine()
    {
        animator.Play(oldNormalToPlagueAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(oldPlague);
    }

    IEnumerator OldPlagueToNormalCoroutine()
    {
        animator.Play(oldPlagueToNormalAnimHash);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        yield return new WaitForSeconds(0.1f);
        stateMachine.MakeTransition(oldNormal);
    }

    public void StartPlagueParticles()
    {
        plagueParticleSystem.Play();
    }

    public void StopPlagueParticleSystem()
    {
        plagueParticleSystem.Stop();
    }
}
