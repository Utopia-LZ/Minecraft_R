using System;
using System.Collections.Generic;

public class PlayerManager
{
    //玩家列表
    public static Dictionary<string, Player> players = new Dictionary<string, Player>();
    //玩家是否在线
    public static bool IsOnline(string id)
    {
        return players.ContainsKey(id);
    }
    //获取玩家
    public static Player GetPlayer(string id)
    {
        return players[id];
    }
    //添加玩家
    public static void AddPlayer(string id, Player player)
    {
        players.Add(id, player);
    }
    //删除玩家
    public static void RemovePlayer(string id)
    {
        players.Remove(id);
    }

    public static Vector3Int GetCenterPosition(int roomId)
    {
        Vector3Int result = Vector3Int.Zero;
        int count = 0;
        foreach(var player in players.Values)
        {
            if(player.roomId == roomId)
            {
                result += player.pos;
                count++;
            }
        }
        return result / count;
    }

    public static void BombExplode(Vector3Int pos, Room room)
    {
        foreach(var player in players.Values)
        {
            if(player.roomId != room.id) continue;
            Vector3Int position = new Vector3Int(player.pos.x, player.pos.y, player.pos.z);
            if ((position - pos).Magnitude > Bomb.radius * Bomb.radius) continue;
            player.hp -= Bomb.damage;
            MsgHit msg = new MsgHit();
            msg.id = player.id;
            msg.damage = Bomb.damage;
            room.Broadcast(msg);
        }
    }
}


