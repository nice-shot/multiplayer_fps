using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : NetworkBehaviour {
    public float initialSpeed;
    public float lifespan;

    public GameObject explosionEffect;

    [SyncVar]
    public int localId;
    [HideInInspector]
    public Rigidbody rb;

    private float creationTime;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * initialSpeed;

        creationTime = Time.time;
    }

    void Update() {
        if (Time.time - creationTime > lifespan) {
            ShowExplosion(transform.position, transform.rotation);
            Explode();
        }
    }

    void OnCollisionEnter(Collision collision) {
        // Local player sends the explosion announcement to everyone
        if (hasAuthority) {
            print("Bullet hit: " + collision.other.name);
            CmdAnnounceExplosion(transform.position, transform.rotation);
            ShowExplosion(transform.position, transform.rotation);
        }
        // Maybe add double checking of collision on the server side
        // if (isServer) {
        // }
    }

    [Command]
    private void CmdAnnounceExplosion(Vector3 position, Quaternion rotation) {
        print("Bullet exploded!");
        RpcPlaceExplosion(position, rotation);
        ShowExplosion(position, rotation);
        Destroy(gameObject);
    }

    [ClientRpc]
    private void RpcPlaceExplosion(Vector3 position, Quaternion rotation) {
        if (!hasAuthority) {
            ShowExplosion(position, rotation);
        }
        Destroy(gameObject);
    }

    private void ShowExplosion(Vector3 position, Quaternion rotation) {
        Instantiate(explosionEffect, position, rotation);
    }

    private void Explode() {
        Destroy(gameObject);
    }

    override public void OnStartAuthority() {
        print("I got authority for player: " + PlayerSetup.localPlayer);
        PlayerSetup.localPlayer.GetComponent<PlayerShoot>().ReplaceLocalBulletWithNetwork(this);
    }

}
