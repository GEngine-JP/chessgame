using System.Collections;
using System.Globalization;
using System.Linq;
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
    /// 其他玩家的listener基类
    /// </summary>
    public abstract class PlayerOtherListener : PlayerInfoListener
    {

        protected override void OnAwake()
        {
            base.OnAwake();
            Ddz2RemoteServer.AddOnUserOutEvt(OnUserOut);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDouble, OnDouble);
            Ddz2RemoteServer.AddOnMsgChatEvt(OnUserTalk);
            Ddz2RemoteServer.AddOnUserIdleEvt(OnUserIdle);
        }

        /// <summary>
        /// 其他玩家离线的设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUserIdle(object sender, DdzbaseEventArgs args)
        {
            if(UserDataTemp==null || !UserDataTemp.ContainsKey(RequestKey.KeySeat))return;

            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeySeat) &&
                data.GetInt(RequestKey.KeySeat) == UserDataTemp.GetInt(RequestKey.KeySeat))
            {
                if (data.ContainsKey(NewRequestKey.KeyUserIdle) && data.GetBool(NewRequestKey.KeyUserIdle))
                {
                    DuanxianSp.SetActive(true);
                    HeadTexture.color = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    DuanxianSp.SetActive(false);
                    HeadTexture.color = new Color(1, 1, 1);
                }
            }

        }

        /// <summary>
        /// 手牌数
        /// </summary>
        [SerializeField]
        protected UILabel CdNumLabel;

        /// <summary>
        /// 正在选择加倍
        /// </summary>
        [SerializeField]
        protected UILabel SeltingDoublelabel;

        /// <summary>
        /// 正在选择叫分的提示 label
        /// </summary>
        [SerializeField]
        protected UILabel CallingScorelabel;

        /// <summary>
        /// 断线图标
        /// </summary>
        [SerializeField]
        protected GameObject DuanxianSp;

        //最后两张开始播放的警灯动画
        [SerializeField]
        protected GameObject JingDengAnim;


        protected override void OnTypeGrabSpeaker(object sender, DdzbaseEventArgs args)
        {
            base.OnTypeGrabSpeaker(sender, args);

            if (CallingScorelabel == null)return;

            CallingScorelabel.gameObject.SetActive(false);

            var data = args.IsfObjData;

            if (data.ContainsKey(RequestKey.KeySeat) && UserDataTemp!=null && UserDataTemp.ContainsKey(RequestKey.KeySeat))
            {
                if (data.GetInt(RequestKey.KeySeat) == UserDataTemp.GetInt(RequestKey.KeySeat))
                {
                    CallingScorelabel.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 当进入获得底牌阶段，清理CallingScorelabel状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void TypeFirstOut(object sender, DdzbaseEventArgs args)
        {
            base.TypeFirstOut(sender, args);
            CallingScorelabel.gameObject.SetActive(false);
        }
        

        /// <summary>
        /// 更新手牌数量
        /// </summary>
        /// <param name="leftCdNum"></param>
        protected void SetHdCdsNumLabel(int leftCdNum)
        {
            CdNumLabel.text = leftCdNum.ToString(CultureInfo.InvariantCulture);
            if (JingDengAnim != null) JingDengAnim.SetActive(leftCdNum>0 && leftCdNum < 3);
        }

        protected override void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {

            base.OnRejoinGame(sender, args);
            CallingScorelabel.gameObject.SetActive(false);
            var data = args.IsfObjData;
            if (data.ContainsKey(NewRequestKey.KeyGameStatus))
            {
                var status = data.GetInt(NewRequestKey.KeyGameStatus);
                if (UserDataTemp != null && (status == GlobalConstKey.StatusChoseBanker &&
                                             UserDataTemp.GetInt(RequestKey.KeySeat) == data.GetInt(NewRequestKey.KeyCurrp)))
                    CallingScorelabel.gameObject.SetActive(true);
            }

        }


        /// <summary>
        /// 从服务器获得底牌信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void OnGetDipai(object sender, DdzbaseEventArgs args)
        {
            base.OnGetDipai(sender,args);
            UpdateCdsNum();
        }

        /// <summary>
        /// 游戏结束后清空CdNumLabel,JingDengAnim隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            base.OnTypeGameOver(sender, args);
            if (CdNumLabel != null) CdNumLabel.text = "0";
            if (UserDataTemp != null && UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum)) 
                UserDataTemp.PutInt(NewRequestKey.KeyCardNum, 0);
            if (JingDengAnim != null) JingDengAnim.SetActive(false);
        }

        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected override void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            SeltingDoublelabel.gameObject.SetActive(false);
            base.OnAlloCateCds(sender,args);
            UpdateCdsNum();
        }

        protected virtual void OnUserOut(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data == null || !data.ContainsKey(RequestKey.KeySeat)) return;
            if (UserDataTemp != null && data.GetInt(RequestKey.KeySeat) == UserDataTemp.GetInt(RequestKey.KeySeat))
            {
/*                if (data.ContainsKey(NewRequestKey.KeyUserIdle) && data.GetBool(NewRequestKey.KeyUserIdle))
                {
                    if (data.ContainsKey(NewRequestKey.Keyfocus) && data.GetBool(NewRequestKey.Keyfocus))
                    {
                        DuanxianSp.SetActive(false);
                        HeadTexture.color = new Color(1, 1, 1);
                    }
                    else
                    {
                        DuanxianSp.SetActive(true);
                        HeadTexture.color = new Color(0.5f, 0.5f, 0.5f);
                    }
                    return;
                }*/


                //如果已经开始游戏了，则不做玩家离开设置
                if (App.GetGameData<GlobalData>().IsStartGame)
                {
                    DuanxianSp.SetActive(true);
                    HeadTexture.color = new Color(0.5f,0.5f,0.5f);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 根据缓存的信息刷新用户信息ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            base.RefreshUiInfo();

            //多一个显示牌数的组件
            if (UserDataTemp != null && CdNumLabel != null && UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum))
                SetHdCdsNumLabel(UserDataTemp.GetInt(NewRequestKey.KeyCardNum));

            //CdNumLabel.text = UserDataTemp.GetInt(NewRequestKey.KeyCardNum).ToString(CultureInfo.InvariantCulture);

            gameObject.SetActive(true);

            if (UserDataTemp != null && UserDataTemp.ContainsKey(NewRequestKey.KeyNetWork))
            {
                if (UserDataTemp.GetBool(NewRequestKey.KeyNetWork))
                {
                    DuanxianSp.SetActive(false);
                    HeadTexture.color = new Color(1f, 1f, 1f);
                }
                else
                {
                    DuanxianSp.SetActive(true);
                    HeadTexture.color = new Color(0.5f, 0.5f, 0.5f);
                }
            }

        }


        /// <summary>
        /// 根据服务器数据设置用户信息
        /// </summary>
        protected override void SetUserInfo(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            SetOtherPlayerData(data);

             //如果是选择加倍阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyGameStatus) &&
                data.GetInt(NewRequestKey.KeyGameStatus) == GlobalConstKey.StatusDouble)
            {
                var users = data.GetSFSArray(RequestKey.KeyUserList);
                if (UserDataTemp == null)
                {
                    ShowSpeakSp.gameObject.SetActive(false);
                    SeltingDoublelabel.gameObject.SetActive(false);
                    return;
                }

                var myseat = UserDataTemp.GetInt(RequestKey.KeySeat);

                for (int i = 0; i < users.Count; i++)
                {
                    ISFSObject user = users.GetSFSObject(i);
                    if (user.GetInt("seat") != myseat) continue;
                    var rate = user.GetInt("rate");
                    if (rate > 0)
                    {
                        SeltingDoublelabel.gameObject.SetActive(false);

                        if (myseat == LandSeat) ShowSpeakSp.gameObject.SetActive(false);
                        else
                        {
                            ShowSpeakSp.gameObject.SetActive(true);
                            ShowSpeakSp.spriteName = rate > 1 ? SpkJiabei : SpkBuJiabei;
                            ShowSpeakSp.MakePixelPerfect();
                        }
                    }
                    else
                    {
                        SeltingDoublelabel.gameObject.SetActive(true);
                        SeltingDoublelabel.text = "正在选择加倍";
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 根据玩家自己座位号存储其他玩家信息
        /// </summary>
        /// <param name="servData">服务器信息</param>
        protected abstract void SetOtherPlayerData(ISFSObject servData);

        //更新手牌数
        private void UpdateCdsNum()
        {
            if (UserDataTemp==null)return;
            if (!UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum)) return;
            var cdNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
            SetHdCdsNumLabel(cdNum);

            //CdNumLabel.text = cdNum.ToString(CultureInfo.InvariantCulture);

            if (!UserDataTemp.ContainsKey(RequestKey.KeySeat)) return;
            var globalUserInfo = App.GetGameData<GlobalData>().GetUserInfo(UserDataTemp.GetInt(RequestKey.KeySeat));
            if (globalUserInfo != null) globalUserInfo.PutInt(NewRequestKey.KeyCardNum, cdNum);
            
        }

        /// <summary>
        /// 检查加倍
        /// </summary>
        /// <param name="data"></param>
        public override void OnCheckSelectDouble(ISFSObject data)
        {
            //如果加倍
            if (!data.ContainsKey(NewRequestKey.KeyJiaBei) || !data.GetBool(NewRequestKey.KeyJiaBei))
            {
                return;
            }

/*            var globaldata = App.GetGameData<GlobalData>();
            if (globaldata.JiaBeiType != 2 || (globaldata.JiaBeiType == 2 && UserDataTemp.GetInt(RequestKey.KeySeat) != data.GetInt(RequestKey.KeySeat)))
            {
                ShowSpeakSp.gameObject.SetActive(false);
                SeltingDoublelabel.gameObject.SetActive(true);
                SeltingDoublelabel.text = "正在选择加倍";
            }*/
            if (data.ContainsKey(NewRequestKey.JiaBeiSeat))
            {
                var jiabeiSeats = data.GetIntArray(NewRequestKey.JiaBeiSeat);
                if (UserDataTemp != null)
                {
                    var thisPlayerSeat = UserDataTemp.GetInt(RequestKey.KeySeat);
                    if (jiabeiSeats.Any(jiabeiSeat => jiabeiSeat == thisPlayerSeat))
                    {
                        ShowSpeakSp.gameObject.SetActive(false);
                        SeltingDoublelabel.gameObject.SetActive(true);
                        SeltingDoublelabel.text = "正在选择加倍";
                    }
                }
            }
        }

        /// <summary>
        /// 当用户进入房间时，刷新用户数据
        /// </summary>
        /// <param name="userData"></param>
        protected void UpdateUserdata(ISFSObject userData)
        {
            //存储牌数
            var cdnumTemp = 0;
            if (UserDataTemp != null && UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum))
                cdnumTemp = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);

            UserDataTemp = userData;

            //如果是用户重练进来可能不含有NewRequestKey.KeyCardNum参数，这时候需要重置牌数
            if (!UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum))
                UserDataTemp.PutInt(NewRequestKey.KeyCardNum, cdnumTemp);
        }

        /// <summary>
        /// 当收到加倍信息
        /// </summary>
        private void OnDouble(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if (UserDataTemp != null && seat == UserDataTemp.GetInt(RequestKey.KeySeat))
            {
                SeltingDoublelabel.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected override void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            base.OnDoubleOver(sender,args);
            SeltingDoublelabel.gameObject.SetActive(false);
        }

        /// <summary>
        /// 接受某个玩家发送的可能是离线或者回来状态的消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUserTalk(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if(UserDataTemp != null && seat!=UserDataTemp.GetInt(RequestKey.KeySeat)) return;
            int exp = data.GetInt(RequestKey.KeyExp);
            switch (exp)
            {
                case GlobalConstKey.AppComBackExpId:
                    DuanxianSp.SetActive(false);
                    HeadTexture.color = new Color(1f, 1f, 1f);
                    break;
                case GlobalConstKey.AppPauseExpId:
                    DuanxianSp.SetActive(true);
                    HeadTexture.color = new Color(0.5f, 0.5f, 0.5f);
                    break;
            }
        }
    }
}
