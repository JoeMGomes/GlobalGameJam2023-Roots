using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class PlagueEvent : Event
{
    public override void OnStartEvent()
    {
        ParticleSystem.EmissionModule _emission = tree.plagueParticleSystem.emission;

        tree.TransitionToPlague();
        tree.StartPlagueParticles();
        _emission.rateOverTime = 10;
        gameObject.transform.position = GameManager.GetGameManagerInstance.tree.transform.position;
        base.OnStartEvent();
    }

    public override void OnEndEvent()
    {
        ParticleSystem.EmissionModule _emission = tree.plagueParticleSystem.emission;

        tree.TransitionToNormal();
        _emission.rateOverTime = 0;
        tree.StopPlagueParticleSystem();
        base.OnEndEvent();
    }

    public override void TakeDamage()
    {
        base.TakeDamage();
    }
}
