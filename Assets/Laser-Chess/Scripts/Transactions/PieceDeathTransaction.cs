using System.Collections;
using UnityEngine;

public class PieceDeathTransaction : Transaction {
    public Piece piece;

    public override bool IsValid() {
        return piece != null ? piece.HitPoints <= 0 : false;
    }

    public override IEnumerator Execute() {
        var explosion = GameObject.Instantiate(piece.IsEnemy ? GameState.Constants.kEnemyExplosion : GameState.Constants.kAllyExplosion);

        explosion.transform.position = piece.gameObject.transform.position;

        GameObject.Destroy(piece.gameObject);

        float timeElapsed = 0;
        while (timeElapsed < 1) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        GameObject.Destroy(explosion);
    }
}
