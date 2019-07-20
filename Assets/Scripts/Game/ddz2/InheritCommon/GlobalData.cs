using System;
using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.ddz2.InheritCommon
{
    public class GlobalData : YxGameData
    {

        //-------------静态相关-------------------------start--

        /// <summary>
        /// 服务器的ServInstance
        /// </summary>
        public static Ddz2RemoteServer ServInstance { private set; get; }

        /// <summary>
        /// 服务器发来的gameinfodata缓存
        /// </summary>
        private static ISFSObject _onGameInfoData;

        /// <summary>
        /// 存储加入游戏的玩家信息
        /// </summary>
        private static readonly Dictionary<int, ISFSObject> UserDic = new Dictionary<int, ISFSObject>();

        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        void OnDestroy()
        {
            ServInstance = null;
            _onGameInfoData = null;
            UserDic.Clear();
        }

        public static void SetServInstance(Ddz2RemoteServer obj)
        {
            ServInstance = obj;
        }
        /// <summary>
        /// 响应服务器OnGetGameInfo事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnGetGameInfo(object sender, DdzbaseEventArgs args)
        {
            _onGameInfoData = args.IsfObjData;
            var userSelf = _onGameInfoData.GetSFSObject(RequestKey.KeyUser);
            UserDic[userSelf.GetInt(RequestKey.KeySeat)] = userSelf;
            if (!_onGameInfoData.ContainsKey(RequestKey.KeyUserList)) return;
            ISFSArray users = _onGameInfoData.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in users)
            {
                UserDic[user.GetInt(RequestKey.KeySeat)] = user;
            }
        }
        /// <summary>
        ///  响应服务器OnGetRejoinData事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnGetRejoinData(object sender, DdzbaseEventArgs args)
        {
            _onGameInfoData = args.IsfObjData;

            var selfUser = _onGameInfoData.GetSFSObject(RequestKey.KeyUser);
            UserDic[selfUser.GetInt(RequestKey.KeySeat)] = selfUser;

            ISFSArray users = _onGameInfoData.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in users)
            {
                UserDic[user.GetInt(RequestKey.KeySeat)] = user;
            }
        }

        /// <summary>
        /// 玩家加入房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnPlayerJoinRoom(object sender, DdzbaseEventArgs args)
        {
            var user = args.IsfObjData.GetSFSObject(RequestKey.KeyUser);
            UserDic[user.GetInt(RequestKey.KeySeat)] = user;
        }

        /// <summary>
        /// 卸载场景时清理容易导致程序崩溃的粒子特效资源
        /// </summary>
        protected override void OnGc()
        {
            base.OnGc();
            ClearParticalGob();
        }

        //---------------------------------------------------------------------------------------end

        void Awake()
        {
            Ddz2RemoteServer.AddOnGameInfoEvt(CheckGameStatus);
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(CheckGameStatus);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, TypeFirstOut);
        }

        /// <summary>
        /// 叫地主以后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat))
                LandSeat = data.GetInt(RequestKey.KeySeat);
        }

        /// <summary>
        /// 当有手牌数更新时,第一个int 是 userSeat 第二个 int 是 leftHandCdNum 
        /// </summary>
        private Action<int, int> _onHdCdsChange;
        /// <summary>
        /// 手牌数更新事件
        /// </summary>
        public Action<int, int> OnhandCdsNumChanged
        {
            set { _onHdCdsChange += value; }
        }
        /// <summary>
        /// 调用手牌数量改变全局事件
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="leftHandCdNum"></param>
        public void OnHdcdsChange(int userSeat, int leftHandCdNum)
        {
            if (_onHdCdsChange == null) return;

            _onHdCdsChange(userSeat, leftHandCdNum);
        }

        /// <summary>
        /// 当玩家分数改变时 第一个int 是 userSeat 第二个 int 是 scoreGold 
        /// </summary>
        private Action<int, int> _onUserScoreChanged;
        public Action<int, int> OnUserScoreChanged
        {
            set { _onUserScoreChanged += value; }
        }
        /// <summary>
        /// 调用玩家数据改变全局事件
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="scoreGold"></param>
        public void OnUserSocreChanged(int userSeat, int scoreGold)
        {
            if (_onUserScoreChanged == null) return;

            _onUserScoreChanged(userSeat, scoreGold);
        }

        //清理例子特效
        private Action _onClearParticalGob;
        public Action OnClearParticalGob
        {
            set { _onClearParticalGob += value; }
        }
        /// <summary>
        /// 清理例子特效，防止返回大厅崩溃
        /// </summary>
        public void ClearParticalGob()
        {
            if (_onClearParticalGob != null) _onClearParticalGob();
        }


        //加载声音clip
        [SerializeField]
        protected List<AudioClip> AudioClips = new List<AudioClip>();
        private readonly Dictionary<string, AudioClip> _audioClipDic = new Dictionary<string, AudioClip>();
        void Start()
        {
            foreach (var clip in AudioClips)
            {
                _audioClipDic[clip.name] = clip;
            }

            IsPlayVoiceChat = PlayerPrefs.GetInt(SettingCtrl.VoiceChatKey, 1) == 1;
            MusicAudioSource.volume = MusicManager.Instance.MusicVolume;
            //PlayerPrefs.GetFloat(SettingCtrl.MusicValueKey, 1);
        }

        public AudioClip GetSoundClip(string clipName)
        {
            return _audioClipDic.ContainsKey(clipName) ? _audioClipDic[clipName] : null;
        }

        /// <summary>
        /// 背景音乐
        /// </summary>
        public AudioSource MusicAudioSource;

        /// <summary>
        /// 是否播放语音
        /// </summary>
        public bool IsPlayVoiceChat;



        /// <summary>
        /// 根据座位号获取玩家信息
        /// </summary>
        /// <param name="userSeat"></param>
        /// <returns></returns>
        public ISFSObject GetUserInfo(int userSeat)
        {
            if (!UserDic.ContainsKey(userSeat)) return null;
            return UserDic[userSeat];
        }

        /// <summary>
        /// 获得所有参与游戏玩家的名字
        /// </summary>
        /// <returns></returns>
        public string[] GetUsersName()
        {
            var len = UserDic.Count;
            var userNames = new string[len];
            for (int i = 0; i < len; i++)
            {
                userNames[i] = UserDic[i].GetUtfString(RequestKey.KeyName);
            }

            return userNames;
        }

        /// <summary>
        /// 是否是开房游戏,默认是开房
        /// </summary>
        public bool IsRoomGame
        {
            get
            {
                var infoData = GetGameInfoData();
                if (infoData != null && infoData.ContainsKey(NewRequestKey.KeyGtype)) return infoData.GetInt(NewRequestKey.KeyGtype) == -1;
                return true;
            }
        }

        

        /// <summary>
        /// 玩家最大人数
        /// </summary>
        public int PlayerMaxNum
        {

            get
            {
                var infoData = GetGameInfoData();
                if (infoData != null && infoData.ContainsKey(NewRequestKey.KeyPlayerNum))
                    return infoData.GetInt(NewRequestKey.KeyPlayerNum);

                return 3;//默认3个
            }
        }

        /// <summary>
        /// 当前客户端玩家信息
        /// </summary>
        public ISFSObject UserSelfData
        {

            get
            {
                var infoData = GetGameInfoData();
                if (infoData != null && infoData.ContainsKey(RequestKey.KeyUser))
                    return infoData.GetSFSObject(RequestKey.KeyUser);

                return null;
            }
        }

        /// <summary>
        /// 玩家自己的座位
        /// </summary>
        public int GetSelfSeat
        {
            get
            {
                //return UserSelfData.GetInt(RequestKey.KeySeat);
                return SelfSeat;
            }
        }

        /// <summary>
        /// 玩家右边的座位
        /// </summary>
        public int GetRightPlayerSeat
        {
            get { return (GetSelfSeat + 1) % PlayerMaxNum; }
        }

        /// <summary>
        /// 玩家左边的位置
        /// </summary>
        public int GetLeftPlayerSeat
        {
            get { return (GetSelfSeat + 2) % PlayerMaxNum; }
        }


        /// <summary>
        /// 加倍类型 0不加倍 1正常加倍 2农民加倍
        /// </summary>
        public int JiaBeiType
        {
            get
            {
                var data = GetGameInfoData();
                if (data == null) throw new ArgumentNullException(paramName: "没有游戏Gameinfo" + "的信息");
                return data.GetInt(NewRequestKey.KeyJiaBei);
            }
        }

        /// <summary>
        /// 地主座位
        /// </summary>
        public int LandSeat { private set; get; }


        /// <summary>
        /// 标记整场比赛已经开始
        /// </summary>
        public bool IsStartGame { private set; get; }
        /// <summary>
        /// 开始叫分时,标记整场比赛已经开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnTypeGrabSpeaker(object sender, DdzbaseEventArgs args)
        {
            IsStartGame = true;
        }

        /// <summary>
        /// 房主Id
        /// </summary>
        private int _owerId;

        public bool IsFangZhu
        {
            get
            {
                var selfUser = UserDic[GetSelfSeat];
                return selfUser != null && selfUser.ContainsKey(RequestKey.KeyId) &&
                       selfUser.GetInt(RequestKey.KeyId) == _owerId;
            }
        }


        /// <summary>
        /// 检查游戏状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckGameStatus(object sender, DdzbaseEventArgs args)
        {


            var data = args.IsfObjData;

            //标记地主座位
            if (data.ContainsKey(NewRequestKey.KeyLandLord)) LandSeat = data.GetInt(NewRequestKey.KeyLandLord);

            if (data.ContainsKey(NewRequestKey.KeyOwerId))
            {
                _owerId = data.GetInt(NewRequestKey.KeyOwerId);
            }

            if (data.ContainsKey(NewRequestKey.KeyGameStatus)
                 && data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusIdle)
            {
                if (data.ContainsKey(NewRequestKey.KeyCurRound) && data.GetInt(NewRequestKey.KeyCurRound) <= 1)
                {
                    IsStartGame = false;
                    return;
                }
            }

            IsStartGame = true;

        }



        //-----------------------------------------------------
        /// <summary>
        /// 获得游戏信息体
        /// </summary>
        /// <returns></returns>
        private ISFSObject GetGameInfoData()
        {
            if (_onGameInfoData != null) return _onGameInfoData;
            return null;
        }

    }

    /// <summary>
    /// 一些服务器返回的值的判断
    /// </summary>
    public static class GlobalConstKey
    {

        //游戏的status
        /// <summary>
        /// 人员还没有都准备时
        /// </summary>
        public const int StatusIdle = 0;
        /// <summary>
        /// 选择庄家阶段
        /// </summary>
        public const int StatusChoseBanker = 1;

        /// <summary>
        /// 加倍及以后阶段
        /// </summary>
        public const int StatusDouble = 2;




        /// <summary>
        /// 游戏类型
        /// </summary>
        public enum GameType
        {
            /// <summary>
            /// 叫分
            /// </summary>
            CallScore = 0,
            /// <summary>
            /// 踢地主
            /// </summary>
            Kick,
            /// <summary>
            /// 抢地主
            /// </summary>
            Grab,
            /// <summary>
            /// 叫分带流局
            /// </summary>
            CallScoreWithFlow,
        }

        /*        /// <summary>
                /// 叫分带流局
                /// </summary>
                public const int CallScoreWithFlow = 3;*/



        //OnServerResponse的各种类型-------------------start
        /// <summary>
        /// 发牌
        /// </summary>
        public const int TypeAllocate = 1;

        /// <summary>
        /// 抢地主
        /// </summary>
        public const int TypeGrab = 2;

        /// <summary>
        /// 出牌
        /// </summary>
        public const int TypeOutCard = 3;

        /// <summary>
        /// 不出
        /// </summary>
        public const int TypePass = 4;

        /// <summary>
        /// 抢到地主,先出牌
        /// </summary>
        public const int TypeFirstOut = 5;

        /// <summary>
        /// 游戏结束 显示结算
        /// </summary>
        public const int TypeGameOver = 6;

        /// <summary>
        /// 指定抢地主或者叫分
        /// </summary>
        public const int TypeGrabSpeaker = 7;

        /// <summary>
        ///流局黄庄
        /// </summary>
        public const int TypeFlow = 8;

        /// <summary>
        /// 加倍
        /// </summary>
        public const int TypeDouble = 9;

        /// <summary>
        /// 加倍结束
        /// </summary>
        public const int TypeDoubleOver = 10;



        /// <summary>
        /// 向服务器发送的请求key名
        /// </summary>
        public const string C_Type = "type";
        public const string C_Cards = "cards";
        public const string C_Sit = "seat";
        public const string C_OPCard = "opCard";
        public const string C_Score = "score";
        public const string C_Magic = "laizi";
        public const string C_ante = "cante";
        public const string C_needRejoin = "needRejoin";


        public const string Cmd = "cmd";
        public const string Hup = "hup";


        /// <summary>
        /// 标记这个玩家程序暂停了 的聊天expid
        /// </summary>
        public const int AppPauseExpId = 888;

        /// <summary>
        /// 标记这个玩家程序解除暂停了
        /// </summary>
        public const int AppComBackExpId = 999;
    }
}
