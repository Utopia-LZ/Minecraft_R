using UnityEngine;

public class ResManager : MonoBehaviour
{
    public static GameObject InstantiatePrefab(string path)
    {
        return Instantiate(LoadResources<GameObject>(path));
    }

    public static T LoadResources<T> (string resName) where T : Object
    {
        return ABManager.Instance.LoadResource<T>(resName.Split('_')[0], resName);
    }
}