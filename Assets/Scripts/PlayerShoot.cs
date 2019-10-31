using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerShoot : MonoBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            InitiateShoot();
        }
    }

    private void InitiateShoot() {
        // Start playing shoot animation
        animator.SetTrigger("Shoot");
    }

    public void Shoot() {
        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    }
}
