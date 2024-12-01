using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

public class DBManager
{
    public static MySqlConnection mysql;

    public static bool Connect(string db, string ip, int port, string username, string password)
    {
        mysql = new MySqlConnection();
        mysql.ConnectionString =
            string.Format("Database={0};Data Source={1}; port={2};User Id={3}; Password={4}", db, ip, port, username, password);
        //连接
        try
        {
            mysql.Open();
            Console.WriteLine("[数据库]connect success, ");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库]connect fail, " + e.Message);
            return false;
        }
    }
    //判定安全字符
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
    }
    //用户是否存在
    public static bool IsAccountExist(string id)
    {
        //防sql注入
        if (!IsSafeString(id)) return false;

        string s = string.Format("select * from account where id='{0}';", id);
        try
        {
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return !hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("IsAccountExist Err, " + e.Message);
            return false;
        }
    }
    public static bool Register(string id, string pw)
    {
        //防sql注入
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] Register fail, id not safe");
            return false;
        }
        if (!IsSafeString(pw))
        {
            Console.WriteLine("[数据库] Register fail, pw not safe");
            return false;
        }
        //能否注册
        if (!IsAccountExist(id))
        {
            Console.WriteLine("[数据库] Register fail, id exist");
            return false;
        }

        string sql = string.Format("insert into account set id = '{0}', pw = '{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine("Register fail, " + e.Message);
            return false;
        }
    }
    public static bool CreatePlayer(string id)
    {
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] CreatePlayer fail, id not safe");
            return false;
        }
        //序列化
        PlayerData playerData = new PlayerData();
        string data = JsonConvert.SerializeObject(playerData);
        //写入数据库
        string sql = string.Format("insert into player set id ='{0}' ,data ='{1}';", id, data);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CreatePlayer err, " + e.Message);
            return false;
        }
    }
    public static bool CheckPassword(string id, string pw)
    {
        if (!IsSafeString(id) || !IsSafeString(pw))
        {
            Console.WriteLine("[数据库] CheckPassword fail, not safe");
            return false;
        }

        string sql = string.Format("select * from account where id='{0}' and pw='{1}';", id, pw);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return hasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] CheckPassword err, " + e.Message);
            return false;
        }
    }
    public static PlayerData GetPlayerData(string id)
    {
        if (!IsSafeString(id))
        {
            Console.WriteLine("[数据库] GetPlayerData fail, id not safe");
            return null;
        }

        string sql = string.Format("select * from player where id ='{0}';", id);
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return null;
            }
            //读取
            dataReader.Read();
            string data = dataReader.GetString("data");
            //反序列化
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(data);
                                  //DecodePlayerData(data);
            dataReader.Close();
            return playerData;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] GetPlayerData fail, " + e.Message);
            return null;
        }
    }
    public static bool UpdatePlayerData(string id, PlayerData playerData)
    {
        if (playerData == null) return false;
        //序列化
        string data = JsonConvert.SerializeObject(playerData);
                    //EncodePlayerData(playerData);
        //sql
        string sql = string.Format("update player set data='{0}' where id ='{1}';", data, id);
        //更新
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(data);
            Console.WriteLine("[数据库] UpdatePlayerData err, " + e.Message);
            return false;
        }
    }

    public static void AddRoomData(int roomId, string chatInfo, List<ChunkInfo> mapInfo, string zombieInfo, string playerInfo)
    {
        string sql = $"insert into room set roomId={roomId},chatInfo='{chatInfo}'," +
            $"zombieInfo='{zombieInfo}',playerInfo='{playerInfo}'";
        //更新
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] AddRoomData err, " + e.Message);
        }

        //地图信息单独存表
        if (mapInfo.Count == 0) return;
        StringBuilder sb = new StringBuilder();
        sb.Append("insert into map(roomId,chunkId,chunkInfo) values ");
        foreach(ChunkInfo chunk in mapInfo)
        {
            string data = JsonConvert.SerializeObject(chunk);
            sb.Append($"({roomId},'{chunk.pos.ToString()}','{data}'),");
        }
        sb.Remove(sb.Length - 1, 1);
        try
        {
            MySqlCommand cmd = new MySqlCommand(sb.ToString(), mysql);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] AddChunkData err, " + e.Message);
        }
    }
    public static Dictionary<int,Room> GetAllRoomData()
    {
        Dictionary<int, Room> rooms = new();
        try
        {
            Dictionary<int, List<string>> maps = new();
            using (MySqlCommand cmdC = new MySqlCommand("select * from map", mysql))
            {
                MySqlDataReader reader = cmdC.ExecuteReader();
                while (reader.Read())
                {
                    int roomId = reader.GetInt32("roomId");
                    if (!maps.ContainsKey(roomId)) maps[roomId] = new List<string>();
                    maps[roomId].Add(reader.GetString("chunkInfo"));
                }
                reader.Close();
            }

            //查询
            string sql = "select * from room";
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return rooms;
            }

            //读取
            while (dataReader.Read())
            {
                int id = dataReader.GetInt32(1);
                string cdata = dataReader.GetString(2);
                string zdata = dataReader.GetString(3);
                string pdata = dataReader.GetString(4);
                Room room = new Room(id, cdata, maps[id], zdata, pdata);
                rooms[id] = room;
            }
            dataReader.Close();
            return rooms;
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] GetRoomData fail, " + e.Message);
            return rooms;
        }
    }
    public static void SaveRoomData(int roomId, string chatInfo, List<ChunkInfo> mapInfo, string zombieInfo, string playerInfo)
    {
        string sql = $"update room set chatInfo='{chatInfo}'," +
            $"zombieInfo='{zombieInfo}',playerInfo='{playerInfo}' where roomId={roomId}";
        //更新
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] SaveRoomData err, " + e.Message);
        }

        //地图信息单独存表
        StringBuilder sb = new StringBuilder();
        sb.Append("INSERT INTO map (roomId, chunkId, chunkInfo) values ");
        foreach (ChunkInfo chunk in mapInfo)
        {
            string data = JsonConvert.SerializeObject(chunk);
            sb.Append($"({roomId},'{chunk.pos.ToString()}','{data}'),");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("ON DUPLICATE KEY UPDATE chunkInfo = VALUES(chunkInfo);");
        try
        {
            MySqlCommand cmd = new MySqlCommand(sb.ToString(), mysql);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] SaveChunkData err, " + e.Message);
        }
    }
    public static void RemoveRoomData(int roomId)
    {
        string sql = $"delete from room where roomId={roomId}";
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine("[数据库] DeleteRoomData err, " + e.Message);
        }
    }
}
