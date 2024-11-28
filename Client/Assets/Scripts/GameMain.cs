using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
public class GameMain : MonoBehaviour
{
    public static string id = "";
    private void Start()
    {
        StartCoroutine(SyncInit());
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);

        //连接服务端
        NetManager.Connect("127.0.0.1", 8888);
    }

    IEnumerator SyncInit()
    {
        yield return DataManager.Instance.Init();
        yield return ABManager.Instance.Init();
        ResManager.Instance.Init();
        BattleManager.Init();
        MapManager.Init();
        BagManager.Instance.Init();
        ItemManager.Instance.Init();
        ChestManager.Instance.Init();
        EntityManager.Instance.Init();
        BombManager.Instance.Init();
        ZombieManager.Instance.Init();
        LightManager.Instance.Init();
        EventHandler.CallAfterLoadRes();
        EventHandler.CallOpenPanel(PanelType.Login);
    }

    private void OnDestroy()
    {
        NetManager.Close();
        LightManager.Instance.StopTimer();
    }

    private void Update()
    {
        NetManager.Update();
        LightManager.Instance.Update();
    }

    private void OnConnectClose(string err)
    {
        Debug.Log("断开连接");
    }
    private void OnMsgKick(MsgBase msgBase)
    {
        MessageBox.Instance.Show("被踢下线");
    }
}