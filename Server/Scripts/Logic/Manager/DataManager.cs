using Newtonsoft.Json;

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