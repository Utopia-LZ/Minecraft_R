using System;

public class ByteArray
{
    public byte[] bytes;
    public int readIdx = 0;
    public int writeIdx = 0;
    const int DEFAULT_SIZE = 1024;
    int initSize = 0;
    private int capacity = 0;
    public int Remain { get { return capacity - writeIdx; } }
    public int Length { get { return writeIdx - readIdx; } }

    public ByteArray(int size = DEFAULT_SIZE)
    {
        bytes = new byte[size];
        capacity = size;
        initSize = size;
        readIdx = 0;
        writeIdx = 0;
    }
    public ByteArray(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity = defaultBytes.Length;
        initSize = defaultBytes.Length;
        readIdx = 0;
        writeIdx = defaultBytes.Length;
    }

    public void Resize(int size)
    {
        /*if (size < Length) return;
        if (size < initSize) return;
        int n = 1;
        while (n < size) n *= 2;
        capacity = n;*/
        capacity *= 2;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes, readIdx, newBytes, 0, writeIdx - readIdx);
        bytes = newBytes;
        writeIdx = Length;
        readIdx = 0;
    }
    public int Read(byte[] bs, int offset, int count)
    {
        count = Math.Min(count, Length);
        Array.Copy(bytes, 0, bs, offset, count);
        readIdx += count;

        return count;
    }
    public void CheckAndMoveBytes()
    {
        if (Length < 8 || readIdx > DEFAULT_SIZE/2)
            MoveBytes();
    }
    public void MoveBytes()
    {
        Array.Copy(bytes, readIdx, bytes, 0, Length);
        writeIdx = Length;
        readIdx = 0;
    }
    public Int16 ReadInt16()
    {
        if (Length < 2) return 0;
        Int16 ret = BitConverter.ToInt16(bytes, readIdx);
        return ret;
    }
    public Int32 ReadInt32()
    {
        if (Length < 4) return 0;
        Int32 ret = BitConverter.ToInt32(bytes, readIdx);
        readIdx += 4;
        CheckAndMoveBytes();
        return ret;
    }

    //打印缓冲区
    public override string ToString()
    {
        return BitConverter.ToString(bytes, readIdx, Length);
    }
    //打印调试信息
    public string Debug()
    {
        return string.Format("readIdx({0}) writeIdx({1}) bytes({2})",
            readIdx,
            writeIdx,
            BitConverter.ToString(bytes, 0, capacity)
        );
    }
}