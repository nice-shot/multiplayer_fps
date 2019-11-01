using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    public static PlayerSetup localPlayer;

    public Behaviour[] componentsToDisable;

    private Camera sceneCamera;

    void Start() {
        // Disable irrelevant components
        if (isLocalPlayer) {
            localPlayer = this;
            sceneCamera = Camera.main;
            if (sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }
        } else {
            foreach (Behaviour component in componentsToDisable) {
                component.enabled = false;
            }
        }

        gameObject.name = "Player " + netId;
    }

    void OnDisable() {
        if (sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
            localPlayer = null;
        }
    }

}
