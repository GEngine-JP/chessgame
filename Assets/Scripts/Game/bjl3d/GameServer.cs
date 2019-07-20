using Assets.Scripts.Game.bjl3d.Scripts;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using UnityEngine;
namespace Assets.Scripts.Game.bjl3d
{
    public class GameServer : RemoteController
    {
        public static GameServer Instance;
        protected void Awake()
        {
            Instance = this;
        }
        protected override void OnGetGameInfo(ISFSObject gameInfo)
        {
            PlayerPrefs.DeleteAll();//需要删除

            CheckReJion(gameInfo);//检查重连
            var gdata = App.GetGameData<GlobalData>();
            gdata.CurrentUser = getUserInfo(gameInfo.GetSFSObject(RequestKey.KeyUser));//得到用户信息
            UserInfoUI.Instance.ShowSelfInfoUI();//显示游戏面板玩家自己的信息
            gdata.BankList = gameInfo.GetSFSArray("bankers");
            gdata.B = gameInfo.GetInt("banker");
            BankerInfoUI.Instance.ShowUserInfoUI();//设置庄家的信息

          

            gdata.Hisidx = gameInfo.GetInt("hisidx");
            gdata.History = gameInfo.GetIntArray("history");

            gdata.BankLimit = gameInfo.GetInt("bankLimit");

            gdata.Maxante = gameInfo.GetInt("maxante");
            BetHowMuchPromptUI.Instance.HowMuchPrompt();//初始化筹码面板上的可下注钱数
            BetHowMuchPromptUI.Instance.SetLuziInfo(gdata.History);//设置游戏的历史纪录信息
            BetHowMuchPromptUI.Instance.BottomLuzi();// 游戏底部的初始化历史纪录显示
            LuziInfoUI.Instance.ShowHistoryEx();//初始化的时候显示历史记录的效果
            BankerInfoUI.Instance.GameInnings();//面板显示游戏运行多少局
            UserInfoUI.Instance.GameConfig.isXianshi = false;
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
            var gdata = App.GetGameData<GlobalData>();
            switch (type)
            {
                //类型1 下注阶段
                case RequestType.Bet:
                    gdata.P = data.GetInt("p");
                    gdata.GoldOne = data.GetInt("gold");
                    gdata.UserSeat = data.GetInt("seat");
                    if (data.ContainsKey("allow"))
                    {
                        gdata.Allow = data.GetIntArray("allow");
                    }
                    PlanScene.Instance.UserNoteDataFun();//接收玩家的下注信息
                    GameScene.Instance.UserNoteDataFun();//下注位置特效的显示
                    UserInfoUI.Instance.ShowSelfInfoUI();//显示游戏面板玩家自己的信息
                    BetHowMuchPromptUI.Instance.HowMuchPrompt();//正常接收筹码面板可下注数
                    break;
                //类型3 申请上庄
                case RequestType.ApplyBanker:
                    break;
                //类型4 申请下庄
                case RequestType.ApplyQuit:
                    break;
                //类型5 开始下注
                case RequestType.BeginBet:
                    if (UserInfoUI.Instance.GameConfig.isXianshi)
                    {
                        BankerInfoUI.Instance.GameInnings();//面板显示游戏运行多少局
                    }
                    UserInfoUI.Instance.GameConfig.isXianshi = true;
                    UserInfoUI.Instance.GameConfig.GameState = 5;
                    StateUI.Instance.StateShow(5);
                    CountDownUI.Instance.NoticeXiaZhuFun();//游戏开始的时候时间的倒计时
                    break;
                //类型6 结束下注
                case RequestType.EndBet:
                    UserInfoUI.Instance.GameConfig.GameState = 6;
                    break;
                //类型7 发牌阶段
                case RequestType.GiveCards:
                    UserInfoUI.Instance.GameConfig.GameState = 7;
                    StateUI.Instance.StateShow(7);
                    ISFSObject xian = data.GetSFSObject("xian");
                    gdata.XianCards = xian.GetIntArray("cards");
                    gdata.XianValue = xian.GetInt("value");

                    ISFSObject zhuang = data.GetSFSObject("zhuang");
                    gdata.ZhuangCards = zhuang.GetIntArray("cards");
                    gdata.ZhuangValue = zhuang.GetInt("value");

                    CountDownUI.Instance.SendCardFun();//发牌阶段的处理操作
                    PaiPathScene.Instance.SendCardFun();//开始发牌
                    BetHowMuchPromptUI.Instance.BottomLuzi();//历史纪录的消息
                    break;
                //类型8 游戏结束
                case RequestType.Result:
                    UserInfoUI.Instance.GameConfig.GameState = 8;
                    App.GameData.GStatus = GameStatus.Normal;
                    gdata.Win = data.GetInt("win");//显示自己输赢
                    gdata.Total = data.GetLong("total");//当前金币数量
                    gdata.CurrentUser.Gold = App.GetGameData<GlobalData>().Total;
                    gdata.BetMoney = data.GetIntArray("pg");//结算的时候面板显示各个下注位置的钱数
                    gdata.BetJiesuan = data.GetIntArray("wp");//结算的时候面板显示各个下注位置的输赢
                    gdata.TodayWin = data.GetInt("todayWin");//今天输赢情况

                    GameScene.Instance.ClearBetEffect();
                    CountDownUI.Instance.ShowWinAreasFun();//显示游戏结束后的中奖区域
                    CountDownUI.Instance.GameResultFun();//打开下拉菜单
                    LuziInfoUI.Instance.ShowHistoryEx();//显示历史记录的效果
                    BetHowMuchPromptUI.Instance.Data();//刷新游戏的历史纪录的点数
                    GameUI.Instance.GameResult();//显示比赛结果
                     
                    break;
                //类型9 庄家列表
                case RequestType.BankerList:
                    UserInfoUI.Instance.GameConfig.GameState = 9;
                    gdata.ResultBnakerTotal = data.GetLong("bankTotal");
                    WaitForRankerListUI.Instance.RankerListData();//先清理庄家列表
                    gdata.BankList = data.GetSFSArray("bankers");
                    gdata.B = data.GetInt("banker");
                    BankerInfoUI.Instance.ShowUserInfoUI();//刷新庄家的信息
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

        public void UserBet(int table, int gold)
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("p", table);
            sfsObject.PutInt("gold", gold);
            sfsObject.PutInt("type", 1);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        public void ApplyBanker()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.ApplyBanker);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }

        public void ApplyQuit()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", RequestType.ApplyQuit);
            SendRequest(new ExtensionRequest(GameKey + RequestCmd.Request, sfsObject));
        }
        private void CheckReJion(ISFSObject requestData)
        {
            long st = requestData.GetLong("st");
            long ct = requestData.GetLong("ct");
            if (st != 0)
            {
                if (ct - st < 14)
                {
                    UserInfoUI.Instance.GameConfig.GameState = 5;
                    StateUI.Instance.StateShow(5);
                }
            }
        }
    }
}
