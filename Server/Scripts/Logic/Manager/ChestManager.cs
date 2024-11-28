public static class ChestManager
{
    public static int CHEST_SLOT_COUNT = 27;

    public static int index = 0;
    public static Dictionary<int, Chest> Chests = new();

    public static void InitConfig(Config config)
    {
        CHEST_SLOT_COUNT = config.ChestSlotCount;
    }

    public static void DestroyChest(int id, Vector3Int corner, Room room)
    {
        if (!Chests.TryGetValue(id, out Chest? value)) return;
        foreach (Slot slot in value.slots)
        {
            if (slot.item.type == BlockType.None) continue;
            MsgDropItem msgD = new MsgDropItem();
            msgD.info = slot.item;
            msgD.pos = corner;
            msgD.locked = false;
            msgD.id = ItemManager.index++;
            DroppedItem droppedItem = new();
            droppedItem.position = msgD.pos;
            droppedItem.id = msgD.id;
            ItemManager.AddItem(droppedItem);
            room.Broadcast(msgD);
        }
    }

    public static List<Entity> GetEntityInRoom(int id)
    {
        List<Entity> list = new List<Entity>();
        foreach(var chest in Chests.Values)
        {
            if(chest.roomId == id)
            {
                list.Add(chest);
            }
        }

        return list;
    }

    public static void LoadChestContent(Player player)
    {
        Thread.Sleep(100); //:等待客户端箱子面板实例化后再发消息
        foreach(var chest in Chests.Values)
        {
            if (chest.roomId != player.roomId) continue;
            MsgLoadChestContent msg = new MsgLoadChestContent();
            msg.chestId = chest.id;
            msg.slots = chest.slots;
            player.Send(msg);
            Console.WriteLine("Send ChestContent");
        }
    }

    public static void BombExplode(Vector3Int pos, Room room)
    {
        foreach(var chest in Chests.Values)
        {
            if(chest.roomId != room.id) continue;
            if ((chest.position - pos).Magnitude > Bomb.radius * Bomb.radius) continue;
            MsgUpdateEntity msg = new MsgUpdateEntity();
            msg.id = chest.id;
            msg.corner = chest.position;
            msg.generate = false;
            msg.type = BlockType.Chest;
            room.Broadcast(msg);
            DestroyChest(chest.id, pos, room);
            Chests.Remove(chest.id);

            MsgDropItem msgD = new MsgDropItem();
            msgD.info = new ItemInfo { count = 1, id = 1, type = BlockType.Chest };
            msgD.pos = msg.corner;
            msgD.locked = false;
            msgD.id = ItemManager.index++;
            DroppedItem droppedItem = new();
            droppedItem.position = msgD.pos;
            droppedItem.id = msgD.id;
            ItemManager.AddItem(droppedItem);
            room.Broadcast(msgD);
        }
    }
}