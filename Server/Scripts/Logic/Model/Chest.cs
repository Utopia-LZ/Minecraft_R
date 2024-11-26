public class Chest : Entity
{
    public Slot[] slots;

    public Chest()
    {
        slots = new Slot[ChestManager.CHEST_SLOT_COUNT];
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].item = new();
            slots[i].item.type = BlockType.None;
            slots[i].idx = (ushort)i;
        }
    }
}