
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Doorkey m_Key;

    public Collider2D blocker;
    public Animator animator;

    public SpriteRenderer mainSprite;
    public Light pointLight;

    MaterialPropertyBlock rendererPropertyBlock;
    public Color color;


    internal void KeyTriggered(Doorkey doorkey) {
        blocker.enabled = false;
        animator.SetBool("open", true);
    }


    void OnValidate() {
        rendererPropertyBlock = null;
        SetColor(color);
    }

    void Start() {
        SetColor(color);
    }


    public void SetColor(Color color) {
        this.color = color;
        if (mainSprite == null) return;
        if (rendererPropertyBlock == null) {
            rendererPropertyBlock = new MaterialPropertyBlock();
            mainSprite.GetPropertyBlock(rendererPropertyBlock);

        }

        rendererPropertyBlock.SetColor("_EmissionColor", color);
        mainSprite.SetPropertyBlock(rendererPropertyBlock);
        if (pointLight != null) pointLight.color = color;
    }
}
