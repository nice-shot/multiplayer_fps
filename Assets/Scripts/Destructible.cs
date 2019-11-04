using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {
    public GameObject brokenPrefab;

    public void Brake() {
        // Create the destoyed version of this object
        GameObject brokenObject = Instantiate(brokenPrefab, transform.position, transform.rotation);
        brokenObject.transform.localScale = transform.localScale;

        Destroy(gameObject);
    }
}
