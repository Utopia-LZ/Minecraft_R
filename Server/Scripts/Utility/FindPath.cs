public class FindPath
{
    // A*寻路算法
    public static List<Vector3Int> AStar(Vector3Int start, Vector3Int target, Room room)
    {
        Console.WriteLine("Start Search");
        // 创建开放列表和关闭列表
        PriorityQueue<Block, int> openList = new();
        HashSet<Vector3Int> openHash = new HashSet<Vector3Int>();
        HashSet<Vector3Int> closedHash = new HashSet<Vector3Int>();

        Block startBlock = room.mapManager.GetBlock(start);
        Block targetBlock = room.mapManager.GetBlock(target);
        startBlock.Parent = null;

        openList.Enqueue(startBlock,int.MaxValue);
        openHash.Add(startBlock.position);

        while (openList.Count > 0)
        {
            // 获取F值最小的节点
            Block currentNode = openList.Dequeue();
            openHash.Remove(currentNode.position);
            closedHash.Add(currentNode.position);

            // 如果找到目标，构建路径
            if (currentNode.position == targetBlock.position)
            {
                Console.WriteLine("I'd better find one");
                List<Vector3Int> path = new List<Vector3Int>();
                Block pathNode = currentNode;
                while (pathNode != null)
                {
                    Console.WriteLine(pathNode.position.ToString());
                    path.Add(pathNode.position);
                    pathNode = pathNode.Parent;
                }
                Console.WriteLine("Assemble success");
                return path;
            }

            // 检查周围的节点
            for(int i = 0; i < 4; i++)
            {
                Block nbBlock = currentNode.GetEdge(i);

                // 确保邻居节点在网格内并且不是障碍
                if (nbBlock != null && !closedHash.Contains(nbBlock.position))
                {
                    int tentativeGCost = currentNode.GCost + 1;

                    // 如果邻居节点不在开放列表中，或者找到了更短的路径
                    if (!openHash.Contains(nbBlock.position))
                    {
                        nbBlock = room.mapManager.GetBlock(nbBlock.position);
                        nbBlock.GCost = tentativeGCost;
                        nbBlock.HCost = GetManhattanDistance(nbBlock.position, targetBlock.position);
                        nbBlock.Parent = currentNode;
                        openList.Enqueue(nbBlock,nbBlock.FCost);
                        openHash.Add(nbBlock.position);
                        //Console.WriteLine("Add in OpenHash: " + nbBlock.position.ToString());
                    }
                    else if (tentativeGCost < nbBlock.GCost)
                    {
                        nbBlock.GCost = tentativeGCost;
                        nbBlock.HCost = GetManhattanDistance(nbBlock.position, targetBlock.position);
                        nbBlock.Parent = currentNode;
                    }
                }
            }
        }

        Console.WriteLine("Can't find any way");
        return new List<Vector3Int>(); // 如果没有路径
    }

    // 获取曼哈顿距离
    public static int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
    }
}
