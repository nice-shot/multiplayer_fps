using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

    public Behaviour[] componentsToDisable;

    void Start() {
        // Disable irrelevant components
        if (!isLocalPlayer) {
            foreach (Behaviour component in componentsToDisable) {
                component.enabled = false;
            }
        } else {
            Camera.main.gameObject.SetActive(false);
        }
    }

}
