using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour {
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public float hovererSpeed = 1;

    private BoxCollider hoverTriggerCollider; // encapsulates all tiles to detect when the user mouse hasn't selected any tile

    private GameObject hoverer = null;
    private GameObject hoveredTile = null;

    private Vector3 hovererTargetPos = new Vector3(0, 0, -1000);

    void Start() {
        GameState.Instance.board = this;

        GameObject[] hovererSearch = GameObject.FindGameObjectsWithTag("Hoverer");
        if (hovererSearch.Length != 0) {
            Debug.Assert(hovererSearch.Length == 1);
            hoverer = hovererSearch[0];
        }

        GenerateBoard();
    }

    void Update() {
        // interpolate hoverer visual position
        hoverer.transform.position = Vector3.Lerp(hoverer.transform.position, hovererTargetPos, Time.deltaTime * hovererSpeed);

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
                    OnHoverExit();
                }
            } else {
                OnHoverExit();
            }
        }
    }

    public void SetHover(GameObject tile) {
        hovererTargetPos = tile.transform.position + Vector3.up * 0.5f;
        if (hoveredTile == null) {
            // teleport if no previous hovered tile
            hoverer.transform.position = tile.transform.position + Vector3.up * 0.4f;
        }
        hoveredTile = tile;
    }

    void GenerateBoard() {
        var boardTiles = new GameObject("BoardTiles");

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                bool isWhite = (x + y) % 2 == 0;
                GameObject tilePrefab = isWhite ? whiteTilePrefab : blackTilePrefab;

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

    private void OnHoverExit() {
        hoveredTile = null;
        hovererTargetPos -= Vector3.up * 0.1f; // sink below
    }
}
