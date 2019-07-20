using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerCdCtrl;
using Assets.Scripts.Game.ddz2.PokerRule;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.BtnCtrlPanel
{
    public class CmanagerListener : SelfHdCdsListener
    {

        [SerializeField]
        protected GameObject BuYaoBtn;
        [SerializeField]
        protected GameObject TiShiBtn;
        [SerializeField]
        protected GameObject ChuPaiBtn;

        [SerializeField]
        protected GameObject DisBuYaoBtn;
        [SerializeField]
        protected GameObject DisTiShiBtn;
        [SerializeField]
        protected GameObject DisChuPaiBtn;

        [SerializeField]
        protected GameObject OnlyBuYaoBtn;
        /// <summary>
        /// 处理牌判断，出牌，提示等各种处理的工具类
        /// </summary>
        private CardManager _cardManager;

        /// <summary>
        /// 用于只能选牌的脚本
        /// </summary>
        private readonly CheckCardTool _checkCardTool = new CheckCardTool(PokerRuleUtil.GetCdsType);

        /// <summary>
        /// 最后一次出牌的各种信息
        /// </summary>
        private ISFSObject _lastOutData = new SFSObject();

/*        /// <summary>
        /// 智能选牌信息缓存，要及时清理，用于比较上次智能选牌的结果是不是和下次一样
        /// </summary>
        private int[] _mayoutCdsTemp;*/


        /// <summary>
        /// 标记地主座位
        /// </summary>
        private int _landSeat;

        /// <summary>
        /// 手牌控制脚本
        /// </summary>
        [SerializeField]
        protected HdCdsCtrl HdCdctrlInstance;

        protected override void OnAwake()
        {   //监听手牌操作事件
            HdCdsCtrl.AddHdSelCdsEvt(OnHdCdsCtrlEvent);
            //获得cardmanager
            _cardManager = new CardManager();
            base.OnAwake();
            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);

            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);

            Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
        }

        /// <summary>
        ///  根据手牌数据缓存刷新相关ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            if (HdCdctrlInstance != null) HdCdctrlInstance.ReSetHandCds(HdCdsListTemp.ToArray());
        }

        /// <summary>
        /// 只能选牌开关
        /// </summary>
        private bool _onoffIchosecCds = false;

        /// <summary>
        /// 当玩家有对手牌进行点击操作时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnHdCdsCtrlEvent(object sender, HdCdCtrlEvtArgs args)
        {
            //如果不该自己行动
            if (!ChuPaiBtn.activeSelf && !DisChuPaiBtn.activeSelf ) return;

            //如果没有选牌
            if (args.SelectedCds.Length == 0)
            {
                DDzUtil.DisableBtn(ChuPaiBtn,DisChuPaiBtn);
                _onoffIchosecCds = true;
                return;
            }

            var selectedCds = args.SelectedCds;
            var lastOutCds = _lastOutData.GetIntArray(RequestKey.KeyCards);


            //-----------------------start 智能选牌过程-------有赖子，或者开关关闭则不用智能选牌----------------------------
            bool isgetcdsWithoutCompare = false;//标记是不是在自己出动出牌时做出的智能选牌
            int[] mayOutCds=null;
            bool selCdshasLaizi = PokerRuleUtil.CheckHaslz(selectedCds);
            if (!selCdshasLaizi && _onoffIchosecCds)
            {
                if (_lastOutData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetSelfSeat)
                {
                    mayOutCds = _checkCardTool.GetcdsWithOutCompare(selectedCds, HdCdsListTemp.ToArray());
                    isgetcdsWithoutCompare = true;
                }
                else
                {
                    DDzUtil.ActiveBtn(BuYaoBtn, DisBuYaoBtn);
                    mayOutCds = _checkCardTool.ChkCanoutCdListWithLastCdList(selectedCds,
                                                                             _cardManager.GetTishiGroupDic, lastOutCds);
                }
            }
            //---------------------------------------------------------------------------------------end


/*            //---start---------------使智能选牌出了相同的牌型，不重复执行-----------------------
            var haschosemayOutCds = DDzUtil.IsTwoArrayEqual(HdCdsCtrl.SortCds(_mayoutCdsTemp), HdCdsCtrl.SortCds(mayOutCds));
            _mayoutCdsTemp = mayOutCds;
            //如果上次智能选牌和本次相同，则直接取消一次智能选牌
            if (haschosemayOutCds)
            {
                mayOutCds = null;
            }
            //----------------------------------------------------------------------------------end*/



            if (mayOutCds == null || mayOutCds.Length == 0)
            {
                bool isMatch;

                //如果_lastOutData不是自己出牌
                if (_lastOutData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetSelfSeat)
                {
                    var lastoutcds = new List<int>();
                    lastoutcds.AddRange(lastOutCds);
                    var cardTypeDic = _cardManager.CheckCanGuanCds(selectedCds, selCdshasLaizi, lastoutcds.ToArray());
                    isMatch = cardTypeDic != null && cardTypeDic.Count>0;
                }
                else
                {
                    var cardTypeDic = _cardManager.CheckCanGuanCds(selectedCds, selCdshasLaizi, null);
                    isMatch = cardTypeDic != null && cardTypeDic.Count>0;
                }

                YxDebug.LogError("isMatch: " + isMatch);
                if (isMatch)
                {
                    HdCdctrlInstance.UpCdList(selectedCds);
                }
                else
                {
                    DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
                    return;
                }
            }
            else
            {
                if (!ChooseMayOutCards(mayOutCds, selectedCds))
                {
                    DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
                    return;
                }
            }


            //经过智能检索后最后检查一遍抬出的牌是否合法----start---

            var finalType =  PokerRuleUtil.GetCdsType(HdCdctrlInstance.GetUpCdList().ToArray());
            if (finalType != CardType.None && finalType != CardType.Exception)
            {
                //如果选出的牌型不是那种单牌，或者对子的小牌型，就先关闭智能选牌
                if (isgetcdsWithoutCompare && finalType != CardType.C1 && finalType != CardType.C2)_onoffIchosecCds = false;
                else if (!isgetcdsWithoutCompare) _onoffIchosecCds = false;
               
                DDzUtil.ActiveBtn(ChuPaiBtn, DisChuPaiBtn);
            }
            else
            {
                DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
            }

            //------------end
        }



/*
        /// <summary>
        /// 重置手牌
        /// </summary>
        /// <param name="cards"></param>
        protected override void ResetHdCds(int[] cards)
        {
            base.ResetHdCds(cards);

            //_cardManager.UpdateCardList(HdCdsListTemp.ToArray());
        }

        /// <summary>
        /// 添加手牌
        /// </summary>
        /// <param name="extrcds"></param>
        protected override void AddHdCds(int[] extrcds)
        {
            base.AddHdCds(extrcds);
            //_cardManager.UpdateCardList(HdCdsListTemp.ToArray());
        }

        /// <summary>
        /// 移除手牌
        /// </summary>
        /// <param name="rmoveCds">要移除的手牌</param>
        protected override void RemoveHdCds(int[] rmoveCds)
        {
            base.RemoveHdCds(rmoveCds);
            //_cardManager.UpdateCardList(HdCdsListTemp.ToArray());
        }*/

        protected override void OnRejoinGame(object sender, DdzbaseEventArgs args)
        {
            StopAllCoroutines();
            //与ResetHdCds，AddHdCds，RemoveHdCds相关
            base.OnRejoinGame(sender, args);

            var data = args.IsfObjData;

            //标记地主座位
            if (data.ContainsKey(NewRequestKey.KeyLandLord)) _landSeat = data.GetInt(NewRequestKey.KeyLandLord);

            //如果是选庄阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyGameStatus))
            {
                switch (data.GetInt(NewRequestKey.KeyGameStatus))
                {
                    case  GlobalConstKey.StatusChoseBanker:
                        {
                            HideAllBtns();
                            return;
                        }
                    case GlobalConstKey.StatusDouble:
                        {
                            HideAllBtns();
                            return;
                        }
                }
            }


            //如果存在最后一次出牌的信息
            if (data.ContainsKey(NewRequestKey.KeyLastOut))
            {
                _lastOutData = data.GetSFSObject(NewRequestKey.KeyLastOut);

            }
            else
            {
                //是自己第一手出牌，你是地主
                _lastOutData.PutInt(RequestKey.KeySeat, App.GetGameData<GlobalData>().GetSelfSeat);
            }


            //没人行动，或者，不是自己行动
            if (!data.ContainsKey(NewRequestKey.KeyCurrp) ||
                data.GetInt(NewRequestKey.KeyCurrp) != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                HideAllBtns();
                return;
            }

            if (_lastOutData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                OnNotSelfOutCds(_lastOutData);
            }
            else
            {
                //如果自己准备出手牌
                DisableAllBtns();
            }

        }

        /// <summary>
        /// 当确定地主后，看自己是不是地主，来判断是否显示按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnFirstOut(object sender, DdzbaseEventArgs args)
        {
            base.OnFirstOut(sender,args);

            var data = args.IsfObjData;

            _landSeat = data.GetInt(RequestKey.KeySeat);
            if (_landSeat != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                HideAllBtns();
                return;
            }

            _lastOutData.PutInt(RequestKey.KeySeat, _landSeat);

            //如果没有加倍设置
            if (!data.GetBool(NewRequestKey.KeyJiaBei))
            {
                DisableAllBtns();
            }
            else
            {
                HideAllBtns();
            }


        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        private void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            //判断是不是该自己操作出牌了
            if (_landSeat == App.GetGameData<GlobalData>().GetSelfSeat) StartCoroutine(ShowCtrlBtnLater(4f));
        }

        private IEnumerator ShowCtrlBtnLater(float time)
        {
            yield return new WaitForSeconds(time);

            DisableAllBtns();
        }

        /// <summary>
        /// 如果有人出牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            //_mayoutCdsTemp = null;
            //与ResetHdCds，AddHdCds，RemoveHdCds相关
            base.OnTypeOutCard(sender, args);

            _lastOutData = args.IsfObjData;

            if (_lastOutData.GetInt(RequestKey.KeySeat) == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                OnNotSelfOutCds(_lastOutData);
                return;
            }

            HideAllBtns();
        }


        /// <summary>
        /// 有人pass的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
       void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            //_mayoutCdsTemp = null;

            //如果发送pass的玩家不是上家，则隐藏所有按钮
            if (args.IsfObjData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                HideAllBtns();
                return;
            }

            //如果最后一次出牌的玩家不是自己，则检测能不能管的上，来决定按钮状态
            if (_lastOutData.GetInt(RequestKey.KeySeat) != App.GetGameData<GlobalData>().GetSelfSeat)
            {
                OnNotSelfOutCds(_lastOutData);
            }
            //如果其他人都不出了（上家不出，说明是其他人都不出了），且最后一次出牌是自己
            else
            {
               DisableAllBtns();
            }
        }


       /// <summary>
       /// 当游戏结算时
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="args"></param>
       void OnTypeGameOver(object sender, DdzbaseEventArgs args)
       {
           //隐藏所有出牌操作按钮
           HideAllBtns();
           //清空手牌
           ResetHdCds(new int[]{});
           RefreshUiInfo();   
       }
       
        /// <summary>
        /// 根据智能选牌结果，抬起手牌与之对应的牌，并过滤掉不应该抬起来的牌
        /// </summary>
        /// <param name="mayOutCds">智能选牌结果</param>
        /// <param name="selectedCds">智能检测前选择抬起的手牌</param>
        /// <returns></returns>
        private bool ChooseMayOutCards(int[] mayOutCds, int[] selectedCds)
        {
            //从选出的牌中拿出智能选牌要的牌
            var mayOutcdsListTemp = new List<int>();
            var len = mayOutCds.Length;
            for (int i = 0; i < len; i++)
            {
                mayOutcdsListTemp.Add(HdCdsCtrl.GetValue(mayOutCds[i]));
            }


            var upCdsList = new List<int>();
            len = selectedCds.Length;
            for (int i = 0; i < len; i++)
            {
                var pureValue = HdCdsCtrl.GetValue(selectedCds[i]);
                if (mayOutcdsListTemp.Contains(pureValue))
                {
                    upCdsList.Add(selectedCds[i]);
                    mayOutcdsListTemp.Remove(pureValue);
                }
            }

            //如果还有没选出的牌，则从手牌中继续选
            if (mayOutcdsListTemp.Count > 0)
            {
                len = HdCdsListTemp.Count;
                for (int i = 0; i < len; i++)
                {
                    var pureValue = HdCdsCtrl.GetValue(HdCdsListTemp[i]);
                    //如果是还没有选出的，在只能选牌牌组中的牌则添加
                    if (!upCdsList.Contains(HdCdsListTemp[i]) &&
                        mayOutcdsListTemp.Contains(pureValue))
                    {
                        upCdsList.Add(HdCdsListTemp[i]);
                        mayOutcdsListTemp.Remove(pureValue);
                    }
                }
            }

            if (upCdsList.Count != mayOutCds.Length)
            {
                return false;
            }

            HdCdctrlInstance.UpCdList(upCdsList.ToArray());

            return true;
        }




        //------------------------------------------------------------------------------------------------
        /// <summary>
        /// 当上一次玩家出牌不是自己时
        /// </summary>
        /// <param name="lastOutData">最后一次出牌的信息</param>
        private void OnNotSelfOutCds(ISFSObject lastOutData)
        {
            _cardManager.FindCds(HdCdsListTemp.ToArray(), lastOutData);
            if (_cardManager.GetTishiGroupList.Count > 0)
            {
                DDzUtil.ActiveBtn(BuYaoBtn, DisBuYaoBtn);
                DDzUtil.ActiveBtn(TiShiBtn, DisTiShiBtn);
                DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);
            }
            else
            {
                HideAllBtns();
                OnlyBuYaoBtn.SetActive(true);
            }
            var args = new HdCdCtrlEvtArgs(HdCdctrlInstance.GetUpCdList().ToArray());
         
            OnHdCdsCtrlEvent(null,args);
           _onoffIchosecCds = true;
        }

        /// <summary>
        /// 显示按钮，并disable所有按钮
        /// </summary>
        private void DisableAllBtns()
        {
            DDzUtil.DisableBtn(BuYaoBtn, DisBuYaoBtn);
            DDzUtil.DisableBtn(TiShiBtn, DisTiShiBtn);
            DDzUtil.DisableBtn(ChuPaiBtn, DisChuPaiBtn);

            var args = new HdCdCtrlEvtArgs(HdCdctrlInstance.GetUpCdList().ToArray());

            OnHdCdsCtrlEvent(null, args);
            _onoffIchosecCds = true;

            CheckCanOutOneTime();
        }

        /// <summary>
        // 隐藏所有按钮
        /// </summary>
        private void HideAllBtns()
        {
            //终止ShowCtrlBtnLater方法，防止暂停回来异常显示出来
            StopAllCoroutines();

            BuYaoBtn.SetActive(false);
            TiShiBtn.SetActive(false);
            ChuPaiBtn.SetActive(false);

            DisBuYaoBtn.SetActive(false);
            DisTiShiBtn.SetActive(false);
            DisChuPaiBtn.SetActive(false);

            OnlyBuYaoBtn.SetActive(false);
        }


        //---------以下按钮相关方法----start---------

        /// <summary>
        /// 点击不出按钮
        /// </summary>
        public void OnBuChuClick()
        {
            GlobalData.ServInstance.TurnPass();
            HdCdctrlInstance.RepositionAllHdCds();
        }

        /// <summary>
        /// 点击提示按钮
        /// </summary>
        public void OnTishiClick()
        {
            HdCdctrlInstance.RepositionAllHdCds();
            var oneTishiGroup = _cardManager.GetOneTishiGroup();
            if (oneTishiGroup == null || oneTishiGroup.Length < 1) return;
            foreach (var purecdvalue in oneTishiGroup)
            {
                HdCdctrlInstance.JustUpCd(purecdvalue);
            }

            //如果提示成功，把智能选牌关闭
            _onoffIchosecCds = false;

            //_mayoutCdsTemp = (int[]) oneTishiGroup.Clone();
            //有提示牌组，点亮出牌按钮
            DDzUtil.ActiveBtn(ChuPaiBtn, DisChuPaiBtn);
        }

        /// <summary>
        /// 点击出牌按钮
        /// </summary>
        public void OnChuPaiClick()
        {
            int[] cardArr = HdCdctrlInstance.GetUpCdList().ToArray();

            //赖子代表的牌
            var laiziRepCds = new int[] {-1};

            //类型
            int curRule = -1;
            var type = PokerRuleUtil.GetCdsType(cardArr);
            if (type != CardType.None && type != CardType.Exception) curRule = (int) type;
            GlobalData.ServInstance.ThrowOutCard(cardArr, laiziRepCds, curRule);
        }

        //---------------end--

        /// <summary>
        /// 检测是否可以一手全出所有手牌，如果可以自动全出。
        /// </summary>
        private void CheckCanOutOneTime()
        {
            var hdCds = HdCdsListTemp.ToArray();
            var cdsType = PokerRuleUtil.GetCdsType(hdCds);
            if (cdsType==CardType.None|| cdsType == CardType.Exception|| cdsType == CardType.C411) return;
            //如果是飞机带单牌，查找是否含有炸弹，有则不自动出了   
            if (cdsType == CardType.C11122234)
            {
                var sotedCds = PokerRuleUtil.GetSortedValues(hdCds);
                var cdNum = 0;
                var curCd = -1;
                if (ExistC42(sotedCds))
                {
                    return;
                }
                foreach (var cd in sotedCds)
                {
                    if (curCd != cd)
                    {
                        curCd = cd;
                        if (cdNum >= 4) return;
                        cdNum = 1;
                        continue;
                    }
                    cdNum++;
                }
                if (cdNum >= 4) return;
            }

            //赖子代表的牌
            var laiziRepCds = new int[] { -1 };
            GlobalData.ServInstance.ThrowOutCard(hdCds, laiziRepCds, (int)cdsType);
        }
        /// <summary>
        /// 小王
        /// </summary>
        private readonly int _bigJoker = 0x51;
        /// <summary>
        /// 大王
        /// </summary>
        private readonly int _smallJoker = 0x61;
        /// <summary>
        /// 判断手牌里是否有王炸
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        private bool ExistC42(int[] arr)
        {
            List<int> data = arr.ToList();
            return data.Contains(_bigJoker)&&data.Contains(_smallJoker);           
        }
    }
}
