/** 
 *文件名称:     TopMenu.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-12-01 
 *描述:         界面中货币的状态信息(临时处理，后期使用大厅提供工具)
 *历史记录: 
*/

using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.ConstDefine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View
{
    public class TopMenu : YxWindow
    {
        [Tooltip("金币")]
        public UILabel UserGold;
        [Tooltip("元宝")]
        public UILabel UserCash;
        [Tooltip("房卡")]
        public UILabel RoomCard;
        [Tooltip("昵称")]
        public UILabel UserName;
        [Tooltip("ID")]
        public UILabel UserId;
        [Tooltip("头像")]
        public UITexture UserHead;

        protected override void OnAwake()
        {
            YxMsgCenterHandler.GetIntance().AddListener(RequestCmd.Sync, OnUserDataChange);
        }

        protected override void OnEnableEx()
        {
            RefreshInfo();
        }

        public void OnUserDataChange(object data=null)
        {
            if (gameObject.activeInHierarchy)
            {
                RefreshInfo();
            }
        }

        private void RefreshInfo()
        {
            var userInfo = UserInfoModel.Instance.UserInfo;
            YxTools.TrySetComponentValue(UserGold, userInfo.CoinA.ToString());
            YxTools.TrySetComponentValue(UserCash, userInfo.CashA.ToString());
            YxTools.TrySetComponentValue(RoomCard, UserInfoModel.Instance.BackPack.GetItem("item2_q").ToString());
            YxTools.TrySetComponentValue(UserId, App.UserId);
            YxTools.TrySetComponentValue(UserName, userInfo.NickM);
            if (UserHead)
            {
                PortraitRes.SetPortrait(userInfo.AvatarX, UserHead, userInfo.SexI);
            }
        }
    }
}
