using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;

    void Start()
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

    void Update()
    {
        
    }
}
