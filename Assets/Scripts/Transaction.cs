using System.Collections;
using UnityEngine;

// We are handling things with transactions, because a real game
// will probably have a system like that in place (or something along those lines)
// to support stuff like undo/redo, replays. Transactions are stuff that happen in 
// the game on the board: piece selection/moving, attacking, etc. (both for humans and AI).
// Besides serialization, using "transactions" helps keep the code orthogonal -
// different parts of the game can queue them up, and all are executed at the end of
// the game loop. The bad alternative would be if every part of the game could execute
// potentially arbitrary many actions, so everything becomes coupled and spaghetti-like.
public abstract class Transaction {
    public abstract bool IsValid();
    public abstract IEnumerator Execute();

}

public class MoveTransaction : Transaction {
    public Piece piece;
    public Vector3 target;

    public override bool IsValid() {
        if (piece.gameObject.transform.position == target)
            return false; // trying to move to the same position
        
        Tile tile;
        GameState.It.Board.PositionToTile.TryGetValue(target, out tile);
        if (tile == null)
            return false;

        if (tile.GetPieceAbove() != null) {
            // TODO: give feedback to the player
            return false; // trying to move to position with a piece above
        }
        return true;
    }

    public override IEnumerator Execute() {
        float timePassed = 0;
        while (timePassed < 0.5f) {
            timePassed += Time.deltaTime;
            yield return null;
        }
        piece.gameObject.transform.position = target;
        piece.HasMovedThisTurn = true;
    }
}