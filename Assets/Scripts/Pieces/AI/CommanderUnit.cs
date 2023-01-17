using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderUnit : Piece {
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
            // It can only move 1 space in two possible directions ­- parallel to the AIs side of the board
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0)
        };
    }
    override public EAttackType AttackType {
        get => EAttackType.None;
    }

    override public IEnumerable<Vector3> AttackTilesByRule {
        get => null;
    }
}
