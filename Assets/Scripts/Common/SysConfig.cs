﻿using System.Collections.Generic; 
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Interface; 

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
                return  AppInfo.StartCfgUrl;
            }
        }

        public string VersionUrl
        {
            get { return null; }
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

        /// <summary>
        /// 资源服务器
        /// </summary>
        /// <param name="gameName">游戏key</param>
        /// <returns></returns>
        public string ResUrl(string gameName)
        {
            var skin = gameName == App.HallName || gameName == App.GameListPath || gameName == App.RoomListPath ||
                       gameName == App.RuleListPath
                           ? _hallResUrl
                           : _gameResUrl;
            return string.Format("{0}{1}/", skin, gameName);
        }

        private string _hallResCfgUrl;
        private string _gameResCfgUrl;

        /// <summary>
        /// 资源配置服务器
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public string ResCfgUrl(string gameName)
        {
            var skin = gameName == App.HallName || gameName == App.GameListPath || gameName == App.RoomListPath ||
                       gameName == App.RuleListPath
                           ? _hallResCfgUrl
                           : _gameResCfgUrl;
            return string.Format("{0}{1}.{2}", skin, gameName, "cfg");
        }

        /// <summary>
        /// 
        /// </summary>
        public string GateWay
        {
            get { return GetUrlWithServer("index.php/Client/Api/gateway"); }
        }

        /// <summary>
        /// 应用分享id
        /// </summary>
        public string WxAppId { get; private set; }

        public string QqAppId { get; private set; }

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
        public bool IsLoadLocalRes { get; private set; }

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
        /// 
        /// </summary>
        /// <param name="subfield"></param>
        /// <param name="getParameter"></param>
        /// <returns></returns>
        public string GetUrlWithServer(string subfield, string getParameter = "")
        {
            return string.Format("{0}/{1}?mt={2}&ver={3}{4}", ServerUrl, subfield, App.YxPlatForm, Application.version,
                                 getParameter);
        }

        public void SkinNames()
        {
            var skinNames = AppInfo.SkinNames.Split('|');
            if (skinNames.Length < 4)
            {
                Debug.LogError(string.Format("skinNames内容少于4个！！！【{0}】", AppInfo.SkinNames));
                return;
            }
            App.HallName = skinNames[0];
            App.GameListPath = skinNames[1];
            App.RoomListPath = skinNames[2];
            App.RuleListPath = skinNames[3];
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="tokend"></param>
        /// <returns></returns>
        public string GetRecharge(string userid, string tokend)
        {
            return WebUrl + "/index.php/home/Payment/index?uid=" + userid + "&token=" + tokend;
        }
          
        private string _loginParm = "_login";

        public string GetFullLoginUrl()
        {
            return string.Format("{0}/index.php/Client/User/{1}?", ServerUrl, _loginParm);
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="dict"></param>
        public void LoadConfig(Dictionary<string, string> dict)
        {
            //服务器url
            ServerUrl = AppInfo.ServerUrl;
            //web主url 
            WebUrl = ServerUrl;
            //登录参数
            _loginParm = AppInfo.LoginUrl;
            //大厅资源url
            _hallResUrl = AppInfo.HallResUrl;
            //游戏资源参数
            _gameResUrl = AppInfo.GameResUrl;
            //大厅资源配置url
            _hallResCfgUrl =  AppInfo.HallCfgUrl;
            //游戏资源配置url
            _gameResCfgUrl = AppInfo.GameResCfgUrl;
            //微信appid
            WxAppId = AppInfo.WxAppId;
            QqAppId = AppInfo.QqAppId;
            HasCache = true;
            ServerPort = 9933;
            IsLoadLocalRes = AppInfo.IsLoadLocalRes;
            NeedCheckNetType = AppInfo.NeedCheckNetType;
            NeedRollNotice = AppInfo.NeedRollNotice;
            FrameRate = AppInfo.FrameRate;
            QuitToLogin = AppInfo.QuitToLogin;
        }
    }
}
