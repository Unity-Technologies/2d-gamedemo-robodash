using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStart : MonoBehaviour {

    AudioSource audioSource;
    public ParticleSystem spawnParticleSystem;
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    public void LevelStarted() {
        if(audioSource != null) audioSource.Play();
        spawnParticleSystem.Play();

    }

}
