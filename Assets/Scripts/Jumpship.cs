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

    override public void Move() {
    
    }
}
