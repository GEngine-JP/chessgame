using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.bjl3d
{
    public class BetMoneyUI : MonoBehaviour// G 2016年11月15日 16:21:14
    {

        public static BetMoneyUI Intance;

        private Text[] selfNoteTexts = new Text[8];
        private Text[] quyuNoteTexts = new Text[8];
        private Transform[] noteAreas = new Transform[8];

        protected void Awake()
        {
            Transform tf = null;
            for (int i = 0; i < 8; i++)
            {
                tf = transform.Find("BetArea" + i + "/otherUI/other");
                if (tf == null)
                    YxDebug.LogError("No Such Object");
                if (tf != null) quyuNoteTexts[i] = tf.GetComponent<Text>();
                if (quyuNoteTexts[i] == null)
                    YxDebug.LogError("No Such Component");

                tf = transform.Find("BetArea" + i + "/selfUI/self");
                if (tf == null)
                    YxDebug.LogError("No Such Object");
                if (tf != null) selfNoteTexts[i] = tf.GetComponent<Text>();
                if (selfNoteTexts[i] == null)
                    YxDebug.LogError("No Such Component");

                noteAreas[i] = transform.Find("BetArea" + i);
                if (noteAreas[i] == null)
                    YxDebug.LogError("No Such Object");
            }

            Intance = this;
        }
        /// <summary>
        /// 下注区域显示
        /// </summary>
        public void BetMoneyArea()
        {
            StartCoroutine("WaitShowUI", 3);
        }

        IEnumerator WaitShowUI(float s)
        {
            yield return new WaitForSeconds(s);
            for (int i = 0; i < 8; i++)
            {
                noteAreas[i].gameObject.SetActive(true);
            }
        }

        public void BetMoneySelfNoteInfo(int area, int gold)
        {
            var goldNum = selfNoteTexts[area].text;
            int gold1;
            Int32.TryParse(goldNum, out gold1);
            selfNoteTexts[area].text = gold1 + gold + "";
            if (!selfNoteTexts[area].transform.parent.gameObject.activeSelf)
                selfNoteTexts[area].transform.parent.gameObject.SetActive(true);
        }

        public void BetMoneyquyuNoteInfo(int area, int gold)
        {
            var goldNum = quyuNoteTexts[area].text;
            int gold1;
            Int32.TryParse(goldNum, out gold1);
            quyuNoteTexts[area].text = gold1 + gold + "";
            if (!quyuNoteTexts[area].transform.parent.gameObject.activeSelf)
                quyuNoteTexts[area].transform.parent.gameObject.SetActive(true);
        }

        /// <summary>
        /// 清空下注信息
        /// </summary>
        public void BetMoneyQingKongInfo()
        {
            for (int i = 0; i < 8; i++)
            {
                selfNoteTexts[i].text = "";
                quyuNoteTexts[i].text = "";
                selfNoteTexts[i].transform.parent.gameObject.SetActive(false);
                quyuNoteTexts[i].transform.parent.gameObject.SetActive(false);
                noteAreas[i].gameObject.SetActive(false);
            }
        }
    }
}