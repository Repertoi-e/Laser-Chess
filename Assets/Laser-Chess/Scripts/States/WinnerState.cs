using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerState : State {
    GameObject rotator = null;

    float elapsedTime = 0;

    public WinnerState() {
        rotator = GameObject.Instantiate(GameState.Constants.kWinnerExplosion);
        rotator.transform.position = new Vector3(3.5f, 1, 3.5f);
    }

    public override void End() {
        GameState.Constants.youWinUI.SetActive(false);
    }

    public override void Update() {
        if (elapsedTime < 10) {
            elapsedTime += Time.deltaTime;
        } else {
            if (rotator != null)
                GameObject.Destroy(rotator);
            GameState.Constants.youWinUI.SetActive(true);
        }
    }
}
