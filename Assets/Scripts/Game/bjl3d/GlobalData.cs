using System.Collections.Generic;
using Assets.Scripts.Game.bjl3d.Scripts;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.bjl3d
{
    public class GlobalData : YxGameData
    {
        /// <summary>
        /// 用户的登陆账号
        /// </summary>
        public  string LoginName;
        public  string Password;
        public  string UserId;
        public  string UserToken;


        /// <summary>
        /// 交互信息
        /// </summary>
        public int[] AreaMaxZhu;//每个区域最多能下多少钱

        public int P;//下注时桌面的位置
        public int GoldOne;//下注的单次筹码大小
        public int UserSeat;//玩家的座位号
        public int[] GoldNum=new int[7];//筹码的钱数
        public int[] XianCards;//闲家的牌组
        public int XianValue;//闲的点数
        public int[] BetMoney=new int[ 8];//游戏结束各个位置下注的钱数
        public int[] BetJiesuan = new int[8];//游戏结算的时候每个位置的输赢 
        public int Win;//当前局数输赢的钱数
        public long Total;//当前的金币数量
        public int TodayWin;//今日总合计钱数
        public ISFSArray BankList;//等待上庄庄家的列表
        public int B;//庄家的座位号
        public int[] History=new int[12];//历史记录信息
        public int Hisidx;//历史纪录信息的索引 
        public int BankLimit;//申请下注的最少钱数
        public int Maxante;//游戏开始时位置钱数的限制
        public int[] Allow=new int[8];//游戏进行时个位置的可下注钱数变化，每隔0.5秒变化一次，所以需要判断是否含有这个字段
        

        public int[] ZhuangCards;//庄家的牌组
        public int ZhuangValue;//庄的点数


        /// <summary>
        /// 自己
        /// </summary>
        public  UserInfo CurrentUser = new UserInfo();
        public  List<UserInfo> UserList = new List<UserInfo>(); 
        public  UserInfo CurrentBanker = new UserInfo();
        public  bool BeginBet = false;//是否开始下注
        public int CurrentCanInGold = -1;
        public  long ThisCanInGold = 0;
        public  long ResultBnakerTotal = 0;
        public  int ResultUserTotal = 0;
        public  bool Forbiden;
        /// <summary>
        /// 程序的启动参数
        /// </summary>
        public  Dictionary<string, string> BootEnv;
        /// <summary>
        /// 房间号
        /// </summary>
        public  int RoomType = 0;

        public  bool IsMusicOn = false;

        public  List<string> RadioList;
    }
}

