using UnityEngine;

public class Light : MonoBehaviour, PoolObject
{
    public int idx;

    public void OnRecycle()
    {
        Destroy(this);
    }
}