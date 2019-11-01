using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : NetworkBehaviour {
    public float initialSpeed;
    public float lifespan;

    [SyncVar]
    public int localId;
    public Rigidbody rb;

    private float creationTime;

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

    override public void OnStartAuthority() {
        print("I got authority for player: " + PlayerSetup.localPlayer);
        PlayerSetup.localPlayer.GetComponent<PlayerShoot>().ReplaceLocalBulletWithNetwork(this);
    }

}
