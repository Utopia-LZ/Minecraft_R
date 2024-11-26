public partial class MsgHandler
{
    public static void MsgTime(ClientState c, MsgBase msgBase)
    {
        MsgTime msg = (MsgTime)msgBase;
        Room room = RoomManager.GetRoom(c.player.roomId);
        if(room != null)
        {
            room.Time = msg.Time;
            room.Broadcast(msg);
        }
    }
}