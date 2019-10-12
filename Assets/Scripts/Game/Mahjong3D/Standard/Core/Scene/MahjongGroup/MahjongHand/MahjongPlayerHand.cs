using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class MahjongPlayerHand : MahjongHand
    {
        public static int PlayerHardLayer = -1;
        public MahjongContainer ChooseMj;

        private IMahPlayerHand mPlayerHand;
        private bool mHasToken;

        public bool HasToken
        {
            get { return mHasToken; }
            set { mHasToken = value; }
        }

        public void SetMahjongNormalState(MahjongContainer item)
        {
            mPlayerHand.SetMahjongNormalState(item);
        }

        protected override void Start()
        {
            PlayerHardLayer = gameObject.layer;
            mPlayerHand = gameObject.GetComponent<IMahPlayerHandCtrl>().OnIni();
        }

        public override void SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            mPlayerHand.SetHandCardState(state, args);
        }

        public override void OnReset()
        {
            base.OnReset();
            if (null != mPlayerHand) mPlayerHand.Reset();
        }

        public override MahjongContainer GetInMahjong(int value)
        {
            var item = base.GetInMahjong(value);
            mPlayerHand.GetInMahjong(item);
            return item;
        }

        /// <summary>
        /// 手牌恢复未抬起状态
        /// </summary>
        public void HandCardsResetPos()
        {
            MahjongContainer item;
            UserContorl.ClearSelectCard();
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                item = mMahjongList[i];
                item.ResetPos();
            }
        }

        protected override void SortMahjong()
        {
            mPlayerHand.SortMahjong();
        }

        public override void SortHandMahjong()
        {
            base.SortHandMahjong();
            UserContorl.ClearSelectCard();
        }

        protected override void AddMahjongToList(MahjongContainer item)
        {
            mPlayerHand.AddMahjong(item);
        }

        public override MahjongContainer SetTingPaiNeedOutCard()
        {
            return mPlayerHand.SetTingPaiNeedOutCard();
        }

        public override MahjongContainer GetMahjongItemByValue(int value)
        {
            for (int i = mMahjongList.Count - 1; i >= 0; i--)
            {
                if (mMahjongList[i].Value == value)
                {
                    return mMahjongList[i];
                }
            }
            return null;
        }

        public override void GameResultRota(float time)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].ChangeToHardLayer(false);
                mMahjongList[i].RemoveMahjongScript();
            }
            base.GameResultRota(time);
        }

        public override void SetLastCardPos(int value)
        {
            if (value == MiscUtility.DefInt || mMahjongList.Find(item => item.Value == value) == null)//当前不是抓牌 是吃碰杠后的
            {
                base.SetLastCardPos(value);
                return;
            }
            if (mMahjongList.Count > 1)
            {
                MahjongContainer findItem = mMahjongList.Find((item) =>
                {
                    return item.Value == value;
                });

                if (findItem != null)
                {
                    mMahjongList.Remove(findItem);
                }               
                SetMahjongPos();

                mMahjongList.Add(findItem);
                findItem.transform.localPosition = GetHardLastMjPos();               
            }
        }

        public void SetLastCardPos(MahjongContainer item)
        {
            item.transform.localPosition = GetHardLastMjPos();
        }

        public override MahjongContainer ThrowOut(int value)
        {
            MahjongContainer Mahjong = base.ThrowOut(value);
            if (Mahjong != null)
            {
                Mahjong.ChangeToHardLayer(false);
            }
            return Mahjong;
        }

        /// <summary>
        /// 查询胡牌，显示听牌标记
        /// </summary>
        public void OnQueryMahjong(IList<int> cards)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].IsTingCard = false;
                if (null != cards)
                {
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (cards[j] == mMahjongList[i].Value)
                        {
                            mMahjongList[i].IsTingCard = true;
                        }
                    }
                }
            }
        }

        public override void OnSendOverSortMahjong(float time, float wait)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].RotaTo(new Vector3(-90, 0, 0), time);
            }
            GameCenter.Shortcuts.DelayTimer.StartTimeLoop(wait, () =>
            {
                SetLaizi(GameCenter.DataCenter.Game.LaiziCard);
                for (int i = 0; i < mMahjongList.Count; i++)
                {
                    mMahjongList[i].RotaTo(new Vector3(0, 0, 0), time);
                }
            });
        }

        protected override void SortMahjongForHand()
        {
            SortMahjong();
        }

        public void RemoveMahjong(MahjongContainer card)
        {
            mMahjongList.Remove(card);
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(card);
        }

        //当抢杠胡
        public virtual void OnQiangganghu(int value)
        {
            MahjongContainer findItem = mMahjongList.Find((item) =>
            {
                return item.Value == value;
            });
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(findItem);
            mMahjongList.Remove(findItem);
            SortHandMahjong();
        }
    }
}