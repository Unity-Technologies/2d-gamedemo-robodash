using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSeekingEnemy : MonoBehaviour
{
    Transform target;
    Rigidbody2D rb;

    public float speed = 1f;

    bool collided = false;
    Animator animator;

    bool spawned = false;
    AudioSource audioSource;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update() {
        if (target == null) {
            var hero = FindObjectOfType<Hero>();
            if (hero != null) target = hero.transform;
        }

        if (target == null) return;

        if (!spawned) {
            float d = Vector3.Distance(transform.position, target.position);
            if(d < 6f) {
                animator.SetTrigger("spawn");
                spawned = true;
            }

        } else {
           
            Vector2 delta = (target.position - transform.position).normalized;
            rb.velocity = delta * speed;
            animator.transform.localScale = rb.velocity.x < 0f ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
            
        }

    }

    void OnTriggerEnter2D(Collider2D other) {
        if (collided) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player hitbox")) {
            other.GetComponentInParent<Hero>().PlayerGotHitBy(gameObject);
            collided = true;
            GetDestroyed();
        }
    }

    public void MinionGotHitBy(GameObject go) {
        GetDestroyed();
    }

    public void GetDestroyed() {
        if (animator != null)
            animator.SetBool("alive", false);
        Destroy(gameObject, 2f);
        collided = true;
        if (audioSource != null) audioSource.Stop();
    }


}
