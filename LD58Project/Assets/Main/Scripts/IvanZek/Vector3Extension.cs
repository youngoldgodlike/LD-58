using UnityEngine;
public static class Vector3Extension {
    public static Vector3 WithX(this Vector3 vector, float x) {
        vector.x = x;
        return vector;
    }
    public static Vector3 WithY(this Vector3 vector, float y) {
        vector.y = y;
        return vector;
    }  
    public static Vector3 WithZ(this Vector3 vector, float z) {
        vector.z = z;
        return vector;
    }
    public static Vector3 AddY(this Vector3 vector, float y) {
        vector.y += y;
        return vector;
    }
    public static float GetLargestX(this Vector2[] vector) {
        float largest = vector[0].x;
        foreach (Vector2 vec in vector) {
            if (largest < vec.x) largest = vec.x;
        }
        return largest;
    }  
    public static float GetLargestY(this Vector2[] vector) {
        float largest = vector[0].y;
        foreach (Vector2 vec in vector) {
            if (largest < vec.x) largest = vec.x;
        }
        return largest;
    }
    public static Vector2 ToVector2(this Vector3 vector) {
        // return new(vector.);
        return new(vector.x, vector.y);
    }
}
