using UnityEngine;

public class SyncSteve : BaseSteve
{
    //Ԥ����Ϣ
    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 forecastPos;
    private Vector3 forecastRot;
    private float forecastTime;

    public override void Init()
    {
        base.Init();
        //���������˶�Ӱ��
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        //��ʼ��Ԥ����Ϣ
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

    //�ƶ�ͬ��
    public void SyncPos(MsgSyncSteve msg)
    {
        //Ԥ��λ��
        Vector3 pos = Vector3Int.V3IntToV3(msg.pos);
        Vector3 rot = Vector3Int.V3IntToV3(msg.rot);
        //forecastPos = pos + 2*(pos - lastPos);
        //forecastRot = rot + 2*(rot - lastRot);
        forecastPos = pos;  //���治Ԥ��
        forecastRot = rot;
        //����
        lastPos = pos;
        lastRot = rot;
        forecastTime = Time.time;
    }

    //����λ��
    public void ForecastUpdate()
    {
        //ʱ��
        float t = (Time.time - forecastTime) / CtrlSteve.syncInterval;
        t = Mathf.Clamp(t, 0f, 1f);
        //λ��
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos, t);
        transform.position = pos;
        //��ת
        Quaternion quat = transform.rotation;
        Quaternion forcastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forcastQuat, t);
        transform.rotation = quat;
    }
}