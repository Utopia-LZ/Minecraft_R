using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public static Dictionary<Vector3Int, Chunk> chunks = new();
    private static int[] offsetX = new int[9]{0,1,1,0,-1,-1,-1,0,1};
    private static int[] offsetZ = new int[9]{0,0,-1,-1,-1,0,1,1,1};

    public static void Init()
    {
        NetManager.AddMsgListener("MsgMapInit", OnMsgMapInit);
        NetManager.AddMsgListener("MsgMapChange", OnMsgMapChange);
        EventHandler.OnLeaveRoom += ClearMap;
        for(int i = 0; i < offsetX.Length; i++)
        {
            offsetX[i] *= Chunk.width;
            offsetZ[i] *= Chunk.width;
        }
    }

    public static void IntantiateChunk(Vector3Int pos)
    {
        Vector3Int opos;
        for(int i = 0; i < 9; i++)
        {
            opos = pos + new Vector3Int(offsetX[i], 0, offsetZ[i]);
            if (chunks.ContainsKey(opos)) continue;

            MsgMapInit msg = new MsgMapInit();
            msg.chunkPos = opos;
            NetManager.Send(msg);
        }
    }

    public static void ClearMap()
    {
        foreach(var chunk in chunks.Values)
        {
            //GameObject.Destroy(chunk.gameObject);
            ResManager.Instance.RecycleObj(chunk.gameObject, ObjType.Chunk);
            chunk.OnRecycle();
        }
        chunks.Clear();
    }

    public static void OnMsgMapInit(MsgBase msgBase)
    {
        MsgMapInit msg = (MsgMapInit)msgBase;
        if (chunks.ContainsKey(msg.chunkPos)) return;

        Vector3 wpos = msg.chunkPos.ToVector3();
        GameObject go = ResManager.Instance.GetGameObject(ObjType.Chunk);
        go.transform.position = wpos;
        chunks[msg.chunkPos] = go.GetComponent<Chunk>();
        chunks[msg.chunkPos].InitMap(msg.chunkId,msg.chunkPos,To3(msg.map));

        BattleManager.FreezePlayers(false);
    }

    public static void OnMsgMapChange(MsgBase msgBase)
    {
        MsgMapChange msg = (MsgMapChange)msgBase;

        Chunk chunk = chunks[msg.chunkPos];
        if(chunk != null)
        {
            Vector3Int v = msg.blockPos;
            if(msg.type == BlockType.None && chunk.map[v.x, v.y, v.z] != BlockType.None)
            {
                ItemInfo info = new();
                info.type = chunk.map[v.x, v.y, v.z];
                info.count = 1;
                if(GameMain.id == msg.id) //更新地形已经是广播消息了，基于广播消息的消息要限制广播
                {
                    Vector3 pos = v.ToVector3() + msg.chunkPos.ToVector3() + Vector3.one * 0.5f;
                    MsgDropItem msgD = new MsgDropItem();
                    msgD.info = info;
                    msgD.pos = Vector3Int.V3ToV3Int(pos);
                    msgD.locked = false;
                    NetManager.Send(msgD);
                }
            }
            else if(msg.type == BlockType.Grass && chunk.map[v.x, v.y, v.z] == BlockType.None)
            {
                if (GameMain.id == msg.id)
                {
                    BagManager.Instance.SelectSlot.Refresh(msg.type);
                }
            }
            chunk.map[v.x, v.y, v.z] = msg.type;
            Debug.Log("OnMsgMapChange " + v.ToString());
            chunk.BuildChunk();
        }
        else
        {
            Debug.LogWarning("Chunk is NULL!");
        }
    }

    public static BlockType[,,] To3(BlockType[] types)
    {
        BlockType[,,] result = new BlockType[Chunk.width, Chunk.height, Chunk.width];
        for (int i = 0; i < Chunk.width; i++)
            for (int j = 0; j < Chunk.height; j++)
                for (int k = 0; k < Chunk.width; k++)
                    result[i, j, k] = types[i + j * Chunk.width * Chunk.width + k * Chunk.width];
        return result;
    }
}