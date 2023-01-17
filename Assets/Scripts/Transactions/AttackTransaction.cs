using UnityEngine;
using System.Collections;

public class AttackTransaction : Transaction {
    public Piece piece;
    Piece[] targets = null;

    public AttackTransaction(Piece target) {
        if (target != null) {
            targets = new[] { target };
        }
    }

    public AttackTransaction(Piece[] targets) {
        this.targets = targets;
    }

    public override bool IsValid() {
        foreach (var target in targets)
            if (piece.IsEnemy == target.IsEnemy)
                return false; // attacking from the same team

        return true;
    }

    public override IEnumerator Execute() {
        foreach (var target in targets) {
            target.HitPoints -= piece.AttackPower;
            yield return new WaitForSeconds(0.1f);
        }
    }
}