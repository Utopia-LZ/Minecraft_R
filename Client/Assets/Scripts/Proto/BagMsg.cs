[System.Serializable]
public struct Slot
{
    public ushort idx;
    public ItemInfo item;
}

public enum ItemPanelType
{
    Inventory, Bag, Chest
}

public class MsgAddItem : MsgBase
{
    public MsgAddItem() { protoName = "MsgAddItem"; }

    //客户端发
    public Slot slot;
    public ItemPanelType panelType; //面板位置
    public int idx = -1; //箱子编号
    //服务端回
    public int result = 1; //:异常情况(未添加)
}

public class MsgRemoveItem : MsgBase
{
    public MsgRemoveItem() { protoName = "MsgRemoveItem"; }

    //客户端发
    public Slot slot;
    public ItemPanelType panelType; //面板位置
    public int idx = -1; //箱子编号
    //服务端回
    public int result = 1; //:异常情况(未添加)
}

public class MsgLoadBag : MsgBase
{
    public MsgLoadBag() { protoName = "MsgLoadBag"; }

    public Slot[] slots;
}

public class MsgDropItem : MsgBase
{
    public MsgDropItem() { protoName = "MsgDropItem"; }

    //Client
    public ItemInfo info;
    public Vector3Int pos;
    public Vector3Int dir;
    public bool locked = false;
    //Server
    public int id = -1;
}

public class MsgDestroyItem : MsgBase
{
    public MsgDestroyItem() { protoName = "MsgDestroyItem"; }

    //Clinet
    public int idx = -1;
    public bool pickedup = true;
    //Server
    public string id = "";
}