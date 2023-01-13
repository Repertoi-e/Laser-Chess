using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : Piece {
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
            // Moves 1 space orthogonally
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
        };
    }
}
