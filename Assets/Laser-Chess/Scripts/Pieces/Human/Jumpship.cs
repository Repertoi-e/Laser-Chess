using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpship : Piece {
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
            new Vector3(-2, 0, -1),
            new Vector3(-2, 0, 1),
            new Vector3(2, 0, -1),
            new Vector3(2, 0, 1),
            new Vector3(-1, 0, -2),
            new Vector3(-1, 0, 2),
            new Vector3(1, 0, -2),
            new Vector3(1, 0, 2)
        };
    }

    public override bool CanIgnorePieceBlock {
        get => true;
    }

    override public EAttackType AttackType {
        get => EAttackType.Region;
    }

    override public IEnumerable<Vector3> AttackTilesByRule {
        get => new Vector3[] {
            // Attacks all enemy pieces in the 4 orthogonally adjacent spaces simultaneously.
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
        };
    }
}
