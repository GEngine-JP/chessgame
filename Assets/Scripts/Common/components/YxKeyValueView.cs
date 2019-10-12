using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.components
{
    /// <inheritdoc />
    /// <summary>
    /// key��value����ͼ
    /// </summary>
    public class YxKeyValueView : YxView
    {
        [Tooltip("key��Label")]
        public YxBaseLabelAdapter KeyLable;
        [Tooltip("value��Label")]
        public YxBaseLabelAdapter ValueLable;
        [Tooltip("value��ͼ��")]
        public YxBaseTextureAdapter Icon;
        [Tooltip("�ָ��ʾ")]
        public char SpliteFlag = ':';

        protected override void OnFreshView()
        {
            var data = GetData<YxKeyValueData>();
            if (data == null) { return;}
            SetKeyLabel(data.Key);
            SetValueLabel(data.Value);
            SetIcon(data.IconUrl);
        }

        /// <summary>
        /// ����key
        /// </summary>
        protected virtual void SetKeyLabel(string key)
        {
            if (KeyLable == null) return;
            string.Format("{0}{1}", key, SpliteFlag);
            KeyLable.Text(key);
        }

        /// <summary>
        /// ����value
        /// </summary>
        protected virtual void SetValueLabel(string value)
        {
            if (ValueLable == null) return;
            ValueLable.Text(value);
        }

        /// <summary>
        /// ����ͼƬ
        /// </summary>
        protected virtual void SetIcon(string iconUrl)
        {
            YxAdapterUtile.SetTexture(Icon, iconUrl);
        }
    }

    public class YxKeyValueData
    {
        public string Key;
        public string Value;
        public string IconUrl;
    }
}
