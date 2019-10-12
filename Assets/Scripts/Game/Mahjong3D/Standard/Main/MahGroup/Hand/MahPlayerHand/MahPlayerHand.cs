using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahPlayerHand : MahHand, IMahPlayerHand
    {
        protected MahjongPlayerHand mPlayerHand;

        protected System.Func<MahjongContainer, bool> mPutOutFunc;

        public MahjongPlayerHand PlayerHand
        {
            get
            {
                if (null == mPlayerHand) { mPlayerHand = gameObject.GetComponent<MahjongPlayerHand>(); }
                return mPlayerHand;
            }
        }

        public override bool SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            if (CurrState == state) return false;
            mCurrState = state;
            switch (mCurrState)
            {
                case HandcardStateTyps.SingleHu:
                case HandcardStateTyps.Ting: SwitchFreezeState(); break;
                case HandcardStateTyps.Normal: SwitchNormalState(); break;
                case HandcardStateTyps.ChooseTingCard: SwitchChooseTingState(args); break;
                case HandcardStateTyps.TingAndShowCard: SwitchTingAndShowCardsState(args); break;
            }
            return true;
        }

        protected virtual void SwitchNormalState()
        {
            MahjongContainer item = null;
            var list = PlayerHand.MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.Lock = false;
                item.SetMahjongScript();
                item.SetAllowOffsetStatus(true);
                item.SetThowOutCall(ThrowCardClickEvent);
                item.ResetPos();
            }
            UserContorl.ClearSelectCard();
        }

        public virtual void SetMahjongNormalState(MahjongContainer item)
        {
            item.Lock = false;
            item.SetMahjongScript();
            item.SetThowOutCall(ThrowCardClickEvent);
            item.ResetPos();
        }

        /// <summary>
        /// 冻结手牌，手牌不能进行何操作
        /// </summary>
        public void SwitchFreezeState()
        {
            MahjongContainer item;
            var list = PlayerHand.MahjongList;
            for (int i = 0; i < list.Count; i++)
            {                
                item = list[i];
                item.Lock = true;
                item.ResetPos();
                item.RemoveMahjongScript();
            }
            UserContorl.ClearSelectCard();
        }

        protected virtual void SwitchChooseTingState(params object[] args)
        {
            MahjongContainer item;
            List<int> tingList = args[0] as List<int>;
            if (tingList == null || tingList.Count == 0) return;
            var list = PlayerHand.MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.ResetPos();
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;
                    item.RemoveMahjongScript();
                }
                else
                {
                    item.SetMahjongScript();
                    item.SetThowOutCall(TingpaiClickEvent);
                }
            }
            UserContorl.ClearSelectCard();
        }

        /// <summary>
        /// 出牌事件
        /// </summary>     
        protected void ThrowCardClickEvent(Transform mahjong)
        {
            //如果允许当前用户出牌
            if (PlayerHand.HasToken)
            {
                var temp = mahjong.GetComponent<MahjongContainer>();
                PlayerHand.ChooseMj = temp;
                //花牌不允许打出
                if (temp.MahjongCard.Value >= (int)MahjongValue.ChunF)
                {
                    return;
                }
                if (temp.Lock)
                {
                    return;
                }
                if (null != mPutOutFunc && mPutOutFunc(temp))
                {
                    return;
                }
                bool flag = MahjongUtility.MahjongFlagCheck(temp.Value);
                //赖子牌是否允许打出
                if (flag && !GameCenter.DataCenter.Config.AllowLaiziPut)
                {
                    return;
                }
                PlayerHand.HasToken = false;
                //通知网络 发送出牌消息
                GameCenter.Network.C2S.ThrowoutCard(temp.Value);
            }
        }

        /// <summary>
        /// 听牌点击事件
        /// </summary>      
        private void TingpaiClickEvent(Transform transf)
        {
            if (PlayerHand.HasToken)
            {
                MahjongContainer item;
                var Mj = transf.GetComponent<MahjongContainer>();
                if (!Mj.Laizi && !Mj.Lock)
                {
                    PlayerHand.HasToken = false;
                    GameCenter.Network.C2S.Custom<C2SCustom>().ThrowoutCardOnTing(Mj.Value);
                    Mj.ResetPos();
                    var list = PlayerHand.MahjongList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        item = list[i];
                        item.SetMahjongScript();
                        item.SetThowOutCall(ThrowCardClickEvent);
                    }
                }
            }
        }

        public override void AddMahjong(MahjongContainer item)
        {
            base.AddMahjong(item);
            item.ChangeToHardLayer(true);
            item.SetMahjongScript();
            item.SetThowOutCall(ThrowCardClickEvent);           
        }

        public virtual MahjongContainer SetTingPaiNeedOutCard()
        {
            var lastMj = PlayerHand.GetLastMj();
            lastMj.Lock = false;
            lastMj.SetMahjongScript();
            lastMj.SetThowOutCall(ThrowCardClickEvent);
            return lastMj;
        }

        public override void SortMahjongForHand() { }

        public virtual void GetInMahjong(MahjongContainer item) { }

        protected override void SwitchTingAndShowCardsState(params object[] args)
        {
            int[] tingList = args[0] as int[];
            if (tingList == null || tingList.Length == 0) return;

            System.Array.Sort(tingList);
            var queue = new Queue<int>(tingList);
            int niuValue = queue.Dequeue();
            MahjongContainer item;
            var list = PlayerHand.MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.Lock = true;
                if (item.Value == niuValue)
                {
                    item.Lock = false;
                    if (queue.Count > 0)
                    {
                        niuValue = queue.Dequeue();
                    }
                    else
                    {
                        niuValue = 0;
                    }
                }
                item.RemoveMahjongScript();
            }
            SortMahjong();
            PlayerHand.SetMahjongPos();
        }
    }
}