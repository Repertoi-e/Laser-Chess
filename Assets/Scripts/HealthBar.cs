using UnityEngine;

public class HealthBar : MonoBehaviour {
    public void SetMaxHitPoints(int max) {
        Debug.Assert(max >= 1);

        var bar = transform.GetChild(0);
        if (bar) {
            for (int i = 1; i < transform.childCount; i++)
                Destroy(transform.GetChild(i));

            for (int i = 1; i < max; i++) {
                var newBar = Instantiate(bar);
                newBar.SetParent(transform, false);
            }

            float factor = 0.54f;

            int t = transform.childCount / 2;
            float startX = -t * factor;
            for (int i = 0; i < transform.childCount; i++) {
                Transform childTransform = transform.GetChild(i).transform;
                childTransform.localPosition = new Vector3(startX, childTransform.localPosition.y, childTransform.localPosition.z);
                startX += factor;
            }
        }
    }

    public void SetHitPoints(int hp) {
        Debug.Assert(hp >= 0);
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(i < hp);
        }
    }
}
