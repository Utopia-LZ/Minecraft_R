[System.Serializable]
public struct DroppedInfo
{
    public int id;
    public int roomId;
    public BlockType type;
    public Vector3Int position;
    public int count;
}
public class DroppedItem
{
    public int id;
    public int roomId;
    public BlockType type;
    public Vector3Int position;
    public int count;
}