using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    public GameObject pauseMenu;
    public GameObject crosshair;

    public void SetPauseMenu(bool isPaused) {
        pauseMenu.SetActive(isPaused);
        crosshair.SetActive(!isPaused);
    }

}
