public class Block
{
    public MapManager mapManager;

    //public static Vector3Int offset = new Vector3Int(50,50,100);
    public static Vector3Int[] edgeOffset = new Vector3Int[5]
    {
        Vector3Int.Back,
        Vector3Int.Front,
        Vector3Int.Left,
        Vector3Int.Right,
        Vector3Int.Zero,
    };

    public Vector3Int position;
    public BlockType type;
    public bool canStand = false;
    public bool canJump = false;

    public int GCost; // 从起点到当前节点的代价
    public int HCost; // 从当前节点到目标节点的启发式代价
    public int FCost => GCost + HCost; // 总代价
    public Block Parent; // 父节点

    public Block(Vector3Int position, MapManager mapManager)
    {
        this.position = position;
        this.mapManager = mapManager;
    }

    public Block[] Edge = new Block[4]; //back front left right

    public void UpdateState()
    {
        if(type == BlockType.None)
        {
            canStand = canJump = false;
            return;
        }
        if (mapManager.GetBlockType(position + Vector3Int.Up) != BlockType.None ||
           mapManager.GetBlockType(position + Vector3Int.Up * 2) != BlockType.None)
            canStand = false;
        else canStand = true;

        canJump = canStand && (mapManager.GetBlockType(position + Vector3Int.Up * 3) == BlockType.None);
    }

    public void UpdateEdge()
    {
        for (int i = 0; i < 4; i++)
            UpdateEdgeAt(i);
    }

    public void UpdateEdgeAt(int dir)
    {
        Vector3Int edgePos = position + edgeOffset[dir];
        for(int i = 2; i >= -1; i--)
        {
            Vector3Int tmpPos = edgePos + Vector3Int.Up*i;
            Block tmp = mapManager.GetBlock(tmpPos);
            if(tmp == null) continue;
            if (tmp.type == BlockType.None) continue;
            if (!tmp.canStand) continue;
            if (i == 2) { Edge[dir] = null; return; }
            if(i == 0 || i == 1 && canJump || i == -1 && tmp.canJump)
            {
                Edge[dir] = tmp;
                //Console.WriteLine(position.ToString() + " " + tmp.position.ToString() + " dir: " + dir);
                return;
            }
        }
        Edge[dir] = null;
    }

    /// <summary>
    /// Judge near edge
    /// </summary>
    /// <param name="dir">[0-3] back front left right</param>
    public bool CanGoto(int dir)
    {
        if (Edge[dir] == null || !Edge[dir].canStand) return false;
        return true;
    }

    //通过落脚点获取脚下方块的坐标(/100)
    public static Vector3Int GetCornerPos(Vector3Int standPos)
    {
        return Vector3Int.Float2Int(standPos-Vector3Int.Up*200);
    }
    //通过方块坐标获取落脚点坐标(*100)
    public static Vector3Int GetStandPos(Vector3Int pos)
    {
        return pos*100 + new Vector3Int(50, 200, 50);
    }

    public Block GetEdge(int dir)
    {
        if (CanGoto(dir)) return Edge[dir];
        return null;
    }

    public Vector3Int RandormGetEdge()
    {
        int count = 0;
        Random rand = new Random();
        while (true)
        {
            int rdm = rand.Next(0, 4);
            if(CanGoto(rdm))
            {
                return Edge[rdm].position;
            }
            count++;
            if (count > 100)
            {
                Console.WriteLine("Too Hard to find a way");
                return Vector3Int.Zero;
            }
        }
    }
}