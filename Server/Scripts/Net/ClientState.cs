using System.Net.Sockets;

public class ClientState
{
    public Socket socket;
    public ByteArray readBuff = new();
    public long lastPingTime = 0;
    public Player player;
}