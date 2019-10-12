using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_Hu : AbsGameLogicBase
    {
        private SingleResultArgs.HuResultType mResultType = SingleResultArgs.HuResultType.HuEndu;
        private SingleResultArgs mArgs;
        private ContinueTaskContainer mDianpaoTask;
        private ContinueTaskContainer mLastCdTask;
        private ContinueTaskContainer mZimoTask;
        private ContinueTaskContainer mHuTask;

        protected Func<int, string> HuMusicFunc;

        public void OnHu(ISFSObject data)
        {
            if (GameCenter.DataCenter.Room.RoomType == MahRoomType.YuLe)
            {
                GameCenter.Network.CtrlYuleRejoin(false);
            }

            if (HuMusicFunc == null)
            {
                HuMusicFunc = (ctype) =>
                {
                    string target = "";
                    var config = DataCenter.Config;
                    if (config.SpecialHuTypes.Length <= 0) return target;
                    for (int i = 0; i < config.SpecialHuTypes.Length; i++)
                    {
                        var temp = config.SpecialHuTypes[i];
                        if (GameUtils.BinaryCheck(temp.HuTypeValue, ctype))
                        {
                            if (config.IsPlayHunHeHuSound && !temp.IsOnly)
                            {
                                target += temp.HuTypeName;
                            }
                            else
                            {
                                return temp.HuTypeName;
                            }
                        }
                    }
                    return "";
                };
            }

            ParseData(data);
            SetPlayersHandCards();
            Game.TableManager.StartTimer(0);
            if (GameCenter.Shortcuts.CheckState(GameSwitchType.AiAgency))
            {
                //关闭托管
                GameCenter.EventHandle.Dispatch((int)GameEventProtocol.AiAgency, new AiAgencyArgs() { State = false });
            }
            //隐藏听箭头
            GameCenter.Scene.MahjongGroups.PlayerHand.OnQueryMahjong(null);
            //隐藏提示标记
            Game.TableManager.GetParts<MahjongOutCardFlag>(TablePartsType.OutCardFlag).Hide();
            //关闭查听
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }

        /// <summary>
        /// 流局
        /// </summary>     
        [S2CResponseHandler(NetworkProtocol.MJReqTypeLastCd)]
        public void OnHu_LastCd(ISFSObject data)
        {
            OnHu(data);
            if (null == mLastCdTask)
            {
                mLastCdTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => LiujuTask())
                .AppendFuncTask(() => ShowBaoTipTask())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mLastCdTask.Start();
        }

        [S2CResponseHandler(NetworkProtocol.MJReqTypeZiMo)]
        public void OnHu_Zimo(ISFSObject data)
        {
            OnHu(data);
            if (null == mZimoTask)
            {
                mZimoTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => ZimoTask())
                .AppendFuncTask(() => HandcardCtrlTask())
                .AppendFuncTask(() => ShowBaoTipTask())
                .AppendFuncTask(() => ZhaNiaoAnimation())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mZimoTask.Start();
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeHu)]
        public void OnHu_Dianpao(ISFSObject data)
        {
            OnHu(data);
            if (null == mDianpaoTask)
            {
                mDianpaoTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => YipaoDuoxiangTask())
                .AppendFuncTask(() => DianpaoTask())
                .AppendFuncTask(() => HandcardCtrlTask())
                .AppendFuncTask(() => ShowBaoTipTask())
                .AppendFuncTask(() => ZhaNiaoAnimation())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mDianpaoTask.Start();
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        private void ParseData(ISFSObject data)
        {
            MahjongResult result;
            var huSeats = new List<int>();
            var mahjongResults = new List<MahjongResult>();
            var playersData = data.GetSFSArray(RequestKey.KeyPlayerList);

            var players = DataCenter.Players;
            for (int i = 0; i < playersData.Count; i++)
            {
                result = new MahjongResult(i);
                var player = playersData.GetSFSObject(i);
                if (player.ContainsKey(RequestKey.KeyCardType))
                {
                    int type = player.GetInt(RequestKey.KeyCardType);
                    if (type >= 1)
                    {
                        result.HuFlag = true;
                        result.HuSeat = i;
                        huSeats.Add(i);
                    }
                    result.UserHuType = type;
                }
                result.PuGlod = player.GetInt("pu");
                result.CType = player.TryGetInt("ctype");
                result.HuCard = player.TryGetInt("hucard");
                result.TotalGold = player.GetLong("ttgold");
                result.HuInfo = player.TryGetString("hname");
                result.Gold = player.TryGetInt(RequestKey.KeyGold);
                result.NiaoGold = player.GetInt(ProtocolKey.KeyNiao);
                result.PiaoGlod = player.GetInt(ProtocolKey.KeyPiao);
                result.GangGlod = player.GetInt(ProtocolKey.KeyGGang);
                result.HuGold = player.GetInt(ProtocolKey.KeyGHu) + player.TryGetInt(ProtocolKey.KeyGHua);
                mahjongResults.Add(result);
                //添加cpg数据
                ISFSArray Groups = player.TryGetSFSArray(ProtocolKey.KeyMjGroup);
                if (Groups != null)
                {
                    var playerData = players[result.Chair];
                    playerData.ClearCpgData();
                    for (int j = 0; j < Groups.Count; j++)
                    {
                        var cpg = MahjongUtility.CreateCpg(Groups.GetSFSObject(j));
                        playerData.AddCpgData(cpg);
                    }
                }
                //获取胡牌明细信息
                result.SetDeatil(player);
            }
            //更新手牌
            var cards = data.GetSFSArray(RequestKey.KeyCardsArr);
            for (int i = 0; i < cards.Count; i++)
            {
                var chair = MahjongUtility.GetChair(i);
                players[chair].HardCards = new List<int>(cards.GetIntArray(i));
                MahjongUtility.SortMahjong(players[chair].HardCards);
            }
            mArgs = new SingleResultArgs()
            {
                HuSeats = huSeats,
                Result = mahjongResults,
                Bao = data.TryGetInt("bao"),
                HuCard = data.TryGetInt("huCard"),
                HuType = data.TryGetInt(RequestKey.KeyType),
                PiaoHu = data.ContainsKey(ProtocolKey.KeyPiaoHu),
                ZhongMa = data.TryGetIntArray(ProtocolKey.KeyZhongma),
                ZhaMa = data.TryGetIntArray(ProtocolKey.KeyZhaNiao),
                ChBao = data.TryGetInt(ProtocolKey.KeyChBao) == 1,
                MoBao = data.TryGetInt(ProtocolKey.KeyMoBao) == 1,
            };
            //解析胡牌顺序
            if (data.ContainsKey("hushunxu"))
            {
                ISFSArray array = data.GetSFSArray("hushunxu");
                if (array != null)
                {
                    var huSort = new Dictionary<int, int>();
                    for (int i = 0; i < array.Count; i++)
                    {
                        int[] sfs = array.GetIntArray(i);
                        for (int j = 0; j < sfs.Length; j++)
                        {
                            int chair = MahjongUtility.GetChair(sfs[j]);
                            if (!huSort.ContainsKey(chair))
                            {
                                huSort.Add(chair, i);
                            }
                        }
                    }
                    mArgs.HuSort = huSort;
                }
            }
            mArgs.ResultType = mResultType;
            GameCenter.DataCenter.Game.BaoCard = mArgs.Bao;
            DataCenter.Room.NextBaner = data.TryGetInt(ProtocolKey.KeyNextBanker);
            //清理麻将听标记           
            GameCenter.Shortcuts.MahjongQuery.Do(p => p.ShowQueryTip(null));

            DataCenter.Game.Laozhuang = data.TryGetInt("lzcnt");
        }

        /// <summary>
        /// 为所有玩家设置手牌
        /// </summary>
        private void SetPlayersHandCards()
        {
            List<int> array;
            MahjongHand handCards;
            var huChair = -1;
            if (mArgs.HuSeats.Count > 0)
            {
                huChair = MahjongUtility.GetChair(mArgs.HuSeats[0]);
            }
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                handCards = Game.MahjongGroups.MahjongHandWall[i];
                handCards.RemoveAllMj();
                if (DataCenter.Game.FenzhangFlag)
                {
                    if (mArgs.HuType == NetworkProtocol.MJReqTypeZiMo && huChair == i)
                    {
                        //移除自摸牌
                        DataCenter.Players[i].HardCards.Remove(mArgs.HuCard);
                    }
                    array = new List<int>(DataCenter.Players[i].HardCards);
                    //移除分张牌               
                    array.Remove(DataCenter.Players[i].FenzhangCard);
                }
                else
                {
                    if (mArgs.HuType == NetworkProtocol.MJReqTypeZiMo && huChair == i)
                    {
                        //移除自摸牌
                        DataCenter.Players[i].HardCards.Remove(mArgs.HuCard);
                        array = new List<int>(DataCenter.Players[i].HardCards);
                    }
                    else
                    {
                        array = DataCenter.Players[i].HardCards;
                    }
                }
                handCards.GetInMahjong(array);
                handCards.SetLaizi(DataCenter.Game.LaiziCard);
            }
        }

        private IEnumerator<float> LiujuTask()
        {
            if (!GameUtils.CheckStopTask())
            {
                var group = Game.MahjongGroups;
                for (int i = 0; i < group.MahjongHandWall.Length; i++)
                {
                    group.MahjongHandWall[i].GameResultRota(Config.TimeHuAniInterval);
                }
                yield return Config.TimeHuAniInterval;
            }
        }

        private IEnumerator<float> ZimoTask()
        {
            if (!GameUtils.CheckStopTask())
            {
                yield return Config.TimeHuAniInterval;
                var huCard = mArgs.HuCard;
                var huChair = MahjongUtility.GetChair(mArgs.HuSeats[0]);
                string huType = "";
                if (DataCenter.Config.PlaySpecialHuSound)
                {
                    huType = IsSpecialHu(mArgs.Result[mArgs.HuSeats[0]].CType);
                }
                if (!string.IsNullOrEmpty(huType))
                {
                    MahjongUtility.PlayOperateSound(huChair, huType);
                    GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(huChair, PoolObjectType.zimo);
                }
                else if (mArgs.PiaoHu)
                {
                    MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.piaohu);
                }
                else if (mArgs.MoBao)
                {
                    //播放摸宝特效
                    MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.mobao);
                }
                else if (mArgs.ChBao)
                {
                    //播放冲宝特效
                    MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.chongbao);
                }
                else
                {
                    MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.zimo);
                }
                //设置胡牌，分张除外
                if (!DataCenter.Game.FenzhangFlag)
                {
                    SetHuCard(huChair, huCard);
                }
            }
        }

        /// <summary>
        /// 一炮多响
        /// </summary> 
        private IEnumerator<float> YipaoDuoxiangTask()
        {
            if (mArgs.HuType == NetworkProtocol.MJRequestTypeHu && mArgs.HuSeats.Count > 1 && !GameUtils.CheckStopTask())
            {
                yield return Config.TimeHuAniInterval;
                var index = 0;
                var seats = mArgs.HuSeats;
                var paoChair = GameCenter.DataCenter.CurrOpChair;
                for (int i = 0; i < seats.Count; i++)
                {
                    var duoHuChair = MahjongUtility.GetChair(seats[i]);
                    //判断是不是抢杠胡
                    var ctype = mArgs.Result[seats[0]].CType;
                    bool isQiangGangHu = ctype != 0 && (ctype & NetworkProtocol.MJQiangGangHuType) != 0;
                    if (index++ == 0)
                    {
                        if (isQiangGangHu)
                        {
                            var item = Game.MahjongCtrl.PopMahjong(mArgs.HuCard);
                            Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(item);
                        }
                        else
                        {
                            var effect = MahjongUtility.PlayMahjongEffectAndAudio(PoolObjectType.shandian);
                            effect.transform.position = Game.MahjongGroups.MahjongThrow[paoChair].GetLastMjPos();
                            effect.Execute();
                            yield return 0.5f;
                            Game.MahjongGroups.MahjongThrow[paoChair].PopMahjong();
                            Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(mArgs.HuCard);
                        }
                    }
                    else
                    {
                        var clone = Game.MahjongCtrl.PopMahjong(mArgs.HuCard);
                        var cloneMj = clone.GetComponent<MahjongContainer>();
                        Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(mArgs.HuCard);
                        Game.MahjongGroups.MahjongOther[duoHuChair].GetInMahjong(cloneMj);
                    }
                    MahjongUtility.PlayOperateEffect(duoHuChair, PoolObjectType.hu);
                }
            }
        }

        private IEnumerator<float> DianpaoTask()
        {
            if (mArgs.HuType == NetworkProtocol.MJRequestTypeHu && mArgs.HuSeats.Count == 1 && !GameUtils.CheckStopTask())
            {
                yield return Config.TimeHuAniInterval;
                //判断是不是抢杠胡
                var seat = mArgs.HuSeats[0];
                var ctype = mArgs.Result[seat].CType;
                var huChair = MahjongUtility.GetChair(seat);
                var duoHuChair = MahjongUtility.GetChair(seat);
                var isQiangGangHu = ctype != 0 && (ctype & NetworkProtocol.MJQiangGangHuType) != 0;
                if (isQiangGangHu)
                {
                    SetHuCard(huChair, mArgs.HuCard);
                }
                else
                {
                    var paoChair = GameCenter.DataCenter.CurrOpChair;
                    var effect = MahjongUtility.PlayMahjongEffect(PoolObjectType.shandian);
                    effect.transform.position = Game.MahjongGroups.MahjongThrow[paoChair].GetLastMjPos();
                    effect.Execute();
                    MahjongUtility.PlayEnvironmentSound("shandian");
                    yield return 0.5f;
                    Game.MahjongGroups.MahjongThrow[paoChair].PopMahjong();
                    SetHuCard(duoHuChair, mArgs.HuCard).Laizi = MahjongUtility.MahjongFlagCheck(mArgs.HuCard);
                }
                string huType = "";
                if (DataCenter.Config.PlaySpecialHuSound)
                {
                    huType = IsSpecialHu(mArgs.Result[mArgs.HuSeats[0]].CType);
                }
                if (!string.IsNullOrEmpty(huType))
                {
                    MahjongUtility.PlayOperateSound(huChair, huType);
                    GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(duoHuChair, PoolObjectType.hu);
                }
                else if (mArgs.PiaoHu)
                {
                    MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.piaohu);
                }
                else
                {
                    MahjongUtility.PlayOperateEffect(duoHuChair, PoolObjectType.hu);
                }
            }
        }

        private IEnumerator<float> ZhaNiaoAnimation()
        {
            if (mArgs.ZhaMa != null && !GameUtils.CheckStopTask())
            {
                yield return Config.TimeZhaniaoAni * 2;
                var zhaArr = mArgs.ZhaMa;
                var zhongArr = mArgs.ZhongMa;
                if (zhaArr != null && zhaArr.Length != 0)
                {
                    //有中码
                    var flag = null != zhongArr && zhongArr.Length > 0;

                    ZhaniaoArgs args = new ZhaniaoArgs();
                    args.ZhaMaList.AddRange(zhaArr);
                    if (flag)
                    {
                        args.ZhongMaAllList.AddRange(zhongArr);
                    }
                    GameCenter.Hud.GetPanel<PanelZhaniao>().Open(args);

                    float time = (zhaArr.Length) * 0.7f + 0.5f;
                    yield return time;
                }
            }
        }

        private IEnumerator<float> HandcardCtrlTask()
        {
            if (mArgs.HuType != NetworkProtocol.MJReqTypeLastCd && !GameUtils.CheckStopTask())
            {
                yield return Config.TimePushCardInterval;
                var huChair = new List<int>();
                var group = Game.MahjongGroups;
                for (int i = 0; i < mArgs.HuSeats.Count; i++)
                {
                    int chair = MahjongUtility.GetChair(mArgs.HuSeats[i]);
                    group.MahjongHandWall[chair].GameResultRota(Config.TimeHuAniInterval);
                    huChair.Add(chair);
                }
                yield return Config.TimePushCardInterval;
                for (int i = 0; i < group.MahjongHandWall.Length; i++)
                {
                    if (!huChair.Contains(i))
                    {
                        group.MahjongHandWall[i].GameResultRota(Config.TimeHuAniInterval);
                    }
                }
            }
        }

        /// <summary>
        /// 宝牌提示
        /// </summary>    
        private IEnumerator<float> ShowBaoTipTask()
        {
            if (mArgs.Bao > 0 && !GameUtils.CheckStopTask())
            {
                var list = new List<int>() { mArgs.Bao };
                GameCenter.Hud.GetPanel<PanelExhibition>().Open(list);
                yield return Config.TimeBaoTip;
            }
        }

        private MahjongContainer SetHuCard(int chair, int card)
        {
            var item = Game.MahjongCtrl.PopMahjong(card);
            Game.MahjongGroups.MahjongOther[chair].GetInMahjong(item);
            Game.TableManager.ShowOutcardFlag(item);
            item.gameObject.SetActive(true);
            return item;
        }

        private void ActionCallback()
        {
            if (GameUtils.CheckStopTask()) return;

            SetGameData();
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.ShowResult, mArgs);
            GameCenter.GameProcess.ChangeState<StateGameEnd>();
        }

        private void SetGameData()
        {
            if (GameUtils.CheckStopTask()) return;

            MahjongResult info;
            var scoreList = new Dictionary<int, long>();
            for (int i = 0; i < mArgs.Result.Count; i++)
            {
                info = mArgs.Result[i];
                scoreList.Add(info.Chair, info.TotalGold);
            }
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlayAddScore, new SetScoreArgs()
            {
                ScoreDic = scoreList,
                Type = (int)SetScoreType.EndScore,
            });
        }

        /// <summary>
        /// 判断是否为特殊胡法
        /// </summary>
        /// <param name="ctype">胡牌时服务器发来的ctype</param>
        /// <returns></returns>
        private string IsSpecialHu(int ctype)
        {
            //string target = "";
            //var config = DataCenter.Config;
            //if (config.SpecialHuTypes.Length <= 0) return target;
            //for (int i = 0; i < config.SpecialHuTypes.Length; i++)
            //{
            //    var temp = config.SpecialHuTypes[i];
            //    if (GameUtils.BinaryCheck(temp.HuTypeValue, ctype))
            //    {
            //        if (config.IsPlayHunHeHuSound && !temp.IsOnly)
            //        {
            //            target += temp.HuTypeName;
            //        }
            //        else
            //        {
            //            return temp.HuTypeName;
            //        }
            //    }
            //}
            //return target;
            return HuMusicFunc(ctype);
        }
    }
}