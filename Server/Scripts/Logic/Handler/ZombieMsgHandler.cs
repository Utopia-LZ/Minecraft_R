public partial class MsgHandler
{
    public static void MsgZombieHit(ClientState c, MsgBase msgBase)
    {
        MsgZombieHit msg = (MsgZombieHit)msgBase;
        Console.WriteLine("MsgZombieHit " + msg.zombieId);
        Room room = RoomManager.GetRoom(c.player.roomId);

        Zombie zombie = room.zombieManager.GetZombie(msg.zombieId);
        if (zombie != null && zombie.hp > 0)
        {
            zombie.TakeDamage(msg.damage);
            if(zombie.hp > 0) room.Broadcast(msg);
        }
    }

    public static void MsgUpdateZombie(ClientState c, MsgBase msgBase)
    {
        MsgUpdateZombie msg = (MsgUpdateZombie)msgBase;
        Room room = RoomManager.GetRoom(c.player.roomId);
        room.zombieManager.Remove(msg.info.id);
    }
}