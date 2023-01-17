using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPieceInteraction : PieceInteraction {
    private Piece selectedPiece;

    public AttackPieceInteraction(Piece piece) {
        selectedPiece = piece;
    }
}
