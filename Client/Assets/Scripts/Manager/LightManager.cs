using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class LightManager : Singleton<LightManager>
{
    public static int CYCLE = 24000;
    public static int TICK = 50;

    public Dictionary<int, Light> Lights;

    private Timer timer;
    private Transform Sun;
    public int Time;
    public bool paused = true;

    public void Init()
    {
        NetManager.AddMsgListener("MsgTime", OnMsgTime);
        CYCLE = DataManager.Instance.Config.CycleTime;
        TICK = DataManager.Instance.Config.TickTime;

        paused = true;
        Lights = new Dictionary<int, Light>();
        GameObject go = GameObject.Find("Directional Light");
        Sun = go.transform;
        timer = new Timer(TICK);
        timer.Elapsed += SyncUpdate;
        timer.AutoReset = true;
        timer.Enabled = true;
        timer.Start();
    }

    public void Pause(bool paused)
    {
        this.paused = paused;
    }

    private void SyncUpdate(object sender, ElapsedEventArgs e)
    {
        if (paused) return;
        Time++;
    }

    public void Update()
    {
        if (paused) return;
        Sun.eulerAngles = (float)Time / CYCLE * 360 * Vector3.right;
    }

    public void SetTime(int time)
    {
        Time = time;
        Sun.eulerAngles = Time / CYCLE * 360 * Vector3.up;
        Debug.Log("SetTime: " +  Time);
    }

    public void AddLight(int idx, Vector3Int corner)
    {
        GameObject go = ResManager.Instance.GetGameObject(ObjType.Light);
        go.transform.position = corner.ToVector3() + new Vector3(0.5f, 0.5f, 0.5f);
        Light newLight = go.GetComponent<Light>();
        Lights[idx] = newLight;
        newLight.idx = idx;
    }

    public void RemoveLight(int idx)
    {
        if (Lights.ContainsKey(idx))
        {
            //GameObject.Destroy(Lights[idx].gameObject);
            ResManager.Instance.RecycleObj(Lights[idx].gameObject, ObjType.Light);
            Lights.Remove(idx);
        }
        else { Debug.Log("Light doesn't exist! " + idx); }
    }

    public void Clear()
    {
        foreach (var light in Lights.Values)
        {
            //GameObject.Destroy(light.gameObject);
            ResManager.Instance.RecycleObj(light.gameObject, ObjType.Light);
        }
        Lights.Clear();
    }

    private void OnMsgTime(MsgBase msgBase)
    {
        MsgTime msg = (MsgTime)msgBase;
        SetTime(msg.Time);
    }

    public void StopTimer()
    {
        timer?.Stop();
        timer?.Dispose();
    }
}