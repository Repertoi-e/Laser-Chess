using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoserState : State {
    IEnumerator raiseOcean;
    Vector3 beginPos;

    public LoserState() {
        raiseOcean = RaiseOcean();
    }

    public override void Update() {
        if (raiseOcean != null && !raiseOcean.MoveNext())
            raiseOcean = null;
    }

    public override void End() {
        GameState.Constants.youLoseUI.SetActive(false);
        var ocean = GameObject.FindGameObjectWithTag("Ocean");
        if (ocean != null)
            ocean.transform.position = beginPos;
    }

    public IEnumerator RaiseOcean() {
        // Wait 2 seconds for drama
        float timeElapsed = 0;
        while (timeElapsed < 2) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        var ocean = GameObject.FindGameObjectWithTag("Ocean");
        if (ocean == null)
            yield break;

        beginPos = ocean.transform.position;
        Vector3 targetPos = new Vector3(beginPos.x, 2, beginPos.z);

        float duration = 2;

        timeElapsed = 0;
        while (timeElapsed < duration) {
            if (ocean) {
                float t = timeElapsed / duration;
                t = 1 - (1 - t) * (1 - t); // ease out quad
                ocean.transform.position = Vector3.Lerp(beginPos, targetPos, t);
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        GameState.Constants.youLoseUI.SetActive(true);
    }
}
