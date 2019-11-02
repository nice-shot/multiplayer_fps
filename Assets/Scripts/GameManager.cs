﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    // UI
    public Transform playerInfoList;
    public UIPlayerInfoController playerInfoControllerPrefab;

    public static GameManager instance;

    private const string PLAYER_ID_PREFIX = "Player ";

    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private Dictionary<string, UIPlayerInfoController> infoControllers = new Dictionary<string, UIPlayerInfoController>();

    void Start() {
        if (instance != null) {
            Destroy(gameObject);
        }

        instance = this;
    }

    public void RegisterPlayer(uint netId, Player player) {
        string playerId = PLAYER_ID_PREFIX + netId;
        players.Add(playerId, player);
        player.transform.name = playerId;

        // Show in UI
        UIPlayerInfoController infoController = Instantiate(
            playerInfoControllerPrefab,
            playerInfoList
        );

        infoController.SetInfo(playerId, 5);
        infoControllers.Add(playerId, infoController);
    }

    public void UnregisterPlayer(string playerId) {
        players.Remove(playerId);
        UIPlayerInfoController infoController = infoControllers[playerId];
        infoControllers.Remove(playerId);
        Destroy(infoController.gameObject);
    }

    public Player GetPlayer(string playerId) {
        return players[playerId];
    }
}