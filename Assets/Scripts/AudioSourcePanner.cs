using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourcePanner : MonoBehaviour {

    AudioSource audioSource;
    float maxVolume;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        maxVolume = audioSource.volume;
    }
    void Update() {
        Vector3 viewport = Camera.main.WorldToViewportPoint(transform.position);
        float distanceFromCenter = Mathf.InverseLerp(.75f, .25f, Vector2.Distance(viewport, new Vector2(.5f, .5f)));

        audioSource.volume = maxVolume * distanceFromCenter;
        audioSource.panStereo = viewport.x * 2f - 1f;

    }
}
