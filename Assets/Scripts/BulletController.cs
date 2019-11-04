using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class BulletController : NetworkBehaviour {
    public float speed;
    public float lifespan;
    public float explosionRadius;
    public float explosionForce;

    public GameObject explosionEffect;
    public LayerMask breakableLayerMask;
    public LayerMask movableLayerMask;

    // Used to sync between a localy created bullet and a network bullet
    [SyncVar]
    [HideInInspector]
    public int localId;

    [HideInInspector]
    public Rigidbody rb;

    private float creationTime;
    private const string PLAYER_TAG = "Player";
    private const string DESTRUCTIBLE_TAG = "Destructible";

    // For roundtrip calculations
    private float collisionStartTime;

    void Awake() {
        // Immediately gives the bullet speed on creation
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        creationTime = Time.time;
    }

    void Update() {
        if (Time.time - creationTime > lifespan) {
            // Explode the bullet when it's lifetime ends
            CreateExplosion(transform.position);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        // Player who shot the bullet is responsible for physics calculation
        if (hasAuthority) {
            collisionStartTime = Time.time;
            // Send explosion information to the server
            CmdAnnounceExplosion(transform.position);

            // Mark player hit
            if (collision.gameObject.CompareTag(PLAYER_TAG)) {
                CmdHitPlayer(collision.gameObject.name);
            }

            // Play explosion for local player
            CreateExplosion(transform.position);
        }
    }

    [Command]
    private void CmdAnnounceExplosion(Vector3 position) {
        // Tell clients to play explosion
        RpcPlaceExplosion(position);
        // Play the explosion on the server as well - don't play if server is also a client
        if (!isClient) {
            CreateExplosion(position);
            Destroy(gameObject);
        }
    }

    // Update the hits score - currently no logic for death
    [Command]
    private void CmdHitPlayer(string playerId) {
        Player player = GameManager.instance.GetPlayer(playerId);
        // SyncVar so it will update on all the clients
        player.hitsTaken += 1;
        // The hook doesn't happen on the server so apply this manually
        player.OnChangeHits(player.hitsTaken);
    }

    [ClientRpc]
    private void RpcPlaceExplosion(Vector3 position) {
        if (hasAuthority) {
            // Calculate roundtrip since the explosion command was sent
            GameManager.instance.roundtripCalculator.CalculateRoundtrip(collisionStartTime, Time.time);
        } else {
            // Play the explosion for all clients except the shooting player
            CreateExplosion(position);
        }
        Destroy(gameObject);
    }

    private void CreateExplosion(Vector3 position) {
        // Show explosion effect - TODO: Add sound
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
        PlayerSetup.localPlayer.GetComponent<PlayerShoot>().ReplaceLocalBulletWithNetwork(this);
    }

}
