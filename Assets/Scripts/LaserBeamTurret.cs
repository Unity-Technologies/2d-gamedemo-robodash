using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LaserBeamTurret : MonoBehaviour {

    Animator animator;
    public LaserBeamTurret nextTurret;
    public bool firstTurret = false;

    public LineRenderer laserBeamLine;
    public List<ParticleSystem> beamParticleSystems;

    public float fireInterval = .5f;

    Vector3 myLastPos;
    Vector3 nextLastPos;
    AudioSource audioSource;
    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (firstTurret && Application.isPlaying) StartCoroutine(Fire());
    }

    void OnValidate()
    {
        AimLaserBeam();
    }

    void AimLaserBeam()
    {
        if (nextTurret == null)
        {

        }
        else
        {
            laserBeamLine.SetPositions(new Vector3[] { Vector3.zero, nextTurret.transform.position - transform.position });

            Vector3 deltaDir = (nextTurret.transform.position - transform.position).normalized;

            foreach (ParticleSystem ps in beamParticleSystems)
            {
                ps.transform.position = transform.position + (nextTurret.transform.position - transform.position) / 2f + Vector3.up * .5f;
                ps.transform.LookAt(ps.transform.position + Vector3.forward, new Vector2(deltaDir.y, -deltaDir.x));
                var shape = ps.shape;
                float d = Vector3.Distance(transform.position, nextTurret.transform.position);
                shape.radius = d / 2f;
                var emission = ps.emission;
                emission.rateOverTime = d * 10f;
            }

        }
    }

    public IEnumerator Fire()
    {
        animator.SetTrigger("fire");
        if (nextTurret != null) nextTurret.animator.SetTrigger("receive_fire");
        yield return new WaitForSeconds(.5f);
        if (audioSource != null) audioSource.Play();
        float t = Time.time;
        bool hitPlayer = false;
        while (Time.time - t < .5f && !hitPlayer)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(transform.position + Vector3.up * .5f, (nextTurret.transform.position - transform.position).normalized, Vector3.Distance(transform.position, nextTurret.transform.position), 1 << LayerMask.NameToLayer("Player hitbox"));
            if (hit.rigidbody != null)
            {
                hitPlayer = true;
                hit.rigidbody.GetComponentInParent<Hero>().PlayerGotHitBy(gameObject);
            }

            hit = Physics2D.Raycast(transform.position + Vector3.up * .5f, (nextTurret.transform.position - transform.position).normalized, Vector3.Distance(transform.position, nextTurret.transform.position), 1 << LayerMask.NameToLayer("Minion"));
            if (hit.rigidbody != null) {
                hit.rigidbody.GetComponentInParent<PlayerSeekingEnemy>().MinionGotHitBy(gameObject);
            }
         
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(fireInterval);
        if (nextTurret != null) nextTurret.StartCoroutine(nextTurret.Fire());
    }

    void Update()
    {
        if (nextTurret == null) return;
        if (transform.position != myLastPos || nextLastPos != nextTurret.transform.position)
        {
            AimLaserBeam();
        }
        myLastPos = transform.position;
        nextLastPos = nextTurret.transform.position;
    }

    public LaserBeamTurret GetPrevious()
    {
        LaserBeamTurret[] all = FindObjectsOfType<LaserBeamTurret>();
        foreach (var other in all)
        {
            if (other.nextTurret == this)
                return other;
        }
        return null;
    }

    public LaserBeamTurret GetFirst()
    {
        if (firstTurret)
            return this;

        LaserBeamTurret next = this.nextTurret;
        while (next != null && next != this)
        {
            if (next.firstTurret)
                return next;

            next = next.nextTurret;
        }
        
        LaserBeamTurret previous = GetPrevious();
        while (previous != null && previous != this)
        {
            if(previous.firstTurret)
                return previous;

            previous = previous.GetPrevious();
        }

        return null;
    }
}
