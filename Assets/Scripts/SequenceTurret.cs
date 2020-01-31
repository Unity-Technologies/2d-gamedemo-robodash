using System.Collections.Generic;
using UnityEngine;

public class SequenceTurret : MonoBehaviour
{
    public List<Vector3Int> m_Targets;
    public List<int> m_Ticks;
    public float m_TickDelay;

    public GameObject bulletPrefab;

    public Transform shootAnchor;
    float fireT = 0f;
    int currentTickIndex = 0;

    enum Facing { Left, Up, Right, Down }
    Facing facing = Facing.Left;
    public Animator animator;
    Grid myGrid;

    AudioSource audioSource;
    public List<AudioClip> fireAudioClips;

    void Start() 
    {
        myGrid = GetComponentInParent<Grid>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update() 
    {
        if (m_Targets.Count == 0) return;

        fireT += Time.deltaTime;
        if (fireT >= m_Ticks[currentTickIndex] * m_TickDelay) 
        {
            GameObject newBullet = Instantiate(bulletPrefab, shootAnchor.position, Quaternion.identity) as GameObject;
            Vector3Int worldTarget = myGrid.WorldToCell(transform.position) + m_Targets[currentTickIndex];
            newBullet.GetComponent<Bullet>().SetTarget(myGrid.GetCellCenterWorld(worldTarget));
            currentTickIndex++;
            if (audioSource != null) audioSource.PlayOneShot(fireAudioClips[Random.Range(0, fireAudioClips.Count)]);
            if (currentTickIndex >= m_Targets.Count) 
            {
                currentTickIndex = 0;
                fireT = 0f;
            }
        }

        Vector2 delta = ((Vector2)(myGrid.GetCellCenterWorld(m_Targets[currentTickIndex]) - transform.position)).normalized;
        Facing newFacing = Facing.Left;

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) 
        {
            if (delta.x < 0) newFacing = Facing.Left;
            else newFacing = Facing.Right;
        } 
        else 
        {
            if (delta.y < 0) newFacing = Facing.Down;
            else newFacing = Facing.Up;
        }

        if (newFacing != facing) 
        {
            facing = newFacing;
            animator.SetTrigger("turn_" + facing.ToString().ToLower());
        }

    }

}
