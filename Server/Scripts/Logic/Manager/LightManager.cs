using static System.Reflection.Metadata.BlobBuilder;

public static class LightManager
{
    public static Dictionary<int, Light> Lights = new();
    public static int index = 0;

    public static List<Entity> GetEntityInRoom(int id)
    {
        List<Entity> list = new List<Entity>();
        foreach (var light in Lights.Values)
        {
            if (light.roomId == id)
            {
                list.Add(light);
            }
        }

        return list;
    }

    public static void LightExplode(Vector3Int pos, Room room)
    {
        foreach (var light in Lights.Values)
        {
            if (light.roomId != room.id) continue;
            if (light.position == pos) continue;
            if ((light.position - pos).Magnitude > Bomb.radius * Bomb.radius) continue;
            MsgUpdateEntity msg = new MsgUpdateEntity();
            msg.id = light.id;
            msg.corner = light.position;
            msg.generate = false;
            msg.type = BlockType.Bomb;
            room.Broadcast(msg);
            Lights.Remove(light.id);

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

    public static int LightValue(Vector3Int position)
    {
        int result = 0;
        foreach(Light light in Lights.Values)
        {
            int delta = FindPath.GetManhattanDistance(light.position, position);
            delta = Light.radius - delta;
            if(delta < 0) delta = 0;
            result += delta;
        }
        return result;
    }
}