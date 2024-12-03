using Newtonsoft.Json;
using System.Text;

public class PlayerManager
{
    //玩家列表
    public Dictionary<string, Player> players = new Dictionary<string, Player>();
    public int onlineCount = 0;

    public PlayerManager() { }
    public PlayerManager(string data)
    {
        Deserialize(data);
    }

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
        return players.ContainsKey(id) && players[id].roomId != -1;
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
        if (players.ContainsKey(player.id))
        {
            player.pos = players[player.id].pos;
            player.rot = players[player.id].rot;
            player.hp = players[player.id].hp;
            player.hunger = players[player.id].hunger;
            player.saturation = players[player.id].saturation;
            Console.WriteLine("LoadSql player pos: " +  player.pos.ToString());
        }
        players[player.id] = player;
        onlineCount++;
    }
    //删除玩家
    public void RemovePlayer(Player player)
    {
        if (players.ContainsKey(player.id))
        {
            players[player.id].roomId = -1;
            onlineCount--;
        }
        player.roomId = -1;
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
            Vector3Int position = Vector3Int.Float2Int(player.pos);
            Console.WriteLine("PlyerPos: " + position.ToString() + " BombPos: " + pos.ToString());
            int damage = (int)Math.Max(Bomb.damage - Math.Sqrt((position - pos).Magnitude) * Bomb.falloff, 0);
            Console.WriteLine("Bomb Damage: " + damage + " toPlayer: " + player.id);
            if (damage == 0) continue;
            player.hp -= damage;
            MsgHit msg = new MsgHit();
            msg.id = player.id;
            msg.damage = damage;
            room.Broadcast(msg);
        }
    }

    public string Serialize()
    {
        StringBuilder sb = new StringBuilder();
        foreach(Player player in players.Values)
        {
            PlayerInfo info = player.GetInfo();
            info.roomId = -1; //默认玩家均离线
            sb.Append(JsonConvert.SerializeObject(info) + "|");
        }
        if(sb.Length > 0) sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    public void Deserialize(string data)
    {
        if (data == null || data == "") return;
        string[] parts = data.Split('|');
        for(int i = 0; i < parts.Length; i++)
        {
            Player player = new Player(JsonConvert.DeserializeObject<PlayerInfo>(parts[i]));
            players[player.id] = player;
        }
    }
}


