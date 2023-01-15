using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameState {
    public void OnPieceMouseEnter(Piece piece) {
        if (CurrentState != State.Playing)
            return;

        piece.SetTargetEmissionColor(piece.IsEnemy ? Constants.kGlowEnemy : Constants.kGlowHuman);
    }

    public void OnPieceMouseExit(Piece piece) {
        piece.SetTargetEmissionColor(Color.black);
    }
}
