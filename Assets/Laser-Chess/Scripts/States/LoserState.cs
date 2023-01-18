using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoserState : State {
    GameObject ocean = null;
    float timeElapsed = 0;

    Vector3 oceanBeginPos;
    Vector3 oceanTargetPos;

    public LoserState() {
        ocean = GameObject.FindGameObjectWithTag("Ocean");
        if (ocean) {
            oceanBeginPos = ocean.transform.position;
            oceanTargetPos = new Vector3(oceanBeginPos.x, 2, oceanBeginPos.z);
        }
    }

    public override void End() {
    }

    public override void Update() {
        if (timeElapsed < 2) {
            if (ocean) {
                float t = timeElapsed / 2;
                t = 1 - (1 - t) * (1 - t); // ease out quad
                ocean.transform.position = Vector3.Lerp(oceanBeginPos, oceanTargetPos, t);
            }
            timeElapsed += Time.deltaTime;
        }
    }
}
