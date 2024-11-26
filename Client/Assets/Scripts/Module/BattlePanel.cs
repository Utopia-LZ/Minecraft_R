using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{
    public Button btnLeave;
    public Slider slider;
    private BaseSteve steve;

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


    //更新信息
    /*private void RefreshCampInfo()
    {
        int count1 = 0;
        int count2 = 0;
        foreach (BaseSteve tank in BattleManager.characters.Values)
        {
            if (tank.IsDie())
            {
                continue;
            }

            if (tank.camp == 1) { count1++; };
            if (tank.camp == 2) { count2++; };
        }
    }*/

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

    //关闭
    public override void OnClose()
    {
        
    }
}
