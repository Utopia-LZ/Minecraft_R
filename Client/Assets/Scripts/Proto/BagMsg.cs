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

    //�ͻ��˷�
    public Slot slot;
    public ItemPanelType panelType; //���λ��
    public int idx = -1; //���ӱ��
    //����˻�
    public int result = 1; //:�쳣���(δ���)
}

public class MsgRemoveItem : MsgBase
{
    public MsgRemoveItem() { protoName = "MsgRemoveItem"; }

    //�ͻ��˷�
    public Slot slot;
    public ItemPanelType panelType; //���λ��
    public int idx = -1; //���ӱ��
    //����˻�
    public int result = 1; //:�쳣���(δ���)
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