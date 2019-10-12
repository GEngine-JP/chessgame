using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_GetCard : AbsGameLogicBase
    {
        private GetCardData mData;
        private GameLogic_Buzhang mBuzhangLogic;
        /// <summary>
        /// 抓牌补张
        /// </summary>
        private ContinueTaskContainer mGetCardBuzhangTask;

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeGetInCard)]
        public void OnGetCard(ISFSObject data)
        {
            DataCenter.LeaveMahjongCnt--;
            mData.SetData(data);

            if (DataCenter.Config.Buzhang)
            {
                //补张
                if (mBuzhangLogic.ExIsNullOjbect())
                {
                    mBuzhangLogic = GameCenter.Network.GetGameResponseLogic<GameLogic_Buzhang>();
                }
                if (mBuzhangLogic.BuzhangQueue.Count > 0)
                {
                    var delayTimer = DataCenter.Config.TimeBuzhangMessageDelay;
                    GameCenter.Network.SetDelayTime(mBuzhangLogic.BuzhangQueue.Count * delayTimer);
                    if (mGetCardBuzhangTask.ExIsNullOjbect())
                    {
                        mGetCardBuzhangTask = ContinueTaskManager.NewTask().AppendFuncTask(GetCardBuZhangTask);
                    }
                    mGetCardBuzhangTask.Start();
                }
                else
                {
                    OnGetCard();
                }
            }
            else
            {
                OnGetCard();
            }

            //手牌检查
            CheckHandCards();
        }

        public void CheckHandCards()
        {
            var db = GameCenter.DataCenter;
            if (!db.Config.CheckHandCard && db.CurrOpChair != 0) return;

            var group = GameCenter.Scene.MahjongGroups;
            int cardCount = 14;
            int handCards = group.PlayerHand.MahjongList.Count;

            var list = group.MahjongCpgs[0].CpgList;
            for (int i = 0; i < list.Count; i++)
            {
                int num = list[i].Data.GetAllCardDatas.Count;
                if (num == 4)
                {
                    cardCount++;
                }
                handCards += num;
            }
            if (handCards > cardCount)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("手牌超出正常数! -> ");

                var array = db.Players[0].HardCards;
                for (int i = 0; i < array.Count; i++)
                {
                    builder.AppendFormat("{0} ", array[i]);
                }
                builder.Append(" | ");
                for (int i = 0; i < group.PlayerHand.MahjongList.Count; i++)
                {
                    builder.AppendFormat("{0} ", group.PlayerHand[i].Value);
                }

                com.yxixia.utile.YxDebug.YxDebug.LogError(builder.ToString());
                //重连请求
                GameCenter.Network.SendReJoinGame();
            }
        }

        public IEnumerator<float> GetCardBuZhangTask()
        {
            while (mBuzhangLogic.BuzhangQueue.Count > 0)
            {
                int card = mBuzhangLogic.BuzhangQueue.Dequeue();   
                //在墙中移除
                DataCenter.LeaveMahjongCnt--;
                Game.MahjongGroups.PopMahFromCurrWall(1);
                //添加到手牌中
                int chair = mBuzhangLogic.BuZhangChair;
                Game.MahjongGroups.MahjongHandWall[chair].GetInMahjong(chair == 0 ? card : 0);
                yield return DataCenter.Config.TimeBuzhangAniDelay;
                //移除手牌中的
                Game.MahjongGroups.MahjongHandWall[chair].RemoveMahjong(card);
                //添加到胡牌中
                Game.MahjongGroups.MahjongOther[chair].GetInMahjong(card);
                MahjongUtility.PlayPlayerSound(DataCenter.CurrOpChair, "buhua");
                yield return DataCenter.Config.TimeBuzhangAniDelay;
            }
            OnGetCard();
            GameCenter.Shortcuts.SwitchCombination.Close((int)GameSwitchType.HasBuzhang);
            if (GameCenter.Shortcuts.SwitchCombination.IsOpen((int)GameSwitchType.AiAgency))
            {
                yield return Config.TimeTingPutCardWait;
                GameCenter.Network.C2S.ThrowoutCard(DataCenter.GetInMahjong);
            }
            if (DataCenter.CurrOpChair == 0)
            {
                //重新展示听提示
                var tingList = DataCenter.Players[0].TingList;
                GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(tingList);
            }
        }

        private void OnGetCard()
        {
            //第一次抓牌 隐藏塞子
            if (DataCenter.Game.FirstGetCard)
            {
                DataCenter.Game.FirstGetCard = false;
                Game.TableManager.StartTimer(Config.TimeOutcard);
                Game.TableManager.SwitchDirection(DataCenter.CurrOpSeat);
            }
            Game.MahjongGroups.PopMahFromCurrWall();
            Game.MahjongGroups.MahjongHandWall[DataCenter.CurrOpChair].GetInMahjong(mData.GetCard);
            MahjongUtility.PlayEnvironmentSound("zhuapai");
        }
    }

    /// <summary>
    /// 抓牌数据
    /// </summary>
    public struct GetCardData : IData
    {
        public int PlayerSeat;
        public int GetCard;

        public void SetData(ISFSObject data)
        {
            var db = GameCenter.DataCenter;
            PlayerSeat = data.TryGetInt(RequestKey.KeySeat);
            GetCard = data.TryGetInt(RequestKey.KeyOpCard);
            db.CurrOpSeat = PlayerSeat;
            db.GetInMahjong = GetCard;
            db.Game.HuangZhuang = data.TryGetBool(ProtocolKey.KeyHuangZhuang);
            db.Players[db.CurrOpChair].IsTuiDan = data.ContainsKey("tuidan");

            if (data.TryGetBool(ProtocolKey.KeyHuangZhuang))
            {
                GameCenter.EventHandle.Dispatch((int)UIEventProtocol.HuangzhuangTip);
            }

            if (db.CurrOpChair == 0)
            {
                int opMenu = data.TryGetInt(ProtocolKey.KeyOp);
                if (GameUtils.BinaryCheck(OperateKey.OpreateGang, opMenu))
                {
                    //根据server数据 指定杠牌
                    var array = data.TryGetIntArray("gangcard");
                    if (array != null)
                    {
                        db.GangCard.AddRange(array);
                    }
                }
            }
        }
    }
}