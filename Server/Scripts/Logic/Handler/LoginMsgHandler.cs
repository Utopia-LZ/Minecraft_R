﻿using System;


public partial class MsgHandler
{
    //注册协议处理
    public static void MsgRegister(ClientState c, MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister)msgBase;
        //注册
        if (DBManager.Register(msg.id, msg.pw))
        {
            DBManager.CreatePlayer(msg.id);
            msg.result = 0;
        }
        else
        {
            msg.result = 1;
        }
        NetManager.Send(c, msg);
    }


    //登陆协议处理
    public static void MsgLogin(ClientState c, MsgBase msgBase)
    {
        MsgLogin msg = (MsgLogin)msgBase;
        //密码校验
        if (!DBManager.CheckPassword(msg.id, msg.pw))
        {
            msg.result = 1;
            NetManager.Send(c, msg);
            return;
        }
        //不允许再次登陆
        if (c.player != null)
        {
            msg.result = 1;
            NetManager.Send(c, msg);
            Console.WriteLine("Have already login");
            return;
        }
        //如果已经登陆，踢下线
        /*if (c.player != null)
        {
            Room room = RoomManager.GetRoom(c.player.roomId);
            if (room.playerManager.IsOnline(msg.id))
            {
                //发送踢下线协议
                Player other = room.playerManager.GetPlayer(msg.id);
                MsgKick msgKick = new MsgKick();
                msgKick.reason = 0;
                other.Send(msgKick);
                //断开连接
                NetManager.Close(other.state);
            }
        }*/
        //获取玩家数据
        PlayerData playerData = DBManager.GetPlayerData(msg.id);
        //构建Player
        Player player = new Player(c);
        if (playerData == null)
        {
            msg.result = 1;
            player.data = new PlayerData();
            NetManager.Send(c, msg);
            return;
        }
        player.id = msg.id;
        player.data = playerData;
        c.player = player;
        //返回协议
        msg.result = 0;
        player.Send(msg);
    }
}
