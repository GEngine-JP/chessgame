using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// Ҫ��ʾ��������ʾ����
    /// </summary>
    public class DisplayGroupView : YxView
    {
        public bool NeedShow;
        /// <summary>
        /// ��Ҫ��ʾ�������صĶ���
        /// </summary>
        public GameObject[] Displays;

        protected override void OnEnable()
        {
            YxWindowUtils.DisplayUI(Displays, NeedShow);
        }

        protected override void OnDisable()
        {
            YxWindowUtils.DisplayUI(Displays, !NeedShow);
        }
    }
}
