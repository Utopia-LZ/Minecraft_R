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
        //监听
        loginBtn.onClick.AddListener(OnLoginClick);
        rgBtn.onClick.AddListener(OnRgClick);
        gameObject.SetActive(false);
    }

    public override void OnInit()
    {
        
    }
    public override void OnShow()
    {
        //网络协议监听
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }

    public override void OnClose()
    {
       
    }
    //连接成功回调
    private void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
    }
    //连接失败回调
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
            Debug.Log("登陆成功");
            //设置id
            GameMain.id = msg.id;
            //打开房间列表界面
            EventHandler.CallOpenPanel(PanelType.RoomList);
            //关闭界面
            Close();
        }
        else
        {
            MessageBox.Instance.Show("登陆失败");
        }
    }
}