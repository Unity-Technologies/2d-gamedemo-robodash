
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Hero : MonoBehaviour
{
	public float m_MoveSpeed;


    public Animator animator;
    public ParticleSystem dashParticleSystem;

    Rigidbody2D rb;

    public enum PlayerState { Alive, Dead }
    public PlayerState playerState = PlayerState.Alive;

    public GameObject deathBits;
    public Vector2 lookFacing;
	public Vector2 respawnPoint;
    AudioSource audioSource;
    float dashCooldown = 0f;
    public bool dead = false;

	void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("alive", true);
        dashParticleSystem.transform.SetParent(null);
        audioSource = GetComponent<AudioSource>();
		GameObject.FindObjectOfType<SwarmSpawner>().Restart();
    }
	void Update () 
	{
        if(playerState == PlayerState.Dead) {
            rb.velocity = Vector2.zero;
            return;
        }

		Vector3 tryMove = Vector3.zero;
		
		if (Input.GetKey(KeyCode.LeftArrow))
			tryMove += Vector3Int.left;
		if (Input.GetKey(KeyCode.RightArrow))
			tryMove += Vector3Int.right;
		if (Input.GetKey(KeyCode.UpArrow))
			tryMove += Vector3Int.up;
		if (Input.GetKey(KeyCode.DownArrow))
			tryMove += Vector3Int.down;

        rb.velocity = Vector3.ClampMagnitude(tryMove, 1f) * m_MoveSpeed;
        animator.SetBool("moving", tryMove.magnitude > 0);
        if (Mathf.Abs(tryMove.x) > 0) {
            animator.transform.localScale = tryMove.x < 0f ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
        }
        if(tryMove.magnitude > 0f) {
            lookFacing = tryMove;
        }

        dashCooldown = Mathf.MoveTowards(dashCooldown, 0f, Time.deltaTime);

        if (Input.GetButtonDown("Jump")) {
            if(dashCooldown <= 0f && tryMove.magnitude > 0) {

                var hit = Physics2D.Raycast(transform.position + Vector3.up * .5f, tryMove.normalized, 3.5f, 1 << LayerMask.NameToLayer("Wall"));

                float distance = 3f;
                if(hit.collider != null) {
                    distance = hit.distance - .5f;
                }

                var main = dashParticleSystem.main;
                main.startLifetimeMultiplier = Mathf.InverseLerp(0f, 3f, distance) * .75f;
                dashParticleSystem.transform.position = transform.position + Vector3.up * .5f;
                dashParticleSystem.transform.up = tryMove.normalized;
                dashParticleSystem.Play();

               
                dashCooldown = 3f;

                var minionHits = Physics2D.CircleCastAll(transform.position + Vector3.up * .5f, 1f, tryMove, distance, 1 << LayerMask.NameToLayer("Minion"));

                foreach(var mh in minionHits) {
                    mh.rigidbody.GetComponent<PlayerSeekingEnemy>().GetDestroyed();
                }

                var bulletHits = Physics2D.CircleCastAll(transform.position + Vector3.up * .5f, 1f, tryMove, distance, 1 << LayerMask.NameToLayer("Bullet"));
                foreach (var mh in bulletHits) {
                    mh.rigidbody.GetComponent<Bullet>().GetDestroyed();
                }

                transform.position = rb.position + Vector2.ClampMagnitude(tryMove, 1f) * distance;

                if (audioSource != null) audioSource.Play();
            }
        }

        animator.SetBool("dash_ready", dashCooldown <= 0f);

	}

    public void PlayerGotHitBy(GameObject gameObject) {
        KillPlayer();
    }

    void KillPlayer() {
        if (playerState == PlayerState.Dead) return;
        playerState = PlayerState.Dead;
        deathBits.SetActive(true);
        foreach(Rigidbody2D rb in deathBits.GetComponentsInChildren<Rigidbody2D>()) {
            rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 100f);
            rb.AddTorque(Random.Range(-360f, 360f));
        }
        animator.SetBool("alive", false);
        deathBits.transform.SetParent(null);
        Destroy(gameObject, 1f);
        GameObject.FindObjectOfType<GameStateManager>().PlayerDied();
        dead = true;
    }

    public void LevelComplete() {
        Destroy(gameObject);
    }
}
