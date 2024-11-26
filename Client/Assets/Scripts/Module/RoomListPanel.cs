using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    
    public Text idText;//�˺��ı�
    public Text scoreText;//ս���ı�    
    public Button createButton;//�������䰴ť
    public Button reflashButton;//ˢ���б�ť
    public Transform content;//�б�����
    public GameObject roomObj;//��������

    public override void OnAwake()
    {
        //�������
        roomObj.SetActive(false);
        //��ʾid
        idText.text = GameMain.id;
        //��ť�¼�
        createButton.onClick.AddListener(OnCreateClick);
        reflashButton.onClick.AddListener(OnReflashClick);
        gameObject.SetActive(false);
    }

    //��ʼ��
    public override void OnInit()
    {
    }

    //��ʾ
    public override void OnShow()
    {
        //Э�����
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);

        //���Ͳ�ѯ
        MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }


    //�ر�
    public override void OnClose()
    {
        //Э�����
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
    }

    //�յ��ɼ���ѯЭ��
    public void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        scoreText.text = msg.win + "ʤ " + msg.lost + "��";
    }

    //�յ������б�Э��
    public void OnMsgGetRoomList(MsgBase msgBase)
    {
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        //��������б�
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            GameObject o = content.GetChild(i).gameObject;
            Destroy(o);
        }
        //���������б�
        if (msg.rooms == null)
        {
            return;
        }
        for (int i = 0; i < msg.rooms.Length; i++)
        {
            GenerateRoom(msg.rooms[i]);
        }
    }

    //����һ�����䵥Ԫ
    public void GenerateRoom(RoomInfo roomInfo)
    {
        //��������
        GameObject o = Instantiate(roomObj);
        o.transform.SetParent(content);
        o.SetActive(true);
        o.transform.localScale = Vector3.one;
        //��ȡ���
        Transform trans = o.transform;
        Text idText = trans.Find("IdText").GetComponent<Text>();
        Text countText = trans.Find("CountText").GetComponent<Text>();
        Text statusText = trans.Find("StatusText").GetComponent<Text>();
        Button btn = trans.Find("JoinButton").GetComponent<Button>();
        //�����Ϣ
        idText.text = roomInfo.id.ToString();
        countText.text = roomInfo.count.ToString();
        if (roomInfo.status == 0)
        {
            statusText.text = "׼����";
        }
        else
        {
            statusText.text = "ս����";
        }
        //��ť�¼�
        btn.name = idText.text;
        btn.onClick.AddListener(delegate () {
            OnJoinClick(btn.name);
        });
    }

    //���ˢ�°�ť
    public void OnReflashClick()
    {
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
    }

    //������뷿�䰴ť
    public void OnJoinClick(string idString)
    {
        MsgEnterRoom msg = new MsgEnterRoom();
        msg.id = int.Parse(idString);
        NetManager.Send(msg);
    }

    //����½����䰴ť
    public void OnCreateClick()
    {
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }

    //�յ��½�����Э��
    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        //:sometimes can't receive success message
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        //�ɹ���������
        if (msg.result == 0 && msg.id != -1)
        {
            MessageBox.Instance.Show("�����ɹ�");
            //EventHandler.CallOpenPanel(PanelType.Room);
            MsgEnterRoom emsg = new MsgEnterRoom();
            emsg.id = msg.id;
            NetManager.Send(emsg);
            Close();
        }
        //��������ʧ��
        else
        {
            MessageBox.Instance.Show("��������ʧ��");
        }
    }
}
