using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.Windows.MatchWindows
{

    /// <summary>
    /// Item
    /// </summary>
    public class YxMatchItem : YxView
    {
        [Tooltip("��������")]
        public YxBaseLabelAdapter MatchName;
        [Tooltip("��������")]
        public YxBaseLabelAdapter NumOfParticipants;
        [Tooltip("������Ϸ")]
        public YxBaseLabelAdapter GameName;
        [Tooltip("����ͼ��")]
        public YxBaseTextureAdapter Icon;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<MatchItemData>();
            if (data == null) { return;}
            FreshMatchName(data.Name);
            FreshNumOfParticipants(data.Pcount);
            FreshGameName(data.GameName);
            FreshIcon(data.IconUrl);
        }

        private void FreshIcon(string dataIconUrl)
        {
            AsyncImage.Instance.GetAsyncImage(dataIconUrl, (t2, code) =>
            {
                if (Icon == null) return;
                Icon.SetTextureWithCheck(t2, code);
            });
        }

        /// <summary>
        /// ˢ�±ȱ�������
        /// </summary>
        private void FreshMatchName(string matchName)
        {
            if (!MatchName) return;
            MatchName.Text(matchName);
        }

        /// <summary>
        /// ˢ�²���������
        /// </summary>
        private void FreshNumOfParticipants(int numOfParticipants)
        {
            if (!NumOfParticipants) return;
            NumOfParticipants.Text(numOfParticipants.ToString());
        }

        /// <summary>
        /// ˢ����Ϸ��
        /// </summary>
        private void FreshGameName(string gameName)
        {
            if (!MatchName) return;
            GameName.Text(gameName);
        }

        /// <summary>
        /// ����
        /// </summary>
        public void OnSignUp()
        {
            //ԤԼ
            //����
            //��ѽ���
        }


        /// <summary>
        /// ����item����
        /// </summary>
        public class MatchItemData
        {
            public string Name;
            public int Pcount;
            public string GameName;
            public string IconUrl;

            public void Parse(Dictionary<string,object> dict)
            {
                if (dict == null) return;
                dict.Parse("name",ref Name);
                dict.Parse("pcount",ref Pcount);
                dict.Parse("gameName",ref GameName);
                dict.Parse("icon", ref IconUrl);
            }
        }
    }
}
