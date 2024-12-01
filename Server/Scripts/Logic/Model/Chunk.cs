using SimplexNoise;
using System.Numerics;

[System.Serializable]
public enum BlockType : byte
{
    None = 0,
    Dirt = 1,
    Grass = 3,
    Gravel = 4,
    Chest,
    Bomb,
    Light,
    Carrion,
    Dropped,
}

[System.Serializable]
public struct ChunkInfo
{
    public int id;
    public Vector3Int pos;
    public BlockType[,,] map;
}

public class Chunk
{
    public static int width;
    public static int height;
    public static int baseHeight;
    public static float frequency;
    public static float amplitude;

    public int Id;
    public Vector3Int position;
    public BlockType[,,] map;

    Vector3 offset0;
    Vector3 offset1;
    Vector3 offset2;

    public static void InitConfig(Config config)
    {
        width = config.ChunkWidth;
        height = config.ChunkHeight;
        baseHeight = config.ChunkBaseHeight;
        frequency = config.Frequency;
        amplitude = config.Amplitude;
    }

    public Chunk(int Id, Vector3Int position)
    {
        this.Id = Id;
        this.position = position;

        //初始化随机种子
        Random rdm = new Random();
        offset0 = new Vector3(rdm.Next(0, 1000), rdm.Next(0, 1000), rdm.Next(0, 1000));
        offset1 = new Vector3(rdm.Next(0, 1000), rdm.Next(0, 1000), rdm.Next(0, 1000));
        offset2 = new Vector3(rdm.Next(0, 1000), rdm.Next(0, 1000), rdm.Next(0, 1000));

        //初始化Map
        map = new BlockType[width, height, width];

        //遍历map，生成其中每个Block的信息
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                //获取当前位置方块随机生成的高度值
                float genHeight = GenerateHeight(new Vector3Int(x, 0, z) + position);
                for (int y = 0; y < height; y++)
                {
                    map[x, y, z] = GenerateBlockType(new Vector3Int(x, y, z) + position, genHeight);
                }
            }
        }
    }
    public Chunk(ChunkInfo info)
    {
        Id = info.id;
        position = info.pos;
        map = info.map;
        //遍历map，生成其中每个Block的信息
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                //获取当前位置方块随机生成的高度值
                float genHeight = GenerateHeight(new Vector3Int(x, 0, z) + position);
                for (int y = 0; y < height; y++)
                {
                   map[x, y, z] = GenerateBlockType(new Vector3Int(x, y, z) + position, genHeight);
                }
            }
        }
    }

    int GenerateHeight(Vector3Int wPos)
    {
        //让随机种子，振幅，频率，应用于我们的噪音采样结果
        float x0 = (wPos.x + offset0.X) * frequency;
        float y0 = (wPos.y + offset0.Y) * frequency;
        float z0 = (wPos.z + offset0.Z) * frequency;

        float x1 = (wPos.x + offset1.X) * frequency * 2;
        float y1 = (wPos.y + offset1.Y) * frequency * 2;
        float z1 = (wPos.z + offset1.Z) * frequency * 2;

        float x2 = (wPos.x + offset2.X) * frequency / 4;
        float y2 = (wPos.y + offset2.Y) * frequency / 4;
        float z2 = (wPos.z + offset2.Z) * frequency / 4;

        float noise0 = Noise.Generate(x0, y0, z0) * amplitude;
        float noise1 = Noise.Generate(x1, y1, z1) * amplitude / 2;
        float noise2 = Noise.Generate(x2, y2, z2) * amplitude / 4;

        //在采样结果上，叠加上baseHeight，限制随机生成的高度下限
        return (int)Math.Floor(noise0 + noise1 + noise2 + baseHeight);
    }

    BlockType GenerateBlockType(Vector3Int wPos, float genHeight)
    {
        //y坐标是否在Chunk内
        if (wPos.y >= height)
        {
            return BlockType.None;
        }

        //当前方块位置高于随机生成的高度值时，当前方块类型为空
        if (wPos.y > genHeight)
        {
            return BlockType.None;
        }
        //当前方块位置等于随机生成的高度值时，当前方块类型为草地
        else if (wPos.y == genHeight)
        {
            return BlockType.Grass;
        }
        //当前方块位置小于随机生成的高度值 且 大于 genHeight - 5时，当前方块类型为泥土
        else if (wPos.y < genHeight && wPos.y > genHeight - 5)
        {
            return BlockType.Dirt;
        }
        //其他情况，当前方块类型为碎石
        return BlockType.Gravel;
    }

    public ChunkInfo GetInfo()
    {
        return new ChunkInfo
        {
            id = Id,
            pos = position,
            map = map
        };
    }
}