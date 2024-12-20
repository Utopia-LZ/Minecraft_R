﻿using System;

public partial class MsgHandler
{

    //查询战绩
    public static void MsgGetAchieve(ClientState c, MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        Player player = c.player;
        if (player == null) return;

        player.Send(msg);
    }


    //请求房间列表
    public static void MsgGetRoomList(ClientState c, MsgBase msgBase)
    {
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        Player player = c.player;
        if (player == null) return;

        player.Send(RoomManager.ToMsg());
    }

    //创建房间
    public static void MsgCreateRoom(ClientState c, MsgBase msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        Player player = c.player;
        if (player == null) return;
        //已经在房间里
        if (player.roomId != -1)
        {
            msg.result = 1;
            player.Send(msg);
            Console.WriteLine("CreateRoom Fail, roomId != -1");
            return;
        }
        //创建
        Room room = RoomManager.AddRoom();

        msg.result = 0;
        msg.id = room.id;
        player.Send(msg);
        Console.WriteLine("CreateRoom Success");
    }

    //进入房间 直接开始战斗
    public static void MsgEnterRoom(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("OnEnterRoom");
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        Player player = c.player;
        if (player == null) return;
        Console.WriteLine("MsgEnterRoom Plyaer HP: " + player.hp);
        //获取房间
        Room room = RoomManager.GetRoom(msg.id);
        if (room == null)
        {
            msg.result = 1;
            player.Send(msg);
            Console.WriteLine("Room is null");
            return;
        }
        //进入
        if (!room.AddPlayer(player))
        {
            Console.WriteLine("AddPlayer Failed");
            msg.result = 1;
            player.Send(msg);
            return;
        }
        Console.WriteLine("MsgEnterRoom PlayerRoomID: " + c.player.roomId);
        //进入房间
        room.StartBattle(msg, player);
    }

    //获取房间信息
    public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        Player player = c.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            player.Send(msg);
            return;
        }

        player.Send(room.ToMsg());
    }

    //离开房间
    public static void MsgLeaveRoom(ClientState c, MsgBase msgBase)
    {
        Console.WriteLine("MsgLeaveRoom");
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        Player player = c.player;
        if (player == null) return;

        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            Console.WriteLine("Room is null with id: " + c.player.roomId);
            player.Send(msg);
            return;
        }
        msg.id = player.id;
        //先广播，后移除
        room.Broadcast(msg);
        room.RemovePlayer(player.id);
    }
}