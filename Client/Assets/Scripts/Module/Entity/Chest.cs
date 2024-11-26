using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Chest : MonoBehaviour
{
    public ChestPanel ChestPanel;
    private GameObject PanelPrefab;
    private Transform Root;

    public int index;

    private void Start()
    {
        PanelPrefab = ResManager.LoadResources<GameObject>("prefab_chestpanel");
        Root = GameObject.Find("Panel").transform;
        ChestPanel = Instantiate(PanelPrefab, Root).GetComponent<ChestPanel>();
        ChestPanel.idx = index;
    }

    private void Update()
    {
        if (!ChestPanel.gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChestPanel.Close();
            EventHandler.CallRefreshChestPanel(null);
            //:¹ØÏä×Ó¶¯»­
        }
    }

    public void OpenChest()
    {
        ChestPanel.gameObject.SetActive(true);
        EventHandler.CallRefreshChestPanel(ChestPanel);
        if (ChestPanel == null)
        {
            Debug.Log("Open Chest ChestPanel is null");
        }
    }
}