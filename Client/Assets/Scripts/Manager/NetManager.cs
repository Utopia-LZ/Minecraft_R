using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using UnityEngine;

public static class NetManager
{
    static Socket socket;
    static ByteArray readBuff;
    static Queue<ByteArray> writeQueue;
    static List<MsgBase> msgList = new();

    static bool isConnecting = false;
    static bool isClosing = false;
    public static bool isUsePing = true;

    static int msgCount = 0; //减少对msgList的访问频率
    readonly static int MAX_MESSAGE_FIRE = 10;
    public static int pingInterval = 30;
    static float lastPingTime = 0;
    static float lastPongTime = 0;

    public enum NetEvent
    {
        ConnectSucc = 1, ConnectFail = 2, Close = 3,
    }

    //事件委托
    public delegate void EventListener(string err);
    private static Dictionary<NetEvent, EventListener> eventListeners = new();

    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] += listener;
        }
        else
        {
            eventListeners.Add(netEvent, listener);
        }
    }
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
        }
    }
    public static void FireEvent(NetEvent netEvent, string err)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent](err);
        }
    }

    //消息委托
    public delegate void MsgListener(MsgBase msgBase);
    private static Dictionary<string, MsgListener> msgListeners = new();

    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += listener;
        }
        else
        {
            msgListeners.Add(msgName, listener);
        }
    }
    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
        }
    }
    public static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }

    public static void Connect(string ip, int port)
    {
        if (socket != null && socket.Connected)
        {
            Debug.Log("Already connected.");
            return;
        }
        if (isConnecting)
        {
            Debug.Log("is Connecting.");
            return;
        }

        InitState();
        socket.NoDelay = true;
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket); 
    }

    public static void InitState()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readBuff = new();
        writeQueue = new();
        msgList = new();
        isClosing = false;
        isConnecting = false;
        msgCount = 0;
        lastPingTime = Time.time;
        lastPongTime = Time.time;

        //监听Pong协议
        if (!msgListeners.ContainsKey("MsgPong")) //非?
        {
            AddMsgListener("MsgPong", OnMsgPong);
        }
    }
    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
            Debug.Log("Socket Connect Succ ");
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
            //开始接收
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx,
                                            readBuff.Remain, 0, ReceiveCallback, socket);

        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Connect fail " + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }

    public static void Close()
    {
        if (socket == null || !socket.Connected || isConnecting){
            return;
        }

        if(writeQueue.Count > 0)
        {
            isClosing = true;
        }
        else
        {
            socket.Close();
            FireEvent(NetEvent.Close,"");
        }
    }

    public static void Send(MsgBase msg, bool print = false)
    {
        if (socket == null || !socket.Connected || isConnecting || isClosing)
        {
            return;
        }

        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int len = nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[len+2];
        sendBytes[0] = (byte)(len%256);
        sendBytes[1] = (byte)(len/256);
        Array.Copy(nameBytes,0,sendBytes,2,nameBytes.Length);
        Array.Copy(bodyBytes,0, sendBytes,2+nameBytes.Length,bodyBytes.Length);
        ByteArray ba = new ByteArray(sendBytes);
        if (print)
        {
            Debug.Log(ba);
        }
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }
        if (count == 1) 
        {
            Int16 bodyLength = (Int16)((sendBytes[1] << 8) | sendBytes[0]);
            socket.BeginSend(sendBytes,0,sendBytes.Length,0,SendCallBack,socket);
        }
    }
    public static void SendCallBack(IAsyncResult ar)
    {
        Socket socket = ar.AsyncState as Socket;
        if (socket == null || !socket.Connected)
            return;

        int count = socket.EndSend(ar);
        ByteArray ba;
        lock (writeQueue)
        {
            ba = writeQueue.First();
        }
        ba.readIdx += count;
        if(ba.Length == 0)
        {
            lock (writeQueue)
            {
                writeQueue.Dequeue();
                ba = writeQueue.First();
            }
        }
        if(ba != null)
        {
            socket.BeginSend(ba.bytes,ba.readIdx,ba.Length,0,SendCallBack,socket);
        }
        else if (isClosing)
        {
            socket.Close();
        }
    }
    public static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            int count = socket.EndReceive(ar);
            readBuff.writeIdx += count;
            OnReceiveData();
            if (readBuff.Remain < 8)
            {
                readBuff.MoveBytes();
                readBuff.Resize(readBuff.Length * 2);
            }
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.Remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Receive fail: " + e.ToString());
        }
    }
    public static void OnReceiveData()
    {
        if (readBuff.Length <= 2)
            return;

        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if (readBuff.Length < bodyLength)
            return;
        readBuff.readIdx += 2;
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == "")
        {
            Debug.Log("OnReceiveData MsgBase.DecodeName fail");
            return;
        }
        readBuff.readIdx += nameCount;
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        lock (msgList)
        {
            msgList.Add(msgBase);
            msgCount++;
        }
        if(readBuff.Length > 2)
        {
            OnReceiveData();
        }
    }

    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }
    public static void MsgUpdate()
    {
        if (msgCount == 0)
            return;
        for(int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            MsgBase msgBase = null;
            lock (msgList)
            {
                if(msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0); //开销?
                    msgCount--;
                }
            }
            if(msgBase != null)
            {
                FireMsg(msgBase.protoName,msgBase);
            }
            else
            {
                break;
            }
        }
    }
    public static void PingUpdate()
    {
        if (!isUsePing)
            return;
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }
        if (Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }
    private static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }
}