using Assets.Scripts.Hall.Models;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ListViews
{
    /// <inheritdoc />
    /// <summary>
    /// ������ʽ����
    /// </summary>
    public class DeskListItemStyleBaseView : YxView
    {
        protected DeskItemData ItemData;


        public override void Init(object initData)
        {
            base.Init(initData);
            ItemData = initData as DeskItemData;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void OnDeskClick()
        {
            var item = MainYxView as DeskListItem;
            if (item == null) return; 
            item.OnDeskClick();
        }
    }
}
