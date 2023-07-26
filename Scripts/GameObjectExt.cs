using UnityEngine;

public static class GameObjectExt
{
    public static void DestroyAllChildren(this GameObject go)
    {
        foreach (Transform transform in go.transform)
        {
            Object.Destroy(transform.gameObject);
        }
    }

    public static void DestroyAllChildren(this Transform t)
    {
        foreach (Transform transform in t)
        {
            Object.Destroy(transform.gameObject);
        }
    }
    
    public static void DestroyAllChildrenImmediate(this Transform t)
    {
        foreach (Transform transform in t)
        {
            Object.DestroyImmediate(transform.gameObject);
        }
    }
}