using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PieceInteraction {
    public virtual void End() {
    }

    public virtual void OnBoardMouseEnter() {
    }

    public virtual void OnBoardMouseExit() {
    }

    public virtual void OnTileMouseEnter(Tile tile) {
    }

    public virtual void OnTileMouseExit(Tile tile) {
    }

    public virtual void OnTileClicked(Tile tile) {
    }

    public virtual void OnPieceMouseEnter(Piece tile) {
    }

    public virtual void OnPieceMouseExit(Piece tile) {
    }

    public virtual void OnPieceClicked(Piece tile) {
    }
}
