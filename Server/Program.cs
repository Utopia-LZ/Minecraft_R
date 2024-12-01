using System;

class MainClass
{
    public static void Main(string[] args)
    {
        DataManager.Init();
        if (!DBManager.Connect("minecraft", "127.0.0.1", 3306, "root", "862714"))
        {
            return;
        }
        RoomManager.Init();
        NetManager.StartLoop(8888);
    }
}