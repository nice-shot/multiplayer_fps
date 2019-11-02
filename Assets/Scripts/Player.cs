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

    public void OnChangeHits(int hits) {
        print("Hits have changed to " + hits + " from " + hitsTaken);
        GameManager.instance.UpdatePlayerStatus(name, hits);
    }
}
