using UnityEngine;

public class BaseSteve : MonoBehaviour
{
    public static int HP;
    public static int Damage;

    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed;
    public Rigidbody rb;
    public int hp;
    public float maxHP;
    public int damage;
    public string id = "";
    public Kind Kind = Kind.None; //生物类别

    public float hpRate {  get { return hp/maxHP; } }

    public virtual void Init()
    {
        rb = GetComponent<Rigidbody>();
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
        hp -= damage;
        if(id == GameMain.id)
        {
            EventHandler.CallHPChanged(hp);
        }
        else 
        {
            Debug.Log("Sync Attacked");
        }
    }
}