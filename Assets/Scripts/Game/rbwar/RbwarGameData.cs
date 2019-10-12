using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.rbwar
{
    public class RbwarGameData : YxGameData
    {
        public bool BeginBet;
        public float UnitTime = 0.1f;
        public List<int> RecordSpot=new List<int>();
        public List<int> RecordCardType = new List<int>();
//        public List<int> RecordWinValue=new List<int>();
        public List<int> LuckRate=new List<int>();
        public int PlayerRecordNum;
        public List<int> GoldRank=new List<int>();//排行中玩家的座位号
        public List<RbwarUserInfo> AllUserInfos = new List<RbwarUserInfo>();
        public long AllUserWinGolds;


        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            PlayerRecordNum = gameInfo.ContainsKey("playerRecordNum") ? gameInfo.GetInt("playerRecordNum") : -1;

            var rankData=new Rank();
            rankData.SetRank(gameInfo);
            GoldRank = rankData.GoldRank;

            var  luckRate =  gameInfo.ContainsKey("luckRate") ? gameInfo.GetIntArray("luckRate") : null;
            if (luckRate != null) LuckRate = new List<int>(luckRate);
            var record = gameInfo.ContainsKey("record") ? gameInfo.GetSFSArray("record") : null;
            if (record != null)
            {
                for (int i = 0; i < record.Count; i++)
                {
                    if(record.GetSFSObject(i)==null)continue;
                    var recordValue = new RecordValue();
                    recordValue.SetRecord(record.GetSFSObject(i));
                    RecordSpot.Add(recordValue.Area);
                    RecordCardType.Add(recordValue.CType);
                }
            }

            var user= gameInfo.ContainsKey("user") ? gameInfo.GetSFSObject("user") : null;
            if (user != null)
            {
                var userInfo = new RbwarUserInfo();
                userInfo.Parse(user);
                GetPlayer<RbwarPlayer>().Info = userInfo;
                AllUserInfos.Add(userInfo);
            }

            var users=gameInfo.ContainsKey("users")?gameInfo.GetSFSArray("users"):null;
            if (users != null)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    var userInfo=new RbwarUserInfo();
                    userInfo.Parse(users.GetSFSObject(i));
                    AllUserInfos.Add(userInfo);
                }
            }
        }
    }

    public enum GameState
    {
        Waiting,
        Start,
        ZhuangGold,
        RollDice,
        XiaZhu,
        Over
    }

    public enum GameResponseState
    {
        BeginBet=105,
        StopBet=106,
        Bet=107,//下注交互 以此发请求
        RollResult= 108,
        GameResult = 109
    }

    public class RecordValue
    {
        public int Area;
        public int CType;

        public void SetRecord(ISFSObject data)
        {
            Area = data.ContainsKey("area") ? data.GetInt("area") : -1;
            CType = data.ContainsKey("ctype") ? data.GetInt("ctype") : -1;
        }
    }


    public class CardValue
    {
        public int Win;

        public int[] BlackCards = new int[3];
        public int BlackCardType;
        public int [] RedCards=new int[3];
        public int RedCardType;


        public void SetCardValue(ISFSObject data)
        {
            var black = data.ContainsKey("black") ? data.GetSFSObject("black") : null;
            if (black != null)
            {
                BlackCards = black.ContainsKey("cards") ? black.GetIntArray("cards") : null;
                BlackCardType = black.ContainsKey("cardsType") ? black.GetInt("cardsType") :-1;
            }
          
            var red = data.ContainsKey("red") ? data.GetSFSObject("red") : null;
            if (red != null)
            {
                 RedCards=red.ContainsKey("cards") ? red.GetIntArray("cards") : null;
                RedCardType = red.ContainsKey("cardsType") ? red.GetInt("cardsType") : -1;
            }
        }
    }

    public class Rank
    {
        public List<int> GoldRank = new List<int>();

        public void SetRank(ISFSObject data)
        {
            var sszSeat = data.ContainsKey("ssz") ? data.GetInt("ssz") : -1;//神算子的座位号
            var goldRank = data.ContainsKey("goldRank") ? data.GetIntArray("goldRank") : null;
            if (goldRank != null)
            {
                GoldRank = new List<int>(goldRank);
//                YxDebug.LogArray(GoldRank);
                GoldRank.Insert(0, sszSeat);
//                YxDebug.LogArray(GoldRank);
            }
        }
    }
    

    public class Result
    {
        public int Win;//赢的钱
        public long Total;//玩家的总钱数
        public int WinArea;
        public int WinType;
        public int WinValue;
        public bool IsHasLuck;
        public ISFSArray Playerlist=new SFSArray();
        

        public void SetResult(ISFSObject data)
        {
            Win = data.ContainsKey("win") ? data.GetInt("win") : -1;
            Total = data.ContainsKey("total") ? data.GetLong("total") :-1;
            WinArea = data.ContainsKey("winArea") ? data.GetInt("winArea") : -1;
            WinType = data.ContainsKey("winType") ? data.GetInt("winType") : -1;
            WinValue = data.ContainsKey("WinValue") ? data.GetInt("WinValue") : -1;
            IsHasLuck = data.ContainsKey("luck") && data.GetBool("luck");
            Playerlist = data.ContainsKey("playerlist") ? data.GetSFSArray("playerlist") : null;
            if (Playerlist != null)
            {
                var gdata = App.GetGameData<RbwarGameData>();
                gdata.AllUserWinGolds = 0;
                for (int i = 0; i < Playerlist.Count; i++)
                {
                    var userData = Playerlist.GetSFSObject(i);
                    var twentyBet = userData.ContainsKey("twentyBet") ? userData.GetInt("twentyBet") : -1;
                    var win = userData.ContainsKey("win") ? userData.GetInt("win") : -1;
                    var ttgold = userData.ContainsKey("ttgold") ? userData.GetLong("ttgold") : -1;
                    var userName = userData.ContainsKey("username") ? userData.GetUtfString("username") : "";
                    var seat = userData.ContainsKey("seat") ? userData.GetInt("seat") : -1;
                    var twentyWin = userData.ContainsKey("twentyWin") ? userData.GetInt("twentyWin") : -1;
                    var userId = userData.ContainsKey("userid") ? userData.GetInt("userid") : -1;

                    if (win > 0)
                    {
                        gdata.AllUserWinGolds += win;
                    }

                    for (int j = 0; j < gdata.AllUserInfos.Count; j++)
                    {
                        if (seat == gdata.AllUserInfos[j].Seat)
                        {
                            gdata.AllUserInfos[j].TwentyBet = twentyBet;
                            gdata.AllUserInfos[j].WinCoin = win;

                            gdata.AllUserInfos[j].CoinA = ttgold;
                            gdata.AllUserInfos[j].NickM = userName;
                            gdata.AllUserInfos[j].TwentyWin = twentyWin;
                            gdata.AllUserInfos[j].UserId = userId.ToString();
                        }

                    }
                } 
            }
        }
    }
}
