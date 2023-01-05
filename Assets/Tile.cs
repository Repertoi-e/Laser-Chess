using UnityEngine;

public class Tile : MonoBehaviour {

    void OnMouseEnter() {
        GameState.Instance.board.SetHover(gameObject);
    }
}
