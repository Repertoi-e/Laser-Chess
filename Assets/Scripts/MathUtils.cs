using UnityEngine;

public static class MathUtils {   
    public static Vector3 SnapVectorToCardinal(Vector3 vec) {
        int largestIndex = 0;
        for (int i = 1; i < 3; i++) {
            largestIndex = Mathf.Abs(vec[i]) > Mathf.Abs(vec[largestIndex]) ? i : largestIndex;
        }
        float newLargest = vec[largestIndex] > 0 ? 1 : -1;
        vec = Vector3.zero;
        vec[largestIndex] = newLargest;
        return vec;
    }
}
