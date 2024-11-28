using System;
using System.Numerics;

public class Player
{
    public static int safeDistance = 4;
    public static int ignoreDistance = 10; //(block scale)

    public static void InitConfig(Config config)
    {
        safeDistance = config.ZombieRefreshMinDis;
        ignoreDistance = config.ZombieRefreshMaxDis;
    }

    //id
    public string id = "";
    //指向ClientState
    public ClientState state;
    //构造函数
    public Player(ClientState state)
    {
        this.state = state;
    }
    //坐标和旋转
    public Vector3Int pos;
    public Vector3Int rot;
    //在哪个房间
    public int roomId = -1;
    //玩家生命值
    public int hp = 10;
    //生物种类
    public Kind Kind = Kind.None;

    //数据库数据
    public PlayerData data;

    public void TakeDamage(int damage)
    {
        if(hp <= 0) return;
        hp -= damage;
        if(hp <= 0)
        {
            Room room = RoomManager.GetRoom(roomId);
            Console.WriteLine("RoomId: " + roomId + " have? " + (room != null));
            DropBag(room);
            MsgLeaveRoom msg = new MsgLeaveRoom();
            msg.id = id;
            msg.reason = 1;
            room.Broadcast(msg);
            room.RemovePlayer(id);
        }
    }

    public void DropBag(Room room)
    {
        for(int i = 0; i < BagManager.SLOT_COUNT; i++)
        {
            if (data.slots[i].item.type == BlockType.None) continue;
            MsgDropItem msg = new MsgDropItem();
            msg.locked = true;
            msg.pos = pos;
            msg.info = data.slots[i].item;
            msg.id = ItemManager.index++;
            DroppedItem droppedItem = new();
            droppedItem.position = msg.pos;
            droppedItem.id = msg.id;
            ItemManager.AddItem(droppedItem);
            data.slots[i].item.type = BlockType.None;
            room.Broadcast(msg);
        }
    }

    //发送信息
    public void Send(MsgBase msgBase)
    {
        NetManager.Send(state, msgBase);
    }
}