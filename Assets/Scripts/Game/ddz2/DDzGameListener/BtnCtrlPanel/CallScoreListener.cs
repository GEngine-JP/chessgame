using System;
using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class CallScoreListener : ServEvtListener
    {
        [SerializeField]
        protected GameObject NoCallBtn;
        [SerializeField]
        protected GameObject Call1Btn;
        [SerializeField]
        protected GameObject Call2Btn;
        [SerializeField]
        protected GameObject Call3Btn;

        //变成灰色的按钮
        [SerializeField]
        protected GameObject DisCall1Btn;
        [SerializeField]
        protected GameObject DisCall2Btn;
        [SerializeField]
        protected GameObject DisCall3Btn;

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnGetRejoionData);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);

            UIEventListener.Get(NoCallBtn).onClick = OnCallScoreClick;
            UIEventListener.Get(Call1Btn).onClick = OnCallScoreClick;
            UIEventListener.Get(Call2Btn).onClick = OnCallScoreClick;
            UIEventListener.Get(Call3Btn).onClick = OnCallScoreClick;
        }

        /// <summary>
        /// 叫分按钮点击
        /// </summary>
        /// <param name="gob"></param>
        private void OnCallScoreClick(GameObject gob)
        {
            int calScore = 0;
            if (ReferenceEquals(gob, NoCallBtn)) calScore = 0;
            else if (ReferenceEquals(gob, Call1Btn)) calScore = 1;
            else if (ReferenceEquals(gob, Call2Btn)) calScore = 2;
            else if (ReferenceEquals(gob, Call3Btn)) calScore = 3;

            //向服务器发送叫分信息
            GlobalData.ServInstance.CallGameScore(App.GetGameData<GlobalData>().GetSelfSeat, calScore);
        }

        /// <summary>
        /// 存储与界面显示相关的服务器消息缓存
        /// </summary>
        protected ISFSObject ServDataTemp;
        

        private void OnGetRejoionData(object obj,DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey(NewRequestKey.KeyGameStatus)
                && data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusChoseBanker)
            {
                if ((data.ContainsKey(NewRequestKey.KeyMinScore) || data.ContainsKey(NewRequestKey.KeyScore))
                    && data.ContainsKey(NewRequestKey.KeyCurrp)
                    && data.ContainsKey(NewRequestKey.KeyQt))
                {
                    //当前谁发言
                    int curCallSeat = data.GetInt(NewRequestKey.KeyCurrp);
                    if (App.GetGameData<GlobalData>().GetSelfSeat == curCallSeat)
                    {
                        ServDataTemp = data;
                        RefreshUiInfo();
                    }
                    else
                    {
                        SetAllBtnsActive(false);
                    }
                }
                else
                {
                    SetAllBtnsActive(false);
                }
            }
            else
            {
                SetAllBtnsActive(false);
            }

        }

        /// <summary>
        /// 当轮到某人要准备要叫分时
        /// </summary>
        protected void OnTypeGrabSpeaker(object obj, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (!DDzUtil.IsServDataContainAllKey(
                new[]
                    {
                        RequestKey.KeySeat, NewRequestKey.KeyTttype
                    }, data))
            {
                return;
            }

            if (App.GetGameData<GlobalData>().GetSelfSeat != data.GetInt(RequestKey.KeySeat))
            {
                SetAllBtnsActive(false);
                return;
            }

            if (ServDataTemp == null) ServDataTemp = new SFSObject();

            ServDataTemp = data;
            //把与"qt"相同引用值的 "ttype" 的值赋值过来
            ServDataTemp.PutInt(NewRequestKey.KeyQt, data.GetInt(NewRequestKey.KeyTttype));
            RefreshUiInfo();
        }


        /// <summary>
        /// 当有人叫了分了
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        protected void OnTypeGrab(object obj, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果是玩家自己叫了分了则隐藏叫分面板
            if (data.ContainsKey(RequestKey.KeySeat) && 
                data.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                SetAllBtnsActive(false);
            }
        }





        public override void RefreshUiInfo()
        {
            if (ServDataTemp == null) return;
            var curPokerGameType = ServDataTemp.GetInt(NewRequestKey.KeyQt);

/*            switch (curPokerGameType)
            {
                case (int)GlobalConstKey.GameType.CallScoreWithFlow:
                    {
                        SetCallScoreWithFlowUi();
                        break;
                    }
            }*/

            SetAllBtnsActive(true);
            SetCallScoreWithFlowUi();
        }

        /// <summary>
        /// 叫分带分流时的ui设置
        /// </summary>
        private void SetCallScoreWithFlowUi()
        {
            if (!ServDataTemp.ContainsKey(NewRequestKey.KeyMinScore) &&
                !ServDataTemp.ContainsKey(NewRequestKey.KeyScore))
            {
                DisableBtns(new[] { Call1Btn, Call2Btn, Call3Btn }, new[] { DisCall1Btn });
                Debug.LogError("轮到自己叫分时服务器没有发minscore或score的类型");
                return;
            }


            var score = 0;
            if (ServDataTemp.ContainsKey(NewRequestKey.KeyScore))
            {
                score = ServDataTemp.GetInt(NewRequestKey.KeyScore);
            }

            if (ServDataTemp.ContainsKey(NewRequestKey.KeyMinScore))
            {
                var minscore = ServDataTemp.GetInt(NewRequestKey.KeyMinScore);

                if (minscore >= score) score = minscore;
            }

            switch (score)
            {
                case 1:
                    {
                        DisableBtns(new[] { Call1Btn }, new[] { DisCall1Btn });
                        break;
                    }
                case 2:
                    {
                        DisableBtns(new[] { Call1Btn, Call2Btn }, new[] { DisCall1Btn, DisCall2Btn });
                        break;
                    }
            }

        }
       
        
/*        /// <summary>
        /// 重置按钮状态
        /// </summary>
        void ResetBtnState()
        {
            var btngobs = new[] {NoCallBtn, Call1Btn, Call2Btn, Call3Btn};
            foreach (var btngob in btngobs)
            {
                btngob.SetActive(true);
                btngob.GetComponent<UISprite>().color = new Color(1, 1, 1);
                btngob.GetComponent<BoxCollider>().enabled = true;
            }
        }*/

        /// <summary>
        /// 让某些按钮失效
        /// </summary>
        /// <param name="btnGobs"></param>
        void DisableBtns(IEnumerable<GameObject> btnGobs,IEnumerable<GameObject> disbtnGobs  )
        {
            foreach (var btngob in btnGobs)
            {
                btngob.SetActive(false);
            }

            foreach (var disbtnGob in disbtnGobs)
            {
                disbtnGob.SetActive(true);
            }
        }
        
        /// <summary>
        /// 设置所有叫分按钮的显示状态
        /// </summary>
        /// <param name="isActive"></param>
        void SetAllBtnsActive(bool isActive)
        {
            NoCallBtn.SetActive(isActive);
            Call1Btn.SetActive(isActive);
            Call2Btn.SetActive(isActive);
            Call3Btn.SetActive(isActive);

            //如果对所有按钮进行操作， 则disBtn都是隐藏
            HideAllDisBtns();
        }

        /// <summary>
        /// 隐藏所有disBtns
        /// </summary>
        private void HideAllDisBtns()
        {
            DisCall1Btn.SetActive(false);
            DisCall2Btn.SetActive(false);
            DisCall3Btn.SetActive(false);
        }
    }
}
