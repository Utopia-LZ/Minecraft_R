using System.Timers;

public class Room
{
    public static int TICK;
    public static int CYCLE;
    public static int DAY_TIME;
    public static int SAVE_INTERVAL;

    //id
    public int id = 0;
    //最大玩家数
    public int maxPlayer = 6;
    //房主id
    public string ownerId = "";
    //暂停
    public bool paused = true;

    public System.Timers.Timer timer;
    private Random rand;
    private int counter = 0;
    public int Time;
    private int saveCounter = 0;

    public ChatManager chatManager;
    public MapManager mapManager;
    public ZombieManager zombieManager;
    public PlayerManager playerManager;

    public Room()
    {
        chatManager = new ChatManager();
        mapManager = new MapManager();
        zombieManager = new ZombieManager(this);
        playerManager = new PlayerManager();
        Time = 0;
        rand = new Random();
        timer = new System.Timers.Timer(TICK);
        timer.Elapsed += Update;
        timer.AutoReset = true;
        timer.Enabled = true;
        timer.Start();
    }
    
    public Room(int id, string cdata, List<string> mdata, string zdata, string pdata)
    {
        this.id = id;
        chatManager = new ChatManager(cdata);
        mapManager = new MapManager(mdata);
        zombieManager = new ZombieManager(this,zdata);
        playerManager = new PlayerManager(pdata);
        Time = 0;
        rand = new Random();
        timer = new System.Timers.Timer(TICK);
        timer.Elapsed += Update;
        timer.AutoReset = true;
        timer.Enabled = true;
        timer.Start();
    }

    public static void InitConfig(Config config)
    {
        TICK = config.TickTime;
        CYCLE = config.CycleTime;
        DAY_TIME = CYCLE / 2;
        SAVE_INTERVAL = config.SaveInterval;
    }

    private void Update(object? sender, ElapsedEventArgs e)
    {
        if (paused || playerManager.onlineCount==0) return;

        if(Time % DAY_TIME == 0) //昼夜交替同步一次
        {
            MsgTime msg = new MsgTime();
            msg.Time = Time;
            Broadcast(msg);
        }
        Time++;
        if (Time >= CYCLE) Time = 0;
//Console.WriteLine("E");
        zombieManager.Update();
    //Console.WriteLine("F");
        playerManager.Update(id);
        saveCounter++;
        if(saveCounter > SAVE_INTERVAL)
        {
            return;
            saveCounter = 0;
            string cData = chatManager.Serialize();
            List<ChunkInfo> mData = mapManager.Serialize();
            string zData = zombieManager.Serialize();
            string pData = playerManager.Serialize();
            DBManager.SaveRoomData(id, cData, mData, zData, pData);
            Console.WriteLine("Room Auto Save " + id);
        }
    }

    public void AddRoomData()
    {
        string cData = chatManager.Serialize();
        List<ChunkInfo> mData = mapManager.Serialize();
        string zData = zombieManager.Serialize();
        string pData = playerManager.Serialize();
        DBManager.AddRoomData(id, cData, mData, zData, pData);
    }

    //添加玩家
    public bool AddPlayer(Player player)
    {
        //房间人数
        if (playerManager.onlineCount >= maxPlayer)
        {
            Console.WriteLine("room.AddPlayer fail, reach maxPlayer");
            return false;
        }
        if(playerManager.onlineCount == 0)
        {
            paused = false;
        }
        else if (playerManager.players.ContainsKey(player.id) && playerManager.players[player.id].roomId!=-1) //已经在房间里
        {
            Console.WriteLine("room.AddPlayer fail, already in this room");
            return false;
        }
        //加入列表
        playerManager.AddPlayer(player);
        //Console.WriteLine("Room.AddPlayer pos: " + player.pos.ToString());
        player.roomId = id;
        //Console.WriteLine("PlayerRoomId: " + player.roomId + " ListPRoomId: " + playerManager.players[player.id].roomId);
        //广播
        Broadcast(ToMsg());
        return true;
    }

    //删除玩家
    public bool RemovePlayer(string id)
    {
        //获取玩家
        Player player = playerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("room.RemovePlayer fail, player is null");
            return false;
        }
        //没有在房间里
        if (!playerManager.players.ContainsKey(id) || playerManager.players[id].roomId==-1)
        {
            Console.WriteLine("room.RemovePlayer fail, not in this room");
            return false;
        }
        //删除列表
        playerManager.RemovePlayer(player);
        Console.WriteLine("RemovePlayer: " + id);
        //房间为空
        if (playerManager.onlineCount == 0)
        {
            paused = true;
        }
        return true;
    }

    //选择房主
    public string SwitchOwner()
    {
        //选择第一个玩家
        foreach (string id in playerManager.players.Keys)
        {
            return id;
        }
        //房间没人
        return "";
    }


    //广播消息
    public void Broadcast(MsgBase msg)
    {
        foreach (Player player in playerManager.players.Values)
        {
            player.Send(msg);
        }
    }

    //生成MsgGetRoomInfo协议
    public MsgBase ToMsg()
    {
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        int count = playerManager.players.Count;
        msg.players = new PlayerInfo[count];
        //players
        int i = 0;
        Console.WriteLine("PlayerCount: " + count);
        foreach (Player player in playerManager.players.Values)
        {
            if (player.roomId == -1) continue;
            Console.WriteLine("AddPlayerToMsg");
            PlayerInfo playerInfo = new PlayerInfo();
            //赋值
            playerInfo = player.GetInfo();
            msg.players[i] = playerInfo;
            i++;
        }
        return msg;
    }

    //能否开战
    public bool CanStartBattle()
    {
        return true;
    }

    //初始化位置
    private void SetBirthPos(Player player)
    {
        player.pos = Vector3Int.Float2Vector(5, 13, 5);
        player.rot = Vector3Int.Zero;
    }

    //玩家数据转成CharacterInfo
    public CharacterInfo PlayerToCharInfo(Player player)
    {
        CharacterInfo info = new CharacterInfo();
        //info.camp = player.camp;
        info.id = player.id;
        info.hp = player.hp;

        info.pos = player.pos;
        info.rot = player.rot;

        return info;
    }

    //重置玩家战斗属性
    private void ResetPlayers()
    {
        //位置和旋转
        foreach (Player player in playerManager.players.Values)
        {
            player.hp = Player.HP;
            SetBirthPos(player);
        }
    }

    //进入房间
    public bool StartBattle(MsgEnterRoom msg, Player pe)
    {
        //玩家战斗属性
        //ResetPlayers();
        //返回数据
        msg.mapId = 1;
        msg.characters = new CharacterInfo[playerManager.players.Count];
        int i = 0;
        foreach(Player player in playerManager.players.Values)
        {
            if (player.roomId == -1) continue;
            msg.characters[i++] = PlayerToCharInfo(player);
        }
        pe.roomId = msg.id;
        Broadcast(msg);

        MsgEntityInit msgE = new MsgEntityInit();
        msgE.roomId = id;
        List<Entity> entities =
        [
            .. ChestManager.GetEntityInRoom(id),
            .. BombManager.GetEntityInRoom(id),
            .. LightManager.GetEntityInRoom(id),
        ];

        if (entities.Count > 0)
        {
            msgE.entities = new Entity[entities.Count];
            int j = 0;
            foreach (Entity entity in entities)
            {
                msgE.entities[j++] = entity;
                Console.WriteLine("Entity: " + entity.type.ToString());
            }
            pe.Send(msgE);
        }
        else Console.WriteLine("No Entity can be send");

        BagManager.LoadBagItem(pe);
        ItemManager.LoadDroppedItems(pe);
        zombieManager.LoadZombies(pe);
        chatManager.LoadChatText(pe);

        MsgTime msgT = new MsgTime();
        msgT.Time = Time;
        pe.Send(msgT);

        ChestManager.LoadChestContent(pe);
        Console.WriteLine("Finish StartBattle");
        return true;
    }
}

