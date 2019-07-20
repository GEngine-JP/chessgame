namespace Assets.Scripts.Common.Utils
{
    /**    
     *QQ:           765858558
     *Unity版本：   5.4.0f3 
     *创建时间:     2018-02-05  
     *历史记录: 
    */
    public class AppInfo
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public const string ServerUrl = "http://121.43.186.14/";
        /// <summary>
        /// 是否有微信登陆
        /// </summary>
        public const bool HasWechatLogin = true;
        /// <summary>
        /// 分享bundleId
        /// </summary>
        public const string ShareBundleId = "";
        /// <summary>
        /// 推广码，暂时没啥用
        /// </summary>
        public const string PromoteCode = "";
        /// <summary>
        /// 大厅配置文件地址
        /// </summary>
        public const string HallCfgUrl = "http://cdn.18yb.com/opensource/cfg/hall1/201803101900/windows/";
        /// <summary>
        /// 游戏配置文件地址
        /// </summary>
        public const string GameResCfgUrl = "http://cdn.18yb.com/opensource/cfg/hall1/201803101900/windows/";
        /// <summary>
        /// 大厅资源文件地址
        /// </summary>
        public const string HallResUrl = "http://cdn.18yb.com/opensource/res/hall1/windows/";
        /// <summary>
        /// 游戏资源文件地址
        /// </summary>
        public const string GameResUrl = "http://cdn.18yb.com/opensource/res/hall1/windows/";
        /// <summary>
        /// 使用的皮肤名称
        /// </summary>
        public const string SkinNames = "hall|gamelist||rulelist";
        /// <summary>
        /// 是否显示超时tip
        /// </summary>
        public const bool NeedTimeOutTip = false;
        /// <summary>
        /// 是否有缓存
        /// </summary>
        public const bool HasCache = true;
        /// <summary>
        /// 是否位本地资源
        /// </summary>
        public const bool IsLoadLocalRes = false;
        /// <summary>
        /// 是否检查网络
        /// </summary>
        public const bool NeedCheckNetType = true;
        /// <summary>
        /// 是否轮询
        /// </summary>
        public const bool NeedRollNotice = true;
        /// <summary>
        /// 大厅退出时是否返回登陆
        /// </summary>
        public const bool QuitToLogin = true;
        /// <summary>
        /// 帧率
        /// </summary>
        public const int FrameRate = 30;
        /// <summary>
        /// 微信appid
        /// </summary>
        public const string WxAppId = "";
        /// <summary>
        /// qq的appId
        /// </summary>
        public const string QqAppId = "";
        /// <summary>
        /// 是否有qq登陆
        /// </summary>
        public const bool HasQqLogin = false;
    }
}
