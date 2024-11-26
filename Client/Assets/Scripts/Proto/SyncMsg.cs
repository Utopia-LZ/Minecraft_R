public class MsgSyncSteve : MsgBase
{
    public MsgSyncSteve() { protoName = "MsgSyncSteve"; }
    //位置，朝向
    public Vector3Int pos;
    public Vector3Int rot;
    //服务端补充
    public string id = "";
}

//同步僵尸信息
public class MsgSyncZombie : MsgBase
{
    public MsgSyncZombie() { protoName = "MsgSyncZombie"; }

    //位置、朝向
    public Vector3Int pos;
    public Vector3Int forward;
    //服务端补充
    public string id = "";		//哪个僵尸
}