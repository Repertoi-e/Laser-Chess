using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour {
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;
    public float hovererMaxSpeed = 12f;
    public float hovererSpeedUpDuration = 1f;

    private BoxCollider hoverTriggerCollider; // encapsulates all tiles to detect when the user mouse hasn't selected any tile

    private GameObject hoverer = null;
    private GameObject hoveredTile = null;
    private float hovererLerpElapsed = 1000;
    private List<Vector3> hovererLerpTargets = new();

    void Start() {
        GameState.Instance.board = this;

        GameObject[] hovererSearch = GameObject.FindGameObjectsWithTag("Hoverer");
        if (hovererSearch.Length != 0) {
            Debug.Assert(hovererSearch.Length == 1);
            hoverer = hovererSearch[0];
        }

        GenerateBoard();
    }

    float Derivative(float t, Func<float, float> func) {
        return (func(0.001f + t) - func(t - 0.001f)) / (2 * 0.001f);
    }

    void Update() {
        // interpolate hoverer visual position
        hovererLerpElapsed += Time.deltaTime;

        if (hovererLerpTargets.Count > 0) {
            var t = hovererLerpElapsed / hovererSpeedUpDuration;

            float minSpeed = 4;

            Func<float, float> cubic = x => Mathf.Pow(x, 3);
            var speed = minSpeed + cubic(t) * (hovererMaxSpeed - minSpeed);

            Vector3 next = hovererLerpTargets[0];
            hoverer.transform.position = Vector3.MoveTowards(hoverer.transform.position, next, Time.deltaTime * speed);

            if (next == hoverer.transform.position) {
                hovererLerpTargets.RemoveAt(0);
            }
        } else {
            // hovererCalculatedPos = hovererTargetPos;
        }

        /*

        var hovererNewY = Mathf.Lerp(hoverer.transform.position.y, hovererTargetPos.y, Time.deltaTime * hovererSpeed);

        float epsilon = 0.05f;

        var diffX = hoverer.transform.position.x - hovererTargetPos.x;
        var diffZ = hoverer.transform.position.z - hovererTargetPos.z;
        if (Mathf.Abs(diffX) > epsilon) {
            float newX;
            if (diffZ > epsilon) {
                if (diffX > epsilon) {
                    newX = hoverer.transform.position.x - Time.deltaTime * hovererSpeed * 10 * Mathf.Sign(diffX);
                } else {
                    newX = hovererTargetPos.x;
                }
            } else {
                newX = Mathf.Lerp(hoverer.transform.position.x, hovererTargetPos.x, Time.deltaTime * hovererSpeed);
            }
            hoverer.transform.position = new Vector3(newX, hovererNewY, hoverer.transform.position.z);
        } else {
            hoverer.transform.position = new Vector3(hoverer.transform.position.x, hovererNewY, Mathf.Lerp(hoverer.transform.position.z, hovererTargetPos.z, Time.deltaTime * hovererSpeed));
        }
        */

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
    public void SetHover(GameObject tile) {
        var target = tile.transform.position + Vector3.up * 0.5f;
        if (hoveredTile == null) {
            // teleport if no previous hovered tile
            hoverer.transform.position = tile.transform.position + Vector3.up * 0.4f;
        }
        hoveredTile = tile;
        HovererLerpGoTo(target);
    }

    private void HovererLerpGoTo(Vector3 target) {
        hovererLerpElapsed = 0;
        if (hovererLerpTargets.Count > 0) {
            hovererLerpTargets.Clear();
        }

        // Add middle waypoint if moving diagonally X
        var diffX = Mathf.Abs(hoverer.transform.position.x - target.x);
        var diffZ = Mathf.Abs(hoverer.transform.position.z - target.z);
        if (diffX > 0.1 && diffZ > 0.1) {
            hovererLerpTargets.Add(new Vector3(Mathf.Round(hoverer.transform.position.x), target.y, target.z));
        }
        hovererLerpTargets.Add(SnapToGrid(target));

        Debug.Log(hovererLerpTargets.Count);
    }

    Vector3 SnapToGrid(Vector3 v) => new Vector3(Mathf.Round(v.x), v.y, Mathf.Round(v.z));

    private void OnHoverExit() {
        hoveredTile = null;

        Vector3 target = hovererLerpTargets.Count > 1 ? hovererLerpTargets[hovererLerpTargets.Count - 1] : hoverer.transform.position;
        target -= Vector3.up * 0.1f;
        HovererLerpGoTo(target);
    }
}
