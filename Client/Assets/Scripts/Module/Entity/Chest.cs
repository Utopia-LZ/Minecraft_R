using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Chest : MonoBehaviour, PoolObject
{
    public ChestPanel ChestPanel;
    private Transform Root;

    public int index;

    private void Start()
    {
        Root = GameObject.Find("Panel").transform;
        GameObject go = ResManager.Instance.GetGameObject(ObjType.ChestPanel,Root);
        ChestPanel = go.GetComponent<ChestPanel>();
        ChestPanel.idx = index;
        ChestPanel.InitSlot();
    }

    private void Update()
    {
        if (!ChestPanel.gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChestPanel.Close();
            EventHandler.CallRefreshChestPanel(null);
            SoundManager.Instance.PlaySound(ObjType.MusicChest);
            //:¹ØÏä×Ó¶¯»­
        }
    }

    public void OpenChest()
    {
        SoundManager.Instance.PlaySound(ObjType.MusicChest);
        ChestPanel.gameObject.SetActive(true);
        EventHandler.CallRefreshChestPanel(ChestPanel);
        if (ChestPanel == null)
        {
            Debug.LogError("Open Chest ChestPanel is null");
        }
    }

    public void OnRecycle()
    {
        Destroy(this);
    }
}