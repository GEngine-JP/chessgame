using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.SysNoticeWindows
{
    /// <summary>
    /// ����item
    /// </summary>
    public class AnnouncementItemView : YxView
    {
        /// <summary>
        /// ����ͼƬ
        /// </summary>
        [Tooltip("ͼƬ����")]
        public UITexture Poster;
        [Tooltip("�ı�����")]
        public GameObject TextContent;
        [Tooltip("����")]
        public UILabel TitleLabel;
        [Tooltip("����")]
        public UILabel ContentLabel;
        [Tooltip("����")]
        public UILabel DataLabel;
        [Tooltip("����")]
        public UILabel AuthorLabel;

        protected override void OnFreshView()
        {
            var acData = GetData<AnnouncementData>();
            if (acData == null) return;
            if (Poster!=null)
            {
                if (!string.IsNullOrEmpty(acData.PosterUrl))
                { 
                    Poster.gameObject.SetActive(true);
                    if(TextContent!=null)TextContent.SetActive(false);
                    YxWindowManager.ShowWaitFor();
                    AsyncImage.Instance.GetAsyncImage(acData.PosterUrl, texture2d =>
                        {
                            YxWindowManager.HideWaitFor();
                            Poster.mainTexture = texture2d;
                        });
                    return;
                }
                Poster.gameObject.SetActive(false);
            }
            if (TextContent == null) return;
            TextContent.SetActive(true);
            if(TitleLabel!=null)TitleLabel.text = acData.Title;
            if (ContentLabel != null) ContentLabel.text = acData.Content;
            if (DataLabel != null) DataLabel.text = acData.Date;
            if (AuthorLabel != null) AuthorLabel.text = acData.Author;
        }

        public void OnActionClick()
        {
            var acData = GetData<AnnouncementData>();
            if (acData == null) return;
            if (string.IsNullOrEmpty(acData.ClickUrl)) return;
            Application.OpenURL(acData.ClickUrl);
        }
    }

    public class AnnouncementData
    {
        /// <summary>
        /// ͼƬurl
        /// </summary>
        public string PosterUrl = "";
        /// <summary>
        /// ���url
        /// </summary>
        public string ClickUrl = "";
        /// <summary>
        /// ����
        /// </summary>
        public string Title = ""; 
        /// <summary>
        /// ����
        /// </summary>
        public string Content = "";
        /// <summary>
        /// ����
        /// </summary>
        public string Date;
        /// <summary>
        /// ����
        /// </summary>
        public string Author;

        public AnnouncementData(IDictionary<string, object> obj)
        {
            if (obj == null) return;
            if (obj.ContainsKey("pic_url_x"))
            {
                var temp = obj["pic_url_x"];
                if (temp != null) PosterUrl = temp.ToString();
            }
            if (obj.ContainsKey("detail_url_x"))
            {
                var temp = obj["detail_url_x"];
                if (temp != null) ClickUrl = temp.ToString();
            }
            if (obj.ContainsKey("title_m"))
            {
                var temp = obj["title_m"];
                if (temp != null) Title = temp.ToString();
            }
            if (obj.ContainsKey("desc_x"))
            {
                var temp = obj["desc_x"];
                if (temp != null) Content = temp.ToString();
            }
            if (obj.ContainsKey("create_dt"))
            {
                var temp = obj["create_dt"];
                if (temp != null) Date = temp.ToString();
            }
            if (obj.ContainsKey("release_auther"))
            {
                var temp = obj["release_auther"];
                if (temp != null) Author = temp.ToString();
            }
        }
    }
}
