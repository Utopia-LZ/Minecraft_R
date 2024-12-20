﻿public class ChatManager
{
    public string Text = "";

    public ChatManager() { }
    public ChatManager(string data)
    {
        Deserialize(data);
    }

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

    public string Serialize()
    {
        return Text;
    }

    public void Deserialize(string data)
    {
        Text = data;
    }
}