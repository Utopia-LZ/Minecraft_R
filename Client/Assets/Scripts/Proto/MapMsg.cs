using System;
using UnityEngine;

[System.Serializable]
public struct Vector3Int
{
    public int x;
    public int y;
    public int z;

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

    public static Vector3Int Up = new Vector3Int(0, 1, 0);
    public static Vector3Int Down = new Vector3Int(0, -1, 0);
    public static Vector3Int Front = new Vector3Int(1, 0, 0);
    public static Vector3Int Back = new Vector3Int(-1, 0, 0);
    public static Vector3Int Right = new Vector3Int(0, 0, 1);
    public static Vector3Int Left = new Vector3Int(0, 0, -1);
    public static Vector3Int Zero = new Vector3Int(0, 0, 0);
    public static Vector3Int One = new Vector3Int(1, 1, 1);

    public static Vector3Int operator +(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static Vector3Int operator +(Vector3Int a, int b)
    {
        return new Vector3Int(a.x + b, a.y + b, a.z + b);
    }
    public static Vector3Int operator-(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x-b.x,a.y-b.y,a.z-b.z);
    }
    public static Vector3Int operator -(Vector3Int a, int b)
    {
        return new Vector3Int(a.x - b, a.y - b, a.z - b);
    }
    public static bool operator ==(Vector3Int a, Vector3Int b)
    {
        if(a.x==b.x && a.y==b.y && a.z==b.z) return true;
        return false;
    }
    public static bool operator !=(Vector3Int a, Vector3Int b)
    {
        if (a.x != b.x || a.y != b.y || a.z != b.z) return true;
        return false;
    }
    public static Vector3Int operator *(Vector3Int a, int b)
    {
        return new Vector3Int(a.x*b,a.y*b,a.z*b);
    }
    public static Vector3Int operator /(Vector3Int a, int b)
    {
        return new Vector3Int(a.x/b,a.y/b,a.z/b);
    }
    public static Vector3Int operator %(Vector3Int a, int b)
    {
        return new Vector3Int(a.x%b,a.y%b,a.z%b);
    }


    public Vector3 ToVector3()
    {
        return new Vector3(x,y,z);
    }
    public static Vector3Int ToVector3Int(Vector3 v)
    {
        return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
    }
    public static Vector3 V3IntToV3(Vector3Int pos)
    {
        return new Vector3((float)pos.x / 100f, (float)pos.y / 100f, (float)pos.z / 100f);
    }
    public static Vector3Int V3ToV3Int(Vector3 pos)
    {
        return new Vector3Int(pos.x*100,pos.y*100,pos.z*100);
    }
    public override string ToString()
    {
        return $"[{x},{y},{z}]";
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3Int @int &&
               x == @int.x &&
               y == @int.y &&
               z == @int.z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }

    public readonly int Magnitude { get { return x * x + y * y + z * z; } }
}

public class MsgMapInit : MsgBase
{
    public MsgMapInit() { protoName = "MsgMapInit"; }

    //客户端发
    public int chunkId;
    public Vector3Int chunkPos;

    //服务端回
    public BlockType[] map;
}

public class MsgMapChange : MsgBase
{
    public MsgMapChange() { protoName = "MsgMapChange"; }

    //客户端发
    public int chunkId;
    public Vector3Int chunkPos;
    public Vector3Int blockPos;
    public BlockType type;

    //服务端回
    public string id = "";
}