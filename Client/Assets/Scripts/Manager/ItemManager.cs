using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    public Dictionary<int, DroppedItem> DroppedItems;
    private GameObject DroppedPrefab;

    public void Init()
    {
        NetManager.AddMsgListener("MsgDropItem", OnMsgDropItem);
        NetManager.AddMsgListener("MsgDestroyItem",OnMsgDestroyItem);
        NetManager.AddMsgListener("MsgLoadDropped", OnMsgLoadDropped);
        DroppedPrefab = ResManager.LoadResources<GameObject>("prefab_droppeditem");
        DroppedItems = new();
    }

    public DroppedItem InstantiateItem(int id, ItemInfo info, Vector3 position, Vector3 direction, bool locked = false) //:λ�ò�̫��
    {
        DroppedItem item = MonoBehaviour.Instantiate(DroppedPrefab).GetComponent<DroppedItem>();

        if (locked) item.Lock();
        if (direction != Vector3.zero)
        {
            position += direction * 0.6f;
            item.GetComponent<Rigidbody>().AddForce(direction * 2f, ForceMode.Impulse);
        }
        item.Init(info, position);
        DroppedItems[id] = item;
        return item;
    }

    public void Clear()
    {

    }

    public void OnMsgDropItem(MsgBase msgBase)
    {
        Debug.Log("OnMsgDropItem");
        MsgDropItem msg = (MsgDropItem)msgBase;
        if (msg != null && msg.id != -1)
        {
            Vector3 pos = Vector3Int.V3IntToV3(msg.pos);
            Vector3 dir = Vector3Int.V3IntToV3(msg.dir);
            DroppedItem item = InstantiateItem(msg.id,msg.info, pos, dir, msg.locked);
            item.id = msg.id;
            DroppedItems[msg.id] = item;
        }
        else
        {
            Debug.Log("DropItem failed. Something went wrong.");
        }
    }

    public void OnMsgDestroyItem(MsgBase msgBase)
    {
        MsgDestroyItem msg = (MsgDestroyItem)msgBase;
        if (msg.id == "") return;
        if (DroppedItems.ContainsKey(msg.idx))
        {
            if (msg.pickedup && msg.id == GameMain.id)
                BagManager.Instance.AddItem(DroppedItems[msg.idx].info);
            GameObject.Destroy(DroppedItems[msg.idx].gameObject);
            DroppedItems.Remove(msg.idx);
        }
    }

    public void OnMsgLoadDropped(MsgBase msgBase)
    {
        MsgLoadDropped msg = (MsgLoadDropped)msgBase;

        for(int i = 0; i < msg.items.Length; i++)
        {
            DroppedInfo item = msg.items[i];
            ItemInfo info = new ItemInfo
            {
                id = item.id,
                type = item.type,
                count = item.count,
            };
            InstantiateItem(item.id, info, Vector3Int.V3IntToV3(item.position), Vector3.zero);
        }
    }
}