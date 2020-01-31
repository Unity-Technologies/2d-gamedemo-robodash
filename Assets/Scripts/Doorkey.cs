using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorkey : MonoBehaviour
{
    public Door m_Door;
    public SpriteRenderer mainSprite;
    public Light pointLight;
    MaterialPropertyBlock rendererPropertyBlock;
    public Color color;
    public ParticleSystem openingParticleSystem;
    AudioSource audioSource;
    public Animator animator;
    void OnTriggerEnter2D(Collider2D other) {
        m_Door.KeyTriggered(this);
        openingParticleSystem.transform.SetParent(null);
        if (audioSource != null) audioSource.Play();
        Destroy(gameObject, 1f);
        animator.SetTrigger("collect");
        var main = openingParticleSystem.main;
        main.startLifetimeMultiplier = Vector3.Distance(transform.position, m_Door.transform.position) / main.startSpeedMultiplier;
        openingParticleSystem.transform.position = transform.position + Vector3.up * .5f;
        openingParticleSystem.transform.up = (m_Door.transform.position - transform.position).normalized;
        openingParticleSystem.Play();
     
    }

    void OnValidate() {
        rendererPropertyBlock = null;
        SetColor(color);
    }

    void Start() {
        SetColor(color);
        audioSource = GetComponent<AudioSource>();
    }


    public void SetColor(Color color) {
        this.color = color;
        if (mainSprite == null) return;
        if(rendererPropertyBlock == null) {
            rendererPropertyBlock = new MaterialPropertyBlock();
            mainSprite.GetPropertyBlock(rendererPropertyBlock);

        }

        rendererPropertyBlock.SetColor("_EmissionColor", color);
        mainSprite.SetPropertyBlock(rendererPropertyBlock);
        if (pointLight != null) pointLight.color = color;

        if (openingParticleSystem != null)
        {
            var main = openingParticleSystem.main;
            main.startColor = color;
        }
    }

}
