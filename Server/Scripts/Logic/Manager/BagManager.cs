[System.Serializable]
public struct ItemInfo
{
    public int id;
    public BlockType type;
    public int count;
}

public static class BagManager
{
    public static int MAX_ITEM_COUNT = 64;
    public static int SLOT_COUNT = 45;

    public static void InitConfig(Config config)
    {
        MAX_ITEM_COUNT = config.SlotMaxItemCount;
        SLOT_COUNT = config.SlotCount;
    }

    public static void LoadBagItem(Player player)
    {
        Slot[] slots = player.data.slots;
        MsgLoadBag msg = new();
        msg.slots = slots;
        player.Send(msg);
    }
}