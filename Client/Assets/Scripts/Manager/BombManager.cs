using System.Collections.Generic;
using UnityEngine;

public enum BombState
{
    None,
    Unlit, //Œ¥µ„»º
    Burning, //»º…’“˝–≈
    Exploded //±¨’®
}

public class BombManager : Singleton<BombManager>
{
    public Dictionary<int, Bomb> Bombs;

    public void Init()
    {
        Bombs = new();
        NetManager.AddMsgListener("MsgBombState",OnMsgBombState);

        Bomb.radius = DataManager.Instance.Config.BombRadius;
    }

    public void AddBomb(int idx, Vector3Int corner)
    {
        GameObject go = ResManager.Instance.GetGameObject(ObjType.Bomb);
        go.transform.position = corner.ToVector3() + new Vector3(0.5f,0.5f,0.5f);
        Bomb newBomb = go.GetComponent<Bomb>();
        Bombs[idx] = newBomb;
        newBomb.idx = idx;
        newBomb.state = BombState.Unlit;
    }

    public void RemoveBomb(int idx)
    {
        if (Bombs.ContainsKey(idx))
        {
            //GameObject.Destroy(Bombs[idx].gameObject);
            ResManager.Instance.RecycleObj(Bombs[idx].gameObject, ObjType.Bomb);
            Bombs.Remove(idx);
        }
        else { Debug.Log("Bomb doesn't exist! " + idx); }
    }

    public void Clear()
    {
        foreach (var bomb in Bombs.Values)
        {
            //GameObject.Destroy(bomb.gameObject);
            ResManager.Instance.RecycleObj(bomb.gameObject, ObjType.Bomb);
        }
        Bombs.Clear();
    }

    public void OnMsgBombState(MsgBase msgBase)
    {
        MsgBombState msg = (MsgBombState)msgBase;
        if (!Bombs.ContainsKey(msg.id)) return;
        Bombs[msg.id].state = BombState.Burning;
        if(msg.state == BombState.Burning)
        {
            Bombs[msg.id].Ignite();
        }
        else if(msg.state == BombState.Exploded)
        {
            Bombs[msg.id].Explode();
        }
    }
}