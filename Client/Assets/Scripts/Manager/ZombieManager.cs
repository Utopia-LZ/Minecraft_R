using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : Singleton<ZombieManager>
{
    public Dictionary<string, Zombie> Zombies;

    public void Init()
    {
        NetManager.AddMsgListener("MsgSyncZombie", OnMsgSyncZombie);
        NetManager.AddMsgListener("MsgUpdateZombie", OnMsgUpdateZombie);
        NetManager.AddMsgListener("MsgZombieAttack", OnMsgZombieAttack);
        NetManager.AddMsgListener("MsgZombieHit", OnMsgZombieHit);
        NetManager.AddMsgListener("MsgLoadZombie", OnMsgLoadZombie);
        Zombies = new();

        Config config = DataManager.Instance.Config;
        Zombie.RecoverHunger = config.RecoverHunger;
        Zombie.RecoverSaturation = config.RecoverSaturation;
        Zombie.TryTauntInterval = config.TryTauntInterval;
        Zombie.TauntChance = config.TauntChance;
    }

    public void Generate(CharacterInfo info)
    {
        Debug.Log("Info.position after trans:" + Vector3Int.V3IntToV3(info.pos));
        GameObject go = ResManager.Instance.GetGameObject(ObjType.Zombie);
        go.transform.position = Vector3Int.V3IntToV3(info.pos);
        Zombie zombie = go.GetComponent<Zombie>();
        zombie.Init();
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
            //GameObject.Destroy(zombie.gameObject);
            ResManager.Instance.RecycleObj(zombie.gameObject, ObjType.Zombie, zombie);
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
                //GameObject.Destroy(Zombies[msg.info.id].gameObject);
                SoundManager.Instance.PlaySound(ObjType.MusicZombieDeath);
                ResManager.Instance.RecycleObj(Zombies[msg.info.id].gameObject, ObjType.Zombie, Zombies[msg.info.id]);
                Zombies.Remove(msg.info.id);    
            }
        }
    }

    public void OnMsgZombieAttack(MsgBase msgBase)
    {
        Debug.Log("OnMsgZombieAttack");
        MsgZombieAttack msg = (MsgZombieAttack)msgBase;
        Zombie zombie = Zombies[msg.zombieId];
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