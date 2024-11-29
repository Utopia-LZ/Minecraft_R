using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager
{
    //ս���е����
    public static Dictionary<string, BaseSteve> characters = new Dictionary<string, BaseSteve>();

    //��ʼ��
    public static void Init()
    {
        //��Ӽ���
        //NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        //NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        //NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgSyncSteve", OnMsgSyncSteve);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);

        Config config = DataManager.Instance.Config;
        CtrlSteve.syncInterval = config.SyncInterval;
        CtrlSteve.actInterval = config.ActInterval;
        CtrlSteve.actDistance = config.ActDistance;
        CtrlSteve.jumpForce = config.JumpForce;
        CtrlSteve.WalkSpeed = config.WalkSpeed;
        CtrlSteve.RunSpeed = config.RunSpeed;
        CtrlSteve.RotateSpeed = config.RotateSpeed;
        BaseSteve.HP = config.PlayerHp;
        BaseSteve.Damage = config.PlayerDamage;
}

    //������
    public static void AddCharacter(string id, BaseSteve tank)
    {
        characters[id] = tank;
    }

    //ɾ�����
    public static void RemoveCharacter(string id)
    {
        characters.Remove(id);
    }

    //��ȡ���
    public static BaseSteve GetCharacter(string id)
    {
        if (characters.ContainsKey(id))
        {
            return characters[id];
        }
        return null;
    }

    //��ȡ��ҿ��Ƶ����
    public static BaseSteve GetCtrlSteve()
    {
        return GetCharacter(GameMain.id);
    }

    //����ս��
    public static void Reset()
    {
        //����
        foreach (BaseSteve steve in characters.Values)
        {
            //MonoBehaviour.Destroy(steve.gameObject);
            ResManager.Instance.RecycleObj(steve.gameObject, ObjType.Steve);
        }
        //�б�
        characters.Clear();
    }

    //��ʼս��
    public static void EnterBattle(MsgEnterRoom msg)
    {
        if (!characters.ContainsKey(GameMain.id))
        {
            EventHandler.CallClosePanel(PanelType.RoomList);
            EventHandler.CallOpenPanel(PanelType.Chat);
            EventHandler.CallOpenPanel(PanelType.Battle);
            LightManager.Instance.Pause(false);
        }
        //�������
        for (int i = 0; i < msg.characters.Length; i++) //:ÿ����Ϣֻ����һ��
        {
            if (!characters.ContainsKey(msg.characters[i].id))
            {
                GenerateSteve(msg.characters[i]);
            }
        }

        FreezePlayers(true);
        //���ɵ�ͼ
        CtrlSteve steve = (CtrlSteve)BattleManager.GetCtrlSteve();
        MapManager.IntantiateChunk(steve.currentChunk);
    }

    //������ɫ
    public static void GenerateSteve(CharacterInfo tankInfo)
    {
        //GameObject
        GameObject steveObj = ResManager.Instance.GetGameObject(ObjType.Steve);
        steveObj.name = "Steve_" + tankInfo.id;
        //AddComponent
        BaseSteve steve = null;
        if (tankInfo.id == GameMain.id)
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
        if (tankInfo.id == GameMain.id)
        {
            CameraFollow cf = steveObj.AddComponent<CameraFollow>();
        }
        //����
        steve.Kind = tankInfo.Kind;
        steve.id = tankInfo.id;
        steve.hp = tankInfo.hp;
        //pos rotation
        Vector3 pos = Vector3Int.V3IntToV3(tankInfo.pos);
        Vector3 rot = Vector3Int.V3IntToV3(tankInfo.rot);
        steve.transform.position = pos;
        steve.transform.eulerAngles = rot;
        //init
        steve.Init();
        //�б�
        AddCharacter(tankInfo.id, steve);        
    }

    //�ڵ�ͼ����ǰ�������
    public static void FreezePlayers(bool value)
    {
        Debug.Log("FreezePlayer");
        foreach(var player in characters.Values)
        {
            Debug.Log("Freeze Id: " +  player.id);
            player.rb.isKinematic = value;
        }
    }

    //�յ����뷿��Э��
    public static void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        //�ɹ����뷿��
        if (msg.result == 0)
        {
            //EventHandler.CallOpenPanel(PanelType.RoomList);
            EnterBattle(msg);
        }
        //���뷿��ʧ��
        else
        {
            MessageBox.Instance.Show("���뷿��ʧ��");
        }
    }

    //�յ�����˳�Э��
    public static void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        if (msg.id == GameMain.id)
        {
            EventHandler.CallLeaveRoom(); //��ֹ��������ڽ�ɫ�ϱ�һͬ����
            EventHandler.CallClosePanel(PanelType.Battle);
            EventHandler.CallClosePanel(PanelType.Chat);
            EventHandler.CallOpenPanel(PanelType.RoomList);
            foreach (var player in characters.Values)
            {
                //MonoBehaviour.Destroy(player.gameObject);
                ResManager.Instance.RecycleObj(player.gameObject, ObjType.Steve);
            }
            characters.Clear();
            if (msg.reason == 1)
            {
                MessageBox.Instance.Show("YOU DIED!");
            }
        }
        else
        {
            if (characters.ContainsKey(msg.id))
            {
                //GameObject.Destroy(characters[msg.id].gameObject);
                ResManager.Instance.RecycleObj(characters[msg.id].gameObject, ObjType.Steve);
                characters.Remove(msg.id);
            }
        }
    }

    //�յ�ͬ��Э��
    public static void OnMsgSyncSteve(MsgBase msgBase)
    {
        MsgSyncSteve msg = (MsgSyncSteve)msgBase;
        //��ͬ���Լ�
        if (msg.id == GameMain.id)
        {
            return;
        }
        //�������
        SyncSteve tank = (SyncSteve)GetCharacter(msg.id);
        if (tank == null)
        {
            return;
        }
        //�ƶ�ͬ��
        tank.SyncPos(msg);
    }

    //�յ�����Э��
    public static void OnMsgHit(MsgBase msgBase)
    {
        MsgHit msg = (MsgHit)msgBase;
        Debug.Log("OnMsgHit " + msg.id + " " + msg.damage);
        //�������
        BaseSteve steve = GetCharacter(msg.id);
        if (steve == null)
        {
            return;
        }
        //������
        steve.Attacked(msg.damage);
    }
}
