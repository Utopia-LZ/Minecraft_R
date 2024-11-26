[System.Serializable]
public struct Vector3Int
{
    public int x;
    public int y;
    public int z;

    public static Vector3Int Up = new Vector3Int(0, 1, 0);
    public static Vector3Int Down = new Vector3Int(0, -1, 0);
    public static Vector3Int Front = new Vector3Int(0, 0, 1);
    public static Vector3Int Back = new Vector3Int(0, 0, -1);
    public static Vector3Int Right = new Vector3Int(1, 0, 0);
    public static Vector3Int Left = new Vector3Int(-1, 0, 0);
    public static Vector3Int Zero = new Vector3Int(0, 0, 0);
    public static Vector3Int One = new Vector3Int(1, 1, 1);

    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3Int(float x, float y, float z)
    {
        this.x = (int)Math.Floor(x);
        this.y = (int)Math.Floor(y);
        this.z = (int)Math.Floor(z);
    }

    public static Vector3Int operator +(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static Vector3Int operator +(Vector3Int a, int b)
    {
        return new Vector3Int(a.x + b, a.y + b, a.z + b);
    }
    public static Vector3Int operator -(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static Vector3Int operator -(Vector3Int a, int b)
    {
        return new Vector3Int(a.x - b, a.y - b, a.z - b);
    }
    public static bool operator ==(Vector3Int a, Vector3Int b)
    {
        if (a.x == b.x && a.y == b.y && a.z == b.z) return true;
        return false;
    }
    public static bool operator !=(Vector3Int a, Vector3Int b)
    {
        if (a.x != b.x || a.y != b.y || a.z != b.z) return true;
        return false;
    }
    public static Vector3Int operator *(Vector3Int a, int b)
    {
        return new Vector3Int(a.x * b, a.y * b, a.z * b);
    }
    public static Vector3Int operator /(Vector3Int a, int b)
    {
        return new Vector3Int(a.x / b, a.y / b, a.z / b);
    }
    public static Vector3Int operator %(Vector3Int a, int b)
    {
        return new Vector3Int(a.x % b, a.y % b, a.z % b);
    }
    public override string ToString()
    {
        return $"[{x},{y},{z}]";
    }

    public static Vector3Int Float2Vector(float x, float y, float z)
    {
        return new Vector3Int(x * 100, y * 100, z * 100);
    }
    public static Vector3Int Float2Int(Vector3Int pos)
    {
        return new Vector3Int(
            (int)Math.Floor((double)pos.x / 100), 
            (int)Math.Floor((double)pos.y / 100), 
            (int)Math.Floor((double)pos.z / 100));
    }

    public static Vector3Int Random(Vector3Int min, Vector3Int max)
    {
        Random rand = new Random();
        Vector3Int res = new();
        res.x = rand.Next(min.x, max.x);
        res.y = rand.Next(min.y, max.y);
        res.z = rand.Next(min.z, max.z);
        return res;
    }

    public readonly int Magnitude { get { return x * x + y * y + z * z; } }
}
