using System.Collections.Generic;
using Sfs2X.Entities.Data;
using DG.Tweening;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [S2CResponseLogic]
    public partial class GameLogic_SendCard : AbsGameLogicBase
    {
        private SendCardData mData;
        private ContinueTaskContainer mSendTask;

        private float mSingleMotionTime = 0.3f;
        private float mSingleWaitTime = 0.2f;

        private bool SkipSend()
        {
            if (DataCenter.IsReconect)
            {
                if (Game.MahjongGroups.PlayerHand.MahjongList.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeAlloCate)]
        public void OnSendCard(ISFSObject data)
        {
            //重连数据在发牌数据之前时，会造成手牌多牌bug
            if (SkipSend()) return;

            mData.SetData(data);
            StartSendCard();
            float delayTime = Config.MahStartAnimation ? 4.2f : 1;
            GameCenter.Network.SetDelayTime(delayTime);
            //手牌记录
            DataCenter.Players.AddRecordMahjongs();
        }

        [S2CResponseHandler(NetworkProtocol.MJRequestTypeAlloCate, GameKey = GameMisc.XlmjKey)]
        [S2CResponseHandler(NetworkProtocol.MJRequestTypeAlloCate, GameKey = GameMisc.XzmjKey)]
        public void OnSendCard_Xzmj(ISFSObject data)
        {
            //重连数据在发牌数据之前时，会造成手牌多牌bug
            if (SkipSend()) return;

            mData.SetData(data);
            StartSendCard();
            float delayTime = Config.MahStartAnimation ? 4.2f : 1;
            GameCenter.Network.SetDelayTime(delayTime);
        }

        /// <summary>
        /// 开始发牌
        /// </summary>
        private void StartSendCard()
        {
            if (mSendTask == null)
            {
                mSendTask = ContinueTaskManager.NewTask()
                .AppendTaskFromFunc(Config.MahStartAnimation, (b) =>
                {
                    if (b) { return TableAnimation; }
                    return null;
                })
                .AppendTaskFromFunc(Config.MahStartAnimation, (b) =>
                {
                    return b ? (Func<IEnumerator<float>>)SendCardAnimation : SendCardNoAnimation;
                })
                .SetTaskExecuteType(TaskExecuteType.AsynLine)
                .SetContainerExeType(ContainerExeType.Single)
                .CallBack(TidyMahjongCards);
            }
            mSendTask.Start();
            //重置娱乐房切换房间标志位
            GameCenter.DataCenter.Room.YuLeBoutState = false;
        }

        /// <summary>
        /// 模拟麻将机动画
        /// </summary>
        /// <returns></returns>
        public IEnumerator<float> TableAnimation()
        {
            if (SkipSend())
            {
                yield return ContinueTaskAgent.Shutdown;
            }

            var group = Game.MahjongGroups;
            var table = Game.TableManager.GetParts<MahjongTable>(TablePartsType.Table);
            var miscSequence = DOTween.Sequence();
            //插入麻将机板下移动画
            miscSequence.Insert(0, table.TableDownAnimation(mSingleMotionTime));
            //插入麻将滑动显示动画
            for (int i = 0; i < group.MahjongWall.Length; i++)
            {
                miscSequence.Insert(mSingleMotionTime + mSingleWaitTime, group.MahjongWall[i].WallSideswayTweener(mSingleMotionTime));
            }
            //插入麻将机板上升
            var wait = 2 * (mSingleMotionTime + mSingleWaitTime);
            miscSequence.Insert(wait, table.TableUpAnimation(mSingleMotionTime));
            //插入麻将上升动画
            for (int i = 0; i < group.MahjongWall.Length; i++)
            {
                miscSequence.Insert(wait, group.MahjongWall[i].WallMoveUpTweener(mSingleMotionTime));
            }
            miscSequence.Play();
            yield return wait + mSingleMotionTime * 2;
        }

        /// <summary>
        /// 不需要发牌动画时 直接设置手牌
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> SendCardNoAnimation()
        {
            yield return 0;
            int sendCardCount = DataCenter.Config.HandCardCount * DataCenter.MaxPlayerCount;
            Game.MahjongGroups.PopMahFromCurrWall(sendCardCount);
            DataCenter.LeaveMahjongCnt -= sendCardCount;
            MahjongUtility.PlayEnvironmentSound("fapai_one");
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                if (DataCenter.Players[i].HardCards.Count == 0)
                {
                    Game.MahjongGroups.MahjongHandWall[i].GetInMahjong(new int[Config.HandCardCount]);
                }
                else
                {
                    Game.MahjongGroups.MahjongHandWall[i].GetInMahjong(DataCenter.Players[i].HardCards);
                }
            }
        }

        public virtual IEnumerator<float> SendCardAnimation()
        {
            //发牌动画时重连游戏，会造成手牌多牌bug
            if (SkipSend())
            {
                yield return ContinueTaskAgent.Shutdown;
            }

            int getInCnt = 4;
            int meOffset = 0;
            int startChair = DataCenter.CurrOpChair;
            int sendCardCount = DataCenter.Config.HandCardCount * DataCenter.MaxPlayerCount;
            MahjongUtility.PlayEnvironmentSound("fapai");
            while (sendCardCount > 0)
            {
                int[] card;
                if (startChair == 0)
                {
                    //根据上一局发牌数，来确定这一轮发牌数
                    int rate = sendCardCount - DataCenter.MaxPlayerCount * getInCnt;
                    if (rate < 1)
                    {
                        //不足时，重新确定发牌数
                        getInCnt = sendCardCount / DataCenter.MaxPlayerCount;
                    }
                    card = new int[getInCnt];
                    for (int i = 0; i < card.Length; i++)
                    {
                        card[i] = DataCenter.OneselfData.HardCards[meOffset + i];
                    }
                    meOffset += card.Length;
                }
                else
                {
                    card = new int[getInCnt];
                }
                Game.MahjongGroups.PopMahFromCurrWall(getInCnt);
                Game.MahjongGroups.MahjongHandWall[startChair].OnSendMahjong(card, Config.TimeSendCardUp, Config.TimeSendCardUpWait);
                yield return Config.TimeSendCardInterval;
                startChair = (startChair + 1) % GameCenter.DataCenter.MaxPlayerCount;
                DataCenter.LeaveMahjongCnt -= getInCnt;
                sendCardCount -= getInCnt;
            }
            yield return Config.TimeSendCardUp + Config.TimeSendCardUpWait;
        }

        private void TidyMahjongCards()
        {
            if (SkipSend()) return;

            //设置翻牌
            if (mData.Fanpai > 0)
            {
                Game.TableManager.SetShowMahjong(mData.Fanpai);
            }

            var group = Game.MahjongGroups;
            //扣下牌-排序-设置赖子-抬起
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                Game.MahjongGroups.MahjongHandWall[i].OnSendOverSortMahjong(Config.TimeSendCardUp, Config.TimeSendCardUp);
            }
            //允许开启托管
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
        }
    }
}