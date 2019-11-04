using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundtripCalculator : MonoBehaviour {
    public Text roundtripText;
    private const string ROUNDTRIP_PREFIX = "AVG ROUNDTRIP: ";

    private int numOfCalculations = 0;
    private float sum = 0;

    void Awake() {
        roundtripText.text = ROUNDTRIP_PREFIX + "???";
    }

    public void CalculateRoundtrip(float sendTime, float returnTime) {
        // Calculate the average of the roundtrip and show it in the UI
        numOfCalculations++;
        sum += (returnTime - sendTime);
        float avg = sum / numOfCalculations;

        roundtripText.text = ROUNDTRIP_PREFIX + (avg * 1000).ToString("N2") + "ms";
    }
}
