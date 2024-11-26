//ʵ������/�ݻ�
using static UnityEngine.EventSystems.EventTrigger;

public class MsgUpdateEntity : MsgBase
{
    public MsgUpdateEntity() { protoName = "MsgUpdateEntity"; }

    //Client
    public bool generate = true; //true:���� false:����
    public Vector3Int corner;
    public BlockType type;
    //Server
    public int id = -1;
}

public class MsgEntityInit : MsgBase
{
    public MsgEntityInit() { protoName = "MsgEntityInit"; }

    public int roomId = -1;
    public Entity[] entities;
}

public class MsgLoadDropped : MsgBase
{
    public MsgLoadDropped() { protoName = "MsgLoadDropped"; }

    public DroppedInfo[] items;
}

public class MsgBombState : MsgBase
{
    public MsgBombState() { protoName = "MsgBombState"; }

    public int id = -1;
    public BombState state;
}

public class MsgLoadChestContent : MsgBase
{
    public MsgLoadChestContent() { protoName = "MsgLoadChestContent"; }

    public int chestId;
    public Slot[] slots;
}