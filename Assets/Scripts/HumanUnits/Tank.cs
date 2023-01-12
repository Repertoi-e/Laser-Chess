using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Piece {
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
}
