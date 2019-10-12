using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Interfaces;
using YxFramwork.Common.Model;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Common
{
    public class SysConfig : ISysCfg
    {
        public bool HasCache { get; private set; }

        /// <summary>
        /// 启动配置url
        /// </summary>
        public string StartCfgUrl
        {
            get
            {
                return Application.isEditor && string.IsNullOrEmpty(AppInfo.StartCfgUrl)
                           ? string.Format("file://{0}/../OtherResource/start.cfg", Application.dataPath)
                           : AppInfo.StartCfgUrl;
            }
        }

        public string PixelCfgUrl { get; private set; }

        private string _cacheVersionUrl;
        public string CacheVersionUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_cacheVersionUrl))
                {
                    var cacheVersion = AppInfo.CacheVersionUrl;
                    _cacheVersionUrl = string.IsNullOrEmpty(cacheVersion)? "index.php/Md/vs" : cacheVersion;
                }
                if (_cacheVersionUrl == "index.php/Md/vs")
                {
                    _cacheVersionUrl = AppInfo.ServerUrl.CombinePath(_cacheVersionUrl);
                }
                return _cacheVersionUrl;
            }
            set { _cacheVersionUrl = value; }
        }

        public string StartActionUrl {
            get { return AppInfo.StartActionUrl; }
        }

        public string ServerExtendId {
            get { return AppInfo.ServerExtendId; }
        }

        /// <summary>
        /// 服务器url
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        ///  Web服务器
        /// </summary>
        public string WebUrl { get; private set; }

        /// <summary>
        /// 是否开发模式
        /// </summary>
        public bool IsDeve { get; private set; }

        private string _hallResUrl;
        private string _gameResUrl;
        private string _shareResUrl;
        /// <summary>
        /// 资源服务器
        /// </summary>
        /// <param name="gameName">游戏key</param>
        /// <returns></returns>
        public string ResUrl(string gameName)
        {
            var skin = App.Skin;
            if (gameName == skin.Hall || gameName == skin.GameInfo)
            {
//                return string.Format("{0}{1}/", _hallResUrl, gameName);
                return _hallResUrl.CombinePath(gameName,"/");
            }
//            return string.Format("{0}{1}/", gameName == skin.Share ? _shareResUrl : _gameResUrl, gameName);
            return gameName == skin.Share || gameName == skin.Rule? _shareResUrl.CombinePath(gameName,"/") : _gameResUrl.CombinePath(gameName,"/");
        }

        public string ShareResUrl(string type)
        {
            return _shareResUrl.CombinePath(type);
        }

        private string _hallResCfgUrl;
        private string _hallCustomUrlPart;
        private string _gameResCfgUrl;
        private string _shareResCfgUrl;

        /// <summary>
        /// 资源配置服务器
        /// </summary>
        /// <param name="gameName"></param>
        /// <param name="fromHall"></param>
        /// <returns></returns>
        public string ResCfgUrl(string gameName, bool fromHall = true)
        {
            var skin = App.Skin;
            string urlRoot;
            if (gameName == skin.Hall || gameName == skin.GameInfo)
            {
                urlRoot = _hallResCfgUrl;
            }
            else if (gameName == skin.Share || gameName == skin.Rule )
            {
                urlRoot = _shareResCfgUrl;
            }
            else if (fromHall) //如果是大厅指定的资源
            {
                var lastIndex = _gameResCfgUrl.TrimEnd('/').LastIndexOf('/');
                urlRoot = _hallCustomUrlPart.CombinePath("games").CombinePath(_gameResCfgUrl.Substring(lastIndex));
            }
            else
            {
                urlRoot = _gameResCfgUrl;
            }
            return urlRoot.CombinePath(gameName,".cfg");
        }

        /// <summary>
        /// php通用接口
        /// </summary>
        public string GateWay
        {
            get { return ServerUrl.CombinePath("index.php/Client/Api/gateway"); }
        }

        public string WxAppId { get; set; }
        public string QqAppId { get; set; }


        /// <summary>
        /// 是否微信登录
        /// </summary>
        //public static bool HasWechatLogin = false;
        public bool HasWechatLogin { get; private set; }
        public bool HasQqLogin { get; private set; }

        /// <summary>
        /// 调试模式
        /// </summary>
        public int IsDebug { get; private set; }

        /// <summary>
        /// 调试玩家
        /// </summary>
        public string LogUserId { get; private set; }

        /// <summary>
        /// 调试服务器
        /// </summary>
        public string LogUrl { get; private set; }

        /// <summary>
        /// 是否加载本地资源
        /// </summary>
        public bool HasLoadLocalRes { get; private set; }

        /// <summary>
        /// 需要检测网络状态
        /// </summary>
        public bool NeedCheckNetType { get; private set; }

        /// <summary>
        /// 是否需要实时广播
        /// </summary>
        public bool NeedRollNotice { get; private set; }

        /// <summary>
        /// 帧率
        /// </summary>
        public int FrameRate { get; private set; }

        /// <summary>
        /// 退出大厅时是否需要返回登录界面
        /// </summary>
        public bool QuitToLogin { get; private set; }
        /// <summary>
        /// 全屏
        /// </summary>
        public bool IsFullScreen { get { return AppInfo.IsFullScreen; } }

        public Vector2 ScreenSize {
            get
            {
                var screenSize = AppInfo.ScreenSize;
                var size = Vector2.zero;
                if (string.IsNullOrEmpty(screenSize)) return size;
                var arr = screenSize.Split(',');
                if (arr.Length < 2) return size;
                float.TryParse(arr[0], out size.x);
                float.TryParse(arr[1], out size.y);
                return size;
            }
        }

        public ScreenManager.ScreenRotateState ScreenRotate {
            get
            {
                return (ScreenManager.ScreenRotateState)AppInfo.ScreenRotate;
            }
        }

        /// <summary>
        /// 多服务器
        /// </summary>
        public string NetSelectCfg { get { return AppInfo.NetSelectCfg; } }

        public TwManager.TwMessageStyle TwMsgStyle {
            get
            {
                const string style = AppInfo.TwMsgStyle;
                if (string.IsNullOrEmpty(style)) return TwManager.TwMessageStyle.MessageBox;
                return (TwManager.TwMessageStyle)Enum.Parse(typeof(TwManager.TwMessageStyle), style);
            }
        }

        public bool NeedCurtainAfterLogin { get; private set; }
        public int DownLoadWaitTime { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subfield"></param>
        /// <param name="getParameter"></param>
        /// <returns></returns>
        public string GetUrlWithServer(string subfield, string getParameter = "")
        { 
            return ServerUrl.CombinePath(string.Format("{0}?mt={1}&ver={2}&token={3}&userid={4}{5}",
                                 subfield,//
                                 App.YxPlatForm,//平台
                                 Application.version,//版本
                                 LoginInfo.Instance.ctoken,
                                 LoginInfo.Instance.user_id,
                                 getParameter));//其他
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetRecharge(string userid, string token)
        {
            return WebUrl.CombinePath(string.Format("index.php/home/Payment/index?uid={0}&token={1}",userid, token));
//            return WebUrl + "/index.php/home/Payment/index?uid=" + userid + "&token=" + tokend;
        }
          
        private string _loginParm = "login_";

        public string GetFullLoginUrl()
        {
            return ServerUrl.CombinePath("index.php/Client/User").CombinePath(_loginParm);
//            return string.Format("{0}/index.php/Client/User/{1}?", ServerUrl, _loginParm);
        }

        public SysConfig()
        {
            ServerUrl = AppInfo.ServerUrl;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="dict"></param>
        public void LoadConfig(Dictionary<string, string> dict)
        {
            //服务器url
            if (string.IsNullOrEmpty(NetSelectCfg))
            {
                ServerUrl = AppInfo.ServerUrl;
            }
            //web主url 
            WebUrl = ServerUrl;
            //登录参数
            _loginParm = AppInfo.LoginUrl;
            //大厅资源url
            _hallResUrl =  AppInfo.HallResUrl;
            //游戏资源参数
            _gameResUrl = AppInfo.GameResUrl;
            //公共资源url
            _shareResUrl =  AppInfo.ShareResUrl;
            //大厅资源配置url
            _hallResCfgUrl =  AppInfo.HallCfgUrl;
            //游戏资源配置url
            _gameResCfgUrl = AppInfo.GameResCfgUrl;
            //公共资源配置url
            _shareResCfgUrl = AppInfo.ShareCfgUrl;
            _hallCustomUrlPart = GetHallCustomUrlPart(_hallResCfgUrl);
            //微信appid
            WxAppId =  AppInfo.WxAppId;
            QqAppId =  AppInfo.QqAppId;

            DownLoadWaitTime = AppInfo.DownLoadWaitTime;
            PixelCfgUrl = AppInfo.PixelCfgUrl;

            NeedCurtainAfterLogin = AppInfo.NeedCurtainAfterLogin;

            HasCache = AppInfo.HasCache;
            HasWechatLogin = AppInfo.HasWechatLogin;
            HasQqLogin = AppInfo.HasQqLogin;
            ServerPort = 9933;
            HasLoadLocalRes = AppInfo.HasLoadLocalRes;
            NeedCheckNetType = AppInfo.NeedCheckNetType;
            NeedRollNotice = AppInfo.NeedRollNotice;
            FrameRate = AppInfo.FrameRate;
            QuitToLogin = AppInfo.QuitToLogin;
        }


        private static string GetHallCustomUrlPart(string hallCfgUrl)
        {
            const string httpStr = "http://";
            var temp = hallCfgUrl.Replace(httpStr, "").Trim('/');
            var arr = temp.Split('/');
            var arrLen = arr.Length - 3;
            var hallResCfgUrlPart = httpStr;
            for (var i = 0; i < arrLen; i++)
            {
                hallResCfgUrlPart = hallResCfgUrlPart.CombinePath(arr[i]);
            }
            return hallResCfgUrlPart;
        }
    }
}
