using System;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.brnn3d
{
    public class BetMoneyUI : MonoBehaviour
    {
        public static BetMoneyUI Instance;
        public Text[] SelfNoteTexts = new Text[4];
        public Text[] QiyuNoteTexts = new Text[4];
        public Transform[] NoteAreas = new Transform[4];

        protected void Awake()
        {
            Instance = this;
        }
        //设置钱数的总界面的状态
        public void SetBetMoneyUI(bool isSelf)
        {
            for (int i = 0; i < 4; i++)
            {
                NoteAreas[i].gameObject.SetActive(true);
            }
            if (isSelf)
            {
                App.GameData.GStatus = GameStatus.PlayAndConfine;
                SetBetMoneyUI_Self();
            }
            SetBetMoneyUI_Other();
        }
        //设置玩家自己的下注钱数的面板
        public void SetBetMoneyUI_Self()
        {
            var b1 = SelfNoteTexts[App.GetGameData<GlobalData>().BetPos].text;
            int gold1;
            Int32.TryParse(b1, out gold1);
            App.GetGameData<GlobalData>().CurrentUser.Gold -= App.GetGameData<GlobalData>().BetMoney;
            SelfNoteTexts[App.GetGameData<GlobalData>().BetPos].text = gold1 + App.GetGameData<GlobalData>().BetMoney + "";
            if (!SelfNoteTexts[App.GetGameData<GlobalData>().BetPos].transform.parent.gameObject.activeSelf)
                SelfNoteTexts[App.GetGameData<GlobalData>().BetPos].transform.parent.gameObject.SetActive(true);
        }
        //设置其他玩家下注的钱数面板
        public void SetBetMoneyUI_Other()
        {
            var b1 = QiyuNoteTexts[App.GetGameData<GlobalData>().BetPos].text;
            int gold1;
            Int32.TryParse(b1, out gold1);
            QiyuNoteTexts[App.GetGameData<GlobalData>().BetPos].text = gold1 + App.GetGameData<GlobalData>().BetMoney + "";
            if (!QiyuNoteTexts[App.GetGameData<GlobalData>().BetPos].transform.parent.gameObject.activeSelf)
                QiyuNoteTexts[App.GetGameData<GlobalData>().BetPos].transform.parent.gameObject.SetActive(true);
        }
        //清空下注的钱数
        public void BetMoneyQingKongInfo()
        {
            for (int i = 0; i < 4; i++)
            {
                SelfNoteTexts[i].text = "";
                QiyuNoteTexts[i].text = "";
                SelfNoteTexts[i].transform.parent.gameObject.SetActive(false);
                QiyuNoteTexts[i].transform.parent.gameObject.SetActive(false);
                NoteAreas[i].gameObject.SetActive(false);
            }
        }
    }
}

