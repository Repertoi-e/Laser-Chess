using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PieceDeathTransaction : Transaction {
    public Piece piece;

    public override bool IsValid() {
        return piece != null ? piece.HitPoints <= 0 : false;
    }

    public override IEnumerator Execute() {
        GameObject.Destroy(piece.gameObject);
        yield return null;
    }
}
