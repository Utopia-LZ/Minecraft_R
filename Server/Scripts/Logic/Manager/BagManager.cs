[System.Serializable]
public struct ItemInfo
{
    public int id;
    public BlockType type;
    public int count;
}

public static class BagManager
{
    public static readonly int MAX_ITEM_COUNT = 64;
    public static readonly int SLOT_COUNT = 45;

    public static void LoadBagItem(Player player)
    {
        Slot[] slots = player.data.slots;
        MsgLoadBag msg = new();
        msg.slots = slots;
        player.Send(msg);
    }
}