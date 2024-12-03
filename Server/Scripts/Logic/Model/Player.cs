using System.Numerics;

[System.Serializable]
public class PlayerInfo
{
    public string id = "";
    public int hp;
    public int hunger;
    public int saturation;
    public Vector3Int pos;
    public Vector3Int rot;
    public int roomId;
}

public class Player
{
    public static int HP;
    public static int Hunger;
    public static int Full;
    public static int CureCost;
    public static int CureInterval;
    public static int Saturation;
    public static int HungerInterval;
    public static int StarveDamage;
    public static int safeDistance;
    public static int ignoreDistance; //(block scale)

    public static void InitConfig(Config config)
    {
        HP = config.PlayerHp;
        Hunger = config.Hunger;
        Full = config.Full;
        CureCost = config.CureCost;
        CureInterval = config.CureInterval;
        Saturation = config.Saturation;
        HungerInterval = config.HungerInterval;
        StarveDamage = config.StarveDamage;
        safeDistance = config.ZombieRefreshMinDis;
        ignoreDistance = config.ZombieRefreshMaxDis;
    }

    //id
    public string id = "";
    //指向ClientState
    public ClientState state;
    //构造函数
    public Player(ClientState state)
    {
        this.state = state;
        hp = HP;
        hunger = Hunger;
        saturation = Saturation;
        pos = Vector3Int.Float2Vector(5, 13, 5);
        rot = Vector3Int.Zero;
    }
    public Player(PlayerInfo info)
    {
        id = info.id;
        hp = info.hp;
        hunger = info.hunger;
        saturation = info.saturation;
        pos = info.pos;
        rot = info.rot;
        roomId = info.roomId;
    }

    //坐标和旋转
    public Vector3Int pos;
    public Vector3Int rot;
    //在哪个房间
    public int roomId = -1;
    //玩家生命值
    public int hp;
    //玩家饥饿值
    public int hunger;
    public int saturation;
    public int cureTimer = 0;
    //生物种类
    public Kind Kind = Kind.None;

    //数据库数据
    public PlayerData data;

    public void Update()
    {
        if (saturation < 0)
        {
            saturation += HungerInterval;
            if(hunger == 0)
            {
                MsgHit msg = new();
                msg.id = id;
                msg.damage = StarveDamage;
                Send(msg);
                TakeDamage(StarveDamage);
            }
            else
            {
                hunger--;
                MsgHungry msg = new MsgHungry();
                msg.id = id;
                Send(msg);
            }
        }
        else
        {
            saturation--;
            //Console.WriteLine("sat:" + saturation + " hunger:" + hunger + " hp:" + hp);
            if(saturation > Full && hunger >= Hunger && hp < HP)
            {
                cureTimer++;
                if(cureTimer > CureInterval)
                {
                    cureTimer = 0;
                    saturation -= CureCost;
                    MsgHit msg = new();
                    msg.id = id;
                    msg.damage = -1;
                    Send(msg);
                    TakeDamage(-1);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if(hp <= 0) return;
        hp -= damage;
        if(hp <= 0)
        {
            Room room = RoomManager.GetRoom(roomId);
            Console.WriteLine("RoomId: " + roomId + " have? " + (room != null));
            DropBag(room);
            MsgLeaveRoom msg = new MsgLeaveRoom();
            msg.id = id;
            msg.reason = 1;
            room.Broadcast(msg);
            room.RemovePlayer(id);

            //Reset
            hp = HP;
            hunger = Hunger;
            saturation = Saturation;
        }
    }

    public void DropBag(Room room)
    {
        for(int i = 0; i < BagManager.SLOT_COUNT; i++)
        {
            if (data.slots[i].item.type == BlockType.None) continue;
            MsgDropItem msg = new MsgDropItem();
            msg.locked = true;
            msg.pos = pos;
            msg.info = data.slots[i].item;
            msg.id = ItemManager.index++;
            DroppedItem droppedItem = new();
            droppedItem.position = msg.pos;
            droppedItem.id = msg.id;
            ItemManager.AddItem(droppedItem);
            data.slots[i].item.type = BlockType.None;
            room.Broadcast(msg);
        }
    }

    //发送信息
    public void Send(MsgBase msgBase)
    {
        NetManager.Send(state, msgBase);
    }

    public PlayerInfo GetInfo()
    {
        return new PlayerInfo
        {
            id = id,
            hp = hp,
            hunger = hunger,
            saturation = saturation,
            pos = pos,
            rot = rot,
            roomId = roomId,
        };
    }
}