using System;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

public class MsgBase
{
    public string protoName = "null";
    //������
    static JavaScriptSerializer Js = new JavaScriptSerializer();
    //����
    public static byte[] Encode(MsgBase msgBase)
    {
        //string s = Js.Serialize(msgBase);
        string s = JsonConvert.SerializeObject(msgBase);
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    //����
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
        //MsgBase msgBase = (MsgBase)Js.Deserialize(s, Type.GetType(protoName));
        Type type = Type.GetType(protoName);
        MsgBase msgBase = (MsgBase)JsonConvert.DeserializeObject(s,type);
        return msgBase;
    }
    //����Э����(2�ֽڳ���+�ַ���)
    public static byte[] EncodeName(MsgBase msg)
    {
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msg.protoName);
        Int16 len = (Int16)nameBytes.Length;
        byte[] bytes = new byte[2+len];
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes,0,bytes,2,len);

        return bytes;
    }
    //����Э����(2�ֽڳ���+�ַ���)
    public static string DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        if(offset + 2 > bytes.Length)
        {
            return "";
        }
        Int16 len = (Int16)((bytes[offset+1] << 8) | bytes[offset]);
        if(offset + 2 + len > bytes.Length)
        {
            return "";
        }
        count += 2 + len;
        string name = System.Text.Encoding.UTF8.GetString(bytes,offset+2,len);
        return name;
    }
}