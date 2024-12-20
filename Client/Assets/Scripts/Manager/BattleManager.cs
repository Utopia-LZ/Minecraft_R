using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    //战场中的玩家
    public static Dictionary<string, BaseSteve> characters = new Dictionary<string, BaseSteve>();
    public static BattlePanel Panel;

    //初始化
    public static void Init()
    {
        //添加监听
        //NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        //NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        //NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgSyncSteve", OnMsgSyncSteve);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);
        NetManager.AddMsgListener("MsgHungry", OnMsgHungry);

        Config config = DataManager.Instance.Config;
        CtrlSteve.syncInterval = config.SyncInterval;
        CtrlSteve.actInterval = config.ActInterval;
        CtrlSteve.actDistance = config.ActDistance;
        CtrlSteve.jumpForce = config.JumpForce;
        CtrlSteve.gravityForce = config.GravityForce;
        CtrlSteve.WalkSpeed = config.WalkSpeed;
        CtrlSteve.RunSpeed = config.RunSpeed;
        CtrlSteve.RotateSpeed = config.RotateSpeed;
        BaseSteve.HP = config.PlayerHp;
        BaseSteve.Damage = config.PlayerDamage;
        BaseSteve.Hunger = config.Hunger;
}

    //添加玩家
    public static void AddCharacter(string id, BaseSteve tank)
    {
        characters[id] = tank;
    }

    //删除玩家
    public static void RemoveCharacter(string id)
    {
        characters.Remove(id);
    }

    //获取玩家
    public static BaseSteve GetCharacter(string id)
    {
        if (characters.ContainsKey(id))
        {
            return characters[id];
        }
        return null;
    }

    //获取玩家控制的玩家
    public static BaseSteve GetCtrlSteve()
    {
        return GetCharacter(GameMain.id);
    }

    //重置战场
    /*public static void Reset()
    {
        //场景
        foreach (BaseSteve steve in characters.Values)
        {
            //MonoBehaviour.Destroy(steve.gameObject);
            ResManager.Instance.RecycleObj(steve.gameObject, ObjType.Steve);
        }
        //列表
        characters.Clear();
    }*/

    //开始战斗
    public static void EnterBattle(MsgEnterRoom msg)
    {
        if (!characters.ContainsKey(GameMain.id))
        {
            EventHandler.CallClosePanel(PanelType.RoomList);
            EventHandler.CallOpenPanel(PanelType.Chat);
            EventHandler.CallOpenPanel(PanelType.Battle);
            LightManager.Instance.Pause(false);
        }
        //产生玩家
        for (int i = 0; i < msg.characters.Length; i++) //:每条消息只新增一个
        {
            if (!characters.ContainsKey(msg.characters[i].id))
            {
                GenerateSteve(msg.characters[i]);
            }
        }

        FreezePlayers(true);
        //生成地图
        CtrlSteve steve = (CtrlSteve)BattleManager.GetCtrlSteve();
        MapManager.IntantiateChunk(steve.currentChunk);
    }

    //产生角色
    public static void GenerateSteve(CharacterInfo Info)
    {
        //GameObject
        GameObject steveObj = ResManager.Instance.GetGameObject(ObjType.Steve);
        steveObj.name = "Steve_" + Info.id;
        //AddComponent
        BaseSteve steve = null;
        if (Info.id == GameMain.id)
        {
            steve = steveObj.AddComponent<CtrlSteve>();
            steveObj.layer = LayerMask.NameToLayer("Own");
        }
        else
        {
            steve = steveObj.AddComponent<SyncSteve>();
            steveObj.layer = LayerMask.NameToLayer("Steve");
        }
        //Camera
        if (Info.id == GameMain.id)
        {
            CameraFollow cf = steveObj.AddComponent<CameraFollow>();
        }
        //属性
        steve.Kind = Info.Kind;
        steve.id = Info.id;
        steve.hp = Info.hp;
        Debug.Log("Generate Steve HP: " + steve.hp);
        //pos rotation
        Vector3 pos = Vector3Int.V3IntToV3(Info.pos);
        Vector3 rot = Vector3Int.V3IntToV3(Info.rot);
        steve.transform.position = pos;
        steve.transform.eulerAngles = rot;
        //init
        steve.Init();
        //列表
        AddCharacter(Info.id, steve);        
    }

    //在地图加载前冻结玩家
    public static void FreezePlayers(bool value)
    {
        Debug.Log("FreezePlayer");
        foreach(var player in characters.Values)
        {
            Debug.Log("Freeze Id: " +  player.id);
            player.rb.isKinematic = value;
        }
        if(!value) PanelManager.Instance.Close(PanelType.Wait);
    }

    public static void RefreshHunger(int delta)
    {
        if (Panel != null)
        {
            Panel.RefreshHunger(delta);
        }
        else Debug.Log("BattlePanel is NULL!");
    }

    //收到进入房间协议
    public static void OnMsgEnterRoom(MsgBase msgBase)
    {
        Debug.Log("OnMsgEnterRoom");
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        //成功进入房间
        if (msg.result == 0)
        {
            //EventHandler.CallOpenPanel(PanelType.RoomList);
            EnterBattle(msg);
        }
        //进入房间失败
        else
        {
            MessageBox.Instance.Show("进入房间失败");
        }
    }

    //收到玩家退出协议
    public static void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        if (msg.id == GameMain.id)
        {
            if (msg.reason == 1)
            {
                Panel.RefreshHp(CtrlSteve.HP);
                Panel.RefreshHunger(CtrlSteve.Hunger-Panel.meatCount);
                MessageBox.Instance.Show("YOU DIED!");
            }
            EventHandler.CallLeaveRoom(); //防止相机挂载在角色上被一同销毁
            EventHandler.CallClosePanel(PanelType.Battle);
            EventHandler.CallClosePanel(PanelType.Chat);
            EventHandler.CallOpenPanel(PanelType.RoomList);
            foreach (var player in characters.Values)
            {
                //MonoBehaviour.Destroy(player.gameObject);
                ResManager.Instance.RecycleObj(player.gameObject, ObjType.Steve, player);
            }
            characters.Clear();
        }
        else
        {
            if (characters.ContainsKey(msg.id))
            {
                //GameObject.Destroy(characters[msg.id].gameObject);
                ResManager.Instance.RecycleObj(characters[msg.id].gameObject, ObjType.Steve,characters[msg.id]);
                characters.Remove(msg.id);
            }
        }
    }

    //收到同步协议
    public static void OnMsgSyncSteve(MsgBase msgBase)
    {
        MsgSyncSteve msg = (MsgSyncSteve)msgBase;
        //不同步自己
        if (msg.id == GameMain.id)
        {
            return;
        }
        //查找玩家
        SyncSteve tank = (SyncSteve)GetCharacter(msg.id);
        if (tank == null)
        {
            return;
        }
        //移动同步
        tank.SyncPos(msg);
    }

    //收到击中协议
    public static void OnMsgHit(MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        Debug.Log("OnMsgHit " + msg.id + " " + msg.damage);
        //查找玩家
        BaseSteve steve = GetCharacter(msg.id);
        if (steve == null)
        {
            return;
        }
        //被击中
        steve.Attacked(msg.damage);
    }

    //收到则掉一格饱食度
    public static void OnMsgHungry(MsgBase msgBase)
    {
        Debug.Log("OnMsgHungry");
        MsgHungry msg = (MsgHungry)msgBase;
        RefreshHunger(-1);
    }
}
