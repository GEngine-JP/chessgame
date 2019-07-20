using System;
using System.Net.Mime;
using Assets.Scripts.Game.lswc.Control.System;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Manager;
using Assets.Scripts.Game.lswc.Tools;
using Assets.Scripts.Game.lswc.Windows;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using YxFramwork.Common;

public class LSUIManager :InstanceControl
{
    private static LSUIManager _instance;

    public static LSUIManager Instance
    {
        get { return _instance; }
    }

    /// <summary>
    ///下注面板 
    /// </summary>
    private LSBetWindow _betWindow;

    /// <summary>
    /// 设置面板
    /// </summary>
    private LSSettingWindow _settingWondow;

    /// <summary>
    /// 历史记录
    /// </summary>
    private LSHistoryWindow _historyWindow;

    /// <summary>
    /// 显示的剩余的时间
    /// </summary>
    private Text _timeText;

    /// <summary>
    /// 庄和闲
    /// </summary>
    private Image _banker;

    /// <summary>
    ///红利？？为什么本地随机
    /// </summary>
    private Text _bonus;
    /// <summary>
    /// 播放动画后的特效？
    /// </summary>
    private GameObject _vfx;

    /// <summary>
    /// 计时器
    /// </summary>
    private List<LSTimeSpan> _spans;

    /// <summary>
    /// 删除的计时器
    /// </summary>
    private List<LSTimeSpan> _removeSpan;

    /// <summary>
    /// 结果界面
    /// </summary>
    private LSResultControl _resultPanel;

    private void Awake()
    {
        _instance = this;
        
    }

    private void Start()
    {
        _spans = new List<LSTimeSpan>();
        _removeSpan = new List<LSTimeSpan>();
        App.GetGameData<GlobalData>().OnBetNumChange += OnBetChange;
        FindChild();
    }

    private void FindChild()
    {
        _betWindow = transform.FindChild("BetWindow").GetComponent<LSBetWindow>();

        _settingWondow = transform.FindChild("SettingWindow").GetComponent<LSSettingWindow>();

        _historyWindow = transform.FindChild("HistoryWindow").GetComponent<LSHistoryWindow>();

        _bonus = transform.FindChild("LeftTop/Bonus/BonusNum").GetComponent<Text>();

        _timeText = transform.FindChild("RightTop/TimeCutDown/timeCut").GetComponent<Text>();

        _banker = transform.FindChild("Image/Image").GetComponent<Image>();

        _vfx = transform.FindChild("VFX").gameObject;

        _resultPanel = transform.FindChild("DisplayResult").GetComponent<LSResultControl>();

    }

    #region 下注窗口
    public void ShowBetWindow()
    {
        _betWindow.Show();
    }

    public void HideBetWindow()
    {
        _betWindow.Hide();
    }

    public void OnBetChange()
    {
        _betWindow.SetTotalBets();
        _betWindow.RefreshItems();
        _betWindow.SetTotalGold();
    }

    public void ChangeAnte()
    {
        _betWindow.ChangeAnte();
    }

    public void SetBetWindow()
    {
        _betWindow.SetBetWindow();
    }

    #endregion

    #region 历史面板

    public void SetHistoryWindow()
    {
        _historyWindow.InitHistorys();
    }

    #endregion

    #region 设置面板

    public void ShowSettingWindow()
    {
        _settingWondow.Show();
    }

    #endregion
    public override void OnExit()
    {
        _instance = null;
        App.GetGameData<GlobalData>().OnBetNumChange -= OnBetChange;
    }

    public void InitUImanager()
    {
        _bonus.text = "0";

        SetShowTime(App.GetGameData<GlobalData>().ShowTime.ToString());

        SetBanker(App.GetGameData<GlobalData>().SetLastBanker());

        SetBonus("0");

        SetHistoryWindow();

        SetBetWindow();     
    }


    public void SetShowTime(string time)
    {
        _timeText.text = time;
    }

    public void SetBanker(Sprite sp)
    {
       _banker.overrideSprite=sp;
    }

    public void SetBonus(string bouns)
    {
        _bonus.text = bouns;
    }

    public void SetVFXActive(bool show)
    {
        _vfx.SetActive(show);
    }

    /// <summary>
    /// 变化庄和闲图片到目标图片
    /// </summary>
    /// <param name="banker"></param>
    /// <param name="time"></param>
    /// <param name="frame"></param>
    public void ChangeBankerTo(LSBankerType banker,float time,float frame)
    {
        LSTimeSpan span=new LSTimeSpan(true,frame,time);
        _spans.Add(span);
        span.OnTimeFrameFinished = delegate()
            {
                SetBanker(App.GetGameData<GlobalData>().GetRandomBanker());
            };
        span.OnTimeFinished = delegate()
            {
                span.OnTimeFinished=null;
                _removeSpan.Add(span);
                SetBanker(LSResourseManager.Instance.GetSprite(App.GetGameData<GlobalData>().GetBankerOrSpriteName(banker)));
            };
    }

    /// <summary>
    /// 本地随机显示彩金数量
    /// </summary>
    /// <param name="totalTime"></param>
    /// <param name="frameTime"></param>
    public void SetRandomBonus(float totalTime,float frameTime)
    {
        LSTimeSpan span =new LSTimeSpan(true,frameTime,totalTime);
        _spans.Add(span);
        span.OnTimeFrameFinished = delegate()
            {
                SetBonus(App.GetGameData<GlobalData>().GetRandomNum().ToString());
            };
        span.OnTimeFinished = delegate()
            {
                span.OnTimeFinished = null;
                _removeSpan.Add(span);
            };
    }

    public void ShowResultPanel()
    {

        _resultPanel.gameObject.SetActive(true);

        _resultPanel.ShowResultInfo(App.GetGameData<GlobalData>().LastResult.ShowResults, App.GetGameData<GlobalData>().TotalBets, App.GetGameData<GlobalData>().LastResult.WinBets, App.GetGameData<GlobalData>().LastResult.Multiple);
    }

    public void HideResultPanel()
    {
        _resultPanel.Hide();
    }

    void Update()
    {
        if (_spans != null)
        {
            foreach (var span in _spans)
            {
                if (span!=null)
                {
                    span.Run(); 
                }             
            }
            if (_removeSpan != null && _removeSpan.Count > 0)
            {
                for (int i = 0; i < _removeSpan.Count; i++)
                {
                    LSTimeSpan span = _removeSpan[i];
                    _spans.Remove(span);
                }
                _removeSpan.Clear();
            }
        }
    }

}
