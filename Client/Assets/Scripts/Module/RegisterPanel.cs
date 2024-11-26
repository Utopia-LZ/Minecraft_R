using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public TMP_InputField rpInput;

    public Button regBtn;
    public Button closeBtn;

    public override void OnAwake()
    {
        //监听
        regBtn.onClick.AddListener(OnRgClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        gameObject.SetActive(false);
    }

    public override void OnInit()
    {
        
    }
    public override void OnShow()
    {
        //网络协议监听
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    }
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
    }

    //按下注册按钮
    public void OnRgClick()
    {
        if(idInput.text == "" || pwInput.text == "")
        {
            MessageBox.Instance.Show("不能为空");
            return;
        }
        if (rpInput.text != pwInput.text)
        {
            MessageBox.Instance.Show("两次输入的密码不同");
            return;
        }
        MsgRegister msgRg = new MsgRegister();
        msgRg.id = idInput.text;
        msgRg.pw = pwInput.text;
        NetManager.Send(msgRg);
    }
    public void OnCloseClick()
    {
        Close();
    }
    public void OnMsgRegister(MsgBase msgBase)
    {
        Debug.Log("OnMsgRegister");
        MsgRegister msg = (MsgRegister)msgBase;
        if(msg.result == 0)
        {
            MessageBox.Instance.Show("注册成功");
            Close();
        }
        else{
            MessageBox.Instance.Show("注册失败");
        }
    }
}