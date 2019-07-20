using System.Linq;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class JiabeiBtnListener : ServEvtListener
    {
        /// <summary>
        /// 加倍按钮
        /// </summary>
        [SerializeField]
        protected GameObject JiabeiCtrlBtns;

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, TypeFirstOut);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDouble, OnDouble);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
           // Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);
        }
        

        public override void RefreshUiInfo()
        {
          
        }

        protected  void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果是选择加倍阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyGameStatus) &&
                data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusDouble)
            {
                var selfRate = data.GetSFSObject(RequestKey.KeyUser).GetInt(NewRequestKey.KeyRate);
                //if(selfRate==0 && App.GetGameData<GlobalData>().GetSelfSeat!=data.GetInt(NewRequestKey.KeyLandLord)) JiabeiCtrlBtns.SetActive(true);
                if (selfRate == 0) JiabeiCtrlBtns.SetActive(true);
            }
            else
            {
                JiabeiCtrlBtns.SetActive(false);
            }

        }


        /// <summary>
        /// 当收到服务TypeFirstOut器相应
        /// </summary>
        private void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //如果加倍
            if (data.ContainsKey(NewRequestKey.KeyJiaBei) && data.ContainsKey(NewRequestKey.JiaBeiSeat))
            {
                var jiabeiSeats = data.GetIntArray(NewRequestKey.JiaBeiSeat);
                //判断是否是自己要显示加倍
                var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
                bool isSelfSeatJiabei = jiabeiSeats.Any(jiabeiSeat => jiabeiSeat == selfSeat);
                if (isSelfSeatJiabei && data.GetBool(NewRequestKey.KeyJiaBei) && JiabeiCtrlBtns != null)
                    JiabeiCtrlBtns.SetActive(true);


                /*
                                var landSeat = data.GetInt(RequestKey.KeySeat);
                                var selfSeat = App.GetGameData<GlobalData>().GetSelfSeat;
                                if (!(App.GetGameData<GlobalData>().JiaBeiType == 2 && selfSeat == landSeat))
                                {
                                    if (JiabeiCtrlBtns != null) JiabeiCtrlBtns.SetActive(true);
                                }*/
            }
        }


        /// <summary>
        /// 当收到加倍信息
        /// </summary>
        private void OnDouble(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                JiabeiCtrlBtns.SetActive(false);
            }
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected  void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            JiabeiCtrlBtns.SetActive(false);
        }


/*        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            JiabeiCtrlBtns.SetActive(false);
        }*/

        /// <summary>
        /// 点击加倍按钮
        /// </summary>
        public void OnClickDoubtn()
        {
            GlobalData.ServInstance.SendDouble(2);
        }

        /// <summary>
        /// 点击不加倍按钮
        /// </summary>
        public void OnClickNoDoubtn()
        {
            GlobalData.ServInstance.SendDouble(1);
        }

    }
}
