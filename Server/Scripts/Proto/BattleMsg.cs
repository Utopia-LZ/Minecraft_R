public enum Kind
{
    None, Steve, Zombie
}

//玩家信息
[System.Serializable]
public class CharacterInfo
{
    public string id = "";  //玩家id
    public Kind Kind = Kind.None; //生物类别
    public int hp = 0;      //生命值

    public Vector3Int pos;
    public Vector3Int rot;
}


//进入战场（服务端推送）
public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle() { protoName = "MsgEnterBattle"; }
    //服务端回
    public CharacterInfo[] characters;
    public int mapId = 1;	//地图，只有一张
}

//战斗结果（服务端推送）
public class MsgBattleResult : MsgBase
{
    public MsgBattleResult() { protoName = "MsgBattleResult"; }
    //服务端回
    public int winCamp = 0;	 //获胜的阵营
}

//玩家退出（服务端推送）
public class MsgLeaveBattle : MsgBase
{
    public MsgLeaveBattle() { protoName = "MsgLeaveBattle"; }
    //服务端回
    public string id = "";	//玩家id
}

public class MsgHit : MsgBase
{
    public MsgHit() { protoName = "MsgHit"; }
    //客户端发
    public int damage = 0;
    public string id = ""; //被击中玩家id
    //服务端暂时无需返回
}

public class MsgHungry : MsgBase
{
    public MsgHungry() { protoName = "MsgHungry"; }

    //Server -> Client掉一格饱食度
    public string id = "";
    //Client -> Server 恢复
    public int hunger; //饥饿值
    public int saturation; //饱食度
}