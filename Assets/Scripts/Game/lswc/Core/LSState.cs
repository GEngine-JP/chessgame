using UnityEngine;
using System.Collections;
using com.yxixia.utile.YxDebug;

/// <summary>
/// 控制游戏中的各个阶段
/// </summary>
/// <typeparam name="Game_State"></typeparam>
public class LSState
{

    /// <summary>
    /// 阶段等待时间
    /// </summary>
    protected float _duraTime;

    public bool ExcuteState = true;

    public bool UpdateState = false;

    public delegate void  LSStateEvent();

    public LSStateEvent OnStateFinished;

    public LSState NextState;

    public virtual void Enter()
    {
        ExcuteState = false;
    }

    public virtual void Excute()
    {
        //YxDebug.Log("<color=red>EXCUTE STATE IS</color>" + "<color=green>" + this + "</color>");
        ExcuteState = true;
    }

    public virtual void Exit()
    {
        ExcuteState = false;
        UpdateState = false;
    }

    public virtual void Update()
    {
    }

    public float DuraTime
    {
        set {  _duraTime=value; }
    }

}
