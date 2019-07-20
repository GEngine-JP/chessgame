using System;
using System.Collections;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class GameServer : RemoteController
    {

        public static GameServer Instance;

        public static GameServer GetInstance()
        {
            return Instance;
        }

        protected override void OnAwake()
        {
            Instance = this;
        }

        protected override void OnGetGameInfo(ISFSObject gameInfo)
        {

            var ratecfg = gameInfo.GetUtfString("ratecfg");
                //string rateCfg = cargs2.GetUtfString("rateCfg");
                if (!string.IsNullOrEmpty(ratecfg)) BeiShuMode.Instance.SetRateInfo(ratecfg.Split('#'));
           
            App.GetGameData<GlobalData>().CurrentUser = getUserInfo(gameInfo.GetSFSObject(RequestKey.KeyUser));//得到用户信息
            UserInfoUI.Instance.SetUserInfoUI();//设置玩家信息
            App.GetGameData<GlobalData>().BankList = gameInfo.GetSFSArray("bankers");
            App.GetGameData<GlobalData>().B = gameInfo.GetInt("banker");
            BankerInfoUI.Instance.SetBankerInfoUIData();//设置庄家的信息
            //CountDownUI.Instance.SetNum();//设置下注的最大值
            App.GetGameData<GlobalData>().Bkmingold = gameInfo.GetInt("bkmingold");
            //CheckReJion(gameInfo);//检查重连
            CameraMove_3D.Instance.CameraMoveByPath(0);//3D摄像机的移动到桌面上
            DownUIController.Instance.ResetChip(gameInfo);
        }
        private UserInfo getUserInfo(ISFSObject userData)
        {
            UserInfo userInfo = new UserInfo
            {
                Name = userData.GetUtfString(RequestKey.KeyName),
                Id = userData.GetInt(RequestKey.KeyId),
                Gold = userData.GetLong(RequestKey.KeyTotalGold),
                Seat = userData.GetInt(RequestKey.KeySeat)
            };

            return userInfo;
        }

        protected override void OnGetRejoinData(ISFSObject data)
        {
            OnGetGameInfo(data);
        }

        protected override void OnServerResponse(ISFSObject data)
        {
            int type = data.GetInt("type");
            switch (type)
            {
                //下注阶段1
                case RequestType.Bet:
                    MusicManager.Instance.Play("GameBegin");
                    App.GetGameData<GlobalData>().UserSeat = data.GetInt("seat");
                    App.GetGameData<GlobalData>().BetPos = data.GetInt("p");
                    App.GetGameData<GlobalData>().BetMoney = data.GetInt("gold");

                    ApplyXiaZhuangMgr.Instance.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态

                    BetModeMgr.Instance.SetBetModeChouMaData();//接收初始化玩家投注筹码的消息

                    
                    UserInfoUI.Instance.SetUserInfoUI();//刷新玩家信息
                    break;
                //判断庄家阶段4
                case RequestType.BankerList:
                    App.GetGameData<GlobalData>().BankList = data.GetSFSArray("bankers");
                    App.GetGameData<GlobalData>().B = data.GetInt("banker");
                    BankerListUI.Instance.DeleteBankerListUI();//更新前先删除
                    BankerInfoUI.Instance.SetBankerInfoUIData();//设置庄家的信息

                    ApplyXiaZhuangMgr.Instance.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态

                    //CountDownUI.Instance.SetNum(); //设置下注的最大值

                    break;
                //开始下注阶段5
                case RequestType.BeginBet:
                    App.GetGameData<GlobalData>().Bundle++;
                    if (App.GetGameData<GlobalData>().CurrentUser.Seat == App.GetGameData<GlobalData>().B)
                        App.GameData.GStatus = GameStatus.PlayAndConfine;
                    else
                        App.GameData.GStatus = GameStatus.Normal;
                    GamesNumUI.Instance.SetGamesNumUI();//设置游戏运行的局数
                    App.GetGameData<GlobalData>().IsBet = true;
                    StateUI.Instance.SetStateUI(2);//游戏运行的阶段显示，此时为下注

                    ApplyXiaZhuangMgr.Instance.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态

                    CameraMove_3D.Instance.CameraMoveByPath(0);//3D摄像机的移动到桌面上

                    NiuNumberUI.Instance.HideNiuNumberUI(); //隐藏牛数

                    PaiMode.Instance.DeletPaiList(); //清空牌的列表

                    CountDownUI.Instance.SetTimeCount(16);//时间的控制

                    DownUIController.Instance.DownUILeftUIOn_OffClicked(true);//隐藏历史纪录的面板


                    break;
                //结束下注阶段6
                case RequestType.EndBet:
                    App.GetGameData<GlobalData>().IsBet = false;
                    BetMoneyUI.Instance.BetMoneyQingKongInfo();//隐藏下注界面
                    break;


                //发牌阶段7
                case RequestType.GiveCards:
                    App.GetGameData<GlobalData>().IsBet = false;
                    if (data.ContainsKey("total"))
                    {
                        App.GetGameData<GlobalData>().CurrentUser.Gold = data.GetLong("total");
                    }
                   
                    App.GetGameData<GlobalData>().Cards=data.GetSFSArray("cards");
                    App.GetGameData<GlobalData>().Nn =data.GetSFSArray("nn");
                    StateUI.Instance.SetStateUI(3);//游戏运行的阶段显示，此时为开牌
                    ApplyXiaZhuangMgr.Instance.SetApplayXiaZhuangUIData();//设置申请上下装按钮的状态


                    App.GetGameData<GlobalData>().DicNum = data.GetInt("dice");
                    App.GetGameData<GlobalData>().BankList = data.GetSFSArray("bankers");
                    App.GetGameData<GlobalData>().B = data.GetInt("banker");
                    App.GetGameData<GlobalData>().SendCardPosition = App.GetGameData<GlobalData>().DicNum - 1;

                    DicMode.Instance.PlayDic(); //扔骰子并显示点数

                    DicMode.Instance.StopDic(); //骰子消失

                    PaiMode.Instance.BeginGiveCards(); //开始发牌

                   
                    break;
                //结束阶段8
                case RequestType.Result:
                    if (App.GetGameData<GlobalData>().CurrentUser.Seat==App.GetGameData<GlobalData>().B)
                        App.GameData.GStatus = GameStatus.PlayAndConfine;
                    else
                        App.GameData.GStatus = GameStatus.Normal;
                    MusicManager.Instance.Play("GameOver");
                    App.GetGameData<GlobalData>().IsBet = false;
                    App.GetGameData<GlobalData>().BankList = data.GetSFSArray("bankers");
                    App.GetGameData<GlobalData>().B = data.GetInt("banker");
                     if (data.ContainsKey("win"))
                    {
                        App.GetGameData<GlobalData>().ResultUserTotal = data.GetInt("win");
                        //                        YxDebug.LogError("玩家" + App.GetGameData<GlobalData>().ResultUserTotal);
                    }
                    else
                    {
                        App.GetGameData<GlobalData>().ResultUserTotal = 0;
                    }
                    if (data.ContainsKey("bwin"))
                    {
                        App.GetGameData<GlobalData>().ResultBnakerTotal = data.GetLong("bwin");
                        //                        YxDebug.LogError("庄" + App.GetGameData<GlobalData>().ResultBnakerTotal);
                    }
                    else
                    {
                        App.GetGameData<GlobalData>().ResultBnakerTotal = 0;
                    }

                    StateUI.Instance.SetStateUI(4);//游戏运行的阶段显示，此时为结算

                    CameraMove_3D.Instance.Move();

                    App.GetGameData<GlobalData>().isOut = true;
                    App.GameData.GStatus = GameStatus.Normal;
                    //CountDownUI.Instance.SetNum();//设置下注的最大值

                    break;
            }
        }

        protected override void OnUserOut(int seat)
        {
        }

        protected override void OnUserIdle(int serverSeat)
        {
        }

        protected override void OnUserOnLine(int serverSeat)
        {
        }

        public void ApplyBanker()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 2);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        public void ApplyQuit()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 3);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        private void CheckReJion(ISFSObject requestData)
        {
            long st = requestData.GetLong("st");
            long ct = requestData.GetLong("ct");
            if (st != 0)
            {
                if (ct - st < 15)
                {
                    CountDownUI.Instance.SetTimeCount(Convert.ToInt32(ct - st));
                }
            }
        }
        public void UserBet(int table, int gold)
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", 1);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
    }
}
