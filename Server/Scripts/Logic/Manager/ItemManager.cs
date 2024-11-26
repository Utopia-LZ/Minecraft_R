public static class ItemManager
{
    public static Dictionary<int, DroppedItem> DroppedItems = new();
    public static int index = 0;

    public static void AddItem(DroppedItem item)
    {
        DroppedItems[item.id] = item;
    }
    public static void RemoveItem(int id)
    {
        if(DroppedItems.ContainsKey(id))
            DroppedItems.Remove(id);
    }

    public static void LoadDroppedItems(Player player)
    {
        List<DroppedItem> list = new();
        foreach (var dropped in DroppedItems.Values)
        {
            if (dropped.roomId == player.roomId)
            {
                list.Add(dropped);
            }
        }
        if (list.Count == 0) return;
        MsgLoadDropped msg = new();
        msg.items = new DroppedInfo[list.Count];
        int i = 0;
        foreach(var item in list)
        {
            msg.items[i++] = new DroppedInfo
            {
                id = item.id,
                roomId = item.roomId,
                type = item.type,
                position = item.position,
                count = item.count,
            };
        }
        if(i != 0) player.Send(msg);
    }

    public static void DroppedExplode(Vector3Int pos, Room room, string playerId)
    {
        Console.WriteLine("ItemManager DroppedExplode");
        foreach (var item in DroppedItems.Values)
        {
            if (item.roomId != room.id) continue;
            if ((item.position - pos).Magnitude > Bomb.radius * Bomb.radius) continue;

            RemoveItem(item.id);

            MsgDestroyItem msg = new MsgDestroyItem();
            msg.id = playerId;
            msg.idx = item.id;
            msg.pickedup = false;
            room.Broadcast(msg);
            Console.WriteLine("Send DestoryItem");
        }
    }
}