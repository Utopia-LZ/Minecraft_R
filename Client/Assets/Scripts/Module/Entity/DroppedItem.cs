using UnityEngine;

[System.Serializable]
public struct DroppedInfo
{
    public int id;
    public int roomId;
    public BlockType type;
    public Vector3Int position;
    public int count;
}

public class DroppedItem : MonoBehaviour, PoolObject
{
    private MeshRenderer MeshRenderer;

    public int id = -1;
    public ItemInfo info;
    public float lockedTime = 0;
    public static float LockTime = 1f;

    public float rotateSpeed = 50f;

    private void Update()
    {
        if (lockedTime > 0) lockedTime -= Time.deltaTime;
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    public void Init(ItemInfo info, Vector3 position)
    {
        transform.position = position;
        this.info = info;
        MeshRenderer = GetComponent<MeshRenderer>();
        ObjType objType = info.type switch
        {
            BlockType.Grass => ObjType.MatGrass,
            BlockType.Chest => ObjType.MatChest,
            BlockType.Bomb => ObjType.MatBomb,
            BlockType.Light => ObjType.MatLight,
            BlockType.Dirt => ObjType.MatDirt,
            BlockType.Carrion => ObjType.MatCarrion,
            _ => ObjType.None
        };
        MeshRenderer.material = ResManager.Instance.LoadResources<Material>(objType);
    }

    public void Lock()
    {
        lockedTime = LockTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (lockedTime > 0) return;

        if(collision.gameObject.layer == LayerMask.NameToLayer("Own"))
        {
            MsgDestroyItem msg = new()
            {
                idx = id,
                pickedup = true
            };
            NetManager.Send(msg);
        }
    }

    public void OnRecycle()
    {
        Destroy(this);
    }
}