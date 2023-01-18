using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoserState : State {
    public LoserState() {
        GameState.Board.StartCoroutine(RaiseOcean());
    }

    public override void End() {
    }

    public IEnumerator RaiseOcean() {
        var ocean = GameObject.FindGameObjectWithTag("Ocean");
        if (!ocean) yield break;

        Vector3 beginPos = ocean.transform.position;
        Vector3 targetPos = new Vector3(beginPos.x, 2, beginPos.z);

        float duration = 2;

        float timeElapsed = 0;
        if (timeElapsed < duration) {
            if (ocean) {
                float t = timeElapsed / duration;
                t = 1 - (1 - t) * (1 - t); // ease out quad
                ocean.transform.position = Vector3.Lerp(beginPos, targetPos, t);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
