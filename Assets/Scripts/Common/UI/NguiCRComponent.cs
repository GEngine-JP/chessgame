using Assets.Scripts.Common.Models.CreateRoomRules;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class NguiCRComponent : NguiView
    {
        /// <summary>
        /// 提示按钮
        /// </summary>
        public GameObject TipBtn;

        protected override void OnAwake()
        {
            InitStateTotal = 2;
            CheckIsStart = true;
            InitWidgetPivot(); 
        }

        /// <summary>
        /// 初始化widget的原点位置
        /// </summary>
        private void InitWidgetPivot()
        {
            var widget = GetComponent<UIWidget>();
            if (widget != null)
            {
                widget.pivot = UIWidget.Pivot.TopLeft;
            }
        }

        protected override void OnFreshView()
        {
            var itemData = GetData<ItemData>();
            if (itemData != null)
            {
                if (TipBtn != null)
                {
                    var tip = itemData.Tip;
                    TipBtn.SetActive(!string.IsNullOrEmpty(tip));
                }
                OnFreshCRCView(itemData);
                var dict = itemData.Parent.CreateArgs;
                dict[itemData.Id] = itemData;
                UpdateWidget(itemData.Width, itemData.Height);
            }
            CallBack(IdCode);
        }

        public override Rect UpdateWidget(float width = 0, float height = 0)
        {
            var widget = GetWidgetAdapter();
            if (widget != null)
            {
                var bound = Bounds;
                bound.size = new Vector3(widget.Width, widget.Height);
                Bounds = bound;
            }
            return base.UpdateWidget(width, height);
        }

        // ReSharper disable once InconsistentNaming
        protected virtual void OnFreshCRCView(ItemData itemData)
        {
        }

        public virtual void UpdateBoxCollider()
        { 
            NGUITools.UpdateWidgetCollider(gameObject,true); 
        }

        private static YxView _curView;
        /// <summary>
        /// 
        /// </summary>
        public void ShowTip()
        {
            if (_curView == this)
            {
                UITooltip.Hide();
                _curView = null;
                return;
            }
            _curView = this;
            var itemData = GetData<ItemData>();
            if (itemData == null) return;
            UITooltip.Show(itemData.Tip);
        }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            return true;
        }
    } 
}
