public partial class MsgHandler
{
    public static void MsgTest(ClientState c, MsgBase msgBase)
    {
        MsgTest msg = (MsgTest)msgBase;
        Console.WriteLine("MsgTest Generate Zombie");
        Room room = RoomManager.GetRoom(c.player.roomId);
        if(room == null)
        {
            Console.WriteLine("MsgTest Room is null with id: " + c.player.roomId);
            return;
        }
        room.zombieManager.Generate(room, Block.GetStandPos(msg.corner));
    }
}