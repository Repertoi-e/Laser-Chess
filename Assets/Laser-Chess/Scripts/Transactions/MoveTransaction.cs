using System;
using System.Collections;
using UnityEngine;

public class MoveTransaction : Transaction {
    public Piece piece;
    public Vector3 target;

    public override bool IsValid() {
        if (piece == null)
            return false;

        if (piece.gameObject.transform.position == target)
            return false; // trying to move to the same position

        Tile tile = GameState.Board.GetTileAt(target);
        if (tile == null)
            return false;

        if (tile.GetPieceAbove() != null) {
            GameState.FeedbackText.DoTryingToMoveToAnOccupiedTile();
            return false; // trying to move to position with a piece above
        }
        return true;
    }

    // Gives rotation that drifty feeling, although it
    // may be only suitable to tanks and not planes for example.
    // TODO: Support different procedural animations for different pieces.
    float easeOutElastic(float x) {
        const float c4 = (2 * Mathf.PI) / 3;

        return x == 0
          ? 0
          : x == 1
          ? 1
          : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
    }

    // Transactions are like coroutines and execute on multiple frames.
    // Btw I love how cleanly this can be done in C#.
    public override IEnumerator Execute() {
        var dir = target - piece.gameObject.transform.position;
        float distance = dir.magnitude;
        dir /= distance; // normalize

        // Execute move animation, lerping position and rotation to the target along the way.

        float timeToTake = distance / GameState.Constants.kUnitSpeed;
        float movementTime = 0.8f * timeToTake; // the other part is for the final rotation adjustment

        var targetQuat = Quaternion.LookRotation(dir);

        Quaternion beginRotation = piece.gameObject.transform.rotation;
        Vector3 beginPosition = piece.gameObject.transform.position;

        // Movement and rotation loop
        float timePassed = 0;
        while (timePassed < movementTime) {
            float t = timePassed / movementTime;

            float posEase = t;
            float rotEase = easeOutElastic(t / 2);

            piece.gameObject.transform.position = Vector3.Lerp(beginPosition, target, posEase);
            piece.gameObject.transform.rotation = Quaternion.Lerp(beginRotation, targetQuat, rotEase);

            timePassed += Time.deltaTime;
            yield return null;
        }

        piece.gameObject.transform.position = target;

        beginRotation = piece.gameObject.transform.rotation;

        // Final rotation adjustment
        var endTargetQuat = Quaternion.LookRotation(MathUtils.SnapVectorToCardinal(dir));
        while (timePassed < timeToTake) {
            float t = (timePassed - movementTime) / (timeToTake - movementTime);
            piece.gameObject.transform.rotation = Quaternion.Lerp(beginRotation, endTargetQuat, t);

            timePassed += Time.deltaTime;
            yield return null;
        }

        piece.gameObject.transform.rotation = endTargetQuat;

        // After moving, if it's the humans turn,
        // go into attack mode for the piece (for better gameplay flow).
        var humanTurn = (GameState.CurrentState as PlayingState)?.Turn as HumanTurn;
        if (humanTurn != null) {
            humanTurn.DoPieceInteraction(piece);
        }
    }
}