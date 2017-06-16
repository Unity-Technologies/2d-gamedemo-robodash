using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemToggler : MonoBehaviour {

    public bool playParticleSystem;
    ParticleSystem ps;

    void Start() {
        ps = GetComponent<ParticleSystem>();
    }
    void Update() {
        if (playParticleSystem && !ps.isPlaying) ps.Play();
        else if (!playParticleSystem && ps.isPlaying) ps.Stop();
    }
}
