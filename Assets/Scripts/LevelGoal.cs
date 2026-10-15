using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour {
    AudioSource audioSource;
    public ParticleSystem victoryParticleSystem;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D other) {
        FindAnyObjectByType<GameStateManager>().GoalReached(this);
        if (audioSource != null) audioSource.Play();
        victoryParticleSystem.Play();
    }

}
