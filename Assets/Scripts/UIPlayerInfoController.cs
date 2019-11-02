using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoController : MonoBehaviour {
    public Text infoText;

    public void SetInfo(string playerName, int hitsTaken) {
        infoText.text = playerName + " was hit " + hitsTaken + " times";
    }
}
