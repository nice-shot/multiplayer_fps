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
    private const string PLAYER_TAG = "Player";

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * initialSpeed;

        creationTime = Time.time;
    }

    void Update() {
        if (Time.time - creationTime > lifespan) {
            ShowExplosion(transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        // Local player sends the details to the server
        if (hasAuthority) {
            print("Bullet hit: " + collision.gameObject.name);
            // Create explosion
            CmdAnnounceExplosion(transform.position, transform.rotation);

            // Mark player hit
            if (collision.gameObject.CompareTag(PLAYER_TAG)) {
                print("Hit another player");
                CmdHitPlayer(collision.gameObject.name);
            }

            // Play explosion for local player
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

    [Command]
    private void CmdHitPlayer(string playerId) {
        print("Another player was hit - marking it down");
        Player player = GameManager.instance.GetPlayer(playerId);
        print("Updating hits for player: " + player + " to" + (player.hitsTaken + 1));
        player.hitsTaken += 1;
        // The hook doesn't happen on the server so change apply this manually
        player.OnChangeHits(player.hitsTaken);
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

    override public void OnStartAuthority() {
        print("I got authority for player: " + PlayerSetup.localPlayer);
        PlayerSetup.localPlayer.GetComponent<PlayerShoot>().ReplaceLocalBulletWithNetwork(this);
    }

}
