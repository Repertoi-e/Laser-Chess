using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderUnit : Piece {
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
            // It can only move 1 space in two possible directions �- parallel to the AIs side of the board
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0)
        };
    }
}