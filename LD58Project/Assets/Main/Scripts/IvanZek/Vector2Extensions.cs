using UnityEngine;
public static class Vector2Extensions {
    public static Vector3 WithX(this Vector2 vec, float x) => new(x, vec.y);
    public static Vector3 WithY(this Vector2 vec, float y) => new(vec.x, y);
    
    public static Vector3 AddX(this Vector2 vec, float x) => new(vec.x + x, vec.y);
    public static Vector3 AddY(this Vector2 vec, float y) => new(vec.x, vec.y + y);
}