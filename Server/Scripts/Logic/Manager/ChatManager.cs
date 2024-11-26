public class ChatManager
{
    public string Text = "";

    public void LoadChatText(Player player)
    {
        if (Text == "") return;
        MsgLoadChat msg = new MsgLoadChat();
        msg.text = Text;
        player.Send(msg);
    }

    public void AddText(string text)
    {
        Text += text;
    }
}