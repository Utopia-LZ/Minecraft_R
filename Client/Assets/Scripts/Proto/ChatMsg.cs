public class MsgChat : MsgBase
{
    public MsgChat() { protoName = "MsgChat"; }
    //�ͻ����ͣ�����˻�
    public string text = "";
}

public class MsgLoadChat : MsgBase
{
    public MsgLoadChat() { protoName = "MsgLoadChat"; }

    public string text = "";
}