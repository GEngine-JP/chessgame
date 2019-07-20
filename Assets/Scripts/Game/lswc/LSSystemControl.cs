using System.Collections.Generic;
using Assets.Scripts.Game.lswc.Control.Scene.Item;
using Assets.Scripts.Game.lswc.Control.Scene.Manager;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Item;
using Assets.Scripts.Game.lswc.Manager;
using Assets.Scripts.Game.lswc.States;
using Assets.Scripts.Game.lswc.Tools;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.View;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.lswc.Control.System
{
    /// <summary>
    /// 系统控制
    /// </summary>
    public class LSSystemControl : InstanceControl
    {
        private static LSSystemControl _instance;

        public static LSSystemControl Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// 是否加载完毕资源
        /// </summary>
        private bool _loadFinished = false;

        private LSState _curState;

        public LSState CurState
        {
            set { _curState = value; }
        }

        private LSState _perState;

        public List<LSTimeSpan> spans;

        private void Awake()
        {
            _instance = this;
            spans = new List<LSTimeSpan>();
        }
        

        private void Start()
        {
            LSResourseManager.Instance.OnLoadFinished = OnLoadResourseFinshed;
            LSOperationManager.Instance.InitListener();
        }

        #region 测试区域
        /// <summary>
        /// 动物顺序测试临时使用
        /// </summary>
        //public int [] Animals = new int[]
        //    {
        //        4,5,6,7,
        //        0,1,2,3,
        //        0,1,2,3,
        //        0,1,2,3,
        //        0,1,2,3,
        //        0,1,2,3,
        //    };

        /// <summary>
        /// 颜色顺序测试临时使用
        /// </summary>
        //public  int[] Colors = new int[]
        //    {
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //        0,1,2,
        //    };
        //private void TestDataInit()
        //{

        //    //初始化信息
        //    user.PutLong(RequestKey.KeyTotalGold, 100000000);
        //    InitInfo.PutSFSObject(RequestKey.KeyUser, user);
        //    InitInfo.PutLong(LSConstant.KeyGameStartTime,1000000000000);
        //    InitInfo.PutLong(RequestCmd.GetServerTime, 1000000001);
        //    InitInfo.PutInt(LSConstant.KeyCDTime,5);
        //    InitInfo.PutInt(LSConstant.KeyGameStatus,2);
        //    int[] arr=new int[10];
        //    InitInfo.PutIntArray(LSConstant.KeyHistoryResult,arr);
        //    InitInfo.PutIntArray(LSConstant.KeyAnimalsPosition,Animals);
        //    InitInfo.PutIntArray(LSConstant.KeyColorPosition,Colors);
        //}


        /// <summary>
        /// 模拟收到返回结果
        /// </summary>
        void TestOnGetResult()
        {
            //Debug.LogError("测试发送接收到游戏消息");
            //int result = 0x002;

            //GlobalData.AddHistory(result);

            //GlobalData.ISGetResult = true;

            //GlobalData.GetLastDetialResult();

        }

        #endregion

        private void OnLoadResourseFinshed()
        {
            _loadFinished = true;
            //TestDataInit();
            //LSServerManager.Instance.TestData(0, InitInfo);
        }

        public void InitState()
        {
            YxDebug.Log("当前游戏阶段是： " + App.GetGameData<GlobalData>().GlobalGameStatu + "剩余时间是 ：" + App.GetGameData<GlobalData>().ShowTime);

            if (_loadFinished)
            {
                _curState = LSInitState.Instance;
                _curState.Enter();
            }
            else
            {
                YxDebug.LogError("Resourse is noe loaded finished");
            }
        }

        public void PlaySuccess(bool success)
        {
            if (success)
            {
                PlayVoice(LSConstant.ButtonSelectVoice);
            }
            else
            {
                PlayVoice(LSConstant.ButtonSelectFailVoice);
            }
        }

        public void QuitGame()
        {
            App.QuitGameWithMsgBox();
            //YxMessageBox.Show("确定要退出游戏吗？", "", (box, btnName) =>
            //    {
            //        if (btnName == YxMessageBox.BtnLeft)
            //        {
            //            App.QuitGame();
            //        }
            //    },false,YxMessageBox.LeftBtnStyle|YxMessageBox.RightBtnStyle,null,5);
        }

        private void Update()
        {
            if (_curState != null)
            {
                if (!_curState.ExcuteState)
                {
                    _curState.Excute();
                }
                if (_curState.UpdateState)
                {
                    _curState.Update();
                }
            }
            if(spans!=null)
            {
                foreach (var span in spans)
                {
                    span.Run();
                }
            }
        }

        public override void OnExit()
        {
            _instance = null;
        }

        void OnDestroy()
        {
            _instance = null;
            _curState = null;   
        }

        #region 状态相关

        public void ChangeState(LSState newState)
        {
            _perState = _curState;
            if (_curState != null)
            {
                _curState.Exit();
            }
            _curState = newState;

            _curState.Enter();
        }

        public void RevertToPreviousState()
        {
            ChangeState(_perState);
        }

        #endregion

        /// <summary>
        /// 每局初始化数据
        /// </summary>
        public void InitEachRound()
        {

            App.GetGameData<GlobalData>().ISGetResult = false; 
            LSColorItemControl.Instance.ResetLayout();
            LSAnimalItemControl.Instance.ResetLayout();      
            LSTurnTableControl.Instance.SetPointPosition(0);
            LSUIManager.Instance.InitUImanager();
            LSCameraManager.Instance.Reset();
        }

        public void SetBonus()
        {
            LSUIManager.Instance.SetBonus(App.GetGameData<GlobalData>().GetRandomNum().ToString());
        }

        /// <summary>
        /// 播放奖励阶段音乐
        /// </summary>
        /// <param name="type"></param>
        public void  PlayeGameStateMusic(LSRewardType type)
        {
            string musicName;

            switch (type)
            {
                case LSRewardType.NORMAL:
                    musicName = LSConstant.BackgroundMusci_Normal;
                    break;
                case LSRewardType.LIGHTING:
                    musicName = LSConstant.BackgroundMusic_Lighting;
                    break;
                case LSRewardType.BIG_THREE:
                    musicName = LSConstant.BackgroundMusic_BigThree;
                    PlayVoice(LSConstant.ThreeOrFourWaring);
                    break;
                case LSRewardType.BIG_FOUR:
                    musicName = LSConstant.BackgroundMusic_BigFour;
                    PlayVoice(LSConstant.ThreeOrFourWaring);
                    break;
                case LSRewardType.SENDLAMP:
                    musicName = LSConstant.BackgroundMusic_SendLamp;
                    break;
                case LSRewardType.HANDSEL:
                   musicName = LSConstant.BackgroundMusic_Handsel;
                    break;
                default:
                    musicName = LSConstant.BackgroundMusic_WaitBet;
                    YxDebug.LogError("Not exist such type music");
                    break;
            }

            PlayeGameStateMusic(musicName);
        }

        /// <summary>
        /// 是否需要上台展示,普通模式，彩金模式，闪电需要上台展示，其他（大四喜，大三元，送灯）不需要上台展示
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool NeedSignAnimal(LSRewardType type)
        {
            switch (type)
            {
                case LSRewardType.NORMAL:
                case LSRewardType.HANDSEL:
                case LSRewardType.SENDLAMP:
                    return true;
                    break;
                default:
                    return false;
            }
        }

        public void PlayeGameStateMusic(string bgMusic)
        {
            LSResourseManager.Instance.PlayBackgroundMusic(bgMusic);
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="voice"></param>
        public void PlayVoice(string voice)
        {
            LSResourseManager.Instance.PlayVoice(voice);
        }
    }
}
