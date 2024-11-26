using UnityEngine;

public class BagPanel : ItemBasePanel
{
    public GameObject Panel; //背包面板

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Panel.SetActive(!Panel.activeSelf);
        }
    }

    protected override void InitSlot()
    {
        Items = new Item[BagManager.SLOT_COUNT];
        BagManager.Instance.BagPanel = this;
        int layer = LayerMask.NameToLayer("InventoryPanel");
        EventHandler.MouseEvents[layer] = ShiftItemsFromInv;
        layer = LayerMask.NameToLayer("BagPanel");
        EventHandler.MouseEvents[layer] = ShiftItemsFromBag;
        EventHandler.OnLeaveRoom += Clear;
    }

    private void ShiftItemsFromInv()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Item item = RemoveItem(ItemPanelType.Inventory);
            if(item != null)
            {
                if (ChestManager.Instance.IsOpened)
                {
                    ChestManager.Instance.SelectedChestPanel.AddItemInChest(item.Info());
                }
                else if (Panel != null)
                {
                    //:物品栏 -> 背包
                }
                else { Debug.Log("No opened panel"); }
            }
            else { Debug.Log("Item is null"); }
        }
    }

    public void Clear()
    {
        for(int i = 0; i < Items.Length; i++)
        {
            if (Items[i] == null) continue;
            //Destroy(Items[i].gameObject);
            ResManager.Instance.RecycleObj(Items[i].gameObject, ObjType.Item);
            Items[i] = null;
        }
    }

    private void ShiftItemsFromBag()
    {
        //:Bag -> Inventory
    }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnShow()
    {
        base.OnShow();
    }

    public override void OnClose()
    {
        base.OnClose();
    }

    private void OnDestroy()
    {
        EventHandler.OnLeaveRoom -= Clear;
    }
}