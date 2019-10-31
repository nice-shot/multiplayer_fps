using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : MonoBehaviour {
    public float initialSpeed;
    public float lifespan;

    private float creationTime;
    private Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * initialSpeed;

        creationTime = Time.time;
    }

    void Update() {
        if (Time.time - creationTime > lifespan) {
            Explode();
        }
    }

    void OnCollisionEnter(Collision collision) {
        print("Bullet hit: " + collision.other.name);
        Explode();
    }

    private void Explode() {
        print("Bullet exploded!");
        Destroy(gameObject);
    }


}
