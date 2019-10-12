namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>  
    /// 游戏事件协议  
    /// </summary>
    public enum GameEventProtocol
    {
        _ = 0,
        AiAgency,//托管
    }

    /// <summary>
    /// UI事件协议
    /// </summary>
    public enum UIEventProtocol
    {
        _ = 1000,
        OnTing,
        SetBanker,
        PlayerOut,
        PlayerJoin,
        PlayerReady,
        PlayAddScore,
        UpdateCurrOpPlayer,
        UpdateRound,
        UpdateMahCount,
        ReadyBtnCtrl,
        OperateMenuCtrl,
        OnShowHandUp,
        OnEventHandUp,
        ShowTotalResult,
        ShowResult,
        ShowChooseOperate,
        ShowTitleMessage,
        HideTitleMessage,
        SetPlayerFlagState,
        ShowDingqueFlag,
        SetSingleHuFlag,
        PanelGmCtrl,
        ScoreDoubleCtrl,
        ShowMahFriendsPanel,
        QueryHuCard,
        QueryBtnCtrl,
        ShowGameExplain,
        HideChangeTitleBtn,
        TipBankerPutCard,
        ChangeCardTip,
        OnCreateNewGame,
        ShowChooseXjfd,
        ShowXjfdList,
        ShowGameRule,
        AgencyAiCtrl,
        HideChangeCardTip,
        ChangeErrorTip,
        SetGangdi,
        ShowExhibtionMahjong_hzmj,
        HuangzhuangTip,  
        ShowOtherHutip,

        PlaybackRestart,
    }
}