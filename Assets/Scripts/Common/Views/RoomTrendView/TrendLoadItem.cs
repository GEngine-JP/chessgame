using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Common.Views.RoomTrendView
{
    public class TrendLoadItem : MonoBehaviour
    {
        /// <summary>
        /// �Ƿ���ʾ��Ӯ
        /// </summary>
        public bool ShowWinOrLose;
        /// <summary>
        /// ��ǰԤ����ı���ͼ
        /// </summary>
        public UISprite BackGround;

        public UIGrid ItemGrid;
        /// <summary>
        /// ����ͼ��SpriteName
        /// </summary>
        public List<string> BackGroundNames;
        /// <summary>
        /// ��ʾ����
        /// </summary>
        public UILabel ShowContent;
        /// <summary>
        /// ��ʾ�ֵĲ�ͬ��ɫ
        /// </summary>
        public List<Color> ShowContentColors;
        /// <summary>
        /// ׯ���ӵĵ�
        /// </summary>
        public GameObject ZhuangDui;
        /// <summary>
        /// �ж��ӵĵ�
        /// </summary>
        public GameObject XianDui;

        protected void OnDisable()
        {
            StopCoroutine("FlashSprite");
            var item = BackGround.GetComponent<TweenAlpha>();
            if(!item)return;
            item.value = 1;
            item.enabled = false;
        }

        /// <summary>
        /// ���ñ���ͼƬ
        /// </summary>
        /// <param name="type"></param>
        public void SetItemBg(int type)
        {
            if (BackGroundNames == null) return;
            gameObject.SetActive(true);
            BackGround.spriteName = BackGroundNames[type];
        }

        public void SetItemBg(List<string> winAreas)
        {
            if (BackGroundNames == null) return;
            gameObject.SetActive(true);

            List<UISprite> roadItemList = new List<UISprite>();

            foreach (Transform child in ItemGrid.transform)
            {
                UISprite item = child.GetComponent<UISprite>();
                if (item)
                {
                    item.gameObject.SetActive(false);
                    roadItemList.Add(item);
                }
            }

            for (int i = 0; i < winAreas.Count; i++)
            {
                UISprite trendLoadItem;
                if (roadItemList.Count > 0 && roadItemList[0] != null)
                {
                    trendLoadItem = roadItemList[0];
                    trendLoadItem.gameObject.SetActive(true);
                    roadItemList.RemoveAt(0);
                }
                else
                {
                    trendLoadItem = YxWindowUtils.CreateItem(BackGround, ItemGrid.transform);
                }

                if (ShowWinOrLose)
                {
                    trendLoadItem.spriteName = winAreas[i].Equals("") ? "dishLose" : "dishWin";
                }
                else
                {
                    trendLoadItem.spriteName = string.Format("dish{0}", winAreas[i]);
                }
            }
        }



        public void StartFlash()
        {
            StartCoroutine("FlashSprite");
        }

        IEnumerator FlashSprite()
        {
            if (BackGround.GetComponent<TweenAlpha>())
            {
                TweenAlpha item = BackGround.GetComponent<TweenAlpha>();
                item.PlayForward();
                var needTime = item.duration;
                yield return new WaitForSeconds(needTime * 5);
                item.value = 1;
                item.enabled = false;
            }

        }

        /// <summary>
        /// ������ʾ����
        /// </summary>
        /// <param name="str"></param>
        /// <param name="col"></param>
        public void SetItemContent(string str, int col = 0)
        {
            if (ShowContent)
            {
                ShowContent.text = str;
                if (ShowContentColors.Count != 0)
                {
                    ShowContent.color = ShowContentColors[col];
                }
            }
        }
        /// <summary>
        /// ����ׯ���ӻ����ж��ӵĵ�ͼƬ����ʾ
        /// </summary>
        public void SetDuiZi(bool showZhuang, bool showXian)
        {
            if (ZhuangDui)
            {
                ZhuangDui.SetActive(showZhuang);
            }

            if (XianDui)
            {
                XianDui.SetActive(showXian);
            }
        }
    }
}
