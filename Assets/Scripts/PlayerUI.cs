using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    public GameObject pauseMenu;

    public void SetPauseMenu(bool isPaused) {
        pauseMenu.SetActive(isPaused);

        // Hide cursor when playing and show it when pausing
        // if (isPaused) {
        //     Cursor.lockState = CursorLockMode.Locked;
        // } else {
        //     Cursor.lockState = CursorLockMode.None;
        // }
    }

}
