using System;
using System.Collections.Generic;

public class RoomManager
{
    //最大id
    private static int maxId = 0;
    //房间列表
    public static Dictionary<int, Room> Rooms = new Dictionary<int, Room>();

    //创建房间
    public static Room AddRoom()
    {
        maxId++;
        Room room = new Room();
        room.id = maxId;
        Rooms.Add(room.id, room);
        return room;
    }

    //删除房间
    public static bool RemoveRoom(int id)
    {
        Rooms.Remove(id);
        return true;
    }

    //获取房间
    public static Room GetRoom(int id)
    {
        if (Rooms.ContainsKey(id))
        {
            return Rooms[id];
        }
        return null;
    }

    //玩家掉线
    public static void RemovePlayer(Player player)
    {
        if (player.roomId == -1) return;
        Room room = Rooms[player.roomId];
        room?.RemovePlayer(player.id);
    }

    //生成MsgGetRoomList协议
    public static MsgBase ToMsg()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        int count = Rooms.Count;
        msg.rooms = new RoomInfo[count];
        //Rooms
        int i = 0;
        foreach (Room room in Rooms.Values)
        {
            RoomInfo roomInfo = new RoomInfo();
            //赋值
            roomInfo.id = room.id;
            roomInfo.count = room.playerIds.Count;

            msg.rooms[i] = roomInfo;
            i++;
        }
        return msg;
    }
}


