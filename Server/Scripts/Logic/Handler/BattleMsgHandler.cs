public partial class MsgHandler
{
    public static void MsgHit(ClientState c, MsgBase msgBase)
    {
        Room room = RoomManager.GetRoom(c.player.roomId);
        MsgHit msg = (MsgHit)msgBase;
        Player player = room.playerManager.GetPlayer(msg.id);
        if (player == null) return;
        player.TakeDamage(msg.damage);
        room?.Broadcast(msg);
    }

    public static void MsgHungry(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("MsgHungry");
        MsgHungry msg = (MsgHungry)msgBase;
        Player player = c.player;
        if (player == null) return;
        player.hunger += msg.hunger;
        player.saturation += msg.saturation;
    }
}