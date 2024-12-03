using UnityEngine;

public class ChestPanel: ItemBasePanel, PoolObject
{
    public static int CHEST_SLOT_COUNT;

    public int idx = -1;
    public int Offset { get {  return idx+1000; } }

    public override void InitSlot()
    {
        slots = new Transform[CHEST_SLOT_COUNT];
        int i = 0;
        foreach(Transform child in transform)
        {
            slots[i++] = child;
        }
        Items = new Item[CHEST_SLOT_COUNT];
        int layer = LayerMask.NameToLayer("ChestPanel") + Offset;
        EventHandler.MouseEvents[layer] = ShiftItems;
        Close();
    }

    public void AddItemInChest(ItemInfo info)
    {
        AddItem(info, ItemPanelType.Chest, idx);
    }

    public void ShiftItems()
    {
        Debug.Log("Chest ShiftItems");
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("Chest Panel ShiftItems " + idx);
            Item item = RemoveItem(ItemPanelType.Chest, idx);
            if (item != null)
            {
                BagManager.Instance.AddItem(item.Info());
            }
            else
            {
                Debug.Log("Shift item is null!");
            }
        }
    }

    public void OnRecycle()
    {
        int layer = LayerMask.NameToLayer("ChestPanel") + Offset;
        EventHandler.MouseEvents[layer] = null;
        Destroy(this);
    }
}