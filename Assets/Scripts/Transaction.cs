using System.Collections;
using Unity.VisualScripting;
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

    static Vector3 SnapVectorToCardinal(Vector3 vec) {
        int largestIndex = 0;
        for (int i = 1; i < 3; i++) {
            largestIndex = Mathf.Abs(vec[i]) > Mathf.Abs(vec[largestIndex]) ? i : largestIndex;
        }
        float newLargest = vec[largestIndex] > 0 ? 1 : -1;
        vec = Vector3.zero;
        vec[largestIndex] = newLargest;
        return vec;
    }

    public override IEnumerator Execute() {
        piece.IsMoving = true;

        var dir = target - piece.gameObject.transform.position;
        float distance = dir.magnitude;
        dir /= distance; // normalize

        float timeToTake = distance / GameState.It.Constants.kUnitSpeed;
        float movementTime = 0.8f * timeToTake; // the other 20% are for the final rotation adjustment

        var targetQuat = Quaternion.LookRotation(dir);

        Quaternion beginRotation = piece.gameObject.transform.rotation;
        Vector3 beginPosition = piece.gameObject.transform.position;

        // Movement and rotation loop
        float timePassed = 0;
        while (timePassed < movementTime) {
            float t = timePassed / movementTime;

            piece.gameObject.transform.position = Vector3.Lerp(beginPosition, target, t);
            piece.gameObject.transform.rotation = Quaternion.Lerp(beginRotation, targetQuat, t * 3);

            timePassed += Time.deltaTime;
            yield return null;
        }

        piece.gameObject.transform.position = target;

        beginRotation = piece.gameObject.transform.rotation;

        // Final rotation adjustment
        var endTargetQuat = Quaternion.LookRotation(SnapVectorToCardinal(dir));
        while (timePassed < timeToTake) {
            float t = (timePassed - movementTime) / (timeToTake - movementTime);
            piece.gameObject.transform.rotation = Quaternion.Lerp(beginRotation, endTargetQuat, t);

            timePassed += Time.deltaTime;
            yield return null;
        }

        piece.IsMoving = false;
        piece.HasMovedThisTurn = true;
    }
}