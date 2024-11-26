public class MsgMapInit : MsgBase
{
    public MsgMapInit() { protoName = "MsgMapInit"; }

    //客户端发
    public int chunkId;
    public Vector3Int chunkPos;

    //服务端回
    public BlockType[] map;
}

public class MsgMapChange : MsgBase
{
    public MsgMapChange() { protoName = "MsgMapChange"; }

    //客户端发
    public int chunkId;
    public Vector3Int chunkPos;
    public Vector3Int blockPos;
    public BlockType type;

    //服务端回
    public string id = ""; //Player id
}