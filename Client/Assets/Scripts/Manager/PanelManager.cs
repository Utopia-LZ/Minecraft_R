using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    private Dictionary<PanelType, BasePanel> panels;

    private void Awake()
    {
        panels = new Dictionary<PanelType, BasePanel>();
        panels[PanelType.Login] = GetComponentInChildren<LoginPanel>();
        panels[PanelType.Register] = GetComponentInChildren<RegisterPanel>();
        //panels[PanelType.Room] = GetComponentInChildren<RoomPanel>();
        panels[PanelType.RoomList] = GetComponentInChildren<RoomListPanel>();
        panels[PanelType.Battle] = GetComponentInChildren<BattlePanel>();
        panels[PanelType.Chat] = GetComponentInChildren<ChatPanel>();
        
        foreach (var panel in panels.Values)
        {
            panel.OnAwake();
        }

        EventHandler.OnOpenPanel += Open;
        EventHandler.OnClosePanel += Close;
    }
    private void OnDestroy()
    {
        EventHandler.OnOpenPanel -= Open;
        EventHandler.OnClosePanel -= Close;
    }

    public void Open(PanelType type)
    {
        panels[type].gameObject.SetActive(true);
        panels[type].OnShow();
    }
    public void Close(PanelType type)
    {
        panels[type].gameObject.SetActive(false);
        panels[type].OnClose();
    }
}