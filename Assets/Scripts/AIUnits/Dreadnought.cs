using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dreadnought : Piece {
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
        get {
            // Moves 1 space in any direction
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    yield return new Vector3(i, 0, j);
                }
            }
        }
    }
}
