using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Piece {
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
        get {
            // Moves like the Queen in chess, up to a maximum of 3 spaces.
            for (int r = 1; r <= 3; r++) {
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        yield return new Vector3(i * r, 0, j * r);
                    }
                }
            }
        }
    }

    override public EAttackType AttackType {
        get => EAttackType.Shoot;
    }

    override public IEnumerable<Vector3> AttackTilesByRule {
        get {
            // Shoots once, orthogonally at any range.
            for (int i = -8; i <= 8; i++) {
                if (i == 0)
                    continue;
                yield return new Vector3(i, 0, 0);
                yield return new Vector3(0, 0, i);
            }
        }
    }

}
