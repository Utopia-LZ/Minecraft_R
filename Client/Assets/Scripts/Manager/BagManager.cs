using System.Collections;
using System.Threading;
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
    public static int MAX_ITEM_COUNT;
    public static int SLOT_COUNT;

    public BagPanel BagPanel;
    public int SelectSlotIndex;

    private SynchronizationContext mainThreadContext;

    public Item SelectSlot { get { return BagPanel.Items[SelectSlotIndex]; } }

    public void Init()
    {
        mainThreadContext = SynchronizationContext.Current;

        NetManager.AddMsgListener("MsgAddItem",OnMsgAddItem);
        NetManager.AddMsgListener("MsgRemoveItem",OnMsgRemoveItem);
        NetManager.AddMsgListener("MsgLoadBag", OnMsgLoadBag);

        Config config = DataManager.Instance.Config;
        MAX_ITEM_COUNT = config.SlotMaxItemCount;
        SLOT_COUNT = config.SlotCount;
        CameraFollow.thirdSpeed = config.ThirdSpeed;
        CameraFollow.followSpeed = config.FollowSpeed;
        CameraFollow.zoomSpeed = config.ZoomSpeed;
        CameraFollow.radius = config.CameraRadius;
        CameraFollow.minRadius = config.CameraMinRadius;
        CameraFollow.maxRadius = config.CameraMaxRadius;
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
        Thread thread = new(new ParameterizedThreadStart(WaitingLoad));
        thread.Start(msg.slots);
    }

    public void WaitingLoad(object obj)
    {
        while (BagPanel == null)
        {
            Thread.Sleep(100);
        }
        mainThreadContext.Post(_ =>
        {
            DelayLoadBag(obj);
        }, null);
    }

    public void DelayLoadBag(object obj)
    {
        Slot[] slots = (Slot[])obj;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item.type != BlockType.None)
            {
                BagPanel.RrefreshBag(slots[i]);
            }
        }
    }
}