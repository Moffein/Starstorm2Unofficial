using UnityEngine;

public static class ComponentHelpers
{
    public static bool HasComponent<T>(this GameObject g) where T : Component
    {
        return g.GetComponent(typeof(T)) != null;
    }

    public static bool HasComponent<T>(this MonoBehaviour m) where T : Component
    {
        return m.GetComponent(typeof(T)) != null;
    }

    public static bool HasComponent<T>(this Transform t) where T : Component
    {
        return t.GetComponent(typeof(T)) != null;
    }

    public static int ComponentCount<T>(this GameObject g) where T : Component
    {
        return g.GetComponents(typeof(T)).Length;
    }

    public static int ComponentCount<T>(this MonoBehaviour m) where T : Component
    {
        return m.GetComponents(typeof(T)).Length;
    }

    public static int ComponentCount<T>(this Transform t) where T : Component
    {
        return t.GetComponents(typeof(T)).Length;
    }

    public static T GetComponent<T>(this GameObject g, int index) where T : Component
    {
        return g.GetComponents(typeof(T))[index] as T;
    }

    public static T GetComponent<T>(this MonoBehaviour m, int index) where T : Component
    {
        return m.gameObject.GetComponents(typeof(T))[index] as T;
    }

    public static T GetComponent<T>(this Transform t, int index) where T : Component
    {
        return t.GetComponents(typeof(T))[index] as T;
    }
    public static T AddComponent<T>(this MonoBehaviour m) where T : Component
    {
        return m.gameObject.AddComponent(typeof(T)) as T;
    }

    public static T AddComponent<T>(this Transform t) where T : Component
    {
        return t.gameObject.AddComponent(typeof(T)) as T;
    }

    public static T AddOrGetComponent<T>(this GameObject g) where T : Component
    {
        return (g.HasComponent<T>() ? g.GetComponent(typeof(T)) : g.AddComponent(typeof(T))) as T;
    }

    public static T AddOrGetComponent<T>(this MonoBehaviour m) where T : Component
    {
        return (m.HasComponent<T>() ? m.GetComponent(typeof(T)) : m.gameObject.AddComponent(typeof(T))) as T;
    }

    public static T AddOrGetComponent<T>(this Transform t) where T : Component
    {
        return (t.HasComponent<T>() ? t.GetComponent(typeof(T)) : t.gameObject.AddComponent(typeof(T))) as T;
    }
}