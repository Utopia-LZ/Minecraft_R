public partial class MsgHandler
{
    public static void MsgChat(ClientState c, MsgBase msgBase)
    {
        MsgChat msg = (MsgChat)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }
        room.chatManager.AddText(msg.text);
        room.Broadcast(msg);
    }
}