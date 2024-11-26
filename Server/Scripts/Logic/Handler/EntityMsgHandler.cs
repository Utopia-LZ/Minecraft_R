public partial class MsgHandler
{
    public static void MsgUpdateEntity(ClientState c, MsgBase msgBase)
    {
        MsgUpdateEntity msg = (MsgUpdateEntity)msgBase;

        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        if (msg.generate)
        {
            if (msg.type == BlockType.Chest)
            {
                msg.id = ChestManager.index++;
                ChestManager.Chests[msg.id] = new()
                { id = msg.id, position = msg.corner, type = BlockType.Chest, roomId = room.id };
            }
            else if (msg.type == BlockType.Bomb)
            {
                msg.id = BombManager.index++;
                BombManager.Bombs[msg.id] = new Bomb()
                { id = msg.id, position = msg.corner, type = BlockType.Bomb, roomId = room.id };
            }
            else if (msg.type == BlockType.Light)
            {
                msg.id = LightManager.index++;
                LightManager.Lights[msg.id] = new Light()
                { id = msg.id, position = msg.corner, type = BlockType.Light, roomId = room.id };
            }
        }
        else
        {
            if (msg.type == BlockType.Chest)
            {
                ChestManager.DestroyChest(msg.id, msg.corner, room);
                ChestManager.Chests.Remove(msg.id);
            }
            else if (msg.type == BlockType.Bomb)
            {
                BombManager.Bombs.Remove(msg.id);
            }
            else if (msg.type == BlockType.Light)
            {
                LightManager.Lights.Remove(msg.id);
            }

            MsgDropItem msgD = new MsgDropItem();
            msgD.info = new ItemInfo { type = msg.type, count = 1 };
            msgD.pos = msg.corner;
            msgD.locked = false;
            msgD.id = ItemManager.index++;
            DroppedItem droppedItem = new()
            {
                id = msgD.info.id,
                roomId = c.player.roomId,
                type = msgD.info.type,
                position = msgD.pos,
                count = msgD.info.count,
            };
            ItemManager.AddItem(droppedItem);
            room.Broadcast(msgD);
        }

        Block block = room.mapManager.GetBlock(msg.corner/100);
        block.type = msg.type;
        room.mapManager.UpdateBlockState(msg.corner);
        room.mapManager.UpdateBlockEdge(msg.corner);

        room.Broadcast(msg);
    }

    public static void MsgBombState(ClientState c, MsgBase msgBase)
    {
        MsgBombState msg = (MsgBombState)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;

        if (msg.state == BombState.Burning)
        {
            BombManager.Ignite(msg.id, player);
        }
    }
}