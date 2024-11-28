using System.Timers;

public class Room
{
    public static int TICK;
    public static int CYCLE;
    public static int DAY_TIME;

    //id
    public int id = 0;
    //最大玩家数
    public int maxPlayer = 6;
    //玩家列表
    public Dictionary<string, bool> playerIds = new Dictionary<string, bool>();
    //房主id
    public string ownerId = "";
    //暂停
    public bool paused = false;

    public System.Timers.Timer timer;
    private Random rand;
    private int counter = 0;
    public int Time;

    public ChatManager chatManager;
    public MapManager mapManager;
    public ZombieManager zombieManager;

    public Room()
    {
        chatManager = new ChatManager();
        mapManager = new MapManager();
        zombieManager = new ZombieManager(this);
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
    }

    private void Update(object? sender, ElapsedEventArgs e)
    {
        if (paused) return;
        if(Time % DAY_TIME == 0) //昼夜交替同步一次
        {
            MsgTime msg = new MsgTime();
            msg.Time = Time;
            Broadcast(msg);
        }
        Time++;
        if (Time >= CYCLE) Time = 0;

        zombieManager.Update();
    }

    //添加玩家
    public bool AddPlayer(string id)
    {
        //获取玩家
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("room.AddPlayer fail, player is null");
            return false;
        }
        //房间人数
        if (playerIds.Count >= maxPlayer)
        {
            Console.WriteLine("room.AddPlayer fail, reach maxPlayer");
            return false;
        }
        if(playerIds.Count == 0)
        {
            paused = false;
        }
        else if (playerIds.ContainsKey(id)) //已经在房间里
        {
            Console.WriteLine("room.AddPlayer fail, already in this room");
            return false;
        }
        //加入列表
        playerIds[id] = true;
        //设置玩家数据
        //player.camp = SwitchCamp();
        player.roomId = this.id;
        //设置房主
        if (ownerId == "")
        {
            ownerId = player.id;
        }
        //广播
        Broadcast(ToMsg());
        return true;
    }

    //是不是房主
    public bool IsOwner(Player player)
    {
        return player.id == ownerId;
    }

    //删除玩家
    public bool RemovePlayer(string id)
    {
        //获取玩家
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("room.RemovePlayer fail, player is null");
            return false;
        }
        //没有在房间里
        if (!playerIds.ContainsKey(id))
        {
            Console.WriteLine("room.RemovePlayer fail, not in this room");
            return false;
        }
        //删除列表
        playerIds.Remove(id);
        //设置玩家数据
        //player.camp = 0;
        player.roomId = -1; //:暂时表示在线状态 -1为离线
        //设置房主
        if (ownerId == player.id)
        {
            ownerId = SwitchOwner();
        }
        //房间为空
        if (playerIds.Count == 0)
        {
            paused = true;
        }
        return true;
    }

    //选择房主
    public string SwitchOwner()
    {
        //选择第一个玩家
        foreach (string id in playerIds.Keys)
        {
            return id;
        }
        //房间没人
        return "";
    }


    //广播消息
    public void Broadcast(MsgBase msg)
    {
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.Send(msg);
        }
    }

    //生成MsgGetRoomInfo协议
    public MsgBase ToMsg()
    {
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        int count = playerIds.Count;
        msg.players = new PlayerInfo[count];
        //players
        int i = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            PlayerInfo playerInfo = new PlayerInfo();
            //赋值
            playerInfo.id = player.id;
            playerInfo.isOwner = 0;
            if (IsOwner(player))
            {
                playerInfo.isOwner = 1;
            }

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
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            SetBirthPos(player);
        }
        //生命值
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            player.hp = 10;
        }
    }

    //进入房间
    public bool StartBattle(MsgEnterRoom msg, Player pe)
    {
        if (!CanStartBattle())
        {
            return false;
        }
        //玩家战斗属性
        ResetPlayers();
        //返回数据
        msg.mapId = 1;
        msg.characters = new CharacterInfo[playerIds.Count];

        int i = 0;
        foreach (string id in playerIds.Keys)
        {
            Player player = PlayerManager.GetPlayer(id);
            msg.characters[i] = PlayerToCharInfo(player);
            i++;
        }
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
        return true;
    }

    //是否死亡
    /*public bool IsDie(Player player)
    {
        return player.hp <= 0;
    }*/
}

