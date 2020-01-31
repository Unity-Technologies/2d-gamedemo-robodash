using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BulletTurret : MonoBehaviour {

    public GameObject bulletPrefab;
    public float fireInterval = 1f;

    public enum FireTarget { Player }
    public FireTarget fireTarget;
    public Transform shootAnchor;

    Transform target;
    float fireT = 0f;

    public enum Facing { Left, Up, Right, Down}
    Facing facing = Facing.Left;
    public Animator animator;

    public float m_MoveSpeed = 1f;
    public AnimationCurve m_MoveCurve;
    public Facing m_StartDirection;
    private Grid m_Grid;
    private Vector3Int m_Position;
    private Vector3Int m_LastPosition;
    private float m_MoveCurvePosition;
    private Tilemap m_Tilemap;

    AudioSource audioSource;
    public List<AudioClip> fireAudioClips;

    void Start()
    {
        m_Tilemap = GetComponentInParent<Tilemap>();
        m_Grid = GetComponentInParent<Grid>();
        m_Position = m_Grid.WorldToCell(transform.position);
        m_LastPosition = m_Position;
        audioSource = GetComponent<AudioSource>();
        switch (m_StartDirection)
        {
            case Facing.Down: m_LastPosition = m_Position + Vector3Int.up; break;
            case Facing.Left: m_LastPosition = m_Position + Vector3Int.right; break;
            case Facing.Right: m_LastPosition = m_Position + Vector3Int.left; break;
            case Facing.Up: m_LastPosition = m_Position + Vector3Int.down; break;
        }
    }

    void Update()
    {
        ShootingLogic();
        MovingLogic();
    }

    private void MovingLogic()
    {
        if (m_MoveCurvePosition > 0.99f)
            JumpToNext();

        Vector3 from = m_Grid.GetCellCenterWorld(m_LastPosition);
        Vector3 to = m_Grid.GetCellCenterWorld(m_Position);
        transform.position = Vector3.Lerp(from, to, m_MoveCurve.Evaluate(m_MoveCurvePosition));
        m_MoveCurvePosition = Mathf.Clamp01(m_MoveCurvePosition + Time.deltaTime * m_MoveSpeed);
    }

    private void ShootingLogic()
    {
        if (target == null)
        {
            switch (fireTarget)
            {
                case FireTarget.Player:
                    var hero = FindObjectOfType<Hero>();
                    if (hero != null) target = hero.transform;
                    break;
            }
        }

        if (target == null) return;

        fireT += Time.deltaTime;
        if (fireT >= fireInterval)
        {
            fireT -= fireInterval;

            GameObject newBullet = Instantiate(bulletPrefab, new Vector3(shootAnchor.position.x, shootAnchor.position.y, 0), Quaternion.identity) as GameObject;
            newBullet.GetComponent<Bullet>().SetTarget(target);
            if(audioSource != null) {
                audioSource.PlayOneShot(fireAudioClips[Random.Range(0, fireAudioClips.Count)]);
            }
        }

        Vector2 delta = (target.position - transform.position).normalized;
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

    void JumpToNext()
    {
        List<Vector3Int> possibleMoves = new List<Vector3Int>();

        if (m_Position + Vector3Int.right != m_LastPosition && HasRail(m_Position + Vector3Int.right))
            possibleMoves.Add(Vector3Int.right);
        if (m_Position + Vector3Int.down != m_LastPosition && HasRail(m_Position + Vector3Int.down))
            possibleMoves.Add(Vector3Int.down);
        if (m_Position + Vector3Int.left != m_LastPosition && HasRail(m_Position + Vector3Int.left))
            possibleMoves.Add(Vector3Int.left); 
        if (m_Position + Vector3Int.up != m_LastPosition && HasRail(m_Position + Vector3Int.up))
            possibleMoves.Add(Vector3Int.up);

        if (possibleMoves.Count == 0 && HasRail(m_LastPosition))
            possibleMoves.Add(m_LastPosition - m_Position);

        if (possibleMoves.Count > 0)
        {
            m_LastPosition = m_Position;
            m_MoveCurvePosition = 0f;
            m_Position += possibleMoves[Random.Range(0, possibleMoves.Count)];
        }
    }

    bool HasRail(Vector3Int position)
    {
        return m_Tilemap.GetTile(position) != null;
    }


}
