using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;

    private GameObject hoverer = null;
    private GameObject hoveredTile = null;

    void Start()
    {
        GameState.Instance.board = this;

        GameObject[] hovererSearch = GameObject.FindGameObjectsWithTag("Hoverer");
        if (hovererSearch.Length != 0)
        {
            Debug.Assert(hovererSearch.Length == 1);
            hoverer = hovererSearch[0];
        }

        GenerateBoard();
    }

    void Update()
    {
        
    }

    public void SetHover(GameObject tile)
    {
        if (hoveredTile != null)
        {
            // TODO: interpolate
            hoverer.transform.position = tile.transform.position + new Vector3(0, 1, 0);
        }
        else
        {
            hoverer.transform.position = tile.transform.position + new Vector3(0, 1, 0);
        }
        hoveredTile = tile;
    }

    public void RemoveHover(GameObject tile)
    {
        if (hoveredTile == tile)
        {
            hoverer.transform.position = new Vector3(0, 0, -10000);
        }
    }

    void GenerateBoard()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                bool isWhite = (x + y) % 2 == 0;
                GameObject tilePrefab = isWhite ? whiteTilePrefab : blackTilePrefab;

                Vector3 pos = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.transform.parent = transform;
            }
        }
    }
}
