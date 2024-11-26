using System;
using System.Diagnostics;

public partial class MsgHandler
{
    public static void MsgAddItem(ClientState c, MsgBase msgBase)
    {
        MsgAddItem msg = (MsgAddItem)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        msg.result = 0;
        Slot[] slots;
        if (msg.panelType == ItemPanelType.Inventory) slots = player.data.slots;
        else if (msg.panelType == ItemPanelType.Chest && msg.idx != -1) slots = ChestManager.Chests[msg.idx].slots;
        else return;

        //先找存在但未满的单元格
        ItemInfo addedItem = msg.slot.item;
        for (int i = 0; i < slots.Length; i++)
        {
            ItemInfo bagItem = slots[i].item;
            if (bagItem.count != 0 && bagItem.type == addedItem.type)
            {
                int delta = Math.Min(BagManager.MAX_ITEM_COUNT - bagItem.count, addedItem.count);
                addedItem.count -= delta;
                msg.slot.idx = (ushort)i;
                msg.slot.item.count = delta;
                slots[i].item.count += delta;
                if(msg.panelType == ItemPanelType.Inventory)
                {
                    player.Send(msg);
                }
                else if(msg.panelType == ItemPanelType.Chest)
                {
                    room?.Broadcast(msg);
                }
                if (addedItem.count == 0) return;
            }
        }
        //再找空单元格
        for (int j = 0; j < slots.Length; j++)
        {
            int delta;
            ItemInfo bagItem = slots[j].item;
            if (bagItem.type == BlockType.None && addedItem.count > 0)
            {
                delta = Math.Min(BagManager.MAX_ITEM_COUNT, addedItem.count);
                addedItem.count -= delta;
                msg.slot.item.count = delta;
                msg.slot.idx = (ushort)j;
                slots[j].item.type = addedItem.type;
                slots[j].item.count = delta;
                if (msg.panelType == ItemPanelType.Inventory)
                {
                    player.Send(msg);
                }
                else if (msg.panelType == ItemPanelType.Chest)
                {
                    room?.Broadcast(msg);
                }
            }
        }
    }

    public static void MsgRemoveItem(ClientState c, MsgBase msgBase)
    {
        MsgRemoveItem msg = (MsgRemoveItem)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        msg.result = 0;
        Slot[] slots;
        if (msg.panelType == ItemPanelType.Inventory) slots = player.data.slots;
        else if (msg.panelType == ItemPanelType.Chest && msg.idx != -1) slots = ChestManager.Chests[msg.idx].slots;
        else return;

        ItemInfo bagItem = slots[msg.slot.idx].item;
        ItemInfo removedItem = msg.slot.item;
        if (removedItem.count > bagItem.count || removedItem.type != bagItem.type) return;
        if (removedItem.count == -1) removedItem.count = bagItem.count;
        slots[msg.slot.idx].item.count -= removedItem.count;
        if(slots[msg.slot.idx].item.count == 0) slots[msg.slot.idx].item.type = BlockType.None;
        msg.slot.item.count = -removedItem.count;
        if (msg.panelType == ItemPanelType.Inventory)
        {
            player.Send(msg);
        }
        else if (msg.panelType == ItemPanelType.Chest)
        {
            room?.Broadcast(msg);
        }
    }

    public static void MsgDropItem(ClientState c, MsgBase msgBase)
    {
        MsgDropItem msg = (MsgDropItem)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        msg.id = ItemManager.index++;
        DroppedItem droppedItem = new();
        droppedItem.position = msg.pos;
        droppedItem.id = msg.id;
        droppedItem.roomId = room.id;
        droppedItem.type = msg.info.type;
        ItemManager.AddItem(droppedItem);
        room?.Broadcast(msg);
    }

    public static void MsgDestroyItem(ClientState c, MsgBase msgBase)
    {
        MsgDestroyItem msg = (MsgDestroyItem)msgBase;
        Player player = c.player;
        if (player == null) return;
        ItemManager.RemoveItem(msg.idx);
        msg.id = player.id;
        Room room = RoomManager.GetRoom(player.roomId);
        room?.Broadcast(msg);
    }
}