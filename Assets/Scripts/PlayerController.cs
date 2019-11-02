using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor), typeof(PlayerShoot))]
public class PlayerController : MonoBehaviour {

    public float speed;
    public float lookSenstivity;

    public PlayerUI playerUIPrefab;

    private PlayerMotor motor;
    private PlayerShoot shooter;
    private PlayerUI ui;

    private bool paused = false;

    void Awake() {
        motor = GetComponent<PlayerMotor>();
        shooter = GetComponent<PlayerShoot>();
    }

    void Start() {
        ui = Instantiate(playerUIPrefab);
        Unpause();
    }

    void OnDisable() {
        Destroy(ui.gameObject);
    }

    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if (paused) {
                Unpause();
            } else {
                Pause();
                return;
            }
        }

        if (paused) {
            return;
        }

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

        // Calculate shooting
        if (Input.GetButtonDown("Fire1")) {
            shooter.StartShooting();
        }
    }

    private void Pause() {
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ui.SetPauseMenu(true);
        motor.Move(Vector3.zero);
        motor.Rotate(Vector3.zero);
        motor.RotateCamera(0f);
    }

    private void Unpause() {
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ui.SetPauseMenu(false);
    }
}
