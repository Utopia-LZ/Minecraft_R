public class MsgChat : MsgBase
{
    public MsgChat() { protoName = "MsgChat"; }
    //客户端送，服务端回
    public string text = "";
}

public class MsgLoadChat : MsgBase
{
    public MsgLoadChat() { protoName = "MsgLoadChat"; }

    public string text = "";
}