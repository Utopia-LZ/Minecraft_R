using UnityEngine;

public class Zombie : BaseSteve, PoolObject
{

    public override void Init()
    {
        base.Init();
        //不受物理运动影响
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
    }

    public void SyncPos(MsgSyncZombie msg)
    {
        Vector3 pos = Vector3Int.V3IntToV3(msg.pos);
        transform.position = pos;
        transform.forward = Vector3Int.V3IntToV3(msg.forward).normalized;
    }

    public void AttackAnim()
    {
        //: animation
    }

    public void TakeDamage(int damage)
    {
        Attacked(damage);
        //: animation
    }

    public void OnRecycle()
    {
        hp = 10;
    }
}