using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.jsys
{
    public class GameServer : RemoteController
    {
        protected override void OnGetGameInfo(ISFSObject gameInfo)
        {
            ISFSObject user = gameInfo.GetSFSObject("user");
            App.GetGameData<GlobalData>().UserMoney = user.GetLong("ttgold");
            BetPanelManager.Instance.SetMoney(App.GetGameData<GlobalData>().UserMoney); //金币显示   
            App.GetGameData<GlobalData>().BetTime = gameInfo.GetInt("cd");
            App.GetGameData<GlobalData>().History = gameInfo.GetIntArray("history");//历史纪录
            App.GetGameData<GlobalData>().Multiplying = gameInfo.ContainsKey("rates") ? gameInfo.GetIntArray("rates") : null;
            if (App.GetGameData<GlobalData>().Multiplying != null)
            {
                //进入游戏的时候下注位置的倍率

                BetPanelManager.Instance.ShowImultiply(App.GetGameData<GlobalData>().Multiplying);

               
            }
            HistoryManager.Instance.ShowHistory(App.GetGameData<GlobalData>().History);
            //判断是否为重连
            if (gameInfo.ContainsKey("rejoin"))
            {
                App.GetGameData<GlobalData>().Rejoin = gameInfo.GetBool("rejoin");
                App.GetGameData<GlobalData>().Svt = gameInfo.GetLong("svt");
                App.GetGameData<GlobalData>().StartTime = gameInfo.GetLong("startTime");
            }
            BetPanelManager.Instance.Gamewaitshow();
            App.GetGameData<GlobalData>().StartBet = false;
            
             BetPanelManager.Instance.ShowBetButton(false);
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
                    //开始下注阶段
                case RequestType.BetStar:
                    App.GetGameData<GlobalData>().StartBet = true;
                    App.GetGameData<GlobalData>().BetTime = data.GetInt("cd");
                    App.GetGameData<GlobalData>().Multiplying = data.ContainsKey("rates") ? data.GetIntArray("rates") : null;
                    if (App.GetGameData<GlobalData>().Multiplying != null)
                    {
                        //接收游戏的下注位置的倍率的变化
                        for (int i = 0; i < App.GetGameData<GlobalData>().Multiplying.Length; i++)
                        {
                            //YxDebug.Log("位置" + i);
                            //YxDebug.Log("倍数" + App.GetGameData<GlobalData>().Multiplying[i]);
                        }
                        BetPanelManager.Instance.ShowImultiply(App.GetGameData<GlobalData>().Multiplying);
                    }
                    //显示下注界面
                    BetPanelManager.Instance.ShowUI();
                    BetPanelManager.Instance.ShowBetButton(true);

                    TimerManager.Instance.SetClock(App.GetGameData<GlobalData>().BetTime);
                    break;
                    //下注结束阶段
                case RequestType.BetFinish:
                    BetPanelManager.Instance.ShowBetButton(false);
                    //收到服务器下注结束后即发送本地筹码到服务器
                    BetPanelManager.Instance.SendBet();

                    break;

                    //结算阶段
                case RequestType.Result:
                    App.GetGameData<GlobalData>().StartBet =false;
                    BetPanelManager.Instance.ShowBetButton(false);
                    App.GetGameData<GlobalData>().IsShark = data.ContainsKey("fish") ? data.GetBool("fish") : false;
                    if (App.GetGameData<GlobalData>().IsShark)
                    {
                        App.GetGameData<GlobalData>().FishIdx =data.ContainsKey("fishIdx")? data.GetInt("fishIdx"):1;
                    }

                    ISFSObject user = data.GetSFSObject("user");
                    App.GetGameData<GlobalData>().Winning = user.ContainsKey("wcj") ? user.GetInt("wcj") : 0;

                    App.GetGameData<GlobalData>().Ante = user.GetInt("ante");
                    if (user.ContainsKey("ttgold"))
                    {
                        App.GetGameData<GlobalData>().BetBehindMoney = user.GetLong("ttgold");
                        App.GetGameData<GlobalData>().UserMoney = App.GetGameData<GlobalData>().BetBehindMoney;
                    }
                    //else
                    //{
                    //    GlobalData.Instance.BetBehindMoney = GlobalData.Instance.UserMoney;
                    //}


                    App.GetGameData<GlobalData>().Gold = user.GetInt("gold");

                    if (data.ContainsKey("rs"))
                    {
                        Random random = new Random();
                        App.GetGameData<GlobalData>().StarPos = random.Next(0, 28);
                        App.GetGameData<GlobalData>().EndPos = data.GetInt("rs");
                    }
                     int time = data.GetInt("cd");
                        //计算此过程所需要的时间
                     TimerManager.Instance.Wait(time);
                     
                     //开始运行游戏
                    TurnGroupsManager.Instance.PlayGame();
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

        //发送押注数据
        public void ClickedToSend(int[] yaZhu)
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type",1);
            sfsObject.PutIntArray("antes", yaZhu);
            
            SendGameRequest(sfsObject);
        }
    }
}
