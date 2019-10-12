using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahHand : MonoBehaviour, IMahHand
    {
        /// <summary>
        /// 显示牌数量
        /// </summary>
        protected int mTingAndShowCardsNum;

        protected HandcardStateTyps mCurrState = HandcardStateTyps.None;

        private MahjongHand mMahjongHand;
        public MahjongHand MahjongHand
        {
            get
            {
                if (null == mMahjongHand) { mMahjongHand = GetComponent<MahjongHand>(); }
                return mMahjongHand;
            }
        }

        public List<MahjongContainer> MahjongList { get { return MahjongHand.MahjongList; } }

        public HandcardStateTyps CurrState
        {
            get { return mCurrState; }
            protected set { mCurrState = value; }
        }

        public virtual bool SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            if (CurrState == state) return false;
            mCurrState = state;
            switch (mCurrState)
            {
                case HandcardStateTyps.Daigu:
                case HandcardStateTyps.SingleHu:
                case HandcardStateTyps.Ting: OnBuckleCard(); break;
                case HandcardStateTyps.TingAndShowCard: SwitchTingAndShowCardsState(args); break;
                case HandcardStateTyps.Normal: break;
            }
            return true;
        }

        public virtual void Reset()
        {
            mTingAndShowCardsNum = 0;
        }

        /// <summary>
        /// 扣牌
        /// </summary>
        public void OnBuckleCard()
        {
            MahjongHand.SetMahjongPos();
            var list = MahjongHand.MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].RotaTo(new Vector3(-90, 0, 0), GameCenter.DataCenter.Config.TimeGetInCardRote);
            }
        }

        public virtual MahjongContainer GetMahjongItemByValue(int value)
        {
            var list = MahjongHand.MahjongList;
            if (list.Count == 0) return null;
            //if (CurrState == HandcardStateTyps.Ting || CurrState == HandcardStateTyps.TingAndShowCard)
            //{
            //    return list[list.Count - 1];
            //}
            //int index = Random.Range(0, list.Count);
            //return list[index];
            return list[list.Count - 1];
        }

        public virtual void AddMahjong(MahjongContainer item)
        {
            if (item == null) return;
            MahjongHand.MahjongList.Add(item);
            item.transform.ExSetParent(transform);
        }

        public virtual void AddMahjong(int value)
        {

        }

        public virtual void SortMahjong()
        {
            var list = MahjongHand.MahjongList;
            list.Sort((a, b) =>
            {
                if (CurrState == HandcardStateTyps.TingAndShowCard)
                {
                    if (a.Lock && !b.Lock) return 1;
                    if (!a.Lock && b.Lock) return -1;
                }
                if (a.Laizi && !b.Laizi) return -1;
                if (!a.Laizi && b.Laizi) return 1;
                if (a.Value < b.Value) return -1;
                if (a.Value > b.Value) return 1;
                if (a.Value == b.Value)
                {
                    if (a.MahjongIndex > b.MahjongIndex) return -1;
                    if (a.MahjongIndex < b.MahjongIndex) return 1;
                }
                return 0;
            });
        }

        /// <summary>
        /// 听之后显示几张手牌
        /// </summary>
        /// <param name="args"></param>
        protected virtual void SwitchTingAndShowCardsState(params object[] args)
        {
            MahjongContainer item;
            int[] tingList = args[0] as int[];
            var list = MahjongHand.MahjongList;
            if (tingList == null || tingList.Length == 0) return;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                if (i < tingList.Length)
                {
                    item.Value = tingList[i];
                    item.Lock = false;
                }
                else
                {
                    item.Lock = true;

                }
                item.ShowNormal();
                item.ResetPos();
            }
            ShowCards(tingList.Length);
            mTingAndShowCardsNum = tingList.Length;
        }

        public virtual void ShowCards(int showNum)
        {
            var list = MahjongHand.MahjongList;
            MahjongHand.SortHandMahjong();
            for (int i = 0; i < list.Count; i++)
            {
                int zhengFu = -1;
                if (i < showNum)
                {
                    zhengFu = 1;
                }
                list[i].RotaTo(new Vector3(90 * zhengFu, 0, 0), GameCenter.DataCenter.Config.TimeGetInCardRote);
            }
        }

        public virtual void SortMahjongForHand()
        {
            var list = MahjongHand.MahjongList;
            if (list.Count <= 2)
            {
                return;
            }
            var index = 0;
            if (CurrState == HandcardStateTyps.TingAndShowCard && list.Count > mTingAndShowCardsNum)
            {
                index = Random.Range(mTingAndShowCardsNum, list.Count - 2);
            }
            else
            {
                index = Random.Range(0, list.Count - 2);
            }
            var last = list[list.Count - 1];
            list.Remove(last);
            list.Insert(index, last);
        }
    }
}