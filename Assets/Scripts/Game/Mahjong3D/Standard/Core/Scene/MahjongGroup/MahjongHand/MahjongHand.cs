using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongHand : MahjongGroup
    {
        private IMahHand mMahHand;

        protected virtual void Start()
        {
            mMahHand = gameObject.GetComponent<IMahHandCtrl>().OnIni();
        }

        public override void OnReset()
        {
            base.OnReset();
            SetHandCardState(0);
            if (null != mMahHand) mMahHand.Reset();
        }

        public virtual T GetMahHandComponent<T>() where T : IMahHand
        {
            return gameObject.GetComponent<T>();
        }

        //显示的是 这个面向值
        protected virtual void SortMahjong()
        {
            mMahHand.SortMahjong();
        }

        /// <summary>
        /// 设置手牌状态
        /// </summary>   
        public virtual void SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            mMahHand.SetHandCardState(state, args);
        }

        protected override void AddMahjongToList(MahjongContainer item)
        {
            mMahHand.AddMahjong(item);
        }

        public virtual MahjongContainer GetMahjongItemByValue(int value)
        {
            return mMahHand.GetMahjongItemByValue(value);
        }

        protected virtual void SortMahjongForHand()
        {
            mMahHand.SortMahjongForHand();
        }

        public override MahjongContainer GetInMahjong(int value)
        {
            MahjongContainer LastGetIn = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(LastGetIn);
            if (MahjongUtility.MahjongFlagCheck(value))
            {
                LastGetIn.Laizi = true;
            }
            //位子放到最后
            LastGetIn.transform.localPosition = GetHardLastMjPos();
            //转动方向
            LastGetIn.RotaTo(new Vector3(-90, 0, 0), Vector3.zero, GameCenter.DataCenter.Config.TimeGetInCardRote, GameCenter.DataCenter.Config.TimeGetInCardWait);
            return LastGetIn;
        }

        public MahjongContainer GetInMahjongNoAni(int value)
        {
            MahjongContainer LastGetIn = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(LastGetIn);
            if (MahjongUtility.MahjongFlagCheck(value))
            {
                LastGetIn.Laizi = true;
            }
            //位子放到最后
            LastGetIn.transform.localPosition = GetHardLastMjPos();
            return LastGetIn;
        }

        public override List<MahjongContainer> GetInMahjong(IList<int> value)
        {
            List<MahjongContainer> list = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(list);
            SortMahjong();
            SetMahjongPos();
            return list;
        }

        public virtual void SetLastCardPos(int value)
        {
            if (mMahjongList.Count > 1)
            {
                MahjongContainer lastMj = mMahjongList[mMahjongList.Count - 1];
                lastMj.transform.localPosition = GetHardLastMjPos();
            }
        }

        public virtual void OnSendMahjong(IList<int> Value, float time, float wait)
        {
            MahjongContainer item;
            List<MahjongContainer> list = GameCenter.Scene.MahjongCtrl.PopMahjong(Value);
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                AddMahjongToList(item);
                item.RotaTo(new Vector3(-90, 0, 0), Vector3.zero, time, wait);
            }
            SetMahjongPos();
        }

        public virtual void OnSendOverSortMahjong(float time, float wait)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].RotaTo(new Vector3(-90, 0, 0), time);
            }
            GameCenter.Shortcuts.DelayTimer.StartTimeLoop(wait, () =>
            {
                SortMahjongForHand();
                SetMahjongPos();
                for (int i = 0; i < mMahjongList.Count; i++)
                {
                    mMahjongList[i].RotaTo(new Vector3(0, 0, 0), time);
                }
            });
        }

        public virtual void SortHandMahjong()
        {
            SortMahjong();
            SetMahjongPos();
        }

        public Vector3 GetHardLastMjPos()
        {
            int count = mMahjongList.Count;
            if (count < 1) return Vector3.one;
            Vector3 pos = mMahjongList[(count - 2) % count].transform.localPosition;
            return new Vector3(MiscUtility.MahjongSize.x * 1.2f + pos.x, 0.2f, pos.z);
        }

        public virtual MahjongContainer ThrowOut(int value)
        {
            MahjongContainer Mahjong = GetMahjongItemByValue(value);
            if (Mahjong == null) return null;
            Mahjong.ShowNormal();
            mMahjongList.Remove(Mahjong);
            SortMahjongForHand();
            SetMahjongPos();
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(Mahjong);
            return Mahjong;
        }

        protected override Vector3 GetPos(MahjongVecter index)
        {
            if ((Chair == 0 || Chair == 2) && !GameCenter.DataCenter.Config.SortByCenter)
            {
                Vector3 mahjongSize = MiscUtility.MahjongSize;
                float Dis = -mMahjongList.Count * mahjongSize.x / 2 - mahjongSize.x * 1.2f / 2;
                Vector3 pos = new Vector3(Dis + mahjongSize.x * (index.x + 0.5f), mahjongSize.y * 0.5f, mahjongSize.z * 0.5f);
                pos.x = (pos.x - (RowCnt - mMahjongList.Count) / 2 * mahjongSize.x / 2);
                return pos;
            }
            else
            {
                Vector3 mahjongSize = MiscUtility.MahjongSize;
                float Dis = -mMahjongList.Count * mahjongSize.x / 2 - mahjongSize.x * 1.2f / 2;
                return new Vector3(Dis + mahjongSize.x * (index.x + 0.5f), mahjongSize.y * 0.5f, mahjongSize.z * 0.5f);
            }
        }

        public void RemoveMahjong(IList<int> value, bool sort = true)
        {
            for (int i = 0; i < value.Count; i++)
            {
                MahjongContainer item = GetMahjongItemByValue(value[i]);
                if (item != null)
                {
                    mMahjongList.Remove(item);
                    GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
                }
            }
            if (sort)
            {
                SetMahjongPos();
            }
        }

        public void RemoveMahjong(int value, bool sort = true)
        {
            MahjongContainer item = GetMahjongItemByValue(value);
            if (item != null)
            {
                mMahjongList.Remove(item);
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
            }
            if (sort)
            {
                SetMahjongPos();
            }
        }

        public void RemoveAllMj()
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(mMahjongList[i]);
            }
            mMahjongList.Clear();
        }

        public virtual void GameResultRota(float time)
        {
            SetMahjongPos();
            MahjongContainer item;
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                item = mMahjongList[i];
                item.ResetPos();
                item.ShowNormal();
                item.RotaTo(new Vector3(90, 0, 0), time);
            }
        }

        public virtual void SetLaizi(int laizi, bool sort = true)
        {
            if (laizi != MiscUtility.DefValue)
            {
                MahjongContainer item;
                for (int i = 0; i < mMahjongList.Count; i++)
                {
                    item = mMahjongList[i];
                    item.Laizi = MahjongUtility.MahjongFlagCheck(item.Value);
                }
            }
            if (sort)
            {
                SortMahjong();
                SetMahjongPos();
            }
        }

        public virtual MahjongContainer SetTingPaiNeedOutCard()
        {
            var lastMj = GetLastMj();
            lastMj.SetMjRota(0, 0, 0);
            return lastMj;
        }

        public virtual MahjongContainer PopMahjong()
        {
            MahjongContainer item = mMahjongList[mMahjongList.Count - 1];
            mMahjongList.RemoveAt(mMahjongList.Count - 1);
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
            return item;
        }

        public List<MahjongContainer> GetInMahjongWithRoat(int[] value)
        {
            List<MahjongContainer> list = GameCenter.Scene.MahjongCtrl.PopMahjong(value);

            if (MahjongList.Count != 0)
            {
                Vector3 pos = MahjongList[MahjongList.Count - 1].transform.localPosition;

                AddMahjongToList(list);
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    pos += new Vector3(MiscUtility.MahjongSize.x, 0, 0);
                    item.transform.localPosition = pos;
                    item.RotaTo(new Vector3(-90, 0, 0), Vector3.zero, 0.15f, 0.15f);
                }
                GameCenter.Shortcuts.DelayTimer.StartTimeLoop(0.3f, () =>
                {
                    SortMahjongForHand();
                    SetMahjongPos();
                });
            }
            else
            {
                AddMahjongToList(list);
                SortMahjongForHand();
                SetMahjongPos();
                GameCenter.Shortcuts.DelayTimer.StartTimeLoop(0.3f, () => { });
            }
            return list;
        }
    }
}