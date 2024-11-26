using System;
using System.Collections.Generic;

public static class EventHandler
{
    public static Dictionary<int, Action> MouseEvents = new();

    public delegate void OpenPanel(PanelType type);
    public static event OpenPanel OnOpenPanel;
    
    public delegate void ClosePanel(PanelType type);
    public static event ClosePanel OnClosePanel;

    public delegate void HPChanged(float rate);
    public static event HPChanged OnHPChanged;

    public delegate void EnterRoom();
    public static event EnterRoom OnEnterRoom;

    public delegate void LeaveRoom();
    public static event LeaveRoom OnLeaveRoom;

    public delegate void RefreshChestPanel(ChestPanel panel);
    public static event RefreshChestPanel OnRefreshChestPanel;

    public static void CallMouseEvent(int layer)
    {
        if (MouseEvents.ContainsKey(layer))
        {
            MouseEvents[layer]?.Invoke();
        }
    }

    public static void CallOpenPanel(PanelType type)
    {
        OnOpenPanel?.Invoke(type);
    }
    public static void CallClosePanel(PanelType type)
    {
        OnClosePanel?.Invoke(type);
    }
    public static void CallHPChanged(float rate)
    {       
        OnHPChanged?.Invoke(rate);
    }
    public static void CallEnterRoom()
    {
        OnEnterRoom?.Invoke();
    }
    public static void CallLeaveRoom()
    {
        OnLeaveRoom?.Invoke();
    }
    public static void CallRefreshChestPanel(ChestPanel panel)
    {
        OnRefreshChestPanel?.Invoke(panel);
    }
}