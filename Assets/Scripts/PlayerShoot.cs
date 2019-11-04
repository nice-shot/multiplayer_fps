using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// In charge of shooting bullets and syncing the shooting data
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerShoot : NetworkBehaviour {

    public BulletController bulletPrefab;
    public Transform bulletSpawn;

    private Animator animator;
    private float roundtripTimer;
    private Dictionary<int, BulletController> localBullets = new Dictionary<int, BulletController>();

    void Awake() {
        animator = GetComponent<Animator>();
    }

    public void StartShooting() {
        roundtripTimer = Time.time;
        CmdPlayerShoot();
    }

    [Command]
    void CmdPlayerShoot() {
        // Validate and send shoot command to all clients to play the animation
        RpcInitiateShoot();

    }

    [ClientRpc]
    private void RpcInitiateShoot() {
        GameManager.instance.roundtripCalculator.CalculateRoundtrip(roundtripTimer, Time.time);
        // Play shoot animation
        animator.SetTrigger("Shoot");
    }

    // The shoot function is called in a specific frame in the animation
    public void Shoot() {
        if (!isLocalPlayer) {
            return;
        }
        // Instantiate a local bullet and index it
        BulletController bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        int bulletId = bullet.GetInstanceID();
        localBullets.Add(bulletId, bullet);
        // Tell the server to create a bullet on the network
        roundtripTimer = Time.time; // Also calculate time here
        CmdSpawnBullet(bulletId, bulletSpawn.position, bulletSpawn.rotation);
    }

    [Command]
    private void CmdSpawnBullet(int localId, Vector3 position, Quaternion rotation) {
        // Create the network bullet
        BulletController bullet = Instantiate(bulletPrefab, position, rotation);
        // Save the local bullet id from the shooting player
        bullet.localId = localId;
        NetworkServer.SpawnWithClientAuthority(bullet.gameObject, gameObject);
    }

    public void ReplaceLocalBulletWithNetwork(BulletController networkBullet) {
        // The bullet was created and recieved local client authority
        GameManager.instance.roundtripCalculator.CalculateRoundtrip(roundtripTimer, Time.time);

        BulletController localBullet;
        if (!localBullets.TryGetValue(networkBullet.localId, out localBullet)) {
            Debug.LogError("Couldn't get local bullet!");
            return;
        }

        // Match the network bullet's parameters with the local bullet's
        networkBullet.transform.position = localBullet.transform.position;
        networkBullet.transform.rotation = localBullet.transform.rotation;
        networkBullet.rb.velocity = localBullet.rb.velocity;
        // And quietly destroy the local bullet
        Destroy(localBullet.gameObject);
        localBullets.Remove(networkBullet.localId);
    }
}
