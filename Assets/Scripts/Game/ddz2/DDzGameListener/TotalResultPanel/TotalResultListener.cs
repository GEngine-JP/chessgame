using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DDz2Common.ImgPress;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using com.yxixia.utile.YxDebug;
using Assets.Scripts.Game.ddz2.InheritCommon;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.TotalResultPanel
{
    public class TotalResultListener : ServEvtListener
    {

        /// <summary>
        /// 总成绩ui父节点
        /// </summary>
        [SerializeField]
        protected GameObject UiGameObject;

        /// <summary>
        /// 玩家结算的数据结构gameobject
        /// </summary>
        [SerializeField]
        protected GameObject ItemsOrgGob;

        /// <summary>
        /// 截图对象
        /// </summary>
        [SerializeField]
        protected CompressImg Img;

        /// <summary>
        /// 每个玩家的信息item的grid
        /// </summary>
        [SerializeField]
        protected GameObject GridGob;

        /// <summary>
        /// 总结算发过来的信息缓存
        /// </summary>
        private ISFSObject _ttGameResultData;
        /// <summary>
        /// 游戏信息
        /// </summary>
        private ISFSObject _gameInfo;

        /// <summary>
        /// 规则说明
        /// </summary>
        [SerializeField]
        private UILabel _ruleInfo;

        /// <summary>
        /// 局数说明
        /// </summary>
        [SerializeField]
        private UILabel _roundInfo;

        /// <summary>
        /// 房间ID
        /// </summary>
        [SerializeField]
        private UILabel _roomId;

        /// <summary>
        /// 当前系统时间
        /// </summary>
        [SerializeField]
        private UILabel _nowTime;

        /// <summary>
        /// 房前房主创建的房间对应名称
        /// </summary>
        [SerializeField]
        private UILabel _roomerInfo;

        public static TotalResultListener Instance { private set; get; }

        protected override void OnAwake()
        {
            Instance = this;
            Ddz2RemoteServer.AddOnGameOverEvt(OnGameOverEvt);
            Ddz2RemoteServer.AddOnGameInfoEvt(OnGameInfo);
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnGameInfo);
        }

        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        private void OnDestroy()
        {
            Instance = null;
        }

        private void OnGameOverEvt(object sender, DdzbaseEventArgs args)
        {
           _ttGameResultData = args.IsfObjData;
        }
        /// <summary>
        /// 游戏初始化部分信息需要缓存，用于结算显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            _gameInfo = args.IsfObjData;
        }

        public override void RefreshUiInfo()
        {
            if (_ttGameResultData == null) return;

            UiGameObject.SetActive(true);
            SetData(_ttGameResultData);

            _ttGameResultData = null;
        }

        /// <summary>
        /// 标记是否已经结束所有牌局
        /// </summary>
        public bool IsEndAllRound
        {
            get { return _ttGameResultData != null; }
        }


        /// <summary>
        /// 设置总结算数据
        /// </summary>
        public void SetData(ISFSObject data)
        {
            ISFSArray userArray = data.GetSFSArray("users");

            DDzUtil.ClearPlayerGrid(GridGob);

            for (int i = 0; i < userArray.Count; i++)
            {
               var gob =  NGUITools.AddChild(GridGob, ItemsOrgGob);
               gob.SetActive(true);
               var resultItem = gob.GetComponent<ResultItem>();
               resultItem.SetUserInfo(userArray.GetSFSObject(i));
            }

            GridGob.GetComponent<UIGrid>().repositionNow = true;
            InfoAbout(data);
        }
        /// <summary>
        /// 大结算周边信息处理
        /// </summary>
        /// <param name="data"></param>
        private void InfoAbout(ISFSObject data)
        {
            if (_roundInfo)
            {
                var now = data.GetInt("round");
                var total = data.GetInt("maxRound");
                _roundInfo.text = string.Format("{0}/{1}", now, total);
            }
            if (_nowTime)
            {
                var time = data.GetLong("svt");
                DateTime nowTime = GetSvtTime(time);
                _nowTime.text = nowTime.ToString("yyyy-MM-dd hh:mm:ss");
            }
            if (_ruleInfo)
            {
                _ruleInfo.text = _gameInfo.GetUtfString("rule");
            }

            if (_roomerInfo)
            {
                _roomerInfo.text = _gameInfo.GetUtfString("roomName");
            }

            if (_roomId)
            {
                _roomId.text = _gameInfo.GetInt("rid").ToString();
            }
        }
        /// <summary>
        /// 时间转化
        /// </summary>
        /// <param name="svt"></param>
        /// <returns></returns>
        public  DateTime GetSvtTime(long svt)
        {
            DateTime s = new DateTime(1970, 1, 1, 8, 0, 0);
            s = s.AddSeconds(svt);
            return s;
        }

        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void OnClickBackHall()
        {
            //App.GetGameData<GlobalData>().ClearParticalGob();
            UiGameObject.SetActive(false);
            App.OnQuitGame();
        }

        /// <summary>
        /// 点击分享战绩按钮
        /// </summary>
        public void OnCLickShare()
        {
            YxWindowManager.ShowWaitFor();

            Facade.Instance<WeChatApi>().InitWechat();

            UserController.Instance.GetShareInfo(delegate(ShareInfo info)
                {
                    YxWindowManager.HideWaitFor();
                    Img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                        {
                            YxDebug.Log("Url == " + imageUrl);
                            if (Application.platform == RuntimePlatform.Android)
                            {
                                imageUrl = "file://" + imageUrl;
                            }
                            info.ImageUrl = imageUrl;
                            info.ShareType = ShareType.Image;
                            Facade.Instance<WeChatApi>().ShareContent(info, str =>
                                {
                                    //成功后给奇哥发消息
                                    var parm = new Dictionary<string, object>
                                        {
                                            {"option", 2},
                                            {"bundle_id", Application.bundleIdentifier},
                                            {"share_plat", SharePlat.WxSenceTimeLine.ToString()},
                                        };
                                    Facade.Instance<TwManger>().SendAction("shareAwards", parm, null);
                                });
                        });
                });

            //Facade.Instance<TwManger>().SendAction("shareAwards",new Dictionary<string,object>() { {"bundle_id",Application.bundleIdentifier}, {"share_plat",} }, );
        }

    }
}
