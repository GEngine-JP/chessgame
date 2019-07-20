using System.Globalization;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 玩家自己的信息ui处理
    /// </summary>
    public class PlayerSelfListener : PlayerInfoListener
    {

        protected override void OnAwake()
        {
            gameObject.SetActive(false);
            base.OnAwake();
            //Ddz2RemoteServer.AddOnGameInfoEvt(SendReadyCmd);


            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
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

            if(curSeat == App.GetGameData<GlobalData>().GetLeftPlayerSeat) 
                ShowSpeakSp.gameObject.SetActive(false);
            else if (curSeat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                ShowSpeakSp.gameObject.SetActive(false);
                var cdsLen = data.GetIntArray(RequestKey.KeyCards).Length;
                if (UserDataTemp != null)
                {
                    var curselfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum) - cdsLen;
                    UserDataTemp.PutInt(NewRequestKey.KeyCardNum,curselfCdsNum);
                    App.GetGameData<GlobalData>().UserSelfData.PutInt(NewRequestKey.KeyCardNum, curselfCdsNum);

                    App.GetGameData<GlobalData>().OnHdcdsChange(curSeat, curselfCdsNum);
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
            if (args.IsfObjData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
                ShowSpeakSp.gameObject.SetActive(false);
            else if (args.IsfObjData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                ShowSpeakSp.gameObject.SetActive(true);
                ShowSpeakSp.spriteName = SpkBuChu;
            }

        }


/*        /// <summary>
        /// 发送准备游戏信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SendReadyCmd(object sender, DdzbaseEventArgs args)
        {
           /*if(App.GetGameData<GlobalData>().IsRoomGame)#1# GlobalData.ServInstance.SendPlayerReadyServCmd();
        }*/

        protected override void SetUserInfo(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            UserDataTemp = GetHostUserData(data);

            RefreshUiInfo();
        }


        /// <summary>
        /// 从服务器获得底牌信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void OnGetDipai(object sender, DdzbaseEventArgs args)
        {
            base.OnGetDipai(sender, args);
            if (UserDataTemp != null)
            {
                var selfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
                App.GetGameData<GlobalData>().UserSelfData.PutInt(NewRequestKey.KeyCardNum, selfCdsNum);
            }
        }

        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected override void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            base.OnAlloCateCds(sender, args);
            if (UserDataTemp != null)
            {
                var selfCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
                App.GetGameData<GlobalData>().UserSelfData.PutInt(NewRequestKey.KeyCardNum, selfCdsNum);
            }
        }


        /// <summary>
        /// 根据缓存的信息刷新用户信息ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            base.RefreshUiInfo();

            gameObject.SetActive(true);
        }
    }
}
