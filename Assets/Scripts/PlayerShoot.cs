using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    void Start() {
        // Adjust bulletSpawn to point to where the camera is pointing
        bulletSpawn.LookAt(Camera.main.transform.forward * 5f);
    }

    void Update() {
        if (isLocalPlayer && Input.GetButtonDown("Fire1")) {
            print("Initiated shooting from player");
            roundtripTimer = Time.time;
            CmdPlayerShoot();
        }
    }

    [Command]
    void CmdPlayerShoot() {
        print("Got command from player to shoot");
        // Validate and send shoot command to all clients to play the animation
        RpcInitiateShoot();

    }

    [ClientRpc]
    private void RpcInitiateShoot() {
        print("Got RPC from server to start shooting animation");
        print("Roundtrip time: " + (Time.time - roundtripTimer));
        // Play shoot animation
        animator.SetTrigger("Shoot");
    }

    public void Shoot() {
        if (!isLocalPlayer) {
            return;
        }
        print("Got animation trigger to shoot");
        // Should instantiate bullet here and index it
        BulletController bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        int bulletId = bullet.GetInstanceID();
        print("Created local bullet with id: " + bulletId);
        localBullets.Add(bulletId, bullet);
        CmdSpawnBullet(bulletId, bulletSpawn.position, bulletSpawn.rotation);
    }

    [Command]
    private void CmdSpawnBullet(int localId, Vector3 position, Quaternion rotation) {
        print("Got command from player to spawn the bullet");
        // Instantiate the network bullet
        BulletController bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.localId = localId;
        // NetworkServer.Spawn(bullet.gameObject);
        NetworkServer.SpawnWithClientAuthority(bullet.gameObject, gameObject);
    }

    public void ReplaceLocalBulletWithNetwork(BulletController networkBullet) {
        print("Replacing local bullet with network: " + networkBullet);
        print("Got bullet id: " + networkBullet.localId);
        BulletController localBullet;
        if (!localBullets.TryGetValue(networkBullet.localId, out localBullet)) {
            Debug.LogError("Couldn't get local bullet!");
            return;
        }

        networkBullet.transform.position = localBullet.transform.position;
        networkBullet.transform.rotation = localBullet.transform.rotation;
        networkBullet.rb.velocity = localBullet.rb.velocity;
        Destroy(localBullet.gameObject);
        localBullets.Remove(networkBullet.localId);
    }
}
