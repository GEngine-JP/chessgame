﻿using YxFramwork.Common;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelSingleResult), UIPanelhierarchy.Popup)]
    public class PanelSingleResult : UIPanelBase, IUIPanelControl<SingleResultArgs>
    {
        public GameObject ContinueGameBtn;
        public GameObject StartGameBtn;
        public GameObject ReturnHallBtn;
        public GameObject TotalResultBtn;
        public SingleResultItem[] PlayersItem;
        public SubPanelHuDetail Detail;

        protected SingleResultArgs mResultArgs;

        public override void OnContinueGameUpdate()
        {
            ContinueGameBtn.SetActive(false);
            Close();
        }

        public void OnContinueGameClick()
        {
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.OnCreateNewGame);
        }

        public override void OnGetInfoUpdate() { Close(); }

        public override void OnStartGameUpdate() { Close(); }

        public override void OnReadyUpdate() { Close(); }

        public void SetBtnState()
        {
            var db = GameCenter.DataCenter;
            if (db.Room.RoomType == MahRoomType.FanKa)
            {
                bool isEndRound = db.IsGameOver;
                if (db.Room.LoopType == MahGameLoopType.Round && db.Room.RealityRound >= db.Room.MaxRound)
                {
                    isEndRound = true;
                }
                StartGameBtn.SetActive(!isEndRound);
                ReturnHallBtn.SetActive(isEndRound);
                TotalResultBtn.SetActive(isEndRound);
                if (db.ConfigData.ContinueNewGame && isEndRound)
                {
                    ContinueGameBtn.SetActive(true);
                    ReturnHallBtn.SetActive(false);
                }
            }
            else
            {
                //返回大厅按钮，娱乐房每局结算时都显示 
                ReturnHallBtn.SetActive(true);
            }
        }

        //开始游戏
        public void OnStartClick()
        {
            GameCenter.Network.C2S.RestartGame();
            Close();
        }

        /// <summary>
        /// 分享
        /// </summary>
        public void OnShareClick()
        {
            MahjongUtility.DoScreenShot(this, new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
             {
                 if (Application.platform == RuntimePlatform.Android)
                 {
                     imageUrl = "file://" + imageUrl;
                 }
                 GameUtils.WeChatShareGameResult(imageUrl);
             });
        }

        /// <summary>
        /// 查看战绩
        /// </summary>
        public void OnShowResultClick()
        {
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowTotalResult);
            Close();
        }

        /// <summary>
        /// 返回大厅
        /// </summary>
        public void OnReturnToHall()
        {
            App.QuitGame();
        }

        private void ItemsSort(SingleResultArgs args)
        {
            var huSort = args.HuSort;
            if (!huSort.ExIsNullOjbect())
            {
                var result = args.Result;
                for (int i = 0; i < result.Count; i++)
                {
                    int index;
                    if (huSort.TryGetValue(result[i].Chair, out index))
                    {
                        result.ExeExchange(i, index);
                    }
                }
            }
            else
            {
                args.Result.Sort((a, b) =>
                {
                    bool isAHu = a.HuSeat < MiscUtility.DefInt;
                    bool isBHu = b.HuSeat < MiscUtility.DefInt;
                    if (isAHu && !isBHu) return -1;
                    if (!isAHu && isBHu) return 1;
                    if (isAHu && isBHu)
                    {
                        int aScore = a.HuGold;
                        int bScore = b.HuGold;
                        if (aScore < bScore) return -1;
                        if (aScore > bScore) return 1;
                    }
                    return 0;
                });
            }
        }

        public void Open(SingleResultArgs args)
        {
            base.Open();
            mResultArgs = args;

            SetBtnState();
            for (int i = 0; i < PlayersItem.Length; i++)
            {
                PlayersItem[i].OnReset();
            }

            ItemsSort(args);
            SingleResultItem item;
            MahjongPlayersData playersData = GameCenter.DataCenter.Players;
            for (int i = 0; i < args.Result.Count; i++)
            {
                item = PlayersItem[i];
                int chair = args.Result[i].Chair;
                item.gameObject.SetActive(true);
                item.Name = args.Result[i].Name;
                item.HuInfoTxt = args.Result[i].HuInfo;
                item.HuTypeTxt = args.Result[i].UserHuType;
                //显示庄                 
                item.Banker = DataCenter.BankerChair == chair;
                //设置cpg
                var play = playersData.GetUserInfoFormBackup(chair);
                if (play == null)
                {
                    continue;
                }

                item.SetCpgCard(play.CpgDatas);
                //设置手牌               
                var playerHardCards = play.HardCards;

                var handCards = item.SetCards(playerHardCards);
                bool huCardIsJue = false;

                if (GameCenter.DataCenter.ConfigData.Jue)
                {
                    var dic = MahjongUtility.MahjongFlagJueCheck(play.CpgDatas, playerHardCards, args.HuCard);
                    huCardIsJue = dic.ContainsKey(true);
                    var jueList = dic[huCardIsJue];
                    if (null != handCards && handCards.ObjList.Count > 0 && jueList.Count > 0)
                    {
                        for (int j = 0; j < jueList.Count; j++)
                        {
                            for (int k = 0; k < playerHardCards.Count; k++)
                            {
                                if (jueList[j] == playerHardCards[k])
                                {
                                    int value = jueList[j];
                                    var cardObj = handCards.ObjList[k];
                                    if (cardObj != null && !GameCenter.DataCenter.IsLaizi(value))
                                    {
                                        var mah = cardObj.GetComponent<Mah2DObject>();
                                        mah.SetOther();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                item.SetDetail(args.Result[i].HuFlag);
                //设置胡牌 
                if (args.HuSeats.Contains(play.Seat))
                {
                    switch (args.ResultType)
                    {
                        case SingleResultArgs.HuResultType.HuSingle:
                            if (MahjongUtility.GameKey != "xlmj")
                            {
                                item.SetCard(args.Result[i].HuCard);
                            }
                            else
                            {
                                item.SetCards(play.HucardList);
                            }
                            break;
                        case SingleResultArgs.HuResultType.HuEndu:
                            //胡牌
                            var uiCards = item.SetCard(args.HuCard);
                            //扎码
                            if (DataCenter.Config.ResultShowZhongma)
                            {
                                item.SetCards(args.ZhongMa);
                            }
                            GameObject cardObj = null;
                            if (null != uiCards && uiCards.ObjList.Count > 0)
                            {
                                cardObj = uiCards.ObjList[0];
                            }
                            //设置宝牌
                            if (GameCenter.DataCenter.Game.BaoCard == args.HuCard)
                            {
                                if (cardObj != null)
                                {
                                    var mah = cardObj.GetComponent<Mah2DObject>();
                                    mah.SetLaizi();
                                }
                            }
                            if (GameCenter.DataCenter.ConfigData.Jue && huCardIsJue)
                            {
                                if (cardObj != null)
                                {
                                    var mah = cardObj.GetComponent<Mah2DObject>();
                                    mah.SetOther();
                                }
                            }
                            break;
                    }
                }
                string info;
                //排序
                item.SortCardGroup();
                //总分
                info = MahjongUtility.GetShowNumberFloat(args.Result[i].Gold).ToString();
                item.SetItem(TextType.TotalSocre, info);
                //胡牌分
                info = MahjongUtility.GetShowNumberFloat(args.Result[i].HuGold).ToString();
                item.SetItem(TextType.HupaiScore, info);
                //杠分               
                info = MahjongUtility.GetShowNumberFloat(args.Result[i].GangGlod).ToString();
                item.SetItem(TextType.GangScore, info);

                //漂分
                var piaoValue = MahjongUtility.GetShowNumberFloat(args.Result[i].PiaoGlod);
                item.SetItem(TextType.PiaoScore, piaoValue);
                //扑分
                var puValue = MahjongUtility.GetShowNumberFloat(args.Result[i].PuGlod);
                item.SetItem(TextType.PuScore, puValue);
                //扎鸟分
                var niaoValue = MahjongUtility.GetShowNumberFloat(args.Result[i].NiaoGold);
                item.SetItem(TextType.NiaoSocre, niaoValue);
            }

            var owner = GameCenter.DataCenter.Players[0];
            if (owner != null)
            {
                bool isMeWin = args.HuSeats.Contains(owner.Seat);
                if (isMeWin)
                {
                    MahjongUtility.PlayEnvironmentSound("win");
                }
            }

            //设置当前轮数,开房模式下才会有
            if (DataCenter.Room.RoomType == MahRoomType.FanKa)
            {
                DataCenter.Room.RealityRound++;
                if (DataCenter.Room.LoopType == MahGameLoopType.Circle)
                {
                    if (DataCenter.Room.NextBaner == DataCenter.FristBankerSeat && DataCenter.BankerSeat != DataCenter.FristBankerSeat)
                    {
                        DataCenter.Room.CurrRound++;
                    }
                }
                else
                {
                    DataCenter.Room.CurrRound++;
                }
            }
            OtherSet();
        }

        private void OtherSet()
        {
            var collections = GetComponent<GameObjectCollections>();
            if (collections != null)
            {
                int bao = DataCenter.Game.BaoCard;
                //显示宝牌
                collections.Get<UIShowMahjong>("ShowBao").SetActive(bao > 0, bao);
                transform.FindChild("Bg/Btns").GetComponent<HorizontalLayoutGroup>().childAlignment = bao > 0 ? TextAnchor.MiddleRight : TextAnchor.LowerCenter;
            }
        }

        public void OnDetailClick(int index)
        {
            Detail.ShowDetail(mResultArgs.Result[index], index);
        }
    }
}