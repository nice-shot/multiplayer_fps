using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate() {
        PerformMovement();
    }

    private void PerformMovement() {
        if (velocity != Vector3.zero) {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    public void Move(Vector3 velocity) {
        this.velocity = velocity;
    }

}
