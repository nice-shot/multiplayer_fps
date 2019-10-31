using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    public float speed;

    private PlayerMotor motor;

    void Awake() {
        motor = GetComponent<PlayerMotor>();
    }

    void Update() {
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 horizontalMovement = transform.right * xMove;
        Vector3 verticalMovement = transform.forward * zMove;

        Vector3 velocity = (horizontalMovement + verticalMovement).normalized * speed;

        motor.Move(velocity);
    }
}
