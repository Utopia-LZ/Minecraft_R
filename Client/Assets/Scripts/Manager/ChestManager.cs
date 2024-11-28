using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : Singleton<ChestManager>
{
    public Dictionary<int, Chest> Chests;
    public ChestPanel SelectedChestPanel;

    public int SelectSlotIndex;
    public Item SelectSlot { get { return SelectedChestPanel.Items[SelectSlotIndex]; } }
    public bool IsOpened
    {
        get
        {
            if(SelectedChestPanel == null) return false;
            return SelectedChestPanel.gameObject.activeSelf;
        }
    }

    public void Init()
    {
        Chests = new();
        EventHandler.OnRefreshChestPanel += RefreshChestPanel;
        NetManager.AddMsgListener("MsgLoadChestContent", OnMsgLoadChesetContent);

        ChestPanel.CHEST_SLOT_COUNT = DataManager.Instance.Config.ChestSlotCount;
    }

    public void Select(int idx)
    {
        if (SelectSlot != null)
            SelectSlot.HighLight.enabled = false;
        SelectSlotIndex = idx;
        SelectSlot.HighLight.enabled = true;
    }

    public void AddChest(int idx, Vector3Int corner)
    {
        GameObject go = ResManager.Instance.GetGameObject(ObjType.Chest);
        go.transform.position = corner.ToVector3() + new Vector3(0.5f, 0.5f, 0.5f);
        Chest newChest = go.GetComponent<Chest>();
        Chests[idx] = newChest;
        newChest.index = idx;
    }

    public void RemoveChest(int idx)
    {
        if (Chests.ContainsKey(idx))
        {
            //GameObject.Destroy(Chests[idx].ChestPanel);
            //GameObject.Destroy(Chests[idx].gameObject);
            ResManager.Instance.RecycleObj(Chests[idx].ChestPanel.gameObject, ObjType.ChestPanel);
            ResManager.Instance.RecycleObj(Chests[idx].gameObject, ObjType.Chest);
            Chests.Remove(idx);
        }
        else { Debug.Log("Chest doesn't exist! " + idx); }
    }

    //idx: Ïä×Ó±àºÅ
    public void RefreshChest(int idx, Slot slot)
    {
        if (Chests.ContainsKey(idx))
        {
            bool active = Chests[idx].ChestPanel.gameObject.activeSelf;
            Chests[idx].ChestPanel.gameObject.SetActive(true);
            Chests[idx].ChestPanel.RrefreshBag(slot);
            Chests[idx].ChestPanel.gameObject.SetActive(active);
        }
        else
        {
            Debug.Log("This Chest doesn't exist");
        }
    }

    private void RefreshChestPanel(ChestPanel panel)
    {
        SelectedChestPanel = panel;
    }

    public void DestroyChest(Vector3Int corner, GameObject go)
    {

        MsgUpdateEntity msg = new MsgUpdateEntity();
        msg.corner = corner;
        msg.id = go.GetComponentInChildren<Chest>().index;
        go.GetComponent<Collider>().enabled = false;
        msg.generate = false;
        NetManager.Send(msg);
    }

    public void Clear()
    {
        foreach(var chest in Chests.Values)
        {
            //GameObject.Destroy(chest.ChestPanel.gameObject);
            //GameObject.Destroy(chest.gameObject);
            ResManager.Instance.RecycleObj(chest.ChestPanel.gameObject, ObjType.ChestPanel);
            ResManager.Instance.RecycleObj(chest.gameObject, ObjType.Chest);
        }
        Chests.Clear();
    }

    private void OnMsgLoadChesetContent(MsgBase msgBase)
    {
        MsgLoadChestContent msg = (MsgLoadChestContent)msgBase;
        if (!Chests.ContainsKey(msg.chestId))
        {
            Debug.Log(msg.chestId + "doesn't exist!");
            return;
        }
        foreach(Slot slot in msg.slots)
        {
            RefreshChest(msg.chestId, slot);
        }
    }
}