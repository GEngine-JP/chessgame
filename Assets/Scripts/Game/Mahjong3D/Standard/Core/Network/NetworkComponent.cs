using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using System.Reflection;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NetworkComponent : BaseComponent, IGameEndCycle
    {
        /// <summary>
        /// 换房间
        /// </summary>
        public Action ChangeRoomEvent;
        /// <summary>
        /// 自定义C2S请求事件
        /// </summary>
        private C2SCustomRequest mCustomC2SRequest;
        /// <summary>
        /// 发送请求
        /// 发送请求的协议是字符串
        /// </summary>
        public Action<string, ISFSObject> SendRequest;
        /// <summary>
        /// 请求事件
        /// </summary>
        private event Action<ISFSObject> mRequestC2SEvent;
        /// <summary>
        /// 持有服务器监听事件的对象缓存
        /// </summary>
        private List<AbsGameLogicBase> mGameResponseLogicCache = new List<AbsGameLogicBase>();
        /// <summary>
        /// 响应服务器事件
        /// </summary>
        private Dictionary<int, Action<ISFSObject>> mResponseHandlers = new Dictionary<int, Action<ISFSObject>>();
        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<KeyValuePair<int, ISFSObject>> mS2CResponseQueue = new Queue<KeyValuePair<int, ISFSObject>>();
        /// <summary>
        /// 不执行Op逻辑的响应
        /// </summary>
        private List<int> mSimpleFilter = new List<int>();
        /// <summary>
        /// 消息延迟事件
        /// </summary>
        private float mResponseDelayTimer = 0.02f;

        /// <summary>
        /// 自定义C2S的请求事件
        /// </summary>
        public C2SCustomRequest C2S
        {
            get
            {
                if (null == mCustomC2SRequest) { mCustomC2SRequest = new C2SCustomRequest(); }
                return mCustomC2SRequest;
            }
        }

        public void RefreshResponseQueue()
        {
            mS2CResponseQueue.Clear();
        }

        /// <summary>
        /// 获取S2C定义的事件
        /// </summary>     
        public Action<ISFSObject> DispatchResponseHandlers(int key)
        {
            if (mResponseHandlers.ContainsKey(key))
            {
                return mResponseHandlers[key];
            }
            return null;
        }

        /// <summary>
        /// 获取S2C定义的事件
        /// </summary>     
        public T GetGameResponseLogic<T>() where T : AbsGameLogicBase
        {
            string name = typeof(T).FullName;
            for (int i = 0; i < mGameResponseLogicCache.Count; i++)
            {
                if (mGameResponseLogicCache[i].GetType().FullName == name)
                {
                    return mGameResponseLogicCache[i] as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 控制重连开关 重后台返回游戏时 执行重连
        /// </summary>
        public void CtrlYuleRejoin(bool flag)
        {
            GetComponent<MahGameManager>().NeedRejoinByFocus = flag;
        }

        public override void OnLoadAssembly(Type type)
        {
            var logicAtts = type.GetCustomAttributes(typeof(S2CResponseLogicAttribute), false);
            if (null == logicAtts || logicAtts.Length == 0)
            {
                return;
            }
            //获取响应对象
            var response = Activator.CreateInstance(type) as AbsGameLogicBase;
            if (response != null)
            {
                mGameResponseLogicCache.Add(response);
                SeteHandlerAction(response, type.GetMethods());
                response.OnInit();
            }
        }

        public void OnGameEndCycle()
        {
            for (int i = 0; i < mGameResponseLogicCache.Count; i++)
            {
                mGameResponseLogicCache[i].OnReset();
            }
        }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
            //注册响应事件 
            CollectionResponseEvent();
            //开启响应事件携程
            ContinueTaskManager.NewTask().AppendFuncTask(() => ContinueS2CResponse()).Start();
        }

        /// <summary>
        /// 获取响应事件
        /// </summary>
        /// <param name="methods">一个对象中的public方法</param>
        private void SeteHandlerAction(AbsGameLogicBase response, MethodInfo[] methods)
        {
            MethodInfo method;
            object[] handlerAtts;
            S2CResponseHandlerAttribute handlerAtt = null;
            foreach (MethodInfo item in methods)
            {
                method = item;
                handlerAtts = method.GetCustomAttributes(typeof(S2CResponseHandlerAttribute), false);
                if (null == handlerAtts || handlerAtts.Length == 0)
                {
                    continue;
                }
                for (int i = 0; i < handlerAtts.Length; i++)
                {
                    handlerAtt = handlerAtts[i] as S2CResponseHandlerAttribute;
                    //先设置默认方法，如果有其他方法可用就表明是Gamekey所对于的方法
                    bool flag = handlerAtt.GameKey.Equals(MahjongUtility.GameKey);
                    if (handlerAtt.GameKey.Equals(MiscUtility.DefName) || flag)
                    {
                        if (mResponseHandlers.ContainsKey(handlerAtt.ProtocolKey))
                        {
                            if (flag)
                            {
                                Action<ISFSObject> handler = CreateHandler(response, method.Name);
                                if (null != handler)
                                {
                                    //替换handler
                                    mResponseHandlers[handlerAtt.ProtocolKey] = handler;
                                }
                            }
                        }
                        else
                        {
                            Action<ISFSObject> handler = CreateHandler(response, method.Name);
                            if (null != handler)
                            {
                                mResponseHandlers.Add(handlerAtt.ProtocolKey, handler);
                            }
                        }
                    }
                }
                //收集过滤事件
                handlerAtts = method.GetCustomAttributes(typeof(FilterOperateMenuAttribute), false);
                if (null != handlerAtts && handlerAtts.Length > 0)
                {
                    mSimpleFilter.Add(handlerAtt.ProtocolKey);
                }
            }
        }

        /// <summary>
        /// 创建监听Handler
        /// </summary>
        /// <param name="response">实例对象</param>
        /// <param name="methodName">方法名字</param>
        /// <returns></returns>
        private Action<ISFSObject> CreateHandler(AbsGameLogicBase response, string methodName)
        {
            Delegate action = Delegate.CreateDelegate(typeof(Action<ISFSObject>), response, methodName);
            if (null != action)
            {
                return action as Action<ISFSObject>;
            }
            return null;
        }

        /// <summary>
        /// 注册网络监听事件
        /// </summary>
        private void CollectionResponseEvent()
        {
            var game = GetComponent<MahGameManager>();
            var server = GetComponent<MahServerManager>();

            if (game == null || server == null) return;

            game.UserOutEvent += OnUserOut;
            game.UserReadyEvent += OnUserReady;
            game.GameStateEvent += OnGameState;
            game.OnGetGameInfoEvent += OnGetGameInfo;
            game.GameResponseEvent += OnGameResponseState;
            game.OnOtherPlayerJoinRoomEvent += OnOtherPlayerJoinRoom;
            server.OnHandsUpEvent += OnHandUp;
            server.OnRollDiceEvent += OnRollDice;
            server.OnGameOverEvent += OnGameOver;
            ChangeRoomEvent += server.OnChangeRoom;
            mRequestC2SEvent += server.OnRequestC2S;
            SendRequest += server.OnSendFrameRequest;
        }

        /// <summary>
        /// 重连请求
        /// </summary>
        public void SendReJoinGame()
        {
            GetComponent<MahServerManager>().SendReJoinGame();
        }

        //打开网络接口，开始接收数据
        public void StartNetworkListener()
        {
            var server = GetComponent<MahServerManager>();
            if (server == null) return;

            server.enabled = true;
        }

        /// <summary>
        /// 向服务器发出请求接口
        /// </summary>
        /// <param name="func">请求数据委托</param>
        /// <code language= "csharp"><![CDATA[  
        /// public void Test()
        /// {
        ///     int param1 = 1;
        ///     int param2 = 2;
        ///     GameCenter.Get<MahjongNetworkManager>().OnRequestC2S((sfs) =>
        ///     {
        ///         sfs.PutInt("1", param1); 
        ///         sfs.PutInt("2", param2);
        ///         return sfs;
        ///     });
        /// }
        /// ]]></code>
        public bool OnRequestC2S(Func<SFSObject, ISFSObject> func)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            ISFSObject data = func(sfsObject);
            if (null != data)
            {
                mRequestC2SEvent(data);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置延迟时间
        /// </summary>       
        public void SetDelayTime(float delayTime)
        {
            mResponseDelayTimer = delayTime;
        }

        /// <summary>
        /// 监听服务器事件
        /// </summary>
        /// <param name="protocolKey"></param>
        /// <param name="data"></param>
        public void OnGameResponseState(int protocolKey, ISFSObject data)
        {
            mS2CResponseQueue.Enqueue(new KeyValuePair<int, ISFSObject>(protocolKey, data));
        }

        /// <summary>
        /// 协程任务
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> ContinueS2CResponse()
        {
            while (true)
            {
                yield return mResponseDelayTimer;
                if (mS2CResponseQueue.Count > 0)
                {
                    mResponseDelayTimer = 0.02f;
                    KeyValuePair<int, ISFSObject> param = mS2CResponseQueue.Dequeue();
                    ExecuteS2CResponse(param.Key, param.Value);
                }
            }
        }

        /// <summary>
        /// 执行响应事件
        /// </summary>
        /// <param name="protocolKey"></param>
        /// <param name="data"></param>
        public void ExecuteS2CResponse(int protocolKey, ISFSObject data)
        {
            Action<ISFSObject> handler = null;
            if (protocolKey != NetworkProtocol.MJOpreateType && mResponseHandlers.TryGetValue(protocolKey, out handler))
            {
                handler(data);
            }
            if (!mSimpleFilter.Contains(protocolKey))
            {
                GameCenter.Hud.UIPanelController.OnOperateUpdatePanel();
                if (protocolKey == NetworkProtocol.MJOpreateType || data.ContainsKey(ProtocolKey.KeyOp))
                {
                    handler = mResponseHandlers[NetworkProtocol.MJOpreateType];
                    handler(data);
                }
            }
            //自定义逻辑
            if (mResponseHandlers.TryGetValue(CustomClientProtocol.CustomTypeCustomLogic, out handler))
            {
                handler(data);
            }
        }

        /// <summary>
        /// 准备
        /// </summary>     
        private void OnUserReady(int localseat, ISFSObject data)
        {
            var player = GameCenter.DataCenter.Players[localseat];
            if (player != null)
            {
                GameCenter.DataCenter.Players[localseat].IsReady = true;
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlayerReady, new PlayerInfoArgs() { Chair = localseat });
            }
        }

        /// <summary>
        /// 玩家退出
        /// </summary>     
        private void OnUserOut(int localseat, ISFSObject data)
        {
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlayerOut, new PlayerInfoArgs() { Chair = localseat });
        }

        /// <summary>
        /// 游戏状态
        /// </summary>     
        private void OnGameState(int state, ISFSObject info) { }

        /// <summary>
        /// 玩家进入
        /// </summary>     
        private void OnOtherPlayerJoinRoom(ISFSObject data)
        {
            var user = data.GetSFSObject(RequestKey.KeyUser);
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlayerJoin, new PlayerInfoArgs() { Chair = MahjongUtility.GetChair(user) });
        }

        private void OnGetGameInfo(ISFSObject gameInfo)
        {
            GameCenter.DataCenter.GetAllDatas(gameInfo);
            GameCenter.GameProcess.ChangeState<StateGameInfoInit>(new SfsFsmStateArgs() { SFSObject = gameInfo });
        }

        private void OnGameOver(ISFSObject data)
        {
            var db = GameCenter.DataCenter;
            db.IsGameOver = true;
            db.Game.SetTotalResult(data);
            if (GameCenter.DataCenter.DissolvedState)
            {
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowTotalResult);
                var fsm = GameCenter.GameProcess;
                if (!fsm.IsCurrState<StateGameEnd>())
                {
                    fsm.ChangeState<StateGameEnd>();
                }
                else if (GameCenter.DataCenter.ConfigData.ContinueNewGame)
                {
                    //切换继续开局状态
                    fsm.ChangeState<StateGameContinue>();
                }
            }
        }

        private void OnRollDice(ISFSObject data)
        {
            GameCenter.Hud.UIPanelController.PlayUIEffect(PoolObjectType.start);
            MahjongUtility.PlayEnvironmentSound("gamestart");
            var db = GameCenter.DataCenter;
            db.BankerSeat = data.GetInt(ProtocolKey.KeyStartP);
            db.SaiziPoint = data.GetIntArray(ProtocolKey.KeyDiceArray);
            db.CurrOpSeat = db.BankerSeat;
            //苏州麻将 滴零
            if (data.ContainsKey("diling"))
            {
                db.Game.Diling = data.TryGetInt("diling");
            }
            if (data.ContainsKey("baozi"))
            {
                db.Game.Baozi = data.TryGetInt("baozi");
            }
            if (MahjongUtility.GameKey == "wmbbmj")
            {
                GameCenter.DataCenter.Game.YuleSetLaozhuang();
            }
            GameCenter.GameProcess.ChangeState<StateGamePlaying>();
        }

        private void OnHandUp(ISFSObject data) { Handup(data); }

        /// <summary>
        /// 投票解散
        /// </summary>
        private void Handup(ISFSObject data)
        {
            if (data.ContainsKey("cmd") && data.GetUtfString("cmd") == "dismiss")
            {
                int type = data.GetInt("type");
                string username = data.GetUtfString("username");
                MahjongPlayersData playersData = GameCenter.DataCenter.Players;
                for (int i = 0; i < GameCenter.DataCenter.MaxPlayerCount; i++)
                {
                    if (playersData[i].NickM == username)
                    {
                        GameCenter.EventHandle.Dispatch((int)UIEventProtocol.OnEventHandUp, new HandupEventArgs()
                        {
                            UserName = username,
                            HandupType = (DismissFeedBack)type,
                            Chair = i
                        });
                        break;
                    }
                }
            }
        }
    }
}