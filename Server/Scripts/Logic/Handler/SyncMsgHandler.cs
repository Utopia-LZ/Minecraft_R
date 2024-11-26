using System;


public partial class MsgHandler
{

    //同步位置协议
    public static void MsgSyncSteve(ClientState c, MsgBase msgBase)
    {
        MsgSyncSteve msg = (MsgSyncSteve)msgBase;
        Player player = c.player;
        if (player == null) return;
        //room
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }
        //是否作弊
        /*if (Math.Abs((player.pos-msg.pos).Magnitude) > 10000)
        {
            Console.WriteLine("疑似作弊 " + player.id);
            Console.WriteLine(player.pos.ToString() + " " + msg.pos.ToString());
        }*/
        //更新信息
        player.pos = msg.pos;
        player.rot = msg.rot;
        //广播
        msg.id = player.id;
        room.Broadcast(msg);
    }

    //开火协议
    /*public static void MsgFire(ClientState c, MsgBase msgBase)
    {
        MsgFire msg = (MsgFire)msgBase;
        Player player = c.player;
        if (player == null) return;
        //room
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            return;
        }
        //status
        if (room.status != Room.Status.FIGHT)
        {
            return;
        }
        //广播
        msg.id = player.id;
        room.Broadcast(msg);
    }*/
}


