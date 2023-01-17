using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AITurn : Turn {
    PriorityQueue<Piece> pieces = new();

    int GetPriority(Piece piece) {
        // Drones move before any other AI piece.
        if (piece as Drone)
            return 100;
        // Dreadnoughts move after all drones have move
        if (piece as Dreadnought)
            return 10;
        // The Command Unit must move after Dreadnoughts have move
        if (piece as CommandUnit)
            return 1;
        Debug.Assert(false);
        return -1;
    }

    IEnumerator play;

    public AITurn() : base() {
        foreach (var p in from p in from p in GameObject.FindGameObjectsWithTag("Piece") select p.GetComponent<Piece>() where p.IsEnemy select p) {
            pieces.Enqueue(GetPriority(p), p);
        }
        play = Play();
    }

    public override void Update() {
        if (!play.MoveNext())
            play = null;
    }

    IEnumerator Play() {
        while (pieces.Count != 0) {
            Piece p = pieces.Dequeue();
            if (TryMovePiece(p)) {
                yield return new WaitForSeconds(0.3f);
            }

            var api = new AttackPieceInteraction(p);
            if (api.IsAvailable()) {
                if (p.AttackType == Piece.EAttackType.Shoot && Shoot(p, api))
                    new WaitForSeconds(0.5f);
                if (p.AttackType == Piece.EAttackType.Region && Stomp(p, api))
                    new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(0.5f);

        playingState.Turn = new HumanTurn();
    }

    IEnumerable<Piece> GetPlayerPieces() {
        return from x in
                   from x in GameObject.FindGameObjectsWithTag("Piece")
                   select x.GetComponent<Piece>()
               where !x.IsEnemy
               select x;
    }

    bool TryMovePiece(Piece p) {
        var moveInteraction = new MovePieceInteraction(p);
        if (moveInteraction.IsAvailable()) {
            Vector3 dest;
            if (p is Drone) {
                // We know it's exactly 1 since the Drone has
                // only one place to go by rule.
                dest = moveInteraction.AllowedMovePositions[0];
            } else if (p is Dreadnought) {
                var closest = GetPlayerPieces().OrderBy(x => (p.gameObject.transform.position - x.gameObject.transform.position).sqrMagnitude).First();

                Vector3 dir = (closest.gameObject.transform.position - p.gameObject.transform.position).normalized;

                dest = moveInteraction.AllowedMovePositions.OrderBy(x => Vector3.Dot(dir, (p.gameObject.transform.position - x).normalized)).First();
            } else if (p is CommandUnit) {
                // TODO!
                dest = p.gameObject.transform.position;
            } else {
                Debug.Assert(false);
                dest = Vector3.zero;
            }

            var transaction = new MoveTransaction() { piece = p, target = dest };
            if (transaction.IsValid()) {
                playingState.QueueUpValidTransaction(transaction);
                return true;
            }
        }
        return false;
    }

    // Sigh. Shoot and Stomp are basically identical...

    bool Shoot(Piece p, AttackPieceInteraction api) {
        // TODO: Have some heuristic to determine a better move
        // if multiple targets are available (e.g. team up multiple
        // drones against 1 piece).
        var target = GameState.Board.GetTileAt(api.AllowedAttackPieces[0])?.GetPieceAbove();

        var transaction = new AttackTransaction(target) { piece = p };
        if (transaction.IsValid()) {
            playingState.QueueUpValidTransaction(transaction);
            return true;
        }
        return false;
    }

    bool Stomp(Piece p, AttackPieceInteraction api) {
        // TODO: Have some heuristic to determine a better move
        // if multiple targets are available (e.g. team up multiple
        // drones against 1 piece).
        var targets = (from c in api.AllowedAttackPieces select GameState.Board.GetTileAt(c).GetPieceAbove()).ToArray();

        var transaction = new AttackTransaction(targets) { piece = p };
        if (transaction.IsValid()) {
            playingState.QueueUpValidTransaction(transaction);
            return true;
        }
        return false;
    }
}