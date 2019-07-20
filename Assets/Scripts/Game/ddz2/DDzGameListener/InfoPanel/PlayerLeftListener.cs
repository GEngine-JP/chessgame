using System;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 玩家自己左手方player
    /// </summary>
    public class PlayerLeftListener : PlayerOtherListener
    {

        protected override void OnAwake()
        {
            base.OnAwake();

            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 当是上家出牌时，之前说的话要消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var curSeat = data.GetInt(RequestKey.KeySeat);

            if (curSeat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
                ShowSpeakSp.gameObject.SetActive(false);

            else if (curSeat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ShowSpeakSp.gameObject.SetActive(false);
                var cdsLen = data.GetIntArray(RequestKey.KeyCards).Length;
                if (UserDataTemp != null)
                {
                    var curselfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum) - cdsLen;
                    UserDataTemp.PutInt(NewRequestKey.KeyCardNum, curselfCdsNum);
                
                    var globalUserInfoLeft = App.GetGameData<GlobalData>().GetUserInfo(curSeat);
                    if (globalUserInfoLeft != null) globalUserInfoLeft.PutInt(NewRequestKey.KeyCardNum, curselfCdsNum);

                    App.GetGameData<GlobalData>().OnHdcdsChange(curSeat, curselfCdsNum);

                    //CdNumLabel.text = curselfCdsNum.ToString(CultureInfo.InvariantCulture);

                    SetHdCdsNumLabel(curselfCdsNum);
                }
            }
        }


        /// <summary>
        /// 如果是自己叫pass则显示“不要”，如果是商家出牌则消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            if (args.IsfObjData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetRightPlayerSeat)
                ShowSpeakSp.gameObject.SetActive(false);
            else if (args.IsfObjData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ShowSpeakSp.gameObject.SetActive(true);
                ShowSpeakSp.spriteName = SpkBuChu;
            }

        }



        /// <summary>
        /// 根据座位号存储其他玩家信息
        /// </summary>
        /// <param name="servData">服务器信息</param>
        protected override void SetOtherPlayerData(ISFSObject servData)
        {
            var dataDic = GetOtherUsesDic(servData);
            var seat = App.GetGameData<GlobalData>().GetLeftPlayerSeat;
            if (dataDic.ContainsKey(seat))
            {
                UserDataTemp = dataDic[seat];
                RefreshUiInfo();
            }
        }

        /// <summary>
        /// 当有玩家加入游戏时
        /// </summary>
        protected override void OnUserJoinRoom(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var user = data.GetSFSObject(RequestKey.KeyUser);
            if (!user.ContainsKey(RequestKey.KeySeat) || user.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetLeftPlayerSeat)
                return;

            UpdateUserdata(user);

            RefreshUiInfo();

            DuanxianSp.SetActive(false);
            HeadTexture.color = new Color(1f, 1f, 1f);
        }
    }
}
