﻿using DG.Tweening;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongSwitchGorup : MonoBehaviour, IGameReadyCycle
    {
        public ObjContainer SwitchNodeContainer;
        public Transform Group1;
        public Transform Group2;
        public float Time = 1;

        public MahjongSwitchNode[] SwitchNodes { get; private set; }
        public Action OnCallback { get; set; }

        public MahjongSwitchNode this[int index]
        {
            get { return SwitchNodes[index]; }
        }

        public void Oninit(int count)
        {
            SwitchNodes = SwitchNodeContainer.GetComponent<MahjongSwitchNode>(count);
            GameCenter.RegisterCycle(this);
        }

        public void OnGameReadyCycle()
        {
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        public void StartRotation(int iType)
        {
            switch (GameCenter.DataCenter.MaxPlayerCount)
            {
                case 3: RotationAnimationBy3(iType); break;
                case 4: NormalRotationAnimation(iType); break;
            }
        }

        /// <summary>
        /// 换张动画
        ///  0 顺时针
        ///  1 逆时针
        ///  2 对家
        /// </summary>
        private void NormalRotationAnimation(int type)
        {
            switch (type)
            {
                case 0:
                    transform.DOLocalRotate(new Vector3(0, 90, 0), 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    });
                    break;
                case 1:
                    transform.DOLocalRotate(new Vector3(0, -90, 0), 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
                    {
                        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    });
                    break;
            }
        }

        /// <summary>
        /// 3人换张动画
        /// 三人血战换牌的时候，g1 转90， g2转180 
        ///  0 顺时针
        ///  1 逆时针   
        /// </summary>
        private void RotationAnimationBy3(int type)
        {
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;
            switch (type)
            {
                case 0:
                    v1 = new Vector3(0, 90, 0);
                    v2 = new Vector3(0, 180, 0);
                    break;
                case 1:
                    v1 = new Vector3(0, -90, 0);
                    v2 = new Vector3(0, -180, 0);
                    break;
            }
            Group1.DOLocalRotate(v1, 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                Group1.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            });
            Group2.DOLocalRotate(v2, 1.2f).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                Group2.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            });

        }
    }
}