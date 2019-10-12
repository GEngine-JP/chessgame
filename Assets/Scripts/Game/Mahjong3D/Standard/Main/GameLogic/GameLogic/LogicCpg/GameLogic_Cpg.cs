using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_Cpg : AbsGameLogicBase
    {
        private CpgLogicData mData;

        public bool IsGangBao
        {
            get { return mData.GangBao; }
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeCPG)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeXFG)]
        [S2CResponseHandler(NetworkProtocol.MJRequestJueGang)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeSelfGang)]
        public void OnResponseCpg(ISFSObject data)
        {
            CpgLogic(data);
            PlayEffect(data);
        }

        private void CpgLogic(ISFSObject data)
        {
            mData.SetData(data);
            var cpgData = mData.CpgData;
            //如果是抓杠 改变类型
            if (cpgData.Type == EnGroupType.ZhuaGang)
            {
                CpgZhuaGang cpg = (CpgZhuaGang)cpgData;
                //抓杠时 会有两条数据回来 当消息为true 杠成功了
                if (cpg.Ok == false) return;
                var cpgList = DataCenter.Players[mData.CurrOpChair].CpgDatas;
                for (int i = 0; i < cpgList.Count; i++)
                {
                    var cpgItem = cpgList[i];
                    if (cpgItem.Type == EnGroupType.Peng && cpgItem.Card == cpgData.Card)
                    {
                        cpgList[i] = cpgData;
                        break;
                    }
                }
                if (0 == mData.CurrOpChair && !mData.GangBao)
                {
                    //删除手牌数据
                    DataCenter.Players[mData.CurrOpChair].HardCards.Remove(cpgData.Card);
                }
            }
            else if (cpgData.Type == EnGroupType.AnGang && mData.GangBao)
            {
                if (0 == mData.CurrOpChair)
                {
                    int temp = 0;
                    var tempList = cpgData.GetHardCards();
                    for (int i = 0; i < tempList.Count - 1; i++)
                    {
                        DataCenter.OneselfData.HardCards.Remove(tempList[i]);
                        temp++;
                    }
                }
            }
            else
            {
                //删除手牌数据
                if (0 == mData.CurrOpChair)
                {
                    var list = cpgData.GetHardCards();
                    for (int i = 0; i < list.Count; i++)
                    {
                        DataCenter.Players[mData.CurrOpChair].HardCards.Remove(list[i]);
                    }
                }
            }
            MahjongGroupsManager group = Game.MahjongGroups;
            //抓杠是特殊的 如果放回的消息ok 为false 证明正在确认是否有抢杠胡 为ture时表示 杠成功了
            if (cpgData.Type == EnGroupType.ZhuaGang)
            {
                CpgZhuaGang zhuanggang = (CpgZhuaGang)cpgData;
                if (!zhuanggang.Ok)
                {
                    group.PlayerToken = false;
                }
                else
                {
                    //如果是抓杠 移除手牌中抓的
                    group.MahjongHandWall[mData.CurrOpChair].RemoveMahjong(cpgData.Card);
                    //设置吃碰杠
                    group.MahjongCpgs[mData.CurrOpChair].SetCpg(cpgData);
                }
            }
            else
            {
                group.MahjongHandWall[mData.CurrOpChair].RemoveMahjong(cpgData.GetHardCards());
            }
            //如果是别人打出的牌
            if (cpgData.GetOutPutCard() != MiscUtility.DefValue)
            {
                group.MahjongThrow[mData.OldOpChair].PopMahjong(cpgData.Card);
                //隐藏箭头
                Game.TableManager.GetParts<MahjongOutCardFlag>(TablePartsType.OutCardFlag).Hide();
            }
            //排序麻将               
            if (mData.CurrOpChair == 0)
            {
                group.PlayerHand.SortHandMahjong();
            }
            group.MahjongCpgs[mData.CurrOpChair].SetCpg(cpgData);
            //如果吃碰杠之后 cpg 加 手牌数量 大于 手牌数量 需要打牌设置最后一张
            if (group.MahjongCpgs[mData.CurrOpChair].GetHardMjCnt() + group.MahjongHandWall[mData.CurrOpChair].MahjongList.Count > DataCenter.Config.HandCardCount)
            {
                group.MahjongHandWall[mData.CurrOpChair].SetLastCardPos(MiscUtility.DefValue);
            }
            //麻将记录
            RecordMahjong(cpgData);
        }

        private void PlayScoreEffect(ISFSObject data)
        {
            if (!data.ContainsKey(ProtocolKey.GangGold)) return;
            var scoresSfsObj = data.GetSFSObject(ProtocolKey.GangGold);
            var scoreList = new Dictionary<int, long>();
            var datas = scoresSfsObj.GetKeys();
            for (int i = 0; i < datas.Length; i++)
            {
                long score = scoresSfsObj.GetInt(datas[i]);
                int chair = MahjongUtility.GetChair(int.Parse(datas[i]));
                scoreList[chair] = score;
            }
            GameCenter.EventHandle.Dispatch((int)UIEventProtocol.PlayAddScore, new SetScoreArgs()
            {
                DelayTime = 1.5f,
                ScoreDic = scoreList,
                Type = (int)SetScoreType.AddScoreAndEffect,
            });
        }

        private void RecordMahjong(CpgData cpgData)
        {
            if (cpgData.Type == EnGroupType.AnGang && (cpgData.Seat != DataCenter.OneselfData.Seat))
            {
                //其他玩家暗杠不进行统计
                if (!DataCenter.Config.ShowAnGang) return;
            }
            if (DataCenter.CurrOpChair != 0 && null != cpgData)
            {
                GameCenter.Shortcuts.MahjongQuery.Do(p => p.AddRecordMahjongs(cpgData.GetCardDatas));
            }
        }

        private void PlayEffect(ISFSObject data)
        {
            var cpgData = mData.CpgData;
            if (mData.CpgData.Type == EnGroupType.ZhuaGang)
            {
                // 抢杠胡不播放特效
                var cpg = (CpgZhuaGang)cpgData;
                if (cpg.Ok == false) return;
            }

            PoolObjectType effect;
            switch (mData.CpgType)
            {
                case EnGroupType.Chi:
                    effect = PoolObjectType.chi;
                    break;
                case EnGroupType.Peng:
                    effect = PoolObjectType.peng;
                    break;
                default:
                    //播放特效 
                    if (DataCenter.Config.IsPlaySpecialEffects)
                        MahjongUtility.PlayEnvironmentEffect(mData.CurrOpChair, PoolObjectType.longjuanfeng);
                    effect = PoolObjectType.gang;
                    PlayScoreEffect(data);
                    break;
            }
            MahjongUtility.PlayOperateEffect(DataCenter.CurrOpChair, effect);
        }

        public struct CpgLogicData : IData
        {
            public EnGroupType CpgType;
            public CpgData CpgData;
            public int CurrOpChair;
            public int OldOpChair;
            public bool GangBao;

            public CpgModel CpgModel;

            public void SetData(ISFSObject data)
            {
                var db = GameCenter.DataCenter;
                db.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
                GangBao = data.ContainsKey("bao");
                CurrOpChair = db.CurrOpChair;
                OldOpChair = db.OldOpChair;
                CpgData = MahjongUtility.CreateCpg(data);
                CpgData.Laizi = db.Game.LaiziCard; //cpg中有赖子牌，标记Icon
                CpgType = CpgData.Type;
                db.Players[db.CurrOpChair].IsTuiDan = data.ContainsKey("tuidan");
                if (CpgType != EnGroupType.ZhuaGang && IsNotXjfdType(CpgType))
                {
                    //将cpg信息添加到玩家数据中
                    GameCenter.DataCenter.Players[CurrOpChair].CpgDatas.Add(CpgData);
                }

                CpgModel = new CpgModel(data);
            }

            public bool IsNotXjfdType(EnGroupType type)
            {
                switch (type)
                {
                    case EnGroupType.XiaoJi:
                    case EnGroupType.YaoDan:
                    case EnGroupType.JiuDan:
                    case EnGroupType.ZFBDan:
                    case EnGroupType.XFDan:
                        return false;
                }
                return true;
            }
        }
    }
}