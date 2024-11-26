using UnityEngine;
using Pair = System.Collections.Generic.KeyValuePair<UnityEngine.Transform, Item>;

public class ItemBasePanel : BasePanel
{
    public Transform[] slots;
    public Item[] Items;

    private void Start()
    {
        InitSlot();
    }

    protected virtual void InitSlot() { }

    public void AddItem(ItemInfo item, ItemPanelType panelType, int idx = -1)
    {
        MsgAddItem msg = new MsgAddItem();
        msg.slot = new Slot
        {
            item = item,
        };
        msg.panelType = panelType;
        msg.idx = idx;
        NetManager.Send(msg);
    }

    //idx: Ïä×Ó±àºÅ
    public Item RemoveItem(ItemPanelType type, int idx = -1, int count = -1)
    {
        if (type == ItemPanelType.Inventory)
        {
            if (BagManager.Instance.SelectSlot == null) return null;
            MsgRemoveItem msg = new();
            msg.slot.idx = (ushort)BagManager.Instance.SelectSlotIndex;
            msg.slot.item = new() { count = count, type = BagManager.Instance.SelectSlot.type };
            msg.panelType = type;
            NetManager.Send(msg);
            return BagManager.Instance.SelectSlot;
        }
        else if (type == ItemPanelType.Chest)
        {
            if(ChestManager.Instance.SelectedChestPanel == null) return null;
            MsgRemoveItem msg = new();
            msg.slot.idx = (ushort)ChestManager.Instance.SelectSlotIndex;
            msg.slot.item = new() { count = count, type = ChestManager.Instance.SelectSlot.type };
            msg.idx = idx;
            msg.panelType = type;
            NetManager.Send(msg);
            return ChestManager.Instance.SelectSlot;
        }
        return null;
    }

    public void RrefreshBag(Slot slot)
    {
        Debug.Log("RefreshBag " + slot.item.type.ToString() + " at " + slot.idx);
        if (Items[slot.idx] == null || !Items[slot.idx].gameObject.activeSelf)
        {
            Debug.Log("It's null, Ins again");
            GameObject go = ResManager.Instance.GetGameObject(ObjType.Item, slots[slot.idx]);
            Item newItem = go.GetComponent<Item>();
            Items[slot.idx] = newItem;
        }
        Items[slot.idx].Refresh(slot.item.type, slot.item.count);
    }

    public override void OnInit()
    {
        
    }

    public override void OnShow()
    {
        base.OnShow();
    }

    public override void OnClose()
    {
        base.OnClose();
    }
}