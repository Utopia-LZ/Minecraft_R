public class PlayerManager
{
    //玩家列表
    public Dictionary<string, Player> players = new Dictionary<string, Player>();

    public void Update(int roomId)
    {
        foreach(var player in players.Values)
        {
            if(player != null && player.roomId == roomId)
                player.Update();
        }
    }

    //玩家是否在线
    public bool IsOnline(string id)
    {
        return players.ContainsKey(id);
    }
    //获取玩家
    public Player GetPlayer(string id)
    {
        if(players.ContainsKey(id))
            return players[id];
        return null;
    }
    //添加玩家
    public void AddPlayer(Player player)
    {
        players[player.id] = player;
    }
    //删除玩家
    public void RemovePlayer(string id)
    {
        players.Remove(id);
    }

    public Vector3Int GetCenterPosition(int roomId)
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

    public void BombExplode(Vector3Int pos, Room room)
    {
        foreach(var player in players.Values)
        {
            if(player.roomId != room.id) continue;
            Vector3Int position = new Vector3Int(player.pos.x, player.pos.y, player.pos.z);
            int damge = (int)Math.Min(Bomb.damage - Math.Sqrt((position - pos).Magnitude) * Bomb.falloff, 0);
            if (damge == 0) continue;
            player.hp -= Bomb.damage;
            MsgHit msg = new MsgHit();
            msg.id = player.id;
            msg.damage = Bomb.damage;
            room.Broadcast(msg);
        }
    }
}


