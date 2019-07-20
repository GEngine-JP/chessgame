using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.Shuihuzhuan.Scripts
{
    public class GlobalData : YxGameData
    {
        public static GlobalData Instance;
        
        public static string GameKeyPre = "wmargin";

        public static  string GameKey = GameKeyPre + ".";
        //正式环境
        public  int ServerPort = 9933;
        public  string ZoneName = "chess";

#if UNITY_EDITOR
        public static string ServerName = "127.0.0.1";
        public static string ServerUrl = "http://test.server.com";
        //public static string ServerName = "192.168.1.2";//"127.0.0.1";//"www.kawuxing.com";        //public static string ServerName = "192.168.1.2";//"127.0.0.1";//"www.kawuxing.com";
        //public static string ServerUrl = "http://test.server.com";//"http://192.168.1.100/chess";

        //public static string ServerName = "121.40.136.206";
        //public static string ServerUrl = "http://www.0712.com";
#elif SIT
        public static string ServerName = "192.168.1.100";
        public static string ServerUrl = "http://192.168.1.100/chess";
#elif PROD
        public static string ServerName = "115.29.40.32";
        public static string ServerUrl = "http://115.29.40.32/chess";
#else
        public static string ServerName = "192.168.1.2";
        public static string ServerUrl = "http://test.server.com";
#endif
        public static string PingTestUrl = ServerUrl + "/pingtest.txt";
        public static string GatewayUrl = ServerUrl + "/index.php/Mobile/";
        public static string ConfigUrl = ServerUrl + "/config/config_{0}.json";
                /// <summary>
        /// 线数
        /// </summary>
        public int BetLineNum = 0;
        /// <summary>
        ///  总压注钱数
        /// </summary>
        public int BetNum = 0;
        /// <summary>
        /// 压注钱
        /// </summary>
        public int BetBaseNum = 0;
        /// <summary>
        /// 自己的总钱数
        /// </summary>
        public int MainMoney = 0;
        /// <summary>
        /// 小
        /// </summary>
        public  int Yazhu1 = 0;
        /// <summary>
        /// 大
        /// </summary>
        public  int Yazhu2 = 1;
        /// <summary>
        /// 和
        /// </summary>
        public  int Yazhu3 = 2;
//-----------------------------------------------------
        /// <summary>
        /// 是否自动
        /// </summary>
        public bool IsAuto = false;
        /// <summary>
        /// 切换游戏状态是否完成
        /// </summary>
        public bool changeState = false;
        /// <summary>
        /// 小玛丽的状态
        /// </summary>
        public bool   isMary =false ;
        /// <summary>
        /// 状态   1 刚下注  2  输了   3  赢了
        /// </summary>
        public int ZhuanState = 1;
        /// <summary>
        /// 进入玛丽的状态
        /// </summary>
        public bool Malizhuantai = false;

        public bool BeginBet = false;
//-----------------------------------------------------服务器数据
        /// <summary>
        /// 水浒传服务器数据 
        /// </summary>
        public int[] iTypeImgid = new int[15];

        /// <summary>
        /// 服务器数据
        /// </summary>
        public int[] iLineImgid = new int[18];
        /// <summary>
        /// 小和大服务器数据 
        /// </summary>
        public int[] iHistory = new int[10];
        /// <summary>
        /// 当局所得
        /// </summary>
        public int iWinMoney = -1;
        /// <summary>
        /// 骰子数
        /// </summary>
        public int iDice1 = 0;
        public int iDice2 = 0;
        /// <summary>
        //  玛丽次数
        /// </summary>
        public int iMaliGames = 0;
        /// <summary>
        /// 接受4个数组
        /// </summary>
        public int[] iMaliImage = new int[4];
        /// <summary>
        /// 玛丽转的图片
        /// </summary>
        public int iMaliZhuanImage = 0;
        /// <summary>
        /// 玛丽赢得钱数
        /// </summary>
        public int MaliWinMony = 0;
        /// <summary>
        ///下注上限
        /// </summary>
        public int iXiazhushangxian = 0;
        /// <summary>
        /// 线数
        /// </summary>
        public int iBetLineNum = 0;
        /// <summary>
        ///  总压注钱数
        /// </summary>
        public int iBetNum = 0;
        /// <summary>
        /// 压注钱
        /// </summary>
        public int iBetBaseNum = 0;
        /// <summary>
        /// 自己的总钱数
        /// </summary>
        public int iMainMoney = 0;

        public bool IsAotozhuangtai = true;
//-----------------------------------------------------
        /// <summary>
        ///9条线
        /// </summary>
        public int[,] m_TypeArray = { { 5, 6, 7, 8, 9 }, { 0, 1, 2, 3, 4 }, { 10, 11, 12, 13, 14 }, { 0, 6, 12, 8, 4 }, { 10, 6, 2, 8, 14 }, { 0, 1, 7, 3, 4 }, { 10, 11, 7, 13, 14 }, { 5, 11, 12, 13, 9 }, { 5, 1, 2, 3, 9 } };
        /// <summary>
        /// 结果的数据
        /// </summary>
        public int[,] m_ResultArray = { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
        /// <summary>
        /// 9条线初始动画
        /// </summary>
        public int[] m_LineType = new int[9];
        /// <summary>
        /// 显示秒动画的位置
        /// </summary>
        public int[] m_ShowSecAnimate = new int[15];
        /// <summary>
        /// 是否显示提示
        /// </summary>
        public bool show=false;

    }

 
    }

