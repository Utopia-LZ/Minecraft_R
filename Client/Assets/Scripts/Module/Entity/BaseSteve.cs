using System.Collections;
using UnityEngine;

public class BaseSteve : MonoBehaviour, PoolObject
{
    public static int HP;
    public static int Hunger;
    public static int Damage;

    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed;
    public Rigidbody rb;
    public MeshRenderer mesh;
    public int hp;
    public float maxHP;
    public int damage;
    public string id = "";
    public Kind Kind = Kind.None; //生物类别

    public float hpRate {  get { return hp/maxHP; } }

    public virtual void Init()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        walkSpeed = CtrlSteve.WalkSpeed;
        runSpeed = CtrlSteve.RunSpeed;
        rotateSpeed = CtrlSteve.RotateSpeed;
        damage = Damage;
        maxHP = HP;
    }

    public bool IsDie()
    {
        return hp <= 0;
    }

    public void Attacked(int damage)
    {
        Debug.Log("Attacked " + hp + " " + damage);
        if (hp-damage >= CtrlSteve.HP || hp-damage < 0) return;
        hp -= damage;
        StartCoroutine(DelayHurt());
        if(id == GameMain.id)
        {
            EventHandler.CallHPChanged(hp);
            SoundManager.Instance.PlaySound(ObjType.MusicHurt);
        }
        else 
        {
            Debug.Log("Sync Attacked");
        }
    }

    IEnumerator DelayHurt()
    {
        mesh.materials[0].color = Color.red;
        yield return new WaitForSeconds(0.2f);
        mesh.material.color = Color.white;
    }

    public void OnRecycle()
    {
        Destroy(GetComponent<CameraFollow>());
        Destroy(this);
    }
}