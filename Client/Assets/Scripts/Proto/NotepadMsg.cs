//��ȡ���±�����
public class MsgGetText : MsgBase
{
    public MsgGetText() { protoName = "MsgGetText"; }
    //����˻�
    public string text = "";
}

//������±�����
public class MsgSaveText : MsgBase
{
    public MsgSaveText() { protoName = "MsgSaveText"; }
    //�ͻ��˷�
    public string text = "";
    //����˻أ�0-�ɹ� 1-����̫����
    public int result = 0;
}