using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.View;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class ReadyBtnListener : ServEvtListener
    {
        /// <summary>
        /// 显示showreadybtn按钮
        /// </summary>
        private bool _isShowReadyBtn;

        /// <summary>
        /// 准备按钮
        /// </summary>
        [SerializeField]
        protected GameObject ReadyBtnSprite;
        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnGameInfoEvt(OnGameInfo);  
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoin);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
        }

        private void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat) &&
                data.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                IsReadyBtnActive(false);
            }
        }

        private void OnRejoin(object sender, DdzbaseEventArgs args)
        {
            CheckReadyBtnShow(args.IsfObjData);
        }


        public override void RefreshUiInfo()
        {
            // throw new System.NotImplementedException();
        }

        /// <summary>
        /// 发送准备游戏信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            CheckReadyBtnShow(args.IsfObjData);
        }



        private void CheckReadyBtnShow(ISFSObject data)
        {
            _isShowReadyBtn = false;
            if (data.ContainsKey(NewRequestKey.KeyCargs2))
            {
                var cargsInfo = data.GetSFSObject(NewRequestKey.KeyCargs2);
                if (cargsInfo.ContainsKey(NewRequestKey.KeyReadyBtn))
                    _isShowReadyBtn = cargsInfo.GetUtfString(NewRequestKey.KeyReadyBtn) == "1";

            }
            //如果不显示准备按钮，则直接自动准备
            if (!_isShowReadyBtn)
            {
                GlobalData.ServInstance.SendPlayerReadyServCmd();
                return;
            }

            if (data.ContainsKey(RequestKey.KeyUser))
            {
                var userData = data.GetSFSObject(RequestKey.KeyUser);
                if (!App.GetGameData<GlobalData>().IsStartGame
                    && userData.ContainsKey(RequestKey.KeyState) && userData.GetBool(RequestKey.KeyState) == false)
                {
                    IsReadyBtnActive(true);
                }

                else
                {
/*                    if (!App.GetGameData<GlobalData>().IsStartGame)
                        GlobalData.ServInstance.SendPlayerReadyServCmd();*/
                    IsReadyBtnActive(!userData.GetBool(RequestKey.KeyState));
                }
            }
        }


        private void IsReadyBtnActive(bool value)
        {
            ReadyBtnSprite.SetActive(value);
        }

        public void OnReadyBtnClick()
        {
            GlobalData.ServInstance.SendPlayerReadyServCmd();
        }

    }
}
