using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Teleport m_Target;

    Rigidbody2D justArrived = null;
    AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    void OnTriggerEnter2D(Collider2D other) {
        var rb = other.attachedRigidbody;
        if (rb == justArrived) {
            return;
        }
        TeleportObject(other.attachedRigidbody);
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        justArrived = null;
    }

    void TeleportObject(Rigidbody2D obj) {
        m_Target.ReceiveObject(obj);
        audioSource.PlayOneShot(audioSource.clip);
    }

    void ReceiveObject(Rigidbody2D obj) {
        Vector2 vel = obj.velocity;
        obj.MovePosition(transform.position);
        justArrived = obj;
    }

}
