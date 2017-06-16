using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Hero currentHero;
    public Camera cam;
    void FindHero() {
        var heroes = FindObjectsOfType<Hero>();
        foreach(var h in heroes) {
            if (!h.dead) currentHero = h;
        }
    }

    void Start() {
        FindHero();
    }

    Vector3 vel;

    void Update() {
        if(currentHero == null || currentHero.dead) {
            FindHero();
        }

        if (currentHero == null) return;

        Vector3 targetPos = currentHero.transform.position + Vector3.Scale((Vector3)currentHero.lookFacing, new Vector3(1.5f, 1f, 0f));
        targetPos.y += .5f;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, .45f);

    }


}
