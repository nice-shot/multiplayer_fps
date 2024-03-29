﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(RoundtripCalculator))]
public class GameManager : MonoBehaviour {

    // UI
    public Transform playerInfoList;
    public UIPlayerInfoController playerInfoControllerPrefab;

    public static GameManager instance;

    // Reference to allow easy access to calculator
    public RoundtripCalculator roundtripCalculator;

    private const string PLAYER_ID_PREFIX = "Player ";

    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private Dictionary<string, UIPlayerInfoController> infoControllers = new Dictionary<string, UIPlayerInfoController>();

    void Awake() {
        roundtripCalculator = GetComponent<RoundtripCalculator>();
    }

    void Start() {
        // Singleton pattern
        if (instance != null) {
            Destroy(gameObject);
        }

        instance = this;
    }

    public void RegisterPlayer(uint netId, Player player) {
        // Keep track of all players using their name
        string playerId = PLAYER_ID_PREFIX + netId;
        players.Add(playerId, player);
        player.transform.name = playerId;

        // Show player information in UI
        UIPlayerInfoController infoController = Instantiate(
            playerInfoControllerPrefab,
            playerInfoList
        );

        infoController.SetInfo(playerId, player.hitsTaken);
        // Keep track of the player status UI
        infoControllers.Add(playerId, infoController);
    }

    public void UnregisterPlayer(string playerId) {
        // Stop tracking player and remove status UI
        players.Remove(playerId);
        UIPlayerInfoController infoController = infoControllers[playerId];
        infoControllers.Remove(playerId);
        if (infoController != null) {
            Destroy(infoController.gameObject);
        }
    }

    public Player GetPlayer(string playerId) {
        return players[playerId];
    }

    public void UpdatePlayerStatus(string playerId, int hits) {
        if (!players.ContainsKey(playerId)) {
            // Player not registered yet - might be bug
            return;
        }
        infoControllers[playerId].SetInfo(playerId, hits);
    }
}
