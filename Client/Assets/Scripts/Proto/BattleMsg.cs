public enum Kind
{
    None,Steve, Zombie
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

public class MsgHit : MsgBase
{
    public MsgHit() { protoName = "MsgHit"; }
    //客户端发
    public int damage = 0;
    public string id = ""; //被击中玩家id
    //服务端暂时无需返回
}

//生成/销毁僵尸
public class MsgUpdateZombie : MsgBase
{
    public MsgUpdateZombie() { protoName = "MsgUpdateZombie"; }

    public CharacterInfo info;
    public bool generate = true;
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