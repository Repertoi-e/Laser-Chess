using UnityEngine;
using System.Collections;
using static UnityEngine.UI.ScrollRect;
using VolumetricLines;

public class AttackTransaction : Transaction {
    public Piece piece;

    Piece[] targets = null;

    public AttackTransaction(Piece target) {
        if (target != null) {
            targets = new[] { target };
        }
    }

    public AttackTransaction(Piece[] targets) {
        this.targets = targets;
    }

    public override bool IsValid() {
        if (piece == null || targets == null)
            return false;
        foreach (var target in targets)
            if (target == null || piece.IsEnemy == target.IsEnemy)
                return false; // attacking from the same team

        return true;
    }

    public override IEnumerator Execute() {
        if (piece.AttackType == Piece.EAttackType.Region) {
            foreach (var target in targets) {
                target.HitPoints -= piece.AttackPower;
                yield return new WaitForSeconds(0.1f);
            }
        } else {
            var target = targets?[0];
            if (target) {
                var dir = target.gameObject.transform.position - piece.gameObject.transform.position;

                Quaternion beginQuat = piece.gameObject.transform.rotation;
                var targetQuat = Quaternion.LookRotation(dir);

                float timePassed = 0;
                while (timePassed < 1) {
                    float t = timePassed / 1;
                    piece.gameObject.transform.rotation = Quaternion.Lerp(beginQuat, targetQuat, t);
                    timePassed += Time.deltaTime;
                    yield return null;
                }

                // Shoot laser
                var laser = GameObject.Instantiate(GameState.Constants.kShootLaser);
                laser.transform.position = new Vector3(0, 0.7f, 0);

                var vlb = laser.GetComponent<VolumetricLineBehavior>();
                vlb.StartPos = piece.gameObject.transform.position;
                vlb.EndPos = target.gameObject.transform.position;
                vlb.LineColor = piece.IsEnemy ? GameState.Constants.kEnemyColor : GameState.Constants.kAllyColor;

                timePassed = 0;
                while (timePassed < 0.15f) {
                    timePassed += Time.deltaTime;
                    yield return null;
                }

                target.HitPoints -= piece.AttackPower;
                GameObject.Destroy(laser);
            }
        }
    }
}