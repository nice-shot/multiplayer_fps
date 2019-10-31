using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    public float speed;
    public float lookSenstivity;

    private PlayerMotor motor;

    void Awake() {
        motor = GetComponent<PlayerMotor>();
    }

    void Update() {
        // Calculate movement
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 horizontalMovement = transform.right * xMove;
        Vector3 verticalMovement = transform.forward * zMove;

        Vector3 velocity = (horizontalMovement + verticalMovement).normalized * speed;

        motor.Move(velocity);

        // Calculate player turning
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSenstivity;

        motor.Rotate(rotation);

        // Calculate camera rotation
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotation = xRot * lookSenstivity;

        motor.RotateCamera(cameraRotation);

    }
}
