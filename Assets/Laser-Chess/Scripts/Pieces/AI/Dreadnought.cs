using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dreadnought : Piece {
    public int maxHitPoints;
    public int attackPower;

    override public int MaxHitPoints {
        get => maxHitPoints;
    }

    override public int AttackPower {
        get => attackPower;
    }

    override public bool IsEnemy {
        get => true;
    }

    override public IEnumerable<Vector3> MoveTilesByRule {
        get {
            // Moves 1 space in any direction
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    yield return new Vector3(i, 0, j);
                }
            }
        }
    }

    override public EAttackType AttackType {
        get => EAttackType.Region;
    }

    override public IEnumerable<Vector3> AttackTilesByRule {
        get => new Vector3[] {
            // Attacks all adjacent enemy units
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1),
            new Vector3(-1, 0, -1),
            new Vector3(1, 0, -1),
        };
    }
}
