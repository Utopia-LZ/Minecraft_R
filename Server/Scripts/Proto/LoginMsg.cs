public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }
    public string id = "";
    public string pw = "";
    //server return
    public int result = 0;
}
public class MsgLogin : MsgBase
{
    public MsgLogin() { protoName = "MsgLogin"; }
    public string id = "";
    public string pw = "";
    //server return
    public int result = 0;

}
public class MsgKick : MsgBase
{
    public MsgKick() { protoName = "MsgKick"; }
    //0 �����˵�¼ͬһ�˺�
    public int reason = 0;
}