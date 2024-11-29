public enum Kind
{
    None,Steve, Zombie
}

//�����Ϣ
[System.Serializable]
public class CharacterInfo
{
    public string id = "";  //���id
    public Kind Kind = Kind.None; //�������
    public int hp = 0;      //����ֵ

    public Vector3Int pos;
    public Vector3Int rot;
}

public class MsgHit : MsgBase
{
    public MsgHit() { protoName = "MsgHit"; }
    //�ͻ��˷�
    public int damage = 0;
    public string id = ""; //���������id
    //�������ʱ���践��
}

//����/���ٽ�ʬ
public class MsgUpdateZombie : MsgBase
{
    public MsgUpdateZombie() { protoName = "MsgUpdateZombie"; }

    public CharacterInfo info;
    public bool generate = true;
}

public class MsgHungry : MsgBase
{
    public MsgHungry() { protoName = "MsgHungry"; }

    //Server -> Client��һ��ʳ��
    public string id = "";
    //Client -> Server �ָ�
    public int hunger; //����ֵ
    public int saturation; //��ʳ��
}