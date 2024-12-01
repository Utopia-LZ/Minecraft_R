using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    
    public TMP_Text idText;//账号文本  
    public Button createButton;//创建房间按钮
    public Button reflashButton;//刷新列表按钮
    public Transform content;//列表容器
    public GameObject roomObj;//房间物体

    public override void OnAwake()
    {
        //不激活房间
        roomObj.SetActive(false);
        //显示id
        idText.text = GameMain.id;
        //按钮事件
        createButton.onClick.AddListener(OnCreateClick);
        reflashButton.onClick.AddListener(OnReflashClick);
        gameObject.SetActive(false);
    }

    //初始化
    public override void OnInit()
    {
    }

    //显示
    public override void OnShow()
    {
        //协议监听
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);

        //发送查询
        MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }


    //关闭
    public override void OnClose()
    {
        //协议监听
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
    }

    //收到成绩查询协议
    public void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
    }

    //收到房间列表协议
    public void OnMsgGetRoomList(MsgBase msgBase)
    {
        Debug.Log("OnMsgGetRoomList");
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        //清除房间列表
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject o = content.GetChild(i).gameObject;
            Destroy(o);
        }
        //重新生成列表
        if (msg.rooms == null)
        {
            return;
        }
        for (int i = 0; i < msg.rooms.Length; i++)
        {
            GenerateRoom(msg.rooms[i]);
        }
    }

    //创建一个房间单元
    public void GenerateRoom(RoomInfo roomInfo)
    {
        //创建物体
        GameObject o = Instantiate(roomObj);
        o.transform.SetParent(content);
        o.SetActive(true);
        o.transform.localScale = Vector3.one;
        //获取组件
        Transform trans = o.transform;
        TMP_Text idText = trans.Find("IdText").GetComponent<TMP_Text>();
        TMP_Text countText = trans.Find("CountText").GetComponent<TMP_Text>();
        Button btn = trans.Find("JoinButton").GetComponent<Button>();
        //填充信息
        idText.text = roomInfo.id.ToString();
        countText.text = roomInfo.count.ToString();
        //按钮事件
        btn.name = idText.text;
        btn.onClick.AddListener(delegate () {
            OnJoinClick(btn.name);
        });
    }

    //点击刷新按钮
    public void OnReflashClick()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }

    //点击加入房间按钮
    public void OnJoinClick(string idString)
    {
        MsgEnterRoom msg = new MsgEnterRoom();
        msg.id = int.Parse(idString);
        NetManager.Send(msg);
        PanelManager.Instance.Open(PanelType.Wait);
    }

    //点击新建房间按钮
    public void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
        PanelManager.Instance.Open(PanelType.Wait);
    }

    //收到新建房间协议
    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        //:sometimes can't receive success message
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        //成功创建房间
        if (msg.result == 0 && msg.id != -1)
        {
            MessageBox.Instance.Show("创建成功");
            //EventHandler.CallOpenPanel(PanelType.Room);
            MsgEnterRoom emsg = new MsgEnterRoom();
            emsg.id = msg.id;
            NetManager.Send(emsg);
            Close();
        }
        //创建房间失败
        else
        {
            MessageBox.Instance.Show("创建房间失败");
        }
    }
}
