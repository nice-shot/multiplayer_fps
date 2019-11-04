using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Basic preparations that differentiate between local player and other players
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    public static PlayerSetup localPlayer;

    public Behaviour[] componentsToDisable;

    private Camera sceneCamera;

    void Start() {
        if (isLocalPlayer) {
            // Set up camera and localPlayer singleton
            localPlayer = this;
            sceneCamera = Camera.main;
            if (sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }
        } else {
            // Disable irrelevant components for other players in scene
            foreach (Behaviour component in componentsToDisable) {
                component.enabled = false;
            }
        }

        GameManager.instance.RegisterPlayer(netId.Value, GetComponent<Player>());
    }

    void OnDisable() {
        // Return to the default scene camera
        if (sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
            localPlayer = null;
        }

        GameManager.instance.UnregisterPlayer(transform.name);
    }

}
