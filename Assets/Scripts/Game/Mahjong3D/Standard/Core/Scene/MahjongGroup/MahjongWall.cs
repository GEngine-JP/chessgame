﻿using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongWall : MahjongGroup
    {
        public int StartIndex;

        /// <summary>
        /// 牌墙初始位置
        /// </summary>
        public Vector3 EndPos;
        /// <summary>
        /// 动画偏移位置
        /// </summary>
        public Vector3 SidewayPos;
        /// <summary>
        /// 牌墙初始位置
        /// </summary>
        public Vector3 StartPos;

        /// <summary>
        /// 牌墙上移动画
        /// </summary>
        public Tween WallMoveUpTweener(float time)
        {
            return transform.DOLocalMove(EndPos, time);
        }

        /// <summary>
        /// 牌墙横移动画
        /// </summary>
        public Tween WallSideswayTweener(float time)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }

            return transform.DOLocalMove(SidewayPos, time);
        }

        /// <summary>
        /// 设置牌墙初始位置
        /// </summary>
        public void ResetWallStartPos()
        {
            if (GameCenter.DataCenter.Config.MahStartAnimation)
            {
                transform.localPosition = StartPos;
            }
            else
            {
                transform.localPosition = EndPos;
            }
            if (!GameCenter.GameProcess.IsCurrState<StateGameReady>())
            {
                transform.localPosition = EndPos;
            }
            if (GameCenter.DataCenter.Config.NoShowMahjongWall)
            {
                transform.localPosition = StartPos;
            }
        }

        public void AddMahjongToWall()
        {
            var list = GameCenter.Scene.MahjongCtrl.PopMahjongByNum(MahjongCnt);
            AddMahjongToList(list);
            SetMahjongPos();
        }

        public void PopFanbaoMahjong(MahjongContainer container)
        {
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(container);
            mMahjongList.Remove(container);
        }

        public MahjongContainer PopMahjong()
        {
            MahjongContainer item = mMahjongList[StartIndex];
            if (null != item)
            {
                mMahjongList.RemoveAt(StartIndex);
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
                return item;
            }
            return null;
        }

        public override void OnReset()
        {
            base.OnReset();
            StartIndex = 0;
            ResetWallStartPos();
            MahjongList.Clear();
        }

        /// <summary>
        /// 设置抓牌起始点
        /// </summary>
        public void SeStartIndext(int value)
        {
            StartIndex = value;
        }

        public void SetRowCnt(int cnt)
        {
            RowCnt = cnt;
            MahjongCnt = RowCnt * 2;
        }

        protected override Vector3 GetPos(MahjongVecter index)
        {
            Vector3 mahjongSize = MiscUtility.MahjongSize;
            float dis = RowCnt * mahjongSize.x / 2;
            if (index.x % 2 == 0)
            {
                index.y = 1;
            }
            else
            {
                index.y = 0;
            }
            return new Vector3(dis - mahjongSize.x * (index.x / 2 + 0.5f), mahjongSize.y * (0.5f), mahjongSize.z * (index.y + 0.5f));
        }

        protected override MahjongVecter GetNextIndex(MahjongVecter mjIndex)
        {
            MahjongVecter next = new MahjongVecter(mjIndex);
            next.x++;
            return next;
        }
    }
}