using System;

class MainClass
{
    public static void Main(string[] args)
    {
        if (!DBManager.Connect("minecraft", "127.0.0.1", 3306, "root", "862714"))
        {
            return;
        }
        NetManager.StartLoop(8888);
    }
}