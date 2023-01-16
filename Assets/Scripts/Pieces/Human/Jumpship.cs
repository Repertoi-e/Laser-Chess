using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpship : Piece {
    public int hitPoints;
    public int attackPower;

    override public int HitPoints {
        get => hitPoints;
    }

    override public int AttackPower {
        get => AttackPower;
    }

    override public bool IsEnemy {
        get => false;
    }

    override public IEnumerable<Vector3> MoveDirectionsByRule {
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
}
