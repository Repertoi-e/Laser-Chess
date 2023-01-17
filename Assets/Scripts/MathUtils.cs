using System;
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
    
    // Moves and shooting attacks are blocked by pieces (except knight-like moving)
    public static bool IsInteractionBlocked(Board board, Vector3 start, Vector3 end) {
        int xIncrement = (start.x == end.x) ? 0 : (start.x < end.x) ? 1 : -1;
        int zIncrement = (start.z == end.z) ? 0 : (start.z < end.z) ? 1 : -1;

        for (int i = 1; i < Math.Max(Math.Abs(start.x - end.x), Math.Abs(start.z - end.z)); i++) {
            var dest = new Vector3(start.x + i * xIncrement, 0, start.z + i * zIncrement);
            // Don't look at the end piece, as it's handled separately anyway (otherwise knight-like jumpship would be able to move through things)
            if (dest == end)
                continue;

            Tile tile = board.GetTileAt(dest);
            if (tile == null || tile.GetPieceAbove() != null) {
                return true;
            }
        }
        return false;
    }
}
