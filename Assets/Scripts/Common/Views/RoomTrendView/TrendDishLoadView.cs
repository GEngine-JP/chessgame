using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views.RoomTrendView
{
    public class TrendDishLoadView : YxView
    {
        [Tooltip("����·��Grid")]
        public UIGrid DishLoadGrid;
        [Tooltip("����·��Item")]
        public TrendLoadItem DishTrendLoadItem;
        [Tooltip("��·��View")]
        public TrendOtherLoadView BigLoadView;
        [Tooltip("����·��View")]
        public TrendOtherLoadView BigEyeLoadView;
        [Tooltip("С·��View")]
        public TrendOtherLoadView SmallLoadView;
        [Tooltip("Сǿ·��View")]
        public TrendOtherLoadView RoachLoadView;
        [Tooltip("�¾�Ԥ���View")]
        public TrendPredictNextView TrendPredictNextView;
        [Tooltip("�����ܵ�����View")]
        public TrendTotalInfoView TrendTotalInfoView;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null) return;
            var roadNodeTable = SetDishLoad((List<ITrendReciveData>)Data);

            if (BigLoadView)
            {
                BigLoadView.UpdateView(roadNodeTable);
            }

            if (BigEyeLoadView)
            {
                var bigEyeRoad = new RoadNodeTable(roadNodeTable, EnumTrendType.BigEyeRoad, 6);
                BigEyeLoadView.UpdateView(bigEyeRoad);
            }

            if (SmallLoadView)
            {
                var smallRoad = new RoadNodeTable(roadNodeTable, EnumTrendType.SmallRoad, 6);
                SmallLoadView.UpdateView(smallRoad);
            }

            if (RoachLoadView)
            {
                var roachRoad = new RoadNodeTable(roadNodeTable, EnumTrendType.RoachRoad, 6);
                RoachLoadView.UpdateView(roachRoad);
            }

            if (TrendPredictNextView)
            {
                TrendPredictNextView.UpdateView(Data);
            }

            if (TrendTotalInfoView)
            {
                TrendTotalInfoView.UpdateView(Data);
            }
        }

        protected RoadNodeTable SetDishLoad(List<ITrendReciveData> recordDatas)
        {
            List<TrendLoadItem> dishLoadItemList = new List<TrendLoadItem>();
            foreach (Transform child in DishLoadGrid.transform)
            {
                TrendLoadItem item = child.GetComponent<TrendLoadItem>();
                if (item)
                {
                    item.gameObject.SetActive(false);
                    dishLoadItemList.Add(item);
                }
            }

            for (int i = 0; i < recordDatas.Count; i++)
            {
                TrendLoadItem trendLoadItem;
                if (dishLoadItemList.Count > 0 && dishLoadItemList[0] != null)
                {
                    trendLoadItem = dishLoadItemList[0];
                    trendLoadItem.gameObject.SetActive(true);
                    dishLoadItemList.RemoveAt(0);
                }
                else
                {
                    trendLoadItem = YxWindowUtils.CreateItem(DishTrendLoadItem, DishLoadGrid.transform);
                }

                var recordData = recordDatas[i];
                trendLoadItem.SetItemBg(recordData.GetResultArea());
                if (i == recordDatas.Count - 1)
                {
                    trendLoadItem.StartFlash();
                }
            }

            DishLoadGrid.Reposition();
            DishLoadGrid.repositionNow = true;

            var roadNodeTable = new RoadNodeTable(recordDatas, 6);
            return roadNodeTable;
        }
    }
}
