using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour {
    public GameObject tilePrefab;

    private Dictionary<Vector3, Tile> positionToTile = new();

    void Start() {
        // Don't call 
        // GenerateBoard();
        // anymore, because being able to see the tiles
        // while not playing is useful. If we need to 
        // regenerate a different layout, we can just copy
        // the BoardTiles object created in the method
        // and paste it in editing again.
        var boardTiles = GameObject.Find("/BoardTiles");
        for (int i = 0; i < boardTiles.transform.childCount; i++) {
            var child = boardTiles.transform.GetChild(i);
            positionToTile[child.position] = child.gameObject.GetComponent<Tile>();
        }

        GameState.Board = this;

        var playingState = new MenuState();
        GameState.CurrentState = playingState;
    }

    public Tile GetTileAt(Vector3 dest) {
        Tile tile;
        positionToTile.TryGetValue(dest, out tile);
        return tile;
    }


    // Using Board as a proxy cause we don't have
    // a GameState object in the world.

    void Update() {
        GameState.Update();
    }

    public void AttackButtonPressed() {
        ((GameState.CurrentState as PlayingState)?.Turn as HumanTurn)?.AttackButtonPressed();
    }

    public void EndTurnButtonPressed() {
        (GameState.CurrentState as PlayingState)?.EndTurnButtonPressed();
    }

    public void LoadLevel(int num) {
        // This should be a million times better, but hey.
        // It's dirty and it works.

        // Remove old units
        foreach (var p in GameObject.FindGameObjectsWithTag("Piece")) {
            p.GetComponent<Piece>().HitPoints = 0;
        }

        if (num >= 1 && num <= 3) {
            StartCoroutine(LoadLevelAsync(num));
        } else {
            Debug.Log("Error, no level " + num);
        }
    }

    IEnumerator LoadLevelAsync(int num) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(num, LoadSceneMode.Additive);

        while (!asyncLoad.isDone) {
            yield return null;
        }

        if (SceneManager.sceneCount > 1) {
            var newScene = SceneManager.GetSceneAt(1);
            SceneManager.MergeScenes(newScene, SceneManager.GetActiveScene());
        }

        var playingState = new PlayingState();
        GameState.CurrentState = playingState;
        playingState.Turn = new HumanTurn();
    }

    public void GoToMenu() {
        GameState.CurrentState = new MenuState();
    }


    void GenerateBoard() {
        var boardTiles = new GameObject("BoardTiles");

        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Vector3 pos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.transform.parent = boardTiles.transform;
            }
        }
    }
}
