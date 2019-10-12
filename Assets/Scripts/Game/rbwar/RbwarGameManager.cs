using System;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.rbwar
{
    public class RbwarGameManager : YxGameManager
    {

        public TrendCtrl TrendCtrl;
        public CardCtrl CardCtrl;
        public BetCtrl BetCtrl;
        public TipCtrl TipCtrl;
        public List<RbwarPlayer> SpecialPlayers = new List<RbwarPlayer>();

        public int CurrentTableCount;

        private List<int> _resultShowList=new List<int>();
        private YxWindow _win;
        private bool _isFirst;

        private RbwarGameData _gdata
        {
            get { return App.GetGameData<RbwarGameData>(); }
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            TableUserShow();

            BetCtrl.InitChips();
            TrendCtrl.SetRecord();
        }

        public void OnTrendWindow()
        {
            _win = YxWindowManager.OpenWindow("TrendWindow");
            _isFirst = true;
            _win.UpdateView(_isFirst);
        }

        private void TableUserShow()
        {     
            var count = Math.Min(_gdata.GoldRank.Count, SpecialPlayers.Count);
            CurrentTableCount = count;

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < _gdata.AllUserInfos.Count; j++)
                {
//                    Debug.LogError("gdata.GoldRank[i]" + gdata.GoldRank[i] +"i"+i+ "gdata.AllUserInfos[j].Seat"+ gdata.AllUserInfos[j].Seat+"j"+j);
                    if (_gdata.GoldRank[i] == _gdata.AllUserInfos[j].Seat)
                    {
                        SpecialPlayers[i].Info = _gdata.AllUserInfos[j];
                    }
                }
            }
        }
        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        
        public override void GameStatus(int status, ISFSObject info)
        {
            var statusData = info.GetInt("status");
            switch (statusData)
            {
                case (int)GameState.Waiting:
                case (int)GameState.Start:
                case (int)GameState.ZhuangGold:
                    break;
                case (int)GameState.RollDice:
                case (int)GameState.XiaZhu:
                case (int)GameState.Over:
                    CardCtrl.CreatCards();
                    TipCtrl.ShowWaiting();
                    break;
            }
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            TipCtrl.Clear();
        }

        //消息容器
        [HideInInspector]
       public Queue<ISFSObject> ResponseQueue = new Queue<ISFSObject>();

        //是否开启消息延迟
        public bool _laterSend = false;

        public bool LaterSend
        {
            get { return _laterSend; }
            set
            {
                _laterSend = value;
                if (!value)
                {
                    while (ResponseQueue.Count > 0 && !LaterSend)
                    {
                        OnServerResponse(ResponseQueue.Dequeue());
                    }
                }
            }
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            if (LaterSend)
            {
                ResponseQueue.Enqueue(response);
            }
            else
            {
                OnServerResponse(response);
            }
        }

        public void OnServerResponse(ISFSObject response)
        {
            var type = response.GetInt("type");
            switch (type)
            {
                case (int)GameResponseState.BeginBet://给goldrank 和ssz

                    Clear();

                    var rankData = new Rank();
                    rankData.SetRank(response);
                    _gdata.GoldRank = rankData.GoldRank;
                    TableUserShow();

                    TipCtrl.CompareCardTip();
                    TipCtrl.BetTime(response.GetInt("cd"));
                    _gdata.BeginBet = true;
                    BetCtrl.ShowChip();
                    BetCtrl.Init();
                    CardCtrl.CreatCards();
                    break;
                case (int)GameResponseState.StopBet:
                    _gdata.BeginBet = false;

                    BetCtrl.HideChip();
                    TipCtrl.StopBetTip();
                    break;
                case (int)GameResponseState.Bet:
                    BetCtrl.Bet(response);

                  
                    break;
                case (int)GameResponseState.RollResult://牌值信息
                    CardValue cardValue = new CardValue();
                    cardValue.SetCardValue(response);
                    CardCtrl.ShowCardValue(cardValue);
                    break;
                case (int)GameResponseState.GameResult:
                    App.GameData.GStatus = YxEGameStatus.Normal;
                  
                    Result result=new Result();
                    result.SetResult(response);

                    _gdata.RecordCardType.Add(result.WinType);
                    _gdata.RecordSpot.Add(result.WinArea);

                    TrendCtrl.SetRecord(true);
                    _resultShowList.Clear();

                    if (result.IsHasLuck){ _resultShowList.Add(2);}
                    _resultShowList.Add(result.WinArea);

                    TipCtrl.Result(_resultShowList);
                  
                    SpecialPlayers[0].Clear();
                    if (_win != null)
                    {
                        _isFirst = false;
                        _win.UpdateView(_isFirst);
                    }
                  
                    break;
            }
        }

        public override void BeginReady()
        {
            base.BeginReady();
            CardCtrl.Clear();
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            var userInfo = new RbwarUserInfo();
            userInfo.Parse(sfsObject.GetSFSObject("user"));
            _gdata.AllUserInfos.Add(userInfo);
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
            base.UserOut(localSeat, responseData);
            var seat = responseData.GetInt("seat");

            for (int i = 0; i < _gdata.AllUserInfos.Count; i++)
            {
                if (seat == _gdata.AllUserInfos[i].Seat)
                {
                    _gdata.AllUserInfos.RemoveAt(i);
                }
            }
        }

        private void Clear()
        {
            foreach (var user in SpecialPlayers)
            {
                user.gameObject.SetActive(false);
            }
            //强制关闭输赢的煽动效果
            foreach (var t in TipCtrl.ResultShowList)
            {
                t.enabled = false;
                t.gameObject.SetActive(false);
            }
        }
    }
}
