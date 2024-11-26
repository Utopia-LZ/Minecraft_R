using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : BasePanel
{
    public TMP_InputField InputField; //打字框
    public TMP_Text Text; //已发出信息
    public Button BtnSend;
    private Image bg;
    private bool showInput = false;

    private void Update()
    {
        if (showInput) return;

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            SwitchInputBox(true);
        }
    }

    public override void OnAwake()
    {
        bg = GetComponent<Image>();
        bg.enabled = false;
        BtnSend.onClick.AddListener(OnClickSend);
        NetManager.AddMsgListener("MsgLoadChat", OnMsgLoadChat);
        SwitchInputBox(false);
        gameObject.SetActive(false);
    }
    public override void OnInit()
    {
        
    }
    public override void OnShow()
    {
        NetManager.AddMsgListener("MsgChat", OnMsgChat);
    }
    public override void OnClose()
    {
        Text.text = "";
    }

    private void OnClickSend()
    {
        MsgChat msg = new MsgChat();
        msg.text = GameMain.id + ':' + InputField.text + '\n';
        InputField.text = "";
        NetManager.Send(msg);
        SwitchInputBox(false);
    }
    
    private void SwitchInputBox(bool value)
    {
        InputField.gameObject.SetActive(value);
        BtnSend.gameObject.SetActive(value);
        bg.enabled = value;
        showInput = value;
    }

    private void OnMsgChat(MsgBase msgBase)
    {
        MsgChat msg = msgBase as MsgChat;
        Debug.Log("OnMsgChat");
        if(msg.text != "")
        {
            Text.text += msg.text;
        }
        else
        {
            Debug.Log("信息为空");
        }
    }

    private void OnMsgLoadChat(MsgBase msgBase)
    {
        MsgLoadChat msg = (MsgLoadChat) msgBase;
        Text.text = msg.text;
    }
}