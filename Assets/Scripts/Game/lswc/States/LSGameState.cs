using Assets.Scripts.Game.lswc.Control.Scene.Manager;
using Assets.Scripts.Game.lswc.Control.System;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Item;
using Assets.Scripts.Game.lswc.Manager;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lswc.States
{
    /// <summary>
    /// 初始化数据状态
    /// </summary>
    public class LSInitState : LSState
    {
        private static LSInitState _instance;

        public static LSInitState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSInitState();
                }
                return _instance;
            }
        }

        public override void Enter()
        {
            base.Enter();
            switch (App.GetGameData<GlobalData>().GlobalGameStatu)
            {
                case GameState.BetState:
                    NextState = LSBetState.Instance;
                    LSBetState.Instance.DuraTime = App.GetGameData<GlobalData>().ShowTime;
                    break;
                case GameState.ResultState:
                    ProcessPlayerInit();
                    break;
                case GameState.Empyt:
                case GameState.InitState:
                case GameState.WaitState:
                    NextState = LSBetState.Instance;
                    LSEmptyState.Instance.Update();
                    break;
            }
        }

        public override void Excute()
        {
            base.Excute();
            LSSystemControl.Instance.ChangeState(NextState);
        }

        private void ProcessPlayerInit()
        {

            if (App.GetGameData<GlobalData>().LastResult.Reward == LSRewardType.SENDLAMP)
            {
                //送灯模式
                YxDebug.LogError("送灯模式暂时未定数据格式，需要重新处理");
                NextState = LSSendLampShowNumState.Instance;
                NextState.DuraTime = LSConstant.ShowSendLightNumTime;
                LSTurnTableControl.Instance.PlayAnimation();
            }
            else
            {
                //非送灯模式
                if (App.GetGameData<GlobalData>().ShowTime <= LSConstant.ShowAnimationTime)
                {
                    NextState = LSShowAnimationState.Instance;
                    NextState.DuraTime = LSConstant.ShowAnimationTime;
                    App.GetGameData<GlobalData>().LastResult.ToAnimal.MoveToCenter(0);
                    QuickRoate();
                    SetShowTime();
                }
                else if (App.GetGameData<GlobalData>().ShowTime <= LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime)
                {
                    NextState = LSAnimalMoveState.Instance;
                    NextState.DuraTime = LSConstant.AnimoveMoveToCenterTime;
                    QuickRoate();
                    SetShowTime();
                }
                else if (App.GetGameData<GlobalData>().ShowTime <= LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime + LSConstant.LookAtAnimalTime)
                {
                    NextState = LSLookAtAnimalState.Instance;
                    NextState.DuraTime = LSConstant.LookAtAnimalTime;
                    QuickRoate();        
                    SetShowTime();
                }
                else if (App.GetGameData<GlobalData>().ShowTime <= LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime + LSConstant.LookAtAnimalTime + LSConstant.RotationTime)
                {
                    NextState = LSRotateState.Instance;
                    NextState.DuraTime = LSConstant.RotationTime;
                    SetShowTime();
                }
                else if (App.GetGameData<GlobalData>().ShowTime <= LSConstant.ShowAnimationTime + LSConstant.AnimoveMoveToCenterTime + LSConstant.LookAtAnimalTime + LSConstant.RotationTime + LSConstant.ShowGameTypeTime)
                {
                    NextState = LSShowGameTypeState.Instance;
                    NextState.DuraTime = LSConstant.ShowGameTypeTime;
   
                    LSSystemControl.Instance.PlayeGameStateMusic(LSConstant.BackgroundMusci_Normal);
                    SetShowTime();
                }
 
                else
                {
                    NextState = LSJudgeResultState.Instance;
                }
            }
        }

        private void QuickRoate()
        {
            LSAnimalItemControl.Instance.QuickRotate(App.GetGameData<GlobalData>().AnimalRandomSeed * 15);
            LSTurnTableControl.Instance.QuickRoate(App.GetGameData<GlobalData>().LastResult.ToAngle.y);
            LSSystemControl.Instance.PlayeGameStateMusic(App.GetGameData<GlobalData>().LastResult.Reward);
        }

        private void SetShowTime()
        {
            LSUIManager.Instance.SetShowTime("0");
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    /// <summary>
    /// 下注阶段
    /// </summary>
    public class LSBetState : LSState
    {
        private static LSBetState _instance;

        public static LSBetState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSBetState();
                }
                return _instance;
            }
        }


        public override void Enter()
        {
            base.Enter();

            LSUIManager.Instance.ShowBetWindow();
            NextState = LSWaitResultState.Instance;
        }

        public override void Excute()
        {
            base.Excute();
            LSSystemControl.Instance.PlayeGameStateMusic(LSConstant.BackgroundMusic_WaitBet);
            LSUIManager.Instance.SetShowTime(App.GetGameData<GlobalData>().ShowTime.ToString());
            if(_duraTime<=0)
            {
                LSSystemControl.Instance.ChangeState(NextState);
            }
            UpdateState = true;
        }

        private float _frameTime = 0;

        private float _ShowCountDownVoiceTime = 5;

        public override void Update()
        {
            base.Update();
            _frameTime += Time.deltaTime;
            if (_frameTime >= 1)
            {
                _duraTime -= 1;
                _frameTime = 0;
                if (_duraTime <= _ShowCountDownVoiceTime && _frameTime >= 0)
                {
                    LSSystemControl.Instance.PlayVoice(LSConstant.BetCountDownVoice);
                }
                if (_duraTime <= 0)
                {
                    _duraTime = 0;
                    LSSystemControl.Instance.ChangeState(NextState);
                    return;
                }
                LSUIManager.Instance.SetShowTime(Mathf.FloorToInt(_duraTime).ToString());
            }

        }

        public override void Exit()
        {
            base.Exit();
            LSUIManager.Instance.SetShowTime(Mathf.FloorToInt(_duraTime).ToString());
            App.GetRServer<LSServerManager>().SendBetRequest();
            LSUIManager.Instance.HideBetWindow();
            _frameTime = 0;
        }
    }

    /// <summary>
    /// 等待结果阶段
    /// </summary>
    public class LSWaitResultState : LSState
    {
        private static LSWaitResultState _instance;

        public static LSWaitResultState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSWaitResultState();
                }
                return _instance;
            }
        }

        public override void Enter()
        {
            base.Enter();

            NextState = LSJudgeResultState.Instance;
        }

        public override void Excute()
        {
            base.Excute();
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            if (App.GetGameData<GlobalData>().ISGetResult)
            {
                LSSystemControl.Instance.ChangeState(NextState);
            }
        }
    }

    /// <summary>
    /// 处理游戏结果
    /// </summary>
    public class LSJudgeResultState : LSState
    {
        private static LSJudgeResultState _instance;

        public static LSJudgeResultState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSJudgeResultState();
                }
                return _instance;
            }
        }

        public override void Enter()
        {
            base.Enter();
            if (App.GetGameData<GlobalData>().LastResult.Reward == LSRewardType.SENDLAMP)
            {
                NextState = LSSendLampShowNumState.Instance;
                NextState.DuraTime = LSConstant.ShowSendLightNumTime;
            }
            else
            {
                NextState = LSShowGameTypeState.Instance;
                NextState.DuraTime = LSConstant.ShowGameTypeTime;
            }
        }

        public override void Excute()
        {
            base.Excute();
            LSSystemControl.Instance.ChangeState(NextState);
        }
    }

    /// <summary>
    /// 送灯前，显示数字(字数为轮数)阶段
    /// </summary>
    public class LSSendLampShowNumState : LSState
    {
        private static LSSendLampShowNumState _instance;

        public static LSSendLampShowNumState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSSendLampShowNumState();
                }
                return _instance;
            }
        }

        public override void Enter()
        {
            base.Enter();
            NextState = LSRotateState.Instance;
            NextState.DuraTime = LSConstant.RotationTime;
        }

        public override void Excute()
        {
            base.Excute();
            SetLastHistory();
            LSSystemControl.Instance.ChangeState(NextState);
        }

        private void SetLastHistory()
        {
            //ToDo 打算在这里处理送灯的每局数据，将每个送灯数据放到历史纪录中，暂时不确定送灯数据结构，等确认后实现
        }
    }
    /// <summary>
    /// 通用旋转阶段
    /// </summary>
    public class LSRotateState : LSState
    {
        private static LSRotateState _instance;

        public static LSRotateState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSRotateState();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 彩金刷新频率
        /// </summary>
        private float _setBonuFrame = 0.3f;

        /// <summary>
        /// 庄和闲图片变换频率
        /// </summary>
        private float _setBankerFrame = 0.5f;

        private float _animalCache = 0;

        private float _animalFrame = 0.1f;

        private int _randomNum = 0;

        public override void Enter()
        {
            base.Enter();
            //需求，如果游戏中，且下注了，就不让退出游戏
            if(App.GetGameData<GlobalData>().TotalBets>0)
            {
                App.GameData.GStatus = GameStatus.PlayAndConfine;
            }  

            if (LSSystemControl.Instance.NeedSignAnimal(App.GetGameData<GlobalData>().LastResult.Reward))
            {
                NextState = LSLookAtAnimalState.Instance;
                NextState.DuraTime = LSConstant.LookAtAnimalTime;
            }
            else
            {
                NextState = LSShowAnimationState.Instance;
                NextState.DuraTime = LSConstant.ShowAnimationTime;
            }
        }

        public override void Excute()
        {
            base.Excute();
            LSTurnTableControl.Instance.Rotate(App.GetGameData<GlobalData>().LastResult.ToAngle.y, _duraTime);//要求指针先停止旋转，然后动物停止
            LSAnimalItemControl.Instance.Rotate(App.GetGameData<GlobalData>().AnimalRandomSeed * 15, _duraTime);
            LSUIManager.Instance.ChangeBankerTo(App.GetGameData<GlobalData>().LastResult.Banker, _duraTime, _setBankerFrame);
            LSUIManager.Instance.SetRandomBonus(_duraTime, _setBonuFrame);
            UpdateState = true;
        }



        public override void Update()
        {
            base.Update();

            _duraTime -= Time.deltaTime;

            if (App.GetGameData<GlobalData>().LastResult.Reward == LSRewardType.BIG_THREE)
            {
                _animalCache += Time.deltaTime;

                if (_animalCache >= _animalFrame)
                {
                    _randomNum++;

                    _animalCache = 0;

                    LSBigThreeReward.Instance.SetCurrentAnimal((LSAnimalType)(_randomNum % 8));
                }
            }

            if (_duraTime <= 0)
            {
                _duraTime = 0;
                LSSystemControl.Instance.ChangeState(NextState);
            }

        }
        public override void Exit()
        {
            base.Exit();
            switch (App.GetGameData<GlobalData>().LastResult.Reward)
            {
                case LSRewardType.BIG_THREE:
                    LSBigThreeReward.Instance.SetCurrentAnimal(App.GetGameData<GlobalData>().LastResult.Ani);
                    break;
                case LSRewardType.BIG_FOUR:
                    //原项目中对应功能不可用，不知道什么表现
                    //LSChangeParticalColor.Instance.OveredChange(GlobalData.LastResult.Cor);
                    break;
            }

            LSSystemControl.Instance.PlayVoice(App.GetGameData<GlobalData>().LastResult.lastResultVoice);
            LSUIManager.Instance.SetHistoryWindow();
        }
    }


    public class LSShowGameTypeState : LSState
    {
        private static LSShowGameTypeState _instance;

        public static LSShowGameTypeState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSShowGameTypeState();
                }
                return _instance;
            }
        }

        private LS_Detail_Result detailRes;

        //private float _animalCache = 0;

        //private float _animalFrame = 0.1f;

        //private int _randomNum = 0;

        public override void Enter()
        {
            base.Enter();
            detailRes = App.GetGameData<GlobalData>().LastResult;
            NextState = LSRotateState.Instance;
            NextState.DuraTime = LSConstant.RotationTime;
        }

        public override void Excute()
        {
            base.Excute();

            //根据类型播放对应的状态前置动画
            switch (detailRes.Reward)
            {
                ///正常模式，送灯模式，彩金模式无前置动画
                case LSRewardType.NORMAL:
                case LSRewardType.HANDSEL:
                case LSRewardType.SENDLAMP:
                    _duraTime = 0;
                    break;
                case LSRewardType.LIGHTING:
                    LSShowGameTypeManager.Instance.ShowGameTypeLighting(detailRes.Multiple);
                    LSSystemControl.Instance.PlayVoice(LSConstant.ShanDianVoice);
                    break;
                case LSRewardType.BIG_THREE:
                    LSShowGameTypeManager.Instance.ShowBigThree();
                    LSTurnTableControl.Instance.PlayAnimation();
                    break; ;
                case LSRewardType.BIG_FOUR:
                    LSShowGameTypeManager.Instance.ShowBigFour(true);
                    LSTurnTableControl.Instance.PlayAnimation();
                    break;
            }
            LSSystemControl.Instance.PlayeGameStateMusic(detailRes.Reward);
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();

            //if (GlobalData.LastResult.Reward == LSRewardType.BIG_THREE)
            //{
            //    _animalCache += Time.deltaTime;

            //    if (_animalCache >= _animalFrame)
            //    {
            //        _randomNum++;

            //        _animalCache = 0;

            //        LSBigThreeReward.Instance.SetCurrentAnimal((LSAnimalType)(_randomNum % 8));
            //    }
            //}
            _duraTime -= Time.deltaTime;
            if (_duraTime <= 0)
            {
                LSSystemControl.Instance.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            _duraTime = 0;
        }


    }

    /// <summary>
    /// 动物展示阶段--看向动物
    /// </summary>
    public class LSLookAtAnimalState : LSState
    {
        private static LSLookAtAnimalState _instance;

        public static LSLookAtAnimalState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSLookAtAnimalState();
                }
                return _instance;
            }
        }

        public override void Enter()
        {
            base.Enter();
            NextState = LSAnimalMoveState.Instance;
            NextState.DuraTime = LSConstant.AnimoveMoveToCenterTime;

        }

        public override void Excute()
        {
            base.Excute();
            LSFireWorksControl.Instance.Show();
            LSCrystalControl.Instance.Show(true);
            LSSystemControl.Instance.PlayeGameStateMusic(LSConstant.WaitForShowAnimal);
            LSTurnTableControl.Instance.PlayAnimation();
            LSCameraManager.Instance.RotateToPosition(App.GetGameData<GlobalData>().LastResult.ToAngle.y, _duraTime);
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            _duraTime -= Time.time;
            if (_duraTime <= 0 && !LSCameraManager.Instance.IsMoving)
            {
                LSSystemControl.Instance.ChangeState(NextState);
            }
        }
    }

    /// <summary>
    /// 动物展示阶段--动物移动
    /// </summary>
    public class LSAnimalMoveState : LSState
    {
        private static LSAnimalMoveState _instance;

        public static LSAnimalMoveState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSAnimalMoveState();
                }
                return _instance;
            }
        }

        public override void Enter()
        {
            base.Enter();
            NextState = LSShowAnimationState.Instance;
            NextState.DuraTime = LSConstant.ShowAnimationTime;
        }

        public override void Excute()
        {
            base.Excute();
            LSUIManager.Instance.SetVFXActive(true);
            LSAnimalItem item = App.GetGameData<GlobalData>().LastResult.ToAnimal;
            item.MoveToCenter(_duraTime);
            LSCameraManager.Instance.MoveDown(_duraTime * 2, _duraTime * 0.5f, item);
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            _duraTime -= Time.deltaTime;
            if (_duraTime < 0 && !LSCameraManager.Instance.IsMoving)
            {
                LSSystemControl.Instance.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            LSUIManager.Instance.SetVFXActive(false);
        }
    }

    /// <summary>
    /// 动物播放动画阶段
    /// </summary>
    public class LSShowAnimationState : LSState
    {
        private static LSShowAnimationState _instance;

        public static LSShowAnimationState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSShowAnimationState();
                }
                return _instance;
            }
        }

        private bool needLook;

        public override void Enter()
        {
            base.Enter();

            needLook = LSSystemControl.Instance.NeedSignAnimal(App.GetGameData<GlobalData>().LastResult.Reward);
            NextState = LSShowResultUIState.Instance;
            NextState.DuraTime = LSConstant.ShowResultUITime;
        }

        public override void Excute()
        {
            base.Excute();
            ///播三次吧
            if (needLook)
            {
                App.GetGameData<GlobalData>().LastResult.ToAnimal.CurAnimation = LSAnimalAnimationType.RAWARD;
                LSAnimalItemControl.Instance.PlayAnimation(3);
            }
            else if (App.GetGameData<GlobalData>().LastResult.Reward == LSRewardType.BIG_THREE)
            {
                LSBigThreeReward.Instance.PlayAnimation(3);
            }
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            _duraTime -= Time.deltaTime;
            if (_duraTime <= 0)
            {
                _duraTime = 0;
                LSSystemControl.Instance.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            App.GetGameData<GlobalData>().LastResult.ToAnimal.CurAnimation = LSAnimalAnimationType.WATCH;
            if (LSSystemControl.Instance.NeedSignAnimal(App.GetGameData<GlobalData>().LastResult.Reward))
            {
                LSCameraManager.Instance.ZoomOut(2);
            }
        }

    }


    /// <summary>
    /// 显示结果阶段
    /// </summary>
    public class LSShowResultUIState : LSState
    {
        private static LSShowResultUIState _instance;

        public static LSShowResultUIState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSShowResultUIState();
                }
                return _instance;
            }
        }

        private float cacheTime = 0;

        private bool isReset;

        public override void Enter()
        {
            base.Enter();
            //应该在打开后关闭，不应该在这处理
            App.GameData.GStatus = GameStatus.Normal;
            NextState = LSEmptyState.Instance;
            App.GetGameData<GlobalData>().ShowTime = 0;
            isReset = false;
            cacheTime = 0;
        }

        public override void Excute()
        {
            base.Excute();
            LSFireWorksControl.Instance.Hide();
            LSUIManager.Instance.SetVFXActive(false);
            LSSystemControl.Instance.PlayeGameStateMusic(LSConstant.GameEnd);
            if (App.GetGameData<GlobalData>().LastResult.Reward == LSRewardType.SENDLAMP)
            {
                //ToDo 送灯显示历史记录面板
            }
            else
            {
                LSUIManager.Instance.ShowResultPanel();
            }
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            cacheTime += Time.deltaTime;
            if (_duraTime <= cacheTime)
            {
                LSSystemControl.Instance.ChangeState(NextState);
            }
        }

        public override void Exit()
        {
            base.Exit();
            cacheTime = 0;
        }
    }

    /// <summary>
    /// 空状态，这个状态不满足完成一个单独状态，什么也不做，作为游戏结束后等待下一次游戏的开始
    /// </summary>
    public class LSEmptyState : LSState
    {
        private static LSEmptyState _instance;

        public static LSEmptyState Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LSEmptyState();
                }
                return _instance;
            }
        }

        public override void Enter()
        {
            base.Enter();
            App.GetGameData<GlobalData>().ReadyToNext = false;
            App.GetGameData<GlobalData>().GlobalGameStatu = GameState.WaitState;
            NextState = LSBetState.Instance;
        }

        public override void Excute()
        {
            base.Excute();
            UpdateState = true;
        }

        public override void Update()
        {
            base.Update();
            if (App.GetGameData<GlobalData>().ReadyToNext)
            {
                if (LSSystemControl.Instance.NeedSignAnimal(App.GetGameData<GlobalData>().LastResult.Reward))
                {
                    App.GetGameData<GlobalData>().LastResult.ToAnimal.MoveBack();
                }
                App.GetGameData<GlobalData>().ReadyToNext = false;
                LSUIManager.Instance.HideResultPanel();
                LSTurnTableControl.Instance.ResetAnimation();
                LSCrystalControl.Instance.Show(false);
                LSBigThreeReward.Instance.HideAll();
                LSShowGameTypeManager.Instance.ShowBigFour(false);
                NextState.DuraTime = App.GetGameData<GlobalData>().ShowTime;
                App.GetGameData<GlobalData>().GlobalGameStatu = GameState.BetState;
                LSSystemControl.Instance.InitEachRound();
                LSSystemControl.Instance.ChangeState(NextState);
                LSAnimalItemControl.Instance.InitPosition();
            }
        }
    }

}