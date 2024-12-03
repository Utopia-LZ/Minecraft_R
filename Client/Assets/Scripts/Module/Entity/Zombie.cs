using UnityEngine;

public class Zombie : BaseSteve, PoolObject
{
    public static int RecoverHunger;
    public static int RecoverSaturation;
    public static int TryTauntInterval;
    public static int TauntChance;

    private float counter = 0;

    private void Update()
    {
        counter += Time.deltaTime;
        if(counter >= TryTauntInterval)
        {
            counter = 0;
            int rdm = Random.Range(0, TauntChance);
            if(rdm == 1)
            {
                SoundManager.Instance.PlaySound(ObjType.MusicTaunt);
            }
        }
    }

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

    public void TakeDamage(int damage)
    {
        Attacked(damage);
    }

    public new void OnRecycle()
    {
        Destroy(this);
    }
}