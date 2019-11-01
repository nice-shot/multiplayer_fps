using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Animator))]
public class PlayerShoot : NetworkBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    private Animator animator;
    private float roundtripTimer;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start() {
        // Adjust bulletSpawn to point to where the camera is pointing
        bulletSpawn.LookAt(Camera.main.transform.forward * 5000f);
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
        CmdSpawnBullet(bulletSpawn.position, bulletSpawn.rotation);
    }

    [Command]
    private void CmdSpawnBullet(Vector3 position, Quaternion rotation) {
        print("Got command from player to spawn the bullet");
        // Instantiate the bullet on all clients
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        NetworkServer.Spawn(bullet);
    }
}
