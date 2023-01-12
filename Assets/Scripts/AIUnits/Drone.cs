using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Drone : Piece {
    public int hitPoints;
    public int attackPower;

    override public int HitPoints {
        get => hitPoints;
    }

    override public int AttackPower {
        get => AttackPower;
    }

    override public bool IsEnemy {
        get => true;
    }

    override public IEnumerable<Vector3> MoveDirectionsByRule {
        get => new[] {
            // Moves forward 1 space like a pawn
            new Vector3(0, 0, -1),
        };
    }
}
