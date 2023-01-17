using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Drone : Piece {
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
        get => new[] {
            // Moves forward 1 space like a pawn
            new Vector3(0, 0, -1),
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
