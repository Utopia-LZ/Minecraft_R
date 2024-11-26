//生成/销毁僵尸
public class MsgUpdateZombie : MsgBase
{
    public MsgUpdateZombie() { protoName = "MsgUpdateZombie"; }

    public CharacterInfo info;
    public bool generate = true;
}

public class MsgZombieAttack : MsgBase
{
    public MsgZombieAttack() { protoName = "MsgZombieAttack"; }

    public string zombieId = "";
    public string playerId = "";
}

public class MsgZombieHit : MsgBase
{
    public MsgZombieHit() { protoName = "MsgZombieHit"; }

    public string zombieId = "";
    public int damage;
}

public class MsgLoadZombie : MsgBase
{
    public MsgLoadZombie() { protoName = "MsgLoadZombie"; }

    public CharacterInfo[] zombies;
}