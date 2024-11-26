public class Bomb : Entity
{
    public static int radius = 3;
    public static float cd = 3f;
    public static int damage = 2;

    public void Burning(Player player)
    {
        Room room = RoomManager.GetRoom(player.roomId);
        MsgBombState msg = new MsgBombState();
        msg.id = id;
        msg.state = BombState.Burning;
        room.Broadcast(msg);

        Thread.Sleep(3000);

        int radius = Bomb.radius;
        for (int i = -radius; i <= radius; i++)
        for (int j = -radius; j <= radius; j++)
        for (int k = -radius; k <= radius; k++)
        {
            Vector3Int pos = new Vector3Int(i, j, k);
            if (pos.Magnitude > radius * radius) continue;

            pos += position;
            MsgMapChange msgMap = new MsgMapChange();
            msgMap.blockPos = (pos % Chunk.width + Chunk.width) % Chunk.width;
            msgMap.chunkPos = pos - msgMap.blockPos;
            msgMap.type = BlockType.None;
            msgMap.id = player.id;

            Chunk chunk = room.mapManager.GetChunk(msgMap.chunkPos);
            if (chunk != null)
            {
                if (msgMap.blockPos.x < 0 || msgMap.blockPos.y < 0 || msgMap.blockPos.z < 0 || msgMap.blockPos.x >= Chunk.width || msgMap.blockPos.y >= Chunk.height || msgMap.blockPos.z >= Chunk.width)
                {
                    Console.WriteLine("Block position illegal! " + msgMap.blockPos.ToString());
                }
                else
                {
                    if (chunk.map[msgMap.blockPos.x, msgMap.blockPos.y, msgMap.blockPos.z] == BlockType.None) continue;
                    chunk.map[msgMap.blockPos.x, msgMap.blockPos.y, msgMap.blockPos.z] = msgMap.type;
                    room.Broadcast(msgMap);
                }
            }
        }
        Explode(id, room, player.id);
    }

    private void Explode(int idx, Room room, string playerId)
    {
        MsgBombState msg = new MsgBombState();
        msg.id = idx;
        msg.state = BombState.Exploded;
        room.Broadcast(msg);

        PlayerManager.BombExplode(position, room);
        ChestManager.BombExplode(position, room);
        BombManager.BombExplode(position, room);
        ItemManager.DroppedExplode(position, room, playerId);
        LightManager.LightExplode(position, room);
    }
}