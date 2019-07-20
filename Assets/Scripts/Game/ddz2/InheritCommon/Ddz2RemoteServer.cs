using System;
using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.ddz2.InheritCommon
{
    public class Ddz2RemoteServer : RemoteController
    {
        //-------静态相关-----------------------start------------------------------------
        private static EventHandler<DdzbaseEventArgs> _onGetGameInfoEvt = GlobalData.OnGetGameInfo;
        /// <summary>
        /// 添加服务器OnGameInfo事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void  AddOnGameInfoEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onGetGameInfoEvt += eventHandler;
        }

        private static EventHandler<DdzbaseEventArgs> _onGetRejoinDataEvt = GlobalData.OnGetRejoinData;
        /// <summary>
        /// 服务器OnGetRejoinData事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnGetRejoinDataEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onGetRejoinDataEvt += eventHandler;
        }


        private static readonly Dictionary<int, EventHandler<DdzbaseEventArgs>> OnServResponseEvtDic = new Dictionary<int, EventHandler<DdzbaseEventArgs>>();
        /// <summary>
        /// 添加服务器OnServResponse事件相关集合
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="eventHandler"></param>
        public static void AddOnServResponseEvtDic(int keyType ,EventHandler<DdzbaseEventArgs> eventHandler)
        {
            if (!OnServResponseEvtDic.ContainsKey(keyType))
            {
                OnServResponseEvtDic[keyType] = eventHandler;
                return;
            }
            OnServResponseEvtDic[keyType] += eventHandler;
        }


        private static EventHandler<DdzbaseEventArgs> _onUserJoinRoomEvt = GlobalData.OnPlayerJoinRoom;
        /// <summary>
        /// 添加服务器OnUserJoinRoom事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnUserJoinRoomEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onUserJoinRoomEvt += eventHandler;
        }

        private static EventHandler<DdzbaseEventArgs> _onUserOutEvt;
        /// <summary>
        /// 添加服务器OnUserOut事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnUserOutEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onUserOutEvt += eventHandler;
        }

        private static EventHandler<DdzbaseEventArgs> _onUserReadyEvt;
        /// <summary>
        /// 添加服务器OnUserReady事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnUserReadyEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onUserReadyEvt += eventHandler;
        }




        private static EventHandler<DdzbaseEventArgs> _onGameOverEvt;
        /// <summary>
        /// 游戏总结算事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnGameOverEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onGameOverEvt += eventHandler;
        }

        private static EventHandler<DdzbaseEventArgs> _onHandUpEvt;
        /// <summary>
        /// 投票解散事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnHandUpEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onHandUpEvt += eventHandler;
        }


        private static EventHandler<DdzbaseEventArgs> _onVoiceChatEvt;
        /// <summary>
        /// 语音聊天消息
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnVoiceChatEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onVoiceChatEvt += eventHandler;
        }


        private static EventHandler<DdzbaseEventArgs> _onMsgChatEvt;
        /// <summary>
        /// msg聊天系统
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnMsgChatEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onMsgChatEvt += eventHandler;
        }



        private static EventHandler<DdzbaseEventArgs> _onChangeDzhuEvt;
        /// <summary>
        /// 改变地主事件
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnchangeDizhu(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onChangeDzhuEvt += eventHandler;
        }

        private static EventHandler<DdzbaseEventArgs> _onGpsInfoReceive;
        /// <summary>
        /// 获得gps信息
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void AddOnGpsInfoReceiveEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onGpsInfoReceive += eventHandler;
        }


        private static EventHandler<DdzbaseEventArgs> _onUserIdle;

        public static void AddOnUserIdleEvt(EventHandler<DdzbaseEventArgs> eventHandler)
        {
            _onUserIdle += eventHandler;
        }

        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        void OnDestroy()
        {
            _onGetGameInfoEvt = GlobalData.OnGetGameInfo;
            _onGetRejoinDataEvt = GlobalData.OnGetRejoinData;
            _onUserJoinRoomEvt = GlobalData.OnPlayerJoinRoom;
            OnServResponseEvtDic.Clear();
            _onUserOutEvt = null;
            _onUserReadyEvt = null;
            _onGameOverEvt = null;
            _onHandUpEvt = null;
            _onVoiceChatEvt = null;
            _onMsgChatEvt = null;
            _onChangeDzhuEvt = null;
            _onGpsInfoReceive = null;
            _onUserIdle = null;
        }
        //-----------------------------------------------------------------------------------------------------------end

        protected override void OnAwake()
        {
            GlobalData.SetServInstance(this);
        }

        public override void Init(Dictionary<string, Action<ISFSObject>> responseDic)
        {

            responseDic[GameKey + RequestCmd.Ready] = OnUserReady;

            responseDic[GameKey + RequestCmd.JoinRoom] = OnUserJoinRoom;

            //改变底注
            responseDic[GameKey + "chAnte"] = OnGetAnte;

            //开房模式总结算
            responseDic[GameKey + "over"] = OnRoomGameOver;

            responseDic[GameKey + NewRequestKey.UserOutRoom] += OnPlayerOut;


            //投票解散
            responseDic["hup"] = OnHandsUp;

            //接收语音
            responseDic["sound"] = OnReceiveVoiceChat;

            responseDic[RequestCmd.UserTalk] = OnUserTalk;

            //收到GPS信息
            responseDic["locat"] = OnReceiveGps;


            //接受玩家离线消息
           // responseDic[NewRequestKey.KeyUserIdle] = OnUserIdle;

            HotRejoin = true;
        }

        /// <summary>
        /// 获得gps消息
        /// </summary>
        /// <param name="data"></param>
        private void OnReceiveGps(ISFSObject data)
        {
            //throw new NotImplementedException();
            if (_onGpsInfoReceive == null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onGpsInfoReceive(this, eventArgs);
        }

        /// <summary>
        /// 接收投票消息 2发起 3同意 -1拒绝
        /// </summary>
        public void OnHandsUp(ISFSObject data)
        {
            if(_onHandUpEvt==null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onHandUpEvt(this, eventArgs);
        }


        protected override void OnGetGameInfo(ISFSObject gameInfo)
        {
            if (_onGetGameInfoEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(gameInfo);
            _onGetGameInfoEvt(this, eventArgs);
        }

        protected override void OnGetRejoinData(ISFSObject data)
        {
            if (_onGetRejoinDataEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onGetRejoinDataEvt(this, eventArgs);
        }

        protected override void OnServerResponse(ISFSObject data)
        {
            var keyType = data.GetInt(RequestKey.KeyType);

            if (!OnServResponseEvtDic.ContainsKey(keyType))return;

            var eventArgs = new DdzbaseEventArgs(data);
            OnServResponseEvtDic[keyType](this, eventArgs);
        }

        protected override void OnUserOut(int seat)
        {
            if (_onUserOutEvt == null) return;

            ISFSObject data = new SFSObject();
            data.PutInt(RequestKey.KeySeat, seat);
            var eventArgs = new DdzbaseEventArgs(data);

            _onUserOutEvt(this, eventArgs);
        }

        protected override void OnUserIdle(int serverSeat)
        {
/*            if (_onUserOutEvt == null) return;

            ISFSObject data = new SFSObject();
            data.PutInt(RequestKey.KeySeat, serverSeat);
            data.PutBool(NewRequestKey.KeyUserIdle, true);
            var eventArgs = new DdzbaseEventArgs(data);

            _onUserOutEvt(this, eventArgs);*/

            if(_onUserIdle==null) return;
            ISFSObject data = new SFSObject();
            data.PutInt(RequestKey.KeySeat, serverSeat);
            data.PutBool(NewRequestKey.KeyUserIdle,true);
            var eventArgs = new DdzbaseEventArgs(data);
            _onUserIdle(this, eventArgs);
        }

        protected override void OnUserOnLine(int serverSeat)
        {
            if (_onUserIdle == null) return;
            ISFSObject data = new SFSObject();
            data.PutInt(RequestKey.KeySeat, serverSeat);
            data.PutBool(NewRequestKey.KeyUserIdle, false);
            var eventArgs = new DdzbaseEventArgs(data);
            _onUserIdle(this, eventArgs);
        }

        /// <summary>
        /// 当有用户准备时
        /// </summary>
        /// <param name="data"></param>
        private void OnUserReady(ISFSObject data)
        {
            if (_onUserReadyEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onUserReadyEvt(this, eventArgs);
        }


        /// <summary>
        /// 玩家加入房间
        /// </summary>
        /// <param name="data"></param>
        private void OnUserJoinRoom(ISFSObject data)
        {
            if (_onUserJoinRoomEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onUserJoinRoomEvt(this, eventArgs);
        }

        /// <summary>
        /// 当开房模式游戏总结算
        /// </summary>
        public void OnRoomGameOver(ISFSObject data)
        {
           if(_onGameOverEvt==null) return;
           var eventArgs = new DdzbaseEventArgs(data);
            _onGameOverEvt(this, eventArgs);
        }

        /// <summary>
        /// 收到音频消息
        /// </summary>
        /// <param name="data"></param>
        private void OnReceiveVoiceChat(ISFSObject data)
        {
            if (_onVoiceChatEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onVoiceChatEvt(this, eventArgs);
        }

        /// <summary>
        /// 收到玩家聊天信息
        /// </summary>
        /// <param name="data"></param>
        private void OnUserTalk(ISFSObject data)
        {
            if (_onMsgChatEvt == null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onMsgChatEvt(this, eventArgs);
        }

        /// <summary>
        /// 收到改变底注
        /// </summary>
        /// <param name="data"></param>
        private void OnGetAnte(ISFSObject data)
        {
            if(_onChangeDzhuEvt ==null) return;
            var eventArgs = new DdzbaseEventArgs(data);
            _onChangeDzhuEvt(this, eventArgs);
        }

        private void OnPlayerOut(ISFSObject data)
        {
/*            if (data.ContainsKey(RequestKey.KeyType) && data.GetInt(RequestKey.KeyType) == 6)
            {
                //房间解散时，清理出牌粒子特效，防止游戏崩溃
                App.GetGameData<GlobalData>().ClearParticalGob();
            }*/

        }


        //-------------------------------------------------------------------------------------------------
        
        
        /// <summary>
        /// 发送玩家准备网络请求
        /// </summary>
        public void SendPlayerReadyServCmd()
        {
            string key = GameKey + RequestCmd.Ready;
            IRequest request = new ExtensionRequest(key, SFSObject.NewInstance());
            SendRequest(request);
        }

        /// <summary>
        /// 发送抢地主 
        /// </summary>
        /// <param name="sit">叫分的座位</param>
        /// <param name="value">叫分值</param>
        public void CallGameScore(int sit,int value)
        {
            var obj = new SFSObject();
            obj.PutInt(GlobalConstKey.C_Type, GlobalConstKey.TypeGrab);
            obj.PutInt(GlobalConstKey.C_Sit, sit);
            obj.PutInt(GlobalConstKey.C_Score, value);
            SendGameRequest(obj);
        }


        /// <summary>
        /// 发起解散投票 2发起 3同意 -1拒绝
        /// </summary>
        public void StartHandsUp(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(GlobalConstKey.Cmd, NewRequestKey.KeyDismiss);
            iobj.PutInt(RequestKey.KeyType, yon);
            IRequest request = new ExtensionRequest(GlobalConstKey.Hup, iobj);
            SendRequest(request);
        }

        /// <summary>
        /// 发送自己不出请求
        /// </summary>
        public void TurnPass()
        {
            var obj = new SFSObject();
            obj.PutInt(GlobalConstKey.C_Type, GlobalConstKey.TypePass);
            obj.PutInt(GlobalConstKey.C_Sit, App.GetGameData<GlobalData>().GetSelfSeat);
            SendGameRequest(obj);
        }

        /// <summary>
        /// 发送出牌信息
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="laiziRepCds"></param>
        /// <param name="ctype"></param>
        public void ThrowOutCard(int[] cards, int[] laiziRepCds, int ctype)
        {
            if (cards == null || cards.Length < 1)
            {
                YxDebug.LogError("out card is wrong!");
                return;
            }

            if (laiziRepCds == null || laiziRepCds.Length < 1) laiziRepCds = new int[1] { -1 };

/*            int[] laizi = new int[1];
            if (magic == -1)
            {
                laizi[0] = -1;
            }
            else laizi[0] = magic;*/
            SFSObject sfsObj = CreateSfsObj(GlobalConstKey.TypeOutCard, App.GetGameData<GlobalData>().GetSelfSeat, cards, laiziRepCds, ctype);
            SendGameRequest(sfsObj); //----
        }


        /// <summary>
        /// 解散房间
        /// </summary>
        public void DismissRoom()
        {
            YxDebug.Log("解散房间!");
            IRequest req = new ExtensionRequest("dissolve", new SFSObject());
            SendRequest(req);
        }

        /// <summary>
        /// 游戏没开始时 ，离开房间
        /// </summary>
        public void LeaveRoom()
        {
            YxDebug.LogError("发出退出游戏房间消息");
            IRequest req = new ExtensionRequest(NewRequestKey.KeyLeaveRoom, new SFSObject());
            SendRequest(req);

            App.QuitGame();
        }


        /// <summary>
        /// 发送语音
        /// </summary>
        /// <param name="url"></param>
        /// <param name="len"></param>
        /// <param name="seat"></param>
        public void SendVoiceChat(string url, int len, int seat)
        {
            var sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, seat);
            sfsObj.PutInt("len", len);
            SendFrameRequest("sound", sfsObj);
        }

        /// <summary>
        /// 发送聊天消息 表情
        /// </summary>
        /// <param name="index"></param>
        public void UserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt("exp", index);
            iobj.PutInt("seat", App.GetGameData<GlobalData>().GetSelfSeat);
            IRequest request = new ExtensionRequest(GameKey + RequestCmd.UserTalk, iobj);
            SendRequest(request);
        }

        /// <summary>
        /// 发送聊天消息 文字
        /// </summary>
        /// <param name="text"></param>
        public void UserTalk(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("text", text);
            iobj.PutInt("seat", App.GetGameData<GlobalData>().GetSelfSeat);
            IRequest request = new ExtensionRequest(GameKey + RequestCmd.UserTalk, iobj);
            SendRequest(request);
        }

        /// <summary>
        /// 发送加倍信息
        /// </summary>
        /// <param name="rate"></param>
        public void SendDouble(int rate)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt("type", GlobalConstKey.TypeDouble);
            iobj.PutInt("rate", rate);
            SendGameRequest(iobj);
        }



        private SFSObject CreateSfsObj(int type, int sit, int[] cards, int[] magicRepcds, int ctype)
        {
            SFSObject obj = CreateSfsObj(type, sit);
            obj.PutIntArray(GlobalConstKey.C_Cards, cards);
            if (magicRepcds[0] != -1)
            {
                YxDebug.LogError("癞子代表的牌=======》" + magicRepcds[0] + magicRepcds.Length);
                obj.PutIntArray(GlobalConstKey.C_Magic, magicRepcds);
            }
            obj.PutInt("ctype", ctype);
            return obj;
        }

        private SFSObject CreateSfsObj(int type, int sit)
        {
            var obj = new SFSObject();
            obj.PutInt(GlobalConstKey.C_Type, type);
            obj.PutInt(GlobalConstKey.C_Sit, sit);
            return obj;
        }
    }
}
