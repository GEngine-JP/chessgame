using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.jh.EventII;
using Sfs2X.Entities.Data;
using UnityEngine;
using Assets.Scripts.Game.jh.Public;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhPlayersView : MonoBehaviour
    {
        public EventObject EventObj;

        public List<JhPlayer> Players;

        public JhUserContrl UserContrl;

        public JhAddBeat AddBeat;

        public JhBeatPool BeatPool;

        public JhPk Pk;

        public JhSendCardAnimation SendCardAnimation;

        public UILabel LeaveTimeTip;

        public GameObject ChooseCompareTip;

        public bool DonotResetOnGameResult;

        //0 默认有动画 1 没有动画 只是滑动下
        public int SendCardType;

        public bool _showTimeTip;

        public int ShowTimeTipTime = 5;
        public void OnReceive(EventData data)
        {
            switch (data.Name)
            {
                case "Status":
                    OnGameStatus(data.Data);
                    break;
                case "Ready":
                    int readySeat = (int)data.Data;
                    Players[readySeat].SetReady(true);
                    if(readySeat==0){
                        Reset(true);
                    }
                    break;
                case "Fapai":
                    OnFapai(data.Data);
                    break;
                case "CurrPlayer":
                    OnCurrPlayer(data.Data);
                    break;
                case "Bet":
                    OnBet(data.Data);
                    break;
                case "GiveUp":
                    OnGiveUp(data.Data);
                    break;
                case "Look":
                    OnLook(data.Data);
                    break;
                case "CompareUsers":
                    OnChooseCompareUser(data.Data);
                    break;
                case "Compare":
                    OnCompare(data.Data);
                    break;
                case "Result":
                    OnResult(data.Data);
                    break;
                case "GZYZ":
                    OnGzyz(data.Data);
                    break;
                case "AllBiPai":
                    OnAllBiPai(data.Data);
                    break;
                case "AutoFollow":
                    OnAutoFollow(data.Data);
                    break;
                case "LiangPai":
                    OnLiangPai(data.Data);
                    break;
                case "Reset":
                    Reset(!DonotResetOnGameResult);
                    break;
            }
        }

        protected void OnLiangPai(object data)
        {
            ISFSObject sendObj = (ISFSObject)data;
            int chair = sendObj.GetInt("Chair");
            int[] cards = sendObj.GetIntArray("Cards");

            Players[chair].SetPlayerCards(cards);
            Players[chair].ShowLiangPai();

            if (chair == 0)
            {
                UserContrl.QiPaiShow();
            }
        }
        protected void OnAutoFollow(object data) 
        {
            ISFSObject sendObj = (ISFSObject)data;
            bool isCurr = sendObj.GetBool("IsCurrPlayer");
            if (isCurr)
            {
                double cdTime = sendObj.GetDouble("CdTime");
                Players[0].ResetTimer();
                Players[0].OnPlayerTrun(cdTime, 0);
                LeaveTimeTip.text = "";
            }

            SetUserContrl(isCurr ? 0 : -1, sendObj);
        }


        private void OnAllBiPai(object data)
        {
            ISFSObject obj = (ISFSObject)data;
            string msg = obj.GetUtfString("msg");
            List<int> lostChair =obj.GetIntArray("lostChair").ToList();
            Pk.On20TrunBiPai(new EventDelegate(() =>
            {
                foreach (var lChair in lostChair)
                {
                    Players[lChair].ShowCardsGray();
                    if (lChair == 0)
                    {
                        UserContrl.LiangPaiShow();
                    }
                }
            }),msg);
        }

        private void OnGzyz(object data)
        {
            ISFSObject obj = (ISFSObject) data;
            int chair = obj.GetInt("Chair");
            int gold = obj.GetInt("Gold");
            bool isWin = obj.GetBool("isWin");
            int[] lostList = obj.GetIntArray("LostList");
            string uname = obj.GetUtfString("Name");

            Players[chair].ResetTimer();
            Players[chair].RefreshCoin();

            Pk.OnGzyz(uname,new EventDelegate(() =>
            {
                foreach (var lChair in lostList)
                {
                    Players[lChair].ShowCardsGray();
                    if (lChair == 0)
                    {
                        UserContrl.LiangPaiShow();
                    }
                }
                if (!isWin && gold!=0)
                {
                    Players[chair].ShowMoveGold(-gold);
                }

                if (obj.ContainsKey("FanCha"))
                {
                    ISFSArray arr = obj.GetSFSArray("FanCha");
                    for(int i = 0 ;i<arr.Count;i++)
                    {
                        ISFSObject arrObj = arr.GetSFSObject(i);
                        int fChair = arrObj.GetInt("Chair");
                        Players[fChair].RefreshCoin();
                    }
                }
            }));
        }

        private void OnResult(object data)
        {
            ISFSArray arr = (ISFSArray) data;
            for (int i = 0; i < arr.Count; i++)
            {
                ISFSObject obj = arr.GetSFSObject(i);
                int result = obj.GetInt("ResultType");
                int chair = obj.GetInt("Chair");
                if (result == 0)
                {
                    int win = obj.GetInt("WinGold");
                    BeatPool.ResultMoveChips(Players[chair].gameObject,new EventDelegate(() =>
                    {
                        Players[chair].ShowMoveGold(win);
                        Players[chair].RefreshCoin();
                        StartCoroutine(DelayReset());
                    }));

                }
                if (obj.ContainsKey("Cards"))
                {
                    int[] cards = obj.GetIntArray("Cards");
                    Players[chair].SetPlayerCards(cards);
                }

            }

            EventObj.SendEvent("TableViewEvent","Result",null);
        }

        private IEnumerator DelayReset()
        {
            yield return new WaitForSeconds(1);
            Reset(!DonotResetOnGameResult);
        }

        private void OnCompare(object data)
        {
            ISFSObject obj = (ISFSObject) data;
            int winner = obj.GetInt("Winner");
            int loster = obj.GetInt("Loster");
//            int gold = obj.GetInt("Gold");
            int chair = obj.GetInt("Chair");

            int chipValue = obj.GetInt("ChipValue");
            int chipIndex = obj.GetInt("ChipIndex");
            int chipCnt = obj.GetInt("ChipCnt");
            if (obj.ContainsKey("AllBeat"))
            {
                Players[chair].SetBeat(obj.GetInt("AllBeat"));
            }
            for (int i = 0; i < chipCnt; i++)
            {
                BeatPool.Beat(Players[chair].gameObject, chipIndex, chipValue);
            }

            Players[chair].RefreshCoin();
            Players[chair].ResetTimer();

            if (chair == 0)
            {
                UserContrl.QiPaiShow();
            }
            int otherChair = chair == winner ? loster : winner;
            Pk.OnPk(Players[chair].HeadBg, Players[otherChair].HeadBg, chair == winner, new EventDelegate(
                () =>
                {
                    //设置牌
                    Players[loster].ShowCardsGray();
                    Players[loster].SetPlayerCardsStatus(JhCardGroup.GroupStatus.CompareOut);
//                    Players[winner].ShowMoveGold(gold);
//                    Players[loster].ShowMoveGold(-gold);
                    if (loster == 0)
                    {
                        UserContrl.QiPaiShow();
                    }
                }));
        }

        private void OnChooseCompareUser(object data)
        {
            List<int> chairList = (List<int>) data;

            if (chairList.Count == 1)
            {
                EventObj.SendEvent("ServerEvent", "CompareReq", chairList[0]);
                UserContrl.ReturnToLastShow(JhUserContrl.BiPai);
            }
            else
            {
                foreach (int i in chairList)
                {
                    Players[i].SetHeadChooseCompare(Players[0].transform.position);
                }
                UserContrl.SetQuXiaoBiPai(true);

                if (ChooseCompareTip != null)
                {
                    ChooseCompareTip.SetActive(true);
                }
            }

        }

        public void OnHeadChooseCompare(int chair)
        {
            foreach (var p in Players)
            {
                p.ResetHeadClick();
            }

            if (ChooseCompareTip != null)
            {
                ChooseCompareTip.SetActive(false);
            }

            EventObj.SendEvent("ServerEvent", "CompareReq",chair);
        }

        public void OnOnHeadDetailClick(int chair)
        {
            EventObj.SendEvent("TableEvent", "ShowUserDetail", chair);
        }

        private void OnLook(object data)
        {
            ISFSObject obj = (ISFSObject) data;
            int chair = obj.GetInt("LookChair");
            int self = obj.GetInt("Chair");
            Players[chair].SetPlayerCardsStatus(JhCardGroup.GroupStatus.Look);
            if (obj.ContainsKey("Cards"))
            {
                int[] cards = obj.GetIntArray("Cards");
                Players[chair].SetPlayerCards(cards);
                Players[chair].ShowCardsFront();
            }
            if (obj.ContainsKey("IsCurPlayer") && obj.GetBool("IsCurPlayer"))
            {
                SetUserContrl(self, obj);
            }

            Players[chair].ResetTimer();
        }

        private void OnGiveUp(object data)
        {
            int chair = (int) data;
            Players[chair].ShowCardsGray();
            Players[chair].SetPlayerCardsStatus(JhCardGroup.GroupStatus.GiveUp);
            if (chair == 0)
            {
                Players[chair].ResetTimer();
                UserContrl.LiangPaiShow();
                Pk.Reset();

                if (ChooseCompareTip != null)
                {
                    ChooseCompareTip.SetActive(false);
                }
            }
        }

        private void OnBet(object data)
        {
            ISFSObject obj = (ISFSObject) data;

            int chair = obj.GetInt("Chair");
            int chipValue = obj.GetInt("ChipValue");
            int chipIndex =  obj.GetInt("ChipIndex");
            int chipCnt = obj.GetInt("ChipCnt");
            int gold = obj.GetInt("Gold");
            if (obj.ContainsKey("AllBeat"))
            {
                Players[chair].SetBeat(obj.GetInt("AllBeat"));
            }
            for (int i = 0; i < chipCnt; i++)
            {
                BeatPool.Beat(Players[chair].gameObject, chipIndex, chipValue);
            }

            Players[chair].ResetTimer();
            Players[chair].RefreshCoin();
        }


        private void OnCurrPlayer(object data)
        {
            AddBeat.Hide();
            foreach (JhPlayer jhPlayer in Players)
            {
                jhPlayer.ResetHeadClick();
            }
            

            ISFSObject sendObj = (ISFSObject)data;
            int chair = sendObj.GetInt("Chair");
            double cdTime = sendObj.GetDouble("CdTime");
            int singleBet = sendObj.GetInt("SingleBet");
            long playerGold = sendObj.GetLong("PlayerGold");
            bool isGzyz = sendObj.ContainsKey("IsGzyz")&&sendObj.GetBool("IsGzyz");
            bool isAutoFollow = sendObj.ContainsKey("IsAutoFollow") && sendObj.GetBool("IsAutoFollow");

            AddBeat.SetBetShow(singleBet, playerGold);

            foreach (JhPlayer jhPlayer in Players)
            {
                jhPlayer.ResetTimer();
            }

            SetUserContrl(chair, sendObj);

            SetCurrChair(chair, cdTime, 0, !isGzyz && isAutoFollow);
        }

        protected bool SelfIsCanQiPai;

        protected void OnSelfPlayerTimePass(double time)
        {
            int leaveTime = (int) time;
            if (LeaveTimeTip != null)
            {
                LeaveTimeTip.text = "[ff0000]" + leaveTime + "秒[-]后" + (SelfIsCanQiPai ? "自动弃牌" : "自动跟注");
            }
        }

        protected void SetUserContrl(int chair,ISFSObject sendObj)
        {
            bool isLook = sendObj.ContainsKey("IsCanLook")&&sendObj.GetBool("IsCanLook");
            bool isGiveup = sendObj.ContainsKey("IsCanGiveup") && sendObj.GetBool("IsCanGiveup");
            if (chair == 0)
            {
                SelfIsCanQiPai = isGiveup;    
            }
            bool isCompare = sendObj.ContainsKey("IsCanCompare") && sendObj.GetBool("IsCanCompare");
            bool playingGame = sendObj.ContainsKey("IsUserPlaying") && sendObj.GetBool("IsUserPlaying");
            bool showCard = sendObj.ContainsKey("ShowCard") && sendObj.GetBool("ShowCard");
            bool isGzyz = sendObj.ContainsKey("IsGzyz") && sendObj.GetBool("IsGzyz");
            bool isAutoFollow = sendObj.ContainsKey("IsAutoFollow") && sendObj.GetBool("IsAutoFollow");

            if (playingGame)
            {
                if (chair == 0)
                {
                    if (isGzyz)
                    {
                        UserContrl.GzyzShow(isLook, isGiveup);
                    }
                    else
                    {
                        UserContrl.OnTrunShow(isLook, isCompare, isGiveup);
                    }
                }
                else
                {
                    UserContrl.NotTrunShow(isLook);
                    
                }

                UserContrl.ZiDongGenZhuShow(isAutoFollow);
            }
            else
            {
                if (showCard)
                {
                    UserContrl.LiangPaiShow();
                }
                else
                {
                    UserContrl.QiPaiShow();
                }
            }
        }

        private void OnFapai(object data)
        {
            ISFSObject obj = (ISFSObject) data;
            int banker = obj.GetInt("Banker");
            SetBanker(banker);
            int[] playing = obj.GetIntArray("Playing");
            int chipValue = obj.GetInt("ChipValue");
            int chipIndex =  obj.GetInt("ChipIndex");
            int singleBet = obj.GetInt("SingleBet");
            if (SendCardType == 0)
            {
                List<List<JhCard>> cards = new List<List<JhCard>>();
                int start = 0;
                bool isAdd = true;
                foreach (int t in playing)
                {

                    Players[t].ShowCardsBack();
                    cards.Add(Players[t].CardGroup.Cards);
                    if (t != banker && isAdd)
                    {
                        isAdd = false;
                        start++;
                    }
                }
                
                SendCardAnimation.OnFinish = new EventDelegate(() =>
                {
                    foreach (int t in playing)
                    {
                        BeatPool.Beat(Players[t].gameObject, chipIndex, chipValue);
                        Players[t].SetBeat(singleBet);
                        Players[t].RefreshCoin();
                    }
                });
                SendCardAnimation.OnSendCard(cards, start);
            }
            else
            {
                foreach (int t in playing)
                {
                    Players[t].ShowCardsBack();
                    Players[t].CardGroup.SendCardAnimation();
                    BeatPool.Beat(Players[t].gameObject, chipIndex, chipValue);
                    Players[t].RefreshCoin();
                }
            }
            

            foreach (JhPlayer jhPlayer in Players)
            {
                jhPlayer.SetReady(false);
            }

        }

        protected void OnGameStatus(object data)
        {
            ISFSObject obj = (ISFSObject)data;
            bool isPlaying = obj.GetBool("IsPlaying");
            Reset(isPlaying||!DonotResetOnGameResult);
            //初始化 加注
            int[] antes = obj.GetIntArray("Antes");

            AddBeat.SetBeatInfo(antes);
            int singleBet = obj.GetInt("SingleBet");
            long playerGold = obj.GetLong("PlayerGold");

            AddBeat.SetBetShow(singleBet, playerGold);

            _showTimeTip = obj.GetBool("isShowTimeTip");

            if (isPlaying)
            {
                int banker = obj.GetInt("Banker");
                SetBanker(banker);

                int curChair = obj.GetInt("CurChair");
                long lastTime = obj.GetLong("LastTime");
                double cdTime = obj.GetDouble("CdTime");
                double passTime = (int) (JhFunc.GetTimeStamp(false) - lastTime);
                if (passTime/1000.0f >= cdTime)
                {
                    passTime = 0;
                }

                bool isGzyz = obj.ContainsKey("IsGzyz") && obj.GetBool("IsGzyz");
                bool isAutoFollow = obj.ContainsKey("IsAutoFollow") && obj.GetBool("IsAutoFollow");

                SetCurrChair(curChair, cdTime, passTime / 1000.0f,!isGzyz&&isAutoFollow);

                SetUserContrl(curChair,obj);
                
                //chip pool
                if (obj.ContainsKey("ChipList"))
                {
                    ISFSArray chips = obj.GetSFSArray("ChipList");
                    for (int i = 0; i < chips.Count; i++)
                    {
                        ISFSObject arrItem = chips.GetSFSObject(i);
                        int chipValue = arrItem.GetInt("ChipValue");
                        int chipIndex = arrItem.GetInt("ChipIndex");
                        int chipCnt = arrItem.GetInt("ChipCnt");

                        for (int j = 0; j < chipCnt; j++)
                        {
                            BeatPool.Beat(chipIndex, chipValue);
                        }
                    }
                }
            }
            else
            {
                UserContrl.QiPaiShow();
            }

            
            
            //玩家信息
            ISFSArray array = obj.GetSFSArray("Players");
            foreach (ISFSObject arrItem  in array)
            {
                int chair = arrItem.GetInt("Chair");

                Players[chair].SetReady(arrItem.ContainsKey("IsReady") && arrItem.GetBool("IsReady"));

                if (arrItem.ContainsKey("IsPlaying") && arrItem.GetBool("IsPlaying"))
                {


                    Players[chair].ShowCardsBack();

                    if (arrItem.ContainsKey("Cards"))
                    {
                        Players[chair].SetPlayerCards(arrItem.GetIntArray("Cards"));
                    }

                    if (arrItem.ContainsKey("IsLook"))
                    {
                        if (arrItem.GetBool("IsLook"))
                        {
                            Players[chair].SetPlayerCardsStatus(JhCardGroup.GroupStatus.Look);
                            
                        }
                    }
                    if (arrItem.ContainsKey("IsGiveUp"))
                    {
                        if (arrItem.GetBool("IsGiveUp"))
                        {
                            Players[chair].SetPlayerCardsStatus(JhCardGroup.GroupStatus.GiveUp);
                            Players[chair].ShowCardsGray();
                       }
                    }
                    if (arrItem.ContainsKey("IsFail"))
                    {
                        if (arrItem.GetBool("IsFail"))
                        {
                            Players[chair].SetPlayerCardsStatus(JhCardGroup.GroupStatus.CompareOut);
                            Players[chair].ShowCardsGray();
                        }
                    }
                    if (arrItem.ContainsKey("AllBeat"))
                    {
                        Players[chair].SetBeat(arrItem.GetInt("AllBeat"));
                    }
                }
                
            }
        }

        private void Reset(bool isResetPlayer)
        {
            UserContrl.QiPaiShow();
            AddBeat.Reset();
            BeatPool.Reset();
            Pk.Reset();
            SendCardAnimation.Reset();
            LeaveTimeTip.text = "";

            if (isResetPlayer)
            {
                foreach (JhPlayer jhPlayer in Players)
                {
                    jhPlayer.Reset();
                }
            }
        }

        protected void SetCurrChair(int chair, double cdTime, double pastTime,bool isAutoFolow)
        {
            if (chair == 0)
            {
                if (isAutoFolow)
                {
                    Players[chair].OnPlayerTrun(cdTime, 0, null, new EventDelegate(() =>
                    {

                        EventObj.SendEvent("ServerEvent", "FollowReq", null);
                    }));
                    LeaveTimeTip.text = "";
                    return;
                }

                if (_showTimeTip)
                {
                    Players[chair].OnPlayerTrun(cdTime, pastTime, new EventDelegate(this, "OnSelfPlayerTimePass"));
                    return;
                }

            }
            else
            {
                if (ChooseCompareTip != null)
                {
                    ChooseCompareTip.SetActive(false);
                }
            }

            Players[chair].OnPlayerTrun(cdTime, pastTime);
            LeaveTimeTip.text = "";
        }

        protected void SetBanker(int chair)
        {
            foreach (JhPlayer jhPlayer in Players)
            {
                jhPlayer.SetBanker(jhPlayer.Chair == chair);
            }
        }

        public void OnGiveUpClick()
        {
            UserContrl.LiangPaiShow();
            EventObj.SendEvent("ServerEvent", "GiveUpReq", null);
        }

        public void OnCompareClick()
        {
            EventObj.SendEvent("TableEvent", "Compare", null);
        }

        public void OnLookClick()
        {
            UserContrl.ReturnToLastShow(JhUserContrl.KanPai);
            EventObj.SendEvent("ServerEvent", "LookReq", null);
        }

        public void OnFollowClick()
        {
            UserContrl.SetBtnShow(UserContrl.NotTrun,JhUserContrl.KanPai);
            EventObj.SendEvent("ServerEvent", "FollowReq", null);
        }

        public void OnAutoFollowClick()
        {
            EventObj.SendEvent("TableEvent", "AutoFollow", null);
        }

        public void OnAddBeat(int value)
        {
            UserContrl.SetBtnShow(UserContrl.NotTrun, JhUserContrl.KanPai);
            AddBeat.Hide();
            EventObj.SendEvent("ServerEvent", "FollowReq", value);
        }

        public void OnGzyzClick()
        {
            EventObj.SendEvent("ServerEvent", "Gzyz", null);
            UserContrl.LiangPaiShow();
        }

        public void OnLiangClick()
        {
            EventObj.SendEvent("ServerEvent", "LiangPai", null);
            UserContrl.QiPaiShow();
        }

        public void OnQuXiaoBiPai()
        {
            UserContrl.SetQuXiaoBiPai(false);
            foreach (var p in Players)
            {
                p.ResetHeadClick();
            }

            if (ChooseCompareTip != null)
            {
                ChooseCompareTip.SetActive(false);
            }
        }

    }
}
