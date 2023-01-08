using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour {
    public GameObject tilePrefab;

    public float kGlowDuration = 0.1f;
    public Color kGlowColor;

    private GameObject hoveredTile = null;
    private BoxCollider hoverTriggerCollider; // encapsulates all tiles to detect when the user mouse hasn't selected any tile

    void Start() {
        GameState.Instance.board = this;

        GameState.Instance.CurrentState = GameState.State.Playing;
        GameState.Instance.WhoIsOnTurn = GameState.OnTurn.Human;

        // GenerateBoard();
    }

    void Update() {
        // check for mouse exit
        // Note: this solution is a bit broken, because it relies
        // on the fact that the board trigger is always on top of everything
        // and would always get triggered by the Raycast, but for now it works
        // and I couldn't figure out how to force Unity to use a specific layer
        // that also ignores other raycasts that the tiles use to check for hover.
        if (hoveredTile != null) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, ~0, QueryTriggerInteraction.Collide)) {
                if (hit.collider.CompareTag("Board")) {
                    // still inside
                } else {
                    hoveredTile = null;
                }
            } else {
                hoveredTile = null;
            }
        }
    }
    void GenerateBoard() {
        var boardTiles = new GameObject("BoardTiles");

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                bool isWhite = (x + y) % 2 == 0;
                Vector3 pos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.transform.parent = boardTiles.transform;
            }
        }

        hoverTriggerCollider = gameObject.AddComponent<BoxCollider>();
        hoverTriggerCollider.isTrigger = true;
        hoverTriggerCollider.size = new Vector3(8, 1, 8);
        hoverTriggerCollider.center = new Vector3(3.5f, .1f, 3.5f);
    }
    public void SetHover(GameObject tile) {
        hoveredTile = tile;
    }
}
