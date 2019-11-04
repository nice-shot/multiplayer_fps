using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
    [SyncVar(hook="OnChangeHits")]
    public int hitsTaken;

    void Awake() {
        hitsTaken = 0;
    }

    // Update the player status on the GUI
    public void OnChangeHits(int hits) {
        GameManager.instance.UpdatePlayerStatus(name, hits);
    }
}
