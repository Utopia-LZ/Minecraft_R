using System.Diagnostics;

public partial class MsgHandler
{
    public static void MsgMapInit(ClientState c, MsgBase msgBase)
    {
        Room room = RoomManager.GetRoom(c.player.roomId);
        MsgMapInit msg = (MsgMapInit)msgBase;
        Chunk chunk = room.mapManager.GetChunk(msg.chunkPos);
        if(chunk == null)
        {
            chunk = new Chunk(room.mapManager.chunkIndex++, msg.chunkPos);
            room.mapManager.InitBlock(chunk);
            room.mapManager.AddChunk(chunk);
        }
        if (room != null)
        {
            msg.map = MapManager.To1(chunk.map);
            room.Broadcast(msg);
        }
    }

    public static void MsgMapChange(ClientState c, MsgBase msgBase)
    {
        MsgMapChange msg = (MsgMapChange)msgBase;
        Room room = RoomManager.GetRoom(c.player.roomId);
        Chunk chunk = room.mapManager.GetChunk(msg.chunkPos);
        if(chunk != null)
        {
            if (msg.blockPos.x < 0 || msg.blockPos.y < 0 || msg.blockPos.z < 0 || msg.blockPos.x >= Chunk.width || msg.blockPos.y >= Chunk.height || msg.blockPos.z >= Chunk.width)
            {
                Console.WriteLine("Block position illegal! " + msg.blockPos.ToString());
            }
            else
            {
                chunk.map[msg.blockPos.x, msg.blockPos.y, msg.blockPos.z] = msg.type;
                Console.WriteLine("MsgMapChange: " + msg.blockPos.ToString());
                room.mapManager.GetBlock(chunk.position + msg.blockPos).type = msg.type;
                room.mapManager.UpdateBlockState(msg.blockPos);
                room.mapManager.UpdateBlockEdge(msg.blockPos);
            }
        }
        else
        {
            Console.WriteLine("Chunk is NULL!");
        }
        if(room != null)
        {
            msg.id = c.player.id;
            room.Broadcast(msg);
        }
    }
}