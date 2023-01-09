using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour {
    abstract public int HitPoints {
        get;
    }
    abstract public int AttackPower {
        get;
    }

    abstract public void Move();
}
