using UnityEngine;

public enum PanelType
{
    Base,
    Login,
    Register,
    RoomList,
    Room,
    Battle,
    Chat,
    Wait,
}

public class BasePanel : MonoBehaviour
{
    public void Init()
    {
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnAwake() { }
    public virtual void OnInit() { }
    public virtual void OnShow() { }
    public virtual void OnClose() { }
}