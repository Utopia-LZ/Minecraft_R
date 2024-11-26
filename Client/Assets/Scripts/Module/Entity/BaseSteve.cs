using UnityEngine;

public class BaseSteve : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    public float rotateSpeed;
    public Rigidbody rb;
    public float hp;
    public float maxHP;
    public int damage;
    public string id = "";
    public Kind Kind = Kind.None; //生物类别

    public float hpRate {  get { return hp/maxHP; } }

    public virtual void Init()
    {
        rb = GetComponent<Rigidbody>();
        walkSpeed = 10f;
        runSpeed = 20f;
        rotateSpeed = 40f;
        damage = 1;
        maxHP = 10f;
    }

    public bool IsDie()
    {
        return hp <= 0;
    }

    public void Attacked(float damage)
    {
        hp -= damage;
        if(id == GameMain.id)
        {
            EventHandler.CallHPChanged(hpRate);
        }
        else 
        {
            Debug.Log("Sync Attacked");
        }
    }
}