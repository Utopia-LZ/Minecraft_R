using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{
    public Button btnLeave;
    private CtrlSteve steve;
    public Image[] hearts;
    public int liveCount;
    public Image[] meats;
    public int meatCount;

    private void Start()
    {
        InitAim();
        BattleManager.Panel = this;
    }

    public override void OnAwake()
    {
        liveCount = hearts.Length;
        meatCount = meats.Length;
        btnLeave.onClick.AddListener(OnClickLeave);
        EventHandler.OnHPChanged += RefreshHp;
        gameObject.SetActive(false);
    }

    //初始化
    public override void OnInit()
    {
    }
    //显示
    public override void OnShow()
    {
        steve = (CtrlSteve)BattleManager.GetCtrlSteve();
        if (steve != null)
        {
            RefreshHp(steve.hp);
        }
    }

    private void OnClickLeave()
    {
        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }

    //更新hp
    public void RefreshHp(int value)
    {
        Debug.Log("RefreshHp " + value);
        int delta = value - liveCount;
        if(delta > 0)
        {
            for(int i = liveCount; i < value; i++)
            {
                hearts[i].enabled = true;
            }
        }
        else if(delta < 0)
        {
            for(int i = liveCount-1; i >= value; i--)
            {
                hearts[i].enabled = false;
            }
        }
        liveCount = value;
    }

    public void RefreshHunger(int delta)
    {
        Debug.Log("RefreshHunger: " + delta);
        if(delta > 0)
        {
            int maxMeat = Mathf.Min(meatCount+delta, CtrlSteve.Hunger);
            for (int i = meatCount; i < maxMeat; i++)
            {
                meats[i].enabled = true;
            }
            meatCount = maxMeat;
        }
        else if(delta < 0)
        {
            int minMeat = Mathf.Max(meatCount+delta, 0);
            for (int i = meatCount-1; i >= minMeat; i--)
            {
                meats[i].enabled = false;
            }
            meatCount = minMeat;
        }
    }

    public void InitAim()
    {
        Material mat = ResManager.Instance.LoadResources<Material>(ObjType.MatAim);
        Transform aimRoot = GameObject.Find("AimStar").transform;
        Debug.Log("InitAim");
        foreach (Transform aim in aimRoot)
        {
            aim.gameObject.GetComponent<Image>().material = mat;
            Debug.Log("InitAim child");
        }
    }

    //关闭
    public override void OnClose()
    {
        
    }
}
