using System;
using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.ddz2.DDzGameListener.TotalResultPanel;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using Assets.Scripts.Game.ddz2.DDz2Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.OneRoundResultPanel
{
    public class OneRoundResultListener : ServEvtListener
    {
        [SerializeField]
        protected GameObject OneRoundUiGob;

        /// <summary>
        /// 单局成绩的缓存
        /// </summary>
        private ISFSObject _oneRoundRecordData;

        //---ui信息项--start

        [SerializeField]
        protected UILabel BoomLabel;

        [SerializeField]
        protected UILabel SpringLabel;

        [SerializeField]
        protected UILabel QdzLabel;

        [SerializeField]
        protected GameObject OnerdRecordItemOrg;


        [SerializeField] 
        protected GameObject PlayerRecordGrid;

        /// <summary>
        /// 获得金币
        /// </summary>
        [SerializeField] 
        protected UILabel GetRewordLabel;

        /// <summary>
        /// 共**倍
        /// </summary>
        [SerializeField] 
        protected UILabel TotalBeiLabel;
        /// <summary>
        /// 返回到大厅的按钮，只有娱乐房中可以出现
        /// </summary>
        [SerializeField]
        protected GameObject BackToHallBtn;
        /// <summary>
        /// 返回大厅与继续游戏按钮控制
        /// </summary>
        [SerializeField]
        protected UIGrid BtnGrid;


        /// <summary>
        /// 标记地主座位
        /// </summary>
        private int _dizhuSeat;

        /// <summary>
        /// 规则说明
        /// </summary>
        [SerializeField] protected UILabel RuleInfoLabel;
        //----------------------------------------------------end

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnGameInfoEvt(OnGameInfo);
            Ddz2RemoteServer.AddOnGetRejoinDataEvt(OnRejoinGame);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeFirstOut, OnTypeFirstOut);
            Ddz2RemoteServer.AddOnUserReadyEvt(OnUserReady);
        }

        private void OnGameInfo(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if(data.ContainsKey(NewRequestKey.KeyRule))
                if (RuleInfoLabel != null) RuleInfoLabel.text = data.GetUtfString(NewRequestKey.KeyRule);
        }


        public override void RefreshUiInfo()
        {
            if(_oneRoundRecordData==null) return;

            OneRoundUiGob.SetActive(true);
            DDzUtil.ClearPlayerGrid(PlayerRecordGrid);


            var boom = _oneRoundRecordData.GetInt("boom");
            //var superBoom = _oneRoundRecordData.GetInt("sboom");
            var rocket = _oneRoundRecordData.GetInt("rocket");
            var spring = _oneRoundRecordData.GetInt(NewRequestKey.KeySpring);
            var modeSize = _oneRoundRecordData.GetInt(GlobalConstKey.C_Score);//得分
            //var nextLandSeat = _oneRoundRecordData.GetInt("bkp");//下一个地主座位
            var ante = _oneRoundRecordData.GetInt("ante");




            BoomLabel.text = (boom + rocket).ToString(CultureInfo.InvariantCulture);

            SpringLabel.text = spring.ToString(CultureInfo.InvariantCulture);

            QdzLabel.text = modeSize.ToString(CultureInfo.InvariantCulture);


            
            //赋值用户成绩列表
            var usersrecord = GetUsersRecord(_oneRoundRecordData);
            var usersLen = usersrecord.Length;
            for (var i = 0; i < usersLen; i++)
            {
                if(usersrecord[i]==null) continue;

                var data = usersrecord[i];
                if (data.Seat == App.GetGameData<GlobalData>().GetSelfSeat)
                {
                    GetRewordLabel.text = data.Gold.ToString(CultureInfo.InvariantCulture);
                    int rate = data.Seat == _dizhuSeat ? Math.Abs(data.Dz) / ante : Math.Abs(data.Nm) / ante;
                    TotalBeiLabel.text = string.Format("共{0}倍", rate);
                }

                AddUserItemInfo(data);

            }
            PlayerRecordGrid.GetComponent<UIGrid>().repositionNow = true;

        }

        //当收到自己已经成功准备了的时候，隐藏onRoundpanel
        protected void OnUserReady(object sender, DdzbaseEventArgs args)
        {
            if (args.IsfObjData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat) OneRoundUiGob.SetActive(false);
        }

        /// <summary>
        /// 重连游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey(NewRequestKey.KeyRule))
                if (RuleInfoLabel != null) RuleInfoLabel.text = data.GetUtfString(NewRequestKey.KeyRule);

            if (data.ContainsKey(NewRequestKey.KeyLandLord))
                _dizhuSeat = data.GetInt(NewRequestKey.KeyLandLord);
        }

        /// <summary>
        /// 当确定地主后,更新地主标记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeFirstOut(object sender, DdzbaseEventArgs args)
        {
           _dizhuSeat = args.IsfObjData.GetInt(RequestKey.KeySeat);
        }


        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            _oneRoundRecordData = args.IsfObjData;
            if (BackToHallBtn)
            {
                BackToHallBtn.SetActive(!App.GetGameData<GlobalData>().IsRoomGame);
                if (BtnGrid)
                {
                    BtnGrid.repositionNow = true;
                }
            }
            StartCoroutine(ShowUiInfoLater());
        }

        IEnumerator  ShowUiInfoLater()
        {
            yield return new WaitForSeconds(3f);
            RefreshUiInfo();
        }

        public void OnClickContinueBtn()
        {
            if (TotalResultListener.Instance.IsEndAllRound)
            {
                OneRoundUiGob.SetActive(false);
                //如果有总结算信息，显示总结算信息
                TotalResultListener.Instance.RefreshUiInfo();
                return;
            }

            GlobalData.ServInstance.SendPlayerReadyServCmd();

/*            //如果不是最后一局弹出结算，则发送准备消息给服务器
            if (App.GetGameData<GlobalData>().IsRoomGame)
            {
                GlobalData.ServInstance.SendPlayerReadyServCmd();
            }
            else
            {
                OnChangeRoom();
            }*/
            
        }

        /// <summary>
        /// 换房间
        /// </summary>
        public void OnChangeRoom()
        {
            GlobalData.ServInstance.ChangeRoom();
        }

        /// <summary>
        /// 返回大厅
        /// </summary>
        public void BackToHall()
        {
            App.QuitGame();
        }
        private UserRecord[] GetUsersRecord(ISFSObject sfsObject)
        {
            ISFSArray tempArr = sfsObject.GetSFSArray("users");
            var totalArr = new UserRecord[3];
            for (int i = 0; i < tempArr.Count; i++)
            {
                var userData = tempArr.GetSFSObject(i);
                totalArr[i] = new UserRecord
                {
                    SuperBoom = userData.GetInt("sboom"),
                    Gold = userData.GetInt("gold"),
                    ThrowOutNum = userData.GetInt("throwout"),
                    BoomNum = userData.GetInt("boom"),
                    TotalGold = userData.GetLong("ttscore"),
                    Score = userData.GetInt("score"),
                    RocketNum = userData.GetInt("rocket"),
                    Seat = userData.GetInt("seat"),
                    Cards = userData.GetIntArray("cards"),
                    Rate = userData.GetInt("rate"),
                    Nm = userData.GetInt("nm"),
                    Dz = userData.GetInt("dz"),
                };

                App.GetGameData<GlobalData>().OnUserSocreChanged(userData.GetInt("seat"), userData.GetInt("gold"));
            }
            return totalArr;
        }

        private void AddUserItemInfo(UserRecord data)
        {
            var gob = NGUITools.AddChild(PlayerRecordGrid, OnerdRecordItemOrg);
            gob.SetActive(true);

            var userRecordItem = gob.GetComponent<OneRoundRecordItem>();

            var userSeat = data.Seat;
            //设置如果是玩家自己则设置字体为黄色,并设置相应成绩
            if (userSeat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                userRecordItem.NiChengLabel.color = Color.yellow;
                userRecordItem.JiaBeiLabe.color = Color.yellow;
                userRecordItem.RecorLabel.color = Color.yellow;
            }

            //地主图标
            userRecordItem.Dizhusp.SetActive(userSeat == _dizhuSeat);

            //昵称
            var userInfo = App.GetGameData<GlobalData>().GetUserInfo(userSeat);
            if (userInfo != null) userRecordItem.NiChengLabel.text = userInfo.GetUtfString(RequestKey.KeyName);

            //加倍
            userRecordItem.JiaBeiLabe.text = "x" + data.Rate;

            //成绩
            userRecordItem.RecorLabel.text = YxUtiles.GetShowNumber(data.Gold).ToString(CultureInfo.InvariantCulture);

            //是否胜利
            userRecordItem.Winsp.SetActive(data.Gold>0);

        }


        /// <summary>
        /// 每盘结束时数据统计记录
        /// </summary>
        class UserRecord
        {
            /// <summary>
            /// 当前余额
            /// </summary>
            public long TotalGold;
            /// <summary>
            /// 本局打出的炸弹数
            /// </summary>
            public int BoomNum;
            /// <summary>
            /// 火箭
            /// </summary>
            public int RocketNum;
            /// <summary>
            /// 本局出牌的次数
            /// </summary>
            public int ThrowOutNum;
            /// <summary>
            /// 超级炸弹
            /// </summary>
            public int SuperBoom;
            public int Score;
            public int[] Cards;

            /// <summary>
            ///  输赢的分数
            /// </summary>
            public int Gold;
            /// <summary>
            /// 座位号
            /// </summary>
            public int Seat;

            /// <summary>
            /// 加倍信息
            /// </summary>
            public int Rate;
            /// <summary>
            /// 农民得分
            /// </summary>
            public int Nm;
            /// <summary>
            /// 地主得分
            /// </summary>
            public int Dz;
        }
    }
}
