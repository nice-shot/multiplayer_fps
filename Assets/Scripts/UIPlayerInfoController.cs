using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoController : MonoBehaviour {
    public Text infoText;

    public void SetInfo(string playerName, int hp) {
        infoText.text = playerName + " - " + hp + "HP";
    }
}
