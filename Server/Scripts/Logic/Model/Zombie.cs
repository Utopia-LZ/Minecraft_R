public class Zombie
{
    public StateMachine sm;
    public Player ChasedPlayer;
    public static int HP;
    public static int frameCount = 20; //插帧
    public static int attackDisSquared = 2 * 1 * 100 * 100;
    public static int chaseDisSquared = 10 * 10 * 100 * 100;

    //id
    public string id = "";
    //坐标和旋转
    public Vector3Int pos;
    public Vector3Int rot;
    //在哪个房间
    public int roomId;
    //生命值
    public int hp = 10;
    //生物种类
    public Kind Kind = Kind.None;

    //数据库数据
    public PlayerData data;

    public Vector3Int Position
    {
        get { return pos / 100; }
        set { pos = value * 100; }
    }

    public Zombie(Vector3Int pos, int roomId)
    {
        this.pos = pos;
        this.roomId = roomId;
        sm = new(this);
    }

    public static void InitConfig(Config config)
    {
        HP = config.ZombieHp;
        frameCount = config.FrameCount;
        attackDisSquared = config.ZombieAttackDis * config.ZombieAttackDis * 100 * 100;
        chaseDisSquared = config.ZombieChaseDis * config.ZombieChaseDis * 100 * 100;
    }

    public void Update(Room room)
    {
        //发送同步协议
        MsgSyncZombie msg = new();
        msg.id = id;
        msg.pos = pos;
        msg.forward = rot;
        room.Broadcast(msg);

        sm.Update();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Room room = RoomManager.GetRoom(roomId);
        MsgUpdateZombie msg = new MsgUpdateZombie();
        msg.generate = false;
        msg.info = new CharacterInfo();
        msg.info.id = id;
        room.Broadcast(msg);
        MsgDropItem msgD = new();
        msgD.pos = pos;
        msgD.dir = Vector3Int.Zero;
        msgD.id = ItemManager.index++;
        msgD.info = new ItemInfo()
        {
            type = BlockType.Carrion,
            count = new Random().Next(1, 3)
        };
        msgD.locked = false;
        room.Broadcast(msgD);
        DroppedItem droppedItem = new();
        droppedItem.position = msgD.pos;
        droppedItem.id = msgD.id;
        ItemManager.AddItem(droppedItem);
        room.zombieManager.Remove(id);
    }
}