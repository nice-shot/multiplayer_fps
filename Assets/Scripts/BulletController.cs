using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : NetworkBehaviour {
    public float initialSpeed;
    public float lifespan;
    public float explosionRadius;
    public float explosionForce;

    public GameObject explosionEffect;
    public LayerMask breakableLayerMask;
    public LayerMask movableLayerMask;

    [SyncVar]
    public int localId;

    [HideInInspector]
    public Rigidbody rb;

    private float creationTime;
    private const string PLAYER_TAG = "Player";
    private const string DESTRUCTIBLE_TAG = "Destructible";

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * initialSpeed;

        creationTime = Time.time;
    }

    void Update() {
        if (Time.time - creationTime > lifespan) {
            ShowExplosion(transform.position);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        // Local player sends the details to the server
        if (hasAuthority) {
            print("Bullet hit: " + collision.gameObject.name);
            // Send explosion information to the server
            CmdAnnounceExplosion(transform.position);

            // Mark player hit
            if (collision.gameObject.CompareTag(PLAYER_TAG)) {
                print("Hit another player");
                CmdHitPlayer(collision.gameObject.name);
            }

            // Play explosion for local player
            ShowExplosion(transform.position);
        }

        // Maybe add double checking of collision on the server side
        // if (isServer) {
        // }
    }

    [Command]
    private void CmdAnnounceExplosion(Vector3 position) {
        print("Bullet exploded!");
        RpcPlaceExplosion(position);
        // Show explosion on the server
        ShowExplosion(position);
        Destroy(gameObject);
    }

    [Command]
    private void CmdHitPlayer(string playerId) {
        print("Another player was hit - marking it down");
        Player player = GameManager.instance.GetPlayer(playerId);
        print("Updating hits for player: " + player + " to" + (player.hitsTaken + 1));
        player.hitsTaken += 1;
        // The hook doesn't happen on the server so apply this manually
        player.OnChangeHits(player.hitsTaken);
    }

    [ClientRpc]
    private void RpcPlaceExplosion(Vector3 position) {
        if (!hasAuthority) {
            ShowExplosion(position);
        }
        Destroy(gameObject);
    }

    private void ShowExplosion(Vector3 position) {
        Instantiate(explosionEffect, position, Quaternion.identity);

        // Brake objects in the explosion radius
        Collider[] collidersToBrake = Physics.OverlapSphere(position, explosionRadius, breakableLayerMask);

        foreach (Collider nearbyObject in collidersToBrake) {
            Destructible destructable = nearbyObject.GetComponent<Destructible>();
            if (destructable != null) {
                destructable.Brake();
            }
        }

        // Move objects near the explosion
        Collider[] collidersToMove = Physics.OverlapSphere(position, explosionRadius, movableLayerMask);
        foreach (Collider nearbyObject in collidersToMove) {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce(explosionForce, position, explosionRadius);
            }
        }
    }

    override public void OnStartAuthority() {
        print("I got authority for player: " + PlayerSetup.localPlayer);
        PlayerSetup.localPlayer.GetComponent<PlayerShoot>().ReplaceLocalBulletWithNetwork(this);
    }

}
