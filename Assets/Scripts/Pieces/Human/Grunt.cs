using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : Piece {
    public int maxHitPoints;
    public int attackPower;

    override public int MaxHitPoints {
        get => maxHitPoints;
    }

    override public int AttackPower {
        get => attackPower;
    }

    override public bool IsEnemy {
        get => false;
    }

    override public IEnumerable<Vector3> MoveTilesByRule {
        get => new[] {
            // Moves 1 space orthogonally
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
        };
    }

    override public EAttackType AttackType {
        get => EAttackType.Shoot;
    }

    override public IEnumerable<Vector3> AttackTilesByRule {
        get {
            // Shoots once, diagonally at any range.
            for (int i = 1; i <= 8; i++) {
                yield return new Vector3(i, 0, i);
                yield return new Vector3(i, 0, -i);
                yield return new Vector3(-i, 0, i);
                yield return new Vector3(-i, 0, -i);
            }
        }
    }
}
