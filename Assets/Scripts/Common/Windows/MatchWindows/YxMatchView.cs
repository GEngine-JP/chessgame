using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows.MatchWindows
{
    /// <summary>
    /// ������ͼ
    /// </summary>
    public class YxMatchView : YxView
    {
        /// <summary>
        /// ����ItemԤ����
        /// </summary>
        public YxMatchItem PrefabMatchItem;
        /// <summary>
        /// gridԤ����
        /// </summary>
        public YxBaseGridAdapter PrefabGridAdapter;
        private YxBaseGridAdapter _gridAdapter;

        public string ActionName = "getMatchList";

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data is string) // ��������
            {
                var param = new Dictionary<string, object>();
                param["type"] = Data.ToString();
                CurTwManager.SendAction(ActionName,param,UpdateView);
                return;
            }
            YxWindowUtils.CreateMonoParent(PrefabGridAdapter, ref _gridAdapter);
            //����
            var dict = GetData<Dictionary<string,object>>();
            if (dict == null) { return;}
            List<object> list = null;
            if (dict.Parse("list", ref list))
            {
                FreshItems(list);
            }
        }

        /// <summary>
        /// ˢ��items
        /// </summary>
        /// <param name="list"></param>
        private void FreshItems(List<object> list)
        {
            var count = list.Count;
            var pts = _gridAdapter.transform;
            for (var i = 0; i < count; i++)
            {
                var itemData = new YxMatchItem.MatchItemData();
                itemData.Parse(list[i] as Dictionary<string,object>);
                var item = YxWindowUtils.CreateItem(PrefabMatchItem, pts);
                item.UpdateView(itemData);
            }
            _gridAdapter.Reposition();
        }
    }
}
