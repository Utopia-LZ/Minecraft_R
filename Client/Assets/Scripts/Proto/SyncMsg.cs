public class MsgSyncSteve : MsgBase
{
    public MsgSyncSteve() { protoName = "MsgSyncSteve"; }
    //λ�ã�����
    public Vector3Int pos;
    public Vector3Int rot;
    //����˲���
    public string id = "";
}

//ͬ����ʬ��Ϣ
public class MsgSyncZombie : MsgBase
{
    public MsgSyncZombie() { protoName = "MsgSyncZombie"; }

    //λ�á�����
    public Vector3Int pos;
    public Vector3Int forward;
    //����˲���
    public string id = "";		//�ĸ���ʬ
}