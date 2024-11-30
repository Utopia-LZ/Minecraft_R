using Newtonsoft.Json;

public class Config
{
    public int ChunkWidth; //区块宽度
    public int ChunkHeight; //区块高度
    public int ChunkBaseHeight; //地面高度
    public float Frequency; //噪声频率
    public float Amplitude; //噪声振幅
    public int TickTime; //时刻(ms)
    public int CycleTime; //一天时长(时刻)
    public int ZombieHp; //僵尸血量
    public int ZombieDamage; //僵尸攻击力
    public int ZombieAttackDis; //僵尸攻击距离
    public int ZombieChaseDis; //僵尸追击距离
    public int ZombieRefreshMinDis; //僵尸刷新最近距离
    public int ZombieRefreshMaxDis; //僵尸刷新最远距离
    public int GenerateInterval; //僵尸尝试刷新间隔(tick)
    public int ZombieAttackInterval; //僵尸攻击间隔(tick)
    public int FrameCount; //僵尸移动单位距离划分帧数
    public int RecoverHunger; //腐肉恢复饥饿值
    public int RecoverSaturation; //腐肉恢复饱食度
    public int PlayerHp; //玩家血量
    public float WalkSpeed; //玩家行走速度
    public float RunSpeed; //玩家跑步速度
    public float RotateSpeed; //玩家转身速度
    public int PlayerDamage; //玩家攻击力
    public float JumpForce; //玩家跳跃力量
    public int Saturation; //玩家饱食度
    public int HungerInterval; //玩家饥饿间隔
    public int Hunger; //玩家饥饿值
    public int StarveDamage; //玩家空腹掉血值
    public int Full; //玩家饱腹值(大于该值回血)
    public int CureCost; //玩家回血消耗饱食度
    public int CureInterval; //玩家回血间隔(tick)
    public float ThirdSpeed; //第三人称视角转动速度
    public float FollowSpeed; //相机跟随速度
    public float ZoomSpeed; //调整视野速度
    public float CameraRadius; //默认相机距离
    public float CameraMinRadius; //最近相机距离
    public float CameraMaxRadius; //最远相机距离
    public float ActInterval; //交互时间间隔(s)
    public int ActDistance; //最远交互距离
    public float SyncInterval; //位置同步频率(s)
    public int SlotMaxItemCount; //物品最大堆叠数
    public int SlotCount; //背包格子数(含物品栏)
    public int ChestSlotCount; //箱子格子数
    public float LockTime; //掉落物冷却时间(s)
    public int LightRadius; //灯发光半径
    public int BombBurnTime; //TNT引信时间
    public int BombDamage; //TNT爆炸伤害(中心)
    public int BombRadius; //TNT爆炸半径
    public int BombFalloff; //TNT伤害衰减
    public string LocalSrcUrl; //本地资源地址
    public string ServerSrcUrl; //服务器资源地址
    public long PingInterval; //Ping间隔
}

public class DataManager
{
    public static string configPath = "C:\\Users\\33572\\Desktop\\Minecraft_R\\Server\\Data\\Config.txt";
    public static Config Config;

    public static void Init()
    {
        string json = File.ReadAllText(configPath);
        if (json == null || json == "") return;
        Config = JsonConvert.DeserializeObject<Config>(json);
        BagManager.InitConfig(Config);
        ChestManager.InitConfig(Config);
        BombManager.InitConfig(Config);
        Chunk.InitConfig(Config);
        Light.InitConfig(Config);
        Player.InitConfig(Config);
        Room.InitConfig(Config);
        Zombie.InitConfig(Config);
        NetManager.InitConfig(Config);
        Console.WriteLine("[DataManager] 配置表数据初始化成功");
    }
}