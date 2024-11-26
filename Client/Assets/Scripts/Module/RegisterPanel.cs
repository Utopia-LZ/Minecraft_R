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
        //����
        regBtn.onClick.AddListener(OnRgClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        gameObject.SetActive(false);
    }

    public override void OnInit()
    {
        
    }
    public override void OnShow()
    {
        //����Э�����
        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    }
    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
    }

    //����ע�ᰴť
    public void OnRgClick()
    {
        if(idInput.text == "" || pwInput.text == "")
        {
            MessageBox.Instance.Show("����Ϊ��");
            return;
        }
        if (rpInput.text != pwInput.text)
        {
            MessageBox.Instance.Show("������������벻ͬ");
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
            MessageBox.Instance.Show("ע��ɹ�");
            Close();
        }
        else{
            MessageBox.Instance.Show("ע��ʧ��");
        }
    }
}