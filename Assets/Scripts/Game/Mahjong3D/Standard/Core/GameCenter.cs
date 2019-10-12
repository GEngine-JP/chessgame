using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum GameType
    {
        /// <summary>
        /// 游戏
        /// </summary>
        Normal,
        /// <summary>
        /// 回放
        /// </summary>
        Playback,
    }

    public class GameCenter : MonoBehaviour
    {
        public static GameCenter Instance;

        public GameType GameType = GameType.Normal;

        private void Awake() { Instance = this; }

        private void Start() { StartCoroutine(MahjongGameStart()); }

        private IEnumerator MahjongGameStart()
        {
            yield return new WaitForSeconds(0.1f);
            // 初始化任务管理器
            ContinueTaskManager.OnInit();
            // 初始化系统组件
            InitBuiltinComponents();
            // 游戏开始 
            GameProcess.StartupProcess();
        }

        private void OnDestroy()
        {
            mComponents.Clear();
            Instance = null;
        }

        private readonly static List<BaseComponent> mComponents = new List<BaseComponent>();

        private EventHandlerComponent mEventHandler;
        public static EventHandlerComponent EventHandle
        {
            get { return Instance.mEventHandler; }
        }

        private GameProcessComponent mGameProcess;
        public static GameProcessComponent GameProcess
        {
            get { return Instance.mGameProcess; }
        }

        private PoolsComponent mPools;
        public static PoolsComponent Pools
        {
            get { return Instance.mPools; }
        }

        private NetworkComponent mNetwork;
        public static NetworkComponent Network
        {
            get { return Instance.mNetwork; }
        }

        private MahjongSceneComponent mMahjongScene;
        public static MahjongSceneComponent Scene
        {
            get { return Instance.mMahjongScene; }
        }

        private HudComponent mHud;
        public static HudComponent Hud
        {
            get { return Instance.mHud; }
        }

        private DataCenterComponent mDataCenter;
        public static DataCenterComponent DataCenter
        {
            get { return Instance.mDataCenter; }
        }

        private ShortcutsComponent mShortcuts;

        public static ShortcutsComponent Shortcuts
        {
            get { return Instance.mShortcuts; }
        }

        private GameLifecycleComponent mLifecycle;
        public static GameLifecycleComponent Lifecycle
        {
            get { return Instance.mLifecycle; }
        }

        private AssetsComponent mAssets;
        public static AssetsComponent Assets
        {
            get { return Instance.mAssets; }
        }

        private PlaybackComponent mPlayback;

        public static PlaybackComponent Playback
        {
            get { return Instance.mPlayback; }
        }

        private ControllerComponent mController;
        public static ControllerComponent Controller
        {
            get { return Instance.mController; }
        }

        private void InitBuiltinComponents()
        {
            mEventHandler = GetSystemComponent<EventHandlerComponent>();
            mLifecycle = GetSystemComponent<GameLifecycleComponent>();
            mGameProcess = GetSystemComponent<GameProcessComponent>();
            mDataCenter = GetSystemComponent<DataCenterComponent>();
            mShortcuts = GetSystemComponent<ShortcutsComponent>();
            mAssets = GetSystemComponent<AssetsComponent>();
            mPools = GetSystemComponent<PoolsComponent>();
            mPlayback = GetSystemComponent<PlaybackComponent>();
            mMahjongScene = GetSystemComponent<MahjongSceneComponent>();
            mNetwork = GetSystemComponent<NetworkComponent>();
            mHud = GetSystemComponent<HudComponent>();
            mController = GetSystemComponent<ControllerComponent>();
            OnInitalization();
        }

        private void OnInitalization()
        {
            for (int i = 0; i < mComponents.Count; i++)
            {
                mComponents[i].OnLoad();
            }
            for (int i = 0; i < mComponents.Count; i++)
            {
                mComponents[i].OnInitalization();
            }

            Type type = null;
            Type[] types = typeof(GameCenter).Assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                type = types[i];
                mDataCenter.OnLoadAssembly(type);
                mNetwork.OnLoadAssembly(type);
            }
        }

        public T GetSystemComponent<T>() where T : BaseComponent
        {
            return (T)GetSystemComponent(typeof(T));
        }

        public BaseComponent GetSystemComponent(Type type)
        {
            for (int i = 0; i < mComponents.Count; i++)
            {
                if (mComponents[i].GetType() == type)
                {
                    return mComponents[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 注册周期函数
        /// </summary>
        /// <param name="obj"> 注册对象 </param>
        public static void RegisterCycle(IGameLifecycle obj)
        {
            Instance.mLifecycle.Register(obj);
        }

        public static void RemoveRegisterCycle(IGameLifecycle obj)
        {
            if (Instance != null) Instance.mLifecycle.RemoveRegister(obj);
        }

        public static void RegisterComponent(BaseComponent component)
        {
            if (component != null) mComponents.Add(component);
        }
    }
}