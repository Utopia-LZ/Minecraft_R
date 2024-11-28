[System.Serializable]
public enum BombState
{
    None,
    Unlit, //未点燃
    Burning, //燃烧引信
    Exploded //爆炸
}

public static class BombManager
{
    public static Dictionary<int, Bomb> Bombs = new();
    public static int index = 0;

    public static void InitConfig(Config config)
    {
        Bomb.radius = config.BombRadius;
        Bomb.cd = config.BombBurnTime;
        Bomb.damage = config.BombDamage;
        Bomb.falloff = config.BombFalloff;
    }

    public static void Ignite(int idx, Player player) //点燃炸弹
    {
        if (!Bombs.ContainsKey(idx)) return;
        Bombs[idx].Burning(player);
    }

    public static List<Entity> GetEntityInRoom(int id)
    {
        List<Entity> list = new List<Entity>();
        foreach (var bomb in Bombs.Values)
        {
            if (bomb.roomId == id)
            {
                list.Add(bomb);
            }
        }

        return list;
    }
    public static void BombExplode(Vector3Int pos, Room room)
    {
        foreach (var bomb in Bombs.Values)
        {
            if (bomb.roomId != room.id) continue;
            if (bomb.position == pos) continue;
            if ((bomb.position - pos).Magnitude > Bomb.radius * Bomb.radius) continue;
            MsgUpdateEntity msg = new MsgUpdateEntity();
            msg.id = bomb.id;
            msg.corner = bomb.position;
            msg.generate = false;
            msg.type = BlockType.Bomb;
            room.Broadcast(msg);
            Bombs.Remove(bomb.id);

            MsgDropItem msgD = new MsgDropItem();
            msgD.info = new ItemInfo { count = 1, id = 1, type = BlockType.Bomb };
            msgD.pos = pos;
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