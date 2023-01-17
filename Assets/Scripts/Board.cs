using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Board : MonoBehaviour {
    public GameObject tilePrefab;

    private Dictionary<Vector3, Tile> positionToTile = new();

    void Start() {
        // We don't call 
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

        // Temporary:
        var playingState = new PlayingState();
        playingState.Turn = new HumanTurn() { playingState = playingState };
        GameState.CurrentState = playingState;
    }

    void Update() {
        GameState.Update();
    }

    public void AttackButtonPressed() {
        ((GameState.CurrentState as PlayingState)?.Turn as HumanTurn)?.AttackButtonPressed();
    }

    public Tile GetTileAt(Vector3 dest) {
        Tile tile;
        positionToTile.TryGetValue(dest, out tile);
        return tile;
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
