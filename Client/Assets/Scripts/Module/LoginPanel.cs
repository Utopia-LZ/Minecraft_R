using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginPanel : BasePanel
{
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public Button loginBtn;
    public Button rgBtn;

    public override void OnAwake()
    {
        //����
        loginBtn.onClick.AddListener(OnLoginClick);
        rgBtn.onClick.AddListener(OnRgClick);
        gameObject.SetActive(false);
    }

    public override void OnInit()
    {
        
    }
    public override void OnShow()
    {
        //����Э�����
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }

    public override void OnClose()
    {
       
    }
    //���ӳɹ��ص�
    private void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
    }
    //����ʧ�ܻص�
    private void OnConnectFail(string err)
    {
        MessageBox.Instance.Show(err);
    }
    public void OnRgClick()
    {
        EventHandler.CallOpenPanel(PanelType.Register);
    }
    public void OnLoginClick()
    {
        if(idInput.text == "" || pwInput.text == "")
        {
            MessageBox.Instance.Show("can't be empty");
            return;
        }
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.id = idInput.text;
        msgLogin.pw = pwInput.text;
        NetManager.Send(msgLogin);
    }
    public void OnMsgLogin(MsgBase msgBase)
    {
        Debug.Log("OnMsgLogin");
        MsgLogin msg = (MsgLogin)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("��½�ɹ�");
            //����id
            GameMain.id = msg.id;
            //�򿪷����б����
            EventHandler.CallOpenPanel(PanelType.RoomList);
            //�رս���
            Close();
        }
        else
        {
            MessageBox.Instance.Show("��½ʧ��");
        }
    }
}