using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.brnn3d
{
    public class GlobalData : YxGameData
    {
        public int UserSeat;//玩家座位
        public int BetMoney;//玩家下注的钱数
        public int BetPos;//玩家在桌子上下注的位置
        public int BetPosSelf;//自己下注的位置
        public int DicNum;//骰子的点数
        public ISFSArray BankList;//庄家列表信息
        public bool IsBet;//是否可以下注
        public int Frame = 1;//记录游戏运行多少局
        public int Bundle;//记录游戏玩了多少把
        public int Bkmingold;//上庄最低限制

        public int SendCardPosition;//发牌的位置

        public long[] I64ChoumaValue;// 10个筹码的值

        public ISFSArray Cards;//获得牌组信息
        public ISFSArray Nn;//获得开奖时各个座位的信息

        public int B;//庄家的座位

        public int ChouMaType;//筹码类型

        public bool isOut=true;
        


        public Dictionary<int, Transform> PaiAllShow = new Dictionary<int, Transform>(); //牌是否全显示完全


        public bool IsKai = true;//路子信息界面是否是开的
        //用户列表是否展开
        public bool IsblKai = true;

        public long CurrentMoneyRoom = 0;

        public long CurrentMoneyBank = 0;

        //
        public bool IsCanShowBetMuch = false;




        /// <summary>
        /// 提示信息文档
        /// </summary>
        public string ShangZhuangMoneyLos = "玩家申请上庄最少{0}金币";
        public string NextXiaZuang = "您当前正在庄上，下局开始前自动下庄！！！";

        /// <summary>
        /// 自己
        /// </summary>
        public UserInfo CurrentUser = new UserInfo();
        public List<UserInfo> UserList = new List<UserInfo>();
        public UserInfo CurrentBanker = new UserInfo();


        public bool BeginBet = false;
        public int CurrentCanInGold = -1;
        public int MiniApplyBanker = 50000;
        public long ThisCanInGold = 0;
        public long ResultBnakerTotal = 0;
        public int ResultUserTotal = 0;

        public bool IsMusicOn = true;

        public List<string> RadioList;
    }
}
