using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    public void Init()
    {
        NetManager.AddMsgListener("MsgUpdateEntity", OnMsgUpdateEntity);
        NetManager.AddMsgListener("MsgEntityInit", OnMsgEntityInit);
        EventHandler.OnLeaveRoom += LeaveRoom;
    }

    public void SendGenerate(Vector3Int corner, BlockType type)
    {
        MsgUpdateEntity msg = new MsgUpdateEntity();
        msg.corner = corner;
        msg.generate = true;
        msg.type = type;
        NetManager.Send(msg);
    }

    public void SendDestroy(Vector3Int corner, BlockType type, GameObject go)
    {
        MsgUpdateEntity msg = new MsgUpdateEntity();
        msg.corner = corner*100;
        msg.type = type;
        if(type == BlockType.Chest)
        {
            msg.id = go.GetComponent<Chest>().index;
            go.GetComponent<Collider>().enabled = false;
        }
        else if(type == BlockType.Bomb)
        {
            msg.id = go.GetComponent<Bomb>().idx;
        }
        else if(type == BlockType.Light)
        {
            msg.id = go.GetComponent<Light>().idx;
        }
        msg.generate = false;
        NetManager.Send(msg);
        SoundManager.Instance.PlaySound(ObjType.MusicBroke);
    }

    private void LeaveRoom()
    {
        ChestManager.Instance.Clear();
        BombManager.Instance.Clear();
        LightManager.Instance.Clear();
        ItemManager.Instance.Clear();
        ZombieManager.Instance.Clear();
    }

    public void OnMsgEntityInit(MsgBase msgBase)
    {
        Debug.Log("OnMsgEntityInit");
        MsgEntityInit msg = (MsgEntityInit)msgBase;
        for(int i = 0; i < msg.entities.Length; i++)
        {
            Entity entity = msg.entities[i];
            switch (entity.type)
            {
                case BlockType.Chest:
                    ChestManager.Instance.AddChest(entity.id, entity.position);
                    break;
                case BlockType.Bomb:
                    BombManager.Instance.AddBomb(entity.id, entity.position);
                    break;
                case BlockType.Light:
                    LightManager.Instance.AddLight(entity.id, entity.position);
                    break;
                default:
                    Debug.Log("None Type");
                    break;
            }
        }
    }

    public void OnMsgUpdateEntity(MsgBase msgBase)
    {
        MsgUpdateEntity msg = (MsgUpdateEntity)msgBase;
        if(msg.id == -1) { Debug.Log("UpdateEntity failed."); return; }

        if (msg.generate)
        {
            if(msg.type == BlockType.Chest)
            {
                ChestManager.Instance.AddChest(msg.id, msg.corner);
            }
            else if(msg.type == BlockType.Bomb)
            {
                BombManager.Instance.AddBomb(msg.id, msg.corner);
            }
            else if(msg.type == BlockType.Light)
            {
                LightManager.Instance.AddLight(msg.id, msg.corner);
            }
        }
        else
        {
            if(msg.type == BlockType.Chest)
            {
                ChestManager.Instance.RemoveChest(msg.id);
            }
            else if (msg.type == BlockType.Bomb)
            {
                BombManager.Instance.RemoveBomb(msg.id);
            }
            else if (msg.type == BlockType.Light)
            {
                LightManager.Instance.RemoveLight(msg.id);
            }
        }
    }
}