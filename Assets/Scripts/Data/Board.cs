using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public GameObject tilePrefab;

    public Dictionary<Vector3, Tile> PositionToTile = new();

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
            PositionToTile[child.position] = child.gameObject.GetComponent<Tile>();
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
    }
}
