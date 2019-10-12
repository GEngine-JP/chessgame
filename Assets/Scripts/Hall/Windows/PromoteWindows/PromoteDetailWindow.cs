using System;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.Windows.PromoteWindows
{
    /// <summary>
    /// �ƹ���ϸ�б�
    /// </summary>
    public class PromoteDetailWindow : YxWindow
    {
        [Tooltip("������")]
        public string ActionName = "spreadDetail";
        [Tooltip("������Ҫ��ʾ������")]
        public int ShowCount = 10;
        [Tooltip("���������б�")]
        public YxBasePopListAdapter DatePopListAdapter;
        [Tooltip("ItemԤ����")]
        public PromoteDetailItem PrefabItem;
        [Tooltip("����������")]
        public GameObject HasDataContainer;
        [Tooltip("û����������")]
        public GameObject NoDataContainer;
        [Tooltip("Item����")]
        public YxBaseGridAdapter PrefabGrid;

        private YxBaseGridAdapter _grid;
        protected override void OnStart()
        {
            var now = DateTime.Now;
            for (var i = 0; i < ShowCount; i++)
            {
                var tmp = now.AddDays(-i);
                var itemName = tmp.ToString("yyyy-MM-dd");
                if (i == 0)
                {
                    DatePopListAdapter.Set(itemName);
                    SendAction(itemName);
                }
                DatePopListAdapter.AddItem(tmp.ToString(itemName));
            }
        }

        private string _curItem;
        protected void SendAction(string item)
        {
            if (_curItem == item) return;
            _curItem = item;
            var dict = new Dictionary<string, object>();
            dict["date"] = item;
            CurTwManager.SendAction(ActionName,dict,UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null)
            {
                SetChangeView(false);
                return;
            }
            if (!dict.ContainsKey("data"))
            {
                SetChangeView(false);
                return;
            }
            var list = dict["data"] as List<object>;
            if (list == null || list.Count <= 0) 
            {
                SetChangeView(false);
                return;
            }

            YxWindowUtils.CreateMonoParent(PrefabGrid,ref _grid,PrefabGrid.transform.parent);
            var parentTs = _grid.transform;
            var count = list.Count;
            for (var i = 0; i < count; i++)
            {
                var infoData = list[i];
                var info = new PromoteDetailInfo();
                info.Parse(infoData);
                var item = CreateItem(parentTs);
                item.UpdateView(info);
            }
            SetChangeView(true);
            _grid.Reposition();
        }

        private YxView CreateItem(Transform parentTs)
        {
            return YxWindowUtils.CreateItem(PrefabItem, parentTs);
        }

        protected void SetChangeView(bool hasData)
        {
            if (NoDataContainer != null)
            {
                NoDataContainer.SetActive(!hasData);
            }
            if (HasDataContainer != null)
            {
                HasDataContainer.SetActive(hasData);
            }
        }

        /// <summary>
        /// �����б�
        /// </summary>
        public void OnPopListChanged(string item)
        {
            SendAction(item);
        }

        public override YxEUIType UIType
        {
            get { return GetComponent<YxBaseAdapter>().UIType; }
        }
    }
}
