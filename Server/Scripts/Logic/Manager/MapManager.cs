public class MapManager
{
    public Dictionary<Vector3Int, Chunk> chunks = new();
    public Dictionary<Vector3Int, Block> blocks = new();
    public int chunkIndex = 0;

    public void AddChunk(Chunk chunk)
    {
        chunks[chunk.position] = chunk;
    }

    public void RemoveChunk(Chunk chunk)
    {
        chunks.Remove(chunk.position);
    }

    public Chunk GetChunk(Vector3Int pos)
    {
        if(chunks.ContainsKey(pos))
            return chunks[pos];
        return null;
    }

    public void AddBlock(Block block)
    {
        blocks[block.position] = block;
    }

    public static BlockType[] To1(BlockType[,,] types)
    {
        BlockType[] result = new BlockType[Chunk.width*Chunk.height*Chunk.width];
        for (int i = 0; i < Chunk.width; i++)
            for (int j = 0; j < Chunk.height; j++)
                for (int k = 0; k < Chunk.width; k++)
                    result[i+j*Chunk.width*Chunk.width+k*Chunk.width] = types[i, j, k];
        return result;  
    }

    public void InitBlock(Chunk chunk)
    {
        HashSet<Vector3Int> updateBlocks = new();
        for (int i = 0; i < Chunk.width; i++)
            for (int j = 0; j < Chunk.height; j++)
                for (int k = 0; k < Chunk.width; k++)
                {
                    Vector3Int pos = chunk.position + new Vector3Int(i, j, k);
                    Block block = new Block(pos,this);
                    block.type = chunk.map[i, j, k];
                    AddBlock(block);
                    bool suc = false;
                    if (block.type != BlockType.None)
                        suc = updateBlocks.Add(block.position);
                }
        for(int i = 0; i < 4; i++)
        {
            Chunk nbChunk = GetChunk(chunk.position + Block.edgeOffset[i] * Chunk.width);
            if (nbChunk == null) continue;
            for(int j = 0; j < Chunk.width; j++)
                for(int k = 0; k < Chunk.height; k++)
                {
                    BlockType type = BlockType.None;
                    Vector3Int pos = Vector3Int.Zero;
                    switch (i)
                    {
                        case 0: type = nbChunk.map[j, k, Chunk.width-1]; pos = new Vector3Int(j, k, Chunk.width-1); break;
                        case 1: type = nbChunk.map[j, k, 0]; pos = new Vector3Int(j, k, 0); break;
                        case 2: type = nbChunk.map[Chunk.width-1, k, j]; pos = new Vector3Int(Chunk.width-1, k, j); break;
                        case 3: type = nbChunk.map[0, k, j]; pos = new Vector3Int(0, k, j); break;
                    }
                    if(type == BlockType.None) continue;
                    updateBlocks.Add(pos+nbChunk.position);
                }
        }
        foreach (var pos in updateBlocks)
        {
            Block block = GetBlock(pos);
            if (block == null) continue;
            if (block.type == BlockType.None) continue;
            block.UpdateState();
        }
        foreach(var pos in updateBlocks)
        {
            Block block = GetBlock(pos);
            if (block == null) continue;
            if (block.type == BlockType.None) continue;
            if(!block.canStand) continue;
            block.UpdateEdge();
        }
    }

    public void UpdateBlockState(Vector3Int pos)
    {
        Vector3Int tmpPos;
        for(int i = 0; i < 4; i++) //向下更新四格
        {
            tmpPos = pos + Vector3Int.Down * i;
            if (blocks.ContainsKey(tmpPos))
            {
                blocks[tmpPos].UpdateState();
                //Console.WriteLine("UpdateState: " + tmpPos.ToString() + "CanStand: " + blocks[tmpPos].canStand);
            }
        }
    }
    public void UpdateBlockEdge(Vector3Int pos)
    {
        if (!blocks.ContainsKey(pos)) return;
        Block tmpBlock = blocks[pos];
        if (tmpBlock.type != BlockType.None)
        {
            tmpBlock.UpdateEdge();
            //Console.WriteLine("UpdateSelf " + tmpBlock.position.ToString());
        }
        //Update under block when destroy block
        if (blocks.ContainsKey(pos + Vector3Int.Down))
        {
            tmpBlock = blocks[pos + Vector3Int.Down];
            if (tmpBlock.canStand)
            {
                tmpBlock.UpdateEdge();
                //Console.WriteLine("UpdateUnder " + tmpBlock.position.ToString());
            }
        }
        //Update edges
        for(int i = 0; i < 4; i++)
        {
            for(int h = 1; h >= -4; h--)
            {
                Vector3Int tmpPos = pos + Block.edgeOffset[i] + Vector3Int.Up * h;
                if (!blocks.ContainsKey(tmpPos)) continue;
                tmpBlock = blocks[tmpPos];
                if (tmpBlock.canStand)
                {
                    tmpBlock.UpdateEdgeAt(i^1); //更新对向边
                    Console.Write(tmpBlock.position + " ");
                    if (tmpBlock.Edge[i^1] != null)
                    {
                        Console.WriteLine(tmpBlock.Edge[i ^ 1].position.ToString());
                    }
                    else
                    {
                        Console.WriteLine("no edge in " + (i^1));
                    }
                }
            }
        }
    }

    public Block GetBlock(Vector3Int position)
    {
        if (!blocks.ContainsKey(position)) return null;
        return blocks[position];
    }
    public BlockType GetBlockType(Vector3Int position)
    {
        if(!blocks.ContainsKey(position)) return BlockType.None;
        return blocks[position].type;
    }

    public Vector3Int GetEdge(Vector3Int position, int dir)
    {
        blocks.TryGetValue(position, out Block center);
        if (center == null) return position;
        if (center.CanGoto(dir)) return center.Edge[dir].position;
        return position;
    }

    public Vector3Int RandomGetEdge(Vector3Int position)
    {
        blocks.TryGetValue(position, out Block center);
        if(center == null) return position;
        Vector3Int res = center.RandormGetEdge();
        return res == Vector3Int.Zero ? position : res;
    }
}