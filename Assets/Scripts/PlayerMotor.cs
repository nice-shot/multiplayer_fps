using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the player using Physics
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    public Camera cam;
    private Rigidbody rb;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    private const float CAMERA_ROTATION_LIMIT = 85f;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        PerformMovement();
        PerformRotation();
    }

    private void PerformMovement() {
        if (velocity != Vector3.zero) {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    private void PerformRotation() {
        // Rotate body
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        // Rotate camera
        if (cam != null) {
            currentCameraRotationX -= cameraRotationX;
            // Clamp rotation to avoid turning the camera all the way behind the player
            currentCameraRotationX = Mathf.Clamp(
                currentCameraRotationX,
                -CAMERA_ROTATION_LIMIT,
                CAMERA_ROTATION_LIMIT
            );

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    // Public methods to set the expected movement change for the next physics frame
    public void Move(Vector3 velocity) {
        this.velocity = velocity;
    }

    public void Rotate(Vector3 rotation) {
        this.rotation = rotation;
    }

    public void RotateCamera(float rotationX) {
        cameraRotationX = rotationX;
    }

}
