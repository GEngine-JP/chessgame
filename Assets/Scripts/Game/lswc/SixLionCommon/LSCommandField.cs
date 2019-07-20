using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.LX.SixLionCommon
{


//////////////////////////////////////////////////////////////////////下面是原来的，额///////////////////////////////////////////////////
    /// <summary>
    /// 动物类型定义
    /// </summary>
    [System.Flags]
    public enum Animal : byte
    {

        none = 0,
        /// <summary>
        /// 狮子
        /// </summary>
        lion = 1,
        /// <summary>
        /// 熊猫
        /// </summary>
        panda = 2,
        /// <summary>
        /// 猴子
        /// </summary>
        monkey = 3,
        /// <summary>
        /// 兔子
        /// </summary>
        rabbit = 4,
        /// <summary>
        /// 金色狮子
        /// </summary>
        GoldenLion = 5,
        /// <summary>
        ///金色熊猫
        /// </summary>
        GoldPanda = 6,
        /// <summary>
        ///金色猴子
        /// </summary>
        GoldMonkey = 7,
        /// <summary>
        ///金色兔子
        /// </summary>
        GoldRabbit = 8
    }

    /// <summary>
    /// 颜色类型定义 
    /// </summary>
    [System.Flags]
    public enum ColorType : byte
    {
        none = 0,
        /// <summary>
        /// 红色
        /// </summary>
        red = 1,
        /// <summary>
        ///黄色
        /// </summary>
        yellow = 2,
        /// <summary>
        /// 绿色
        /// </summary>
        green = 3,
    }


    /// <summary>
    /// 开奖记录图片显示
    /// </summary>
    public enum AwardRecordType : byte
    {

        point = 0,
        hz_c = 1,
        hz_r = 2,
        hz_y = 3,
        hz_g = 4,
        jhz_r = 5,
        jhz_y = 6,
        jhz_g = 7,
        sz_c = 8,
        sz_r = 9,
        sz_y = 10,
        sz_g = 11,
        jsz_r = 12,
        jsz_y = 13,
        jsz_g = 14,
        tz_c = 15,
        tz_r = 16,
        tz_y = 17,
        tz_g = 18,
        jtz_r = 19,
        jtz_y = 20,
        jtz_g = 21,
        xm_c = 22,
        xm_r = 23,
        xm_y = 24,
        xm_g = 25,
        jxm_r = 26,
        jxm_y = 27,
        jxm_g = 28,
        all_r = 29,
        all_y = 30,
        all_g = 31

    }


    /// <summary>
    ///开奖结果图片显示
    /// </summary>
    public enum AwardResult : byte
    {

        ar_tz_r = 1,
        ar_tz_g,
        ar_tz_y,
        ar_hz_r,
        ar_hz_g,
        ar_hz_y,
        ar_xm_r,
        ar_xm_g,
        ar_xm_y,
        ar_sz_r,
        ar_sz_g,
        ar_sz_y,
        ar_zx_z,
        ar_zx_h,
        ar_zx_x


    }


    /// <summary>
    /// 与服务器的协议定义
    /// </summary>
    public class ProtocolDefine
    {



        /// <summary>
        ///  初始化游戏状态
        /// </summary>
        public const uint ASS_GAME_STATION = 2;
        public const uint ASS_BEGIN = 55;
        /// <summary>
        /// 下注
        /// </summary>

        public const uint ASS_XIAZHU = 56;
        /// <summary>
        /// 下注结果
        /// </summary>
        public const uint ASS_XIAZHU_RESULT = 57;
        /// <summary>
        /// 开始转盘
        /// </summary>
        public const uint ASS_PLAY_GAME = 58;
        /// <summary>
        ///  开始新一局 
        /// </summary>
        public const uint ASS_NEWTURN_START = 59;
        /// <summary>
        /// 兑换金币 ，服务端用
        /// </summary>

        public const uint ASS_EXCHANGE = 60;
        /// <summary>
        ///  兑换金币,客户端用
        /// </summary>
        public const uint ASS_EXCHANGE_RESULT = 61;
        /// <summary>
        /// 是否为超端用户
        /// </summary>
        public const uint ASS_S_C_ISSUPER = 62;
        /// <summary>
        /// 上传超端控制结果
        /// </summary>
        public const uint T_C_S_SUPERCONTROL = 63;
        /// <summary>
        /// 超端控制成功和失败
        /// </summary>
        public const uint T_S_C_SUPERCONTROL_RES = 64;

        /// <summary>
        /// 改变赔率
        /// </summary>
        public const uint S_C_CHANGE_MULTIPLY = 66;

        //  public const uint ASS_CHECK_RESULT = 100;


        //  public const uint ASS_CHECK_RESULT_2 = 101;


    }

    public enum SeverGameState : byte
    {


        //游戏状态定义
        GS_WAIT_SETGAME = 0,			//等待东家设置状态
        GS_BET = 20,				//下注状态
        GS_COLLECTION_BET = 21,				//收集下注筹码
        GS_PLAY_GAME = 22,			//开奖过程
        GS_WAIT_NEXT = 23,			//等待下一盘开始 

    }


    /// <summary>
    /// 游戏玩法定义 ,游戏环节类型
    /// </summary>
    public enum GameType : byte
    {
        /// <summary>
        /// 正常环节
        /// </summary>
        GS_CHILD_NORMAL = 1,
        /// <summary>
        /// 大三元环节
        /// </summary>
        GS_CHILD_BIG_THREE = 2,
        /// <summary>
        /// 大四喜环节
        /// </summary>
        GS_CHILD_BIG_FOUR = 3,
        /// <summary>
        /// 闪电环节
        /// </summary>
        GS_CHILD_LINGHTNING = 4,
        /// <summary>
        /// 送灯环节
        /// </summary>
        GS_CHILD_SEND_LAMP = 5,
        /// <summary>
        /// 彩金环节 
        /// </summary>
        GS_CHILD_HANDSEL = 6,
    }

    public enum PlatformConstant
    {
        /// <summary>
        /// 框架消息,bMainID
        /// </summary>
        MDM_GM_GAME_FRAME = 150,

        /// <summary>
        /// 游戏消息,bMainID
        /// </summary>
        MDM_GM_GAME_NOTIFY = 180,
        /// <summary>
        /// 游戏信息,bAssistantID
        /// </summary>
        ASS_GM_GAME_INFO = 1,

        /// <summary>
        /// 游戏状态,bAssistantID
        /// </summary>
        ASS_GM_GAME_STATION = 2,
        /// <summary>
        /// 强行退出,bAssistantID
        /// </summary>
        ASS_GM_FORCE_QUIT = 3,



    }
}
