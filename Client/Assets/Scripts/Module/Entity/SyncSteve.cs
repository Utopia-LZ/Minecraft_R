using UnityEngine;

public class SyncSteve : BaseSteve
{
    //预测信息
    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 forecastPos;
    private Vector3 forecastRot;
    private float forecastTime;

    public override void Init()
    {
        base.Init();
        //不受物理运动影响
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        //初始化预测信息
        lastPos = transform.position;
        lastRot = transform.eulerAngles;
        forecastPos = transform.position;
        forecastRot = transform.eulerAngles;
        forecastTime = Time.time;
    }

    private void Update()
    {
        ForecastUpdate();
    }

    //移动同步
    public void SyncPos(MsgSyncSteve msg)
    {
        //预测位置
        Vector3 pos = Vector3Int.V3IntToV3(msg.pos);
        Vector3 rot = Vector3Int.V3IntToV3(msg.rot);
        //forecastPos = pos + 2*(pos - lastPos);
        //forecastRot = rot + 2*(rot - lastRot);
        forecastPos = pos;  //跟随不预测
        forecastRot = rot;
        //更新
        lastPos = pos;
        lastRot = rot;
        forecastTime = Time.time;
    }

    //更新位置
    public void ForecastUpdate()
    {
        //时间
        float t = (Time.time - forecastTime) / CtrlSteve.syncInterval;
        t = Mathf.Clamp(t, 0f, 1f);
        //位置
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos, t);
        transform.position = pos;
        //旋转
        Quaternion quat = transform.rotation;
        Quaternion forcastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forcastQuat, t);
        transform.rotation = quat;
    }
}