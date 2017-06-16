using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Transform target;
    public Vector3 targetPosition;
    public float speed = 1f;
    public Rigidbody2D rb;
    bool collided = false;
    public Animator animator;
    AudioSource audioSource;
    public List<AudioClip> collisionAudioClips;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    public void SetTarget(Transform target) {
        this.target = target;
        rb.velocity = (target.position - transform.position).normalized * speed;
    }

    public void SetTarget(Vector3 targetPosition) {
        this.targetPosition = targetPosition;
        rb.velocity = (targetPosition - transform.position).normalized * speed;
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (collided) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Teleport")) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player hitbox")) {
            other.GetComponentInParent<Hero>().PlayerGotHitBy(gameObject);
        } else if (other.gameObject.layer == LayerMask.NameToLayer("Minion")) {
            other.GetComponentInParent<PlayerSeekingEnemy>().MinionGotHitBy(gameObject);
        }
        GetDestroyed();
    }

    public void GetDestroyed() {
        collided = true;

        Destroy(gameObject, 1f);
        rb.velocity = Vector2.zero;
        if (animator != null) animator.SetTrigger("destroy");
        if (audioSource != null) audioSource.PlayOneShot(collisionAudioClips[Random.Range(0, collisionAudioClips.Count)]);
    }

}
