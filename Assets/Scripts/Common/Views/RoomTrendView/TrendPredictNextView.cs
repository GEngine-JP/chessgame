using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views.RoomTrendView
{
    public class TrendPredictNextView : YxView
    {
        [Tooltip("�¾ֺ��д���·��Item")]
        public TrendLoadItem NextBlackBigEye;
        [Tooltip("�¾ֺ���С·��Item")]
        public TrendLoadItem NextBlackSmall;
        [Tooltip("�¾ֺ���Сǿ·��Item")]
        public TrendLoadItem NextBlackRoach;
        [Tooltip("�¾ֺ��д���·��Item")]
        public TrendLoadItem NextRedBigEye;
        [Tooltip("�¾ֺ���С·��Item")]
        public TrendLoadItem NextRedSmall;
        [Tooltip("�¾ֺ���Сǿ·��Item")]
        public TrendLoadItem NextRedRoach;


        private ITrendCfg TrendCfg
        {
            get
            {
                return GetComponent<ITrendCfg>();
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null) return;
            GetNext((List<ITrendReciveData>)Data);
        }

        private void GetNext(List<ITrendReciveData> recorDatas)
        {
            var nextData = TrendCfg.CreatTrendReciveData(null).SetResultArea();
            recorDatas.Add(nextData);

            var road = new RoadNodeTable(recorDatas, 6);
            var bigEyeRoad = new RoadNodeTable(road, EnumTrendType.BigEyeRoad, 6);
            GetNextItem(bigEyeRoad, NextBlackBigEye, NextRedBigEye);

            var smallRoad = new RoadNodeTable(road, EnumTrendType.SmallRoad, 6);
            GetNextItem(smallRoad, NextBlackSmall, NextRedSmall);

            var roachRoad = new RoadNodeTable(road, EnumTrendType.RoachRoad, 6);
            GetNextItem(roachRoad, NextBlackRoach, NextRedRoach);

            recorDatas.RemoveAt(recorDatas.Count - 1);
        }

        private void GetNextItem(RoadNodeTable road, TrendLoadItem nextBlack, TrendLoadItem nextRed)
        {
            if (road.Nodes.Count == 0)
            {
                nextBlack.gameObject.SetActive(false);
                nextRed.gameObject.SetActive(false);
                return;
            }
            nextBlack.gameObject.SetActive(true);
            var nextBlackType = road.Nodes[road.Nodes.Count - 1].IsRed ? 0 : 1;
            nextBlack.SetItemBg(nextBlackType);
            var nextRedType = !road.Nodes[road.Nodes.Count - 1].IsRed ? 0 : 1;
            nextRed.SetItemBg(nextRedType);
            nextRed.gameObject.SetActive(true);
        }
    }
}
