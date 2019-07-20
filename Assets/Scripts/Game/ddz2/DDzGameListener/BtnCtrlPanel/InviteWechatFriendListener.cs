using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class InviteWechatFriendListener : ServEvtListener
    {
        [SerializeField] protected UIGrid BtnsGrid;

        [SerializeField]
        protected GameObject IvtWechatFriendBtn;

        [SerializeField]
        protected GameObject BackToHallBtn;

        /// <summary>
        /// 房间号
        /// </summary>
        private string _roomId="";
        /// <summary>
        /// 最大局数
        /// </summary>
        private string _maxRound = "";

        private int _jiabei = 0;

        /// <summary>
        /// 服务器发送过来的牌局信息
        /// </summary>
        private string _ruleInfo;

        private GlobalConstKey.GameType _curGameType = GlobalConstKey.GameType.CallScore;

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnGameInfoEvt(OnGetGameInfoData);
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnGetGameInfoData);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);
        }


        private void OnGetGameInfoData(object obj, DdzbaseEventArgs args)
        {
            SetAllBtnsActive(false);
            var data = args.IsfObjData;

            if(data.ContainsKey(NewRequestKey.KeyRoomId))_roomId = data.GetInt(NewRequestKey.KeyRoomId).ToString(CultureInfo.InvariantCulture);

            if (data.ContainsKey(NewRequestKey.KeyMaxRound)) _maxRound = data.GetInt(NewRequestKey.KeyMaxRound).ToString(CultureInfo.InvariantCulture);

            if (data.ContainsKey(NewRequestKey.KeyJiaBei)) _jiabei = data.GetInt(NewRequestKey.KeyJiaBei);

            if (data.ContainsKey(NewRequestKey.KeyQt)) _curGameType = (GlobalConstKey.GameType)data.GetInt(NewRequestKey.KeyQt);

            if (data.ContainsKey(NewRequestKey.KeyGameStatus)
                && data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusIdle&&App.GetGameData<GlobalData>().IsRoomGame)
            {
                if (data.ContainsKey(NewRequestKey.KeyCurRound) && data.GetInt(NewRequestKey.KeyCurRound) <= 1) SetAllBtnsActive(true);
            }

            if(data.ContainsKey("rule"))
            {
                _ruleInfo=data.GetUtfString("rule");
            }
        }

        /// <summary>
        /// 已经开始发牌了说明人齐了，开始游戏了，可以隐藏微信好友按钮了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            SetAllBtnsActive(false);
        }

        /// <summary>
        /// 当点击分享邀请好友
        /// </summary>
        public void OnClickInviteFriend()
        {
            Facade.Instance<WeChatApi>().InitWechat();
            var dic = new Dictionary<string, object>();
            dic.Add("type", 0);
            dic.Add("roomid", _roomId);
            dic.Add("event", "findroom");
            dic.Add("roomRule", _ruleInfo);
            dic.Add("sharePlat", 0);
            UserController.Instance.GetShareInfo(dic, (info) =>
            {
                Facade.Instance<WeChatApi>().ShareContent(info);
            }, ShareType.Website, SharePlat.WxSenceSession, null, App.GameKey);
        }


        /// <summary>
        /// 点击解散房间按钮
        /// </summary>
        public void OnClickDismisRoomBtn()
        {
            if (!App.GetGameData<GlobalData>().IsStartGame)
            {
                if (App.GetGameData<GlobalData>().IsFangZhu)
                    GlobalData.ServInstance.DismissRoom();
                else
                    GlobalData.ServInstance.LeaveRoom();
            }
        }


        private void SetAllBtnsActive(bool isActive)
        {
            IvtWechatFriendBtn.SetActive(isActive);
            BackToHallBtn.SetActive(isActive);
            if (isActive) BtnsGrid.repositionNow = true;
        }

        public override void RefreshUiInfo()
        {
            
        }
    }
}
