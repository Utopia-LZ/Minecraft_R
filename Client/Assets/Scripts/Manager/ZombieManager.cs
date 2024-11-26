using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : Singleton<ZombieManager>
{
    public Dictionary<string, Zombie> Zombies;
    private GameObject ZombiePrefab;

    public void Init()
    {
        NetManager.AddMsgListener("MsgSyncZombie", OnMsgSyncZombie);
        NetManager.AddMsgListener("MsgUpdateZombie", OnMsgUpdateZombie);
        NetManager.AddMsgListener("MsgZombieAttack", OnMsgZombieAttack);
        NetManager.AddMsgListener("MsgZombieHit", OnMsgZombieHit);
        NetManager.AddMsgListener("MsgLoadZombie", OnMsgLoadZombie);
        ZombiePrefab = ResManager.LoadResources<GameObject>("prefab_zombie");
        Zombies = new();
    }

    public void Generate(CharacterInfo info)
    {
        Debug.Log("Info.position after trans:" + Vector3Int.V3IntToV3(info.pos));
        GameObject go = MonoBehaviour.Instantiate(ZombiePrefab);
        go.transform.position = Vector3Int.V3IntToV3(info.pos);
        Zombie zombie = go.GetComponent<Zombie>();
        zombie.id = info.id;
        zombie.hp = info.hp;
        zombie.Kind = info.Kind;
        zombie.damage = 1; //:temp
        Zombies.Add(zombie.id, zombie);
    }

    public void Clear()
    {
        foreach(var zombie in Zombies.Values)
        {
            GameObject.Destroy(zombie.gameObject);
        }
        Zombies.Clear();
    }

    //收到僵尸同步协议
    public void OnMsgSyncZombie(MsgBase msgBase)
    {
        Debug.Log("OnMsgSyncZombie");
        MsgSyncZombie msg = (MsgSyncZombie)msgBase;
        //查找僵尸
        if (!Zombies.ContainsKey(msg.id)) return;
        Zombie zombie = (Zombie)Zombies[msg.id];
        //移动同步
        zombie.SyncPos(msg);
    }

    public void OnMsgUpdateZombie(MsgBase msgBase)
    {
        Debug.Log("OnMsgUpdateZombie");
        MsgUpdateZombie msg = (MsgUpdateZombie)msgBase;
        if (msg.generate)
        {
            Generate(msg.info);
        }
        else
        {
            if (Zombies.ContainsKey(msg.info.id))
            {
                GameObject.Destroy(Zombies[msg.info.id].gameObject);
                Zombies.Remove(msg.info.id);    
            }
        }
    }

    public void OnMsgZombieAttack(MsgBase msgBase)
    {
        Debug.Log("OnMsgZombieAttack");
        MsgZombieAttack msg = (MsgZombieAttack)msgBase;
        Zombie zombie = Zombies[msg.zombieId];
        zombie.AttackAnim();
        MsgHit msgH = new MsgHit();
        msgH.id = msg.playerId;
        msgH.damage = zombie.damage;
        NetManager.Send(msgH);
    }

    public void OnMsgZombieHit(MsgBase msgBase)
    {
        Debug.Log("OnMsgZombieHit");
        MsgZombieHit msg = (MsgZombieHit)msgBase;
        Zombie zombie = Zombies[msg.zombieId];  
        zombie.TakeDamage(msg.damage);
    }

    public void OnMsgLoadZombie(MsgBase msgBase)
    {
        Debug.Log("OnMsgLoadZombie");
        MsgLoadZombie msg = (MsgLoadZombie)msgBase;
        CharacterInfo[] info = msg.zombies;
        for(int i = 0; i < info.Length; i++)
        {
            Generate(info[i]);
        }
    }
}