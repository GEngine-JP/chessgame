using System;
using Assets.Scripts.Game.lswc.Control.System;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.States;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lswc.Manager
{
    public class LSServerManager : RemoteController
    {
        #region 测试数据


        public void TestData(int type, ISFSObject data)
        {
            if(type==0)
            {
                OnGetGameInfo(data);
            }
            else
            {
                OnServerResponse(data);
            }
        }

        #endregion
        protected override void OnGetGameInfo(ISFSObject gameInfo)
        {
            App.GetGameData<GlobalData>().InitGameInfo(gameInfo);
            LSSystemControl.Instance.InitState();
        }

        protected override void OnGetRejoinData(ISFSObject data)
        {
            OnGetGameInfo(data);
        }

        protected override void OnServerResponse(ISFSObject data)
        {
            if (!App.GetGameData<GlobalData>().GameInfoInit)
            {
                YxDebug.LogError("<color=red>游戏未初始化，不能收这个消息，Return</color>");
                return;
            }
            var type = data.GetInt(RequestKey.KeyType);

            YxDebug.Log("<color=green>收到了服务器返回的消息，消息类型是：" + type + "</color>");
            switch (type)
            {
                case (int)LSRequestMessageType.ON_BEGIN_BET:
                    App.GetGameData<GlobalData>().OnNewPage(data);
                    LSEmptyState.Instance.NextState = LSBetState.Instance;
                    LSEmptyState.Instance.Update(); 
                    break;
                case (int)LSRequestMessageType.ON_COLLECT_BET:
                    SendBetRequest(); 
                    break;
                case (int)LSRequestMessageType.ON_GET_RESULT:
                    App.GetGameData<GlobalData>().ISGetResult = true;
                    App.GetGameData<GlobalData>().InitNewResult(data);
                    break;
                case (int)LSRequestMessageType.BET:
                    YxDebug.LogError("下注成功");
                    break;

            }

        }

        protected override void OnUserOut(int seat)
        {
            YxDebug.LogError("当玩家退出");
        }

        protected override void OnUserIdle(int serverSeat)
        {
        }

        protected override void OnUserOnLine(int serverSeat)
        {
        }

        public void SendBetRequest()
        {
            if (App.GetGameData<GlobalData>().GlobalGameStatu == GameState.BetState)
            {
                App.GetGameData<GlobalData>().GlobalGameStatu = GameState.ResultState;
                App.GetGameData<GlobalData>().ISGetResult = false;
                if (App.GetGameData<GlobalData>().TotalBets == 0)
                {
                    YxDebug.Log("未下注，不发送");
                    return;
                }
                ISFSObject data = new SFSObject();
                data.PutInt(RequestKey.KeyType,(int)LSRequestMessageType.BET);
                data.PutIntArray(LSConstant.KeyAntes, App.GetGameData<GlobalData>().Bets);
                SendGameRequest(data);
                YxDebug.Log("发送下注请求,下注总金额是：" + App.GetGameData<GlobalData>().TotalBets);
            }
        }
    }
}
