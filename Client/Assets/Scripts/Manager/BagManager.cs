using UnityEngine;

[System.Serializable]
public struct ItemInfo
{
    public int id;
    public BlockType type;
    public int count;
}

public class BagManager:Singleton<BagManager>
{
    public static readonly int MAX_ITEM_COUNT = 64;
    public static readonly int SLOT_COUNT = 45;

    public BagPanel BagPanel;
    public int SelectSlotIndex;

    public Item SelectSlot { get { return BagPanel.Items[SelectSlotIndex]; } }

    public void Init()
    {
        NetManager.AddMsgListener("MsgAddItem",OnMsgAddItem);
        NetManager.AddMsgListener("MsgRemoveItem",OnMsgRemoveItem);
        NetManager.AddMsgListener("MsgLoadBag", OnMsgLoadBag);
    }

    public void AddItem(ItemInfo item) 
    {
        BagPanel.AddItem(item, ItemPanelType.Inventory);
    }
    public ItemInfo RemoveItem(int count = -1)
    {
        ItemInfo item = new ItemInfo
        {
            id = -1,
            count = count==-1 ? SelectSlot.count : count,
            type = SelectSlot.type
        };
        BagPanel.RemoveItem(ItemPanelType.Inventory,-1,count);
        return item;
    }

    public void Select(int idx)
    {
        if(SelectSlot != null)
            SelectSlot.HighLight.enabled = false;
        SelectSlotIndex = idx;
        SelectSlot.HighLight.enabled = true;
    }

    public void OnMsgAddItem(MsgBase msgBase)
    {
        Debug.Log("OnMsgAddItem");
        MsgAddItem msg = (MsgAddItem)msgBase;
        if(msg == null || msg.result == 1)
        {
            Debug.Log("RefreshBag fail. Something went wrong");
            return;
        }
        if(msg.panelType == ItemPanelType.Inventory)
        {
            Debug.Log("Refresh Inventory");
            BagPanel.RrefreshBag(msg.slot);
        }
        else if(msg.panelType == ItemPanelType.Chest)
        {
            Debug.Log("Refresh Chest");
            ChestManager.Instance.RefreshChest(msg.idx,msg.slot);
        }
    }

    public void OnMsgRemoveItem(MsgBase msgBase)
    {
        Debug.Log("OnMsgRemoveItem");
        MsgRemoveItem msg = (MsgRemoveItem)msgBase;
        if (msg == null || msg.result == 1)
        {
            Debug.Log("RefreshBag fail. Something went wrong");
            return;
        }
        if (msg.panelType == ItemPanelType.Inventory)
        {
            Debug.Log("Refresh Inventory");
            BagPanel.RrefreshBag(msg.slot);
        }
        else if (msg.panelType == ItemPanelType.Chest)
        {
            Debug.Log("Refresh Chest");
            ChestManager.Instance.RefreshChest(msg.idx, msg.slot);
        }
    }

    public void OnMsgLoadBag(MsgBase msgBase)
    {
        Debug.Log("OnMsgLoadBag");
        MsgLoadBag msg = (MsgLoadBag)msgBase;
        Slot[] slots = msg.slots;
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item.type != BlockType.None)
            {
                BagPanel.RrefreshBag(slots[i]);
            }
        }
    }
}