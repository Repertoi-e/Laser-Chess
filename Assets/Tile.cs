using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    void OnMouseEnter()
    {
        GameState.Instance.board.SetHover(gameObject);
    }

    void OnMouseExit()
    {
        GameState.Instance.board.RemoveHover(gameObject);
    }
}
