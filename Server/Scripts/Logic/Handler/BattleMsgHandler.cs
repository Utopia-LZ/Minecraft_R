public partial class MsgHandler
{
    public static void MsgHit(ClientState c, MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        Player player = PlayerManager.GetPlayer(msg.id);
        if (player == null) return;
        player.TakeDamage(msg.damage);
        Room room = RoomManager.GetRoom(player.roomId);
        room?.Broadcast(msg);
    }
}