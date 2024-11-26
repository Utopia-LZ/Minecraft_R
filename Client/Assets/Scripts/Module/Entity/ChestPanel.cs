using UnityEngine;
using Pair = System.Collections.Generic.KeyValuePair<UnityEngine.Transform, Item>;

public class ChestPanel: ItemBasePanel
{
    public static readonly int CHEST_SLOT_COUNT = 27;

    public int idx = -1;
    public int Offset { get {  return idx+1000; } }

    protected override void InitSlot()
    {
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
}