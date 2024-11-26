using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{
    public Button btnLeave;
    public Slider slider;
    private BaseSteve steve;

    private void Start()
    {
        InitAim();
    }

    public override void OnAwake()
    {
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
        steve = BattleManager.GetCtrlSteve();
        if (steve != null)
        {
            RefreshHp(steve.hpRate);
        }
    }

    private void OnClickLeave()
    {
        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }

    //更新hp
    private void RefreshHp(float value)
    {
        slider.value = value;
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
