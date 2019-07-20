using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.bjl3d
{
    public class GameUI : MonoBehaviour//改11 15
    {

        public static GameUI Instance;

        public Button SuperBtn;
        public Transform SuperUI;

        public Transform GameBackUItf;

        public Transform SettleMentUItf;

        private Transform note_textTF;

        int aliasingValue = 1;

        protected void Awake()
        {
            Instance = this;
            note_textTF = transform.Find("NoteText");
        }

        /// <summary>
        /// 游戏结算面板
        /// </summary>
        public void GameResult()
        {
            StartCoroutine("Wait");
        }
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(7);
            if (!SettleMentUItf.gameObject.activeSelf)
                SettleMentUItf.gameObject.SetActive(true);
            SettleMentUI st = SettleMentUItf.GetComponent<SettleMentUI>();

            if (st != null)
                st.GameResultFun();
            UserInfoUI.Instance.ShowSelfInfoUI();//刷新玩家自己的信息
            Invoke("HideSettleMentUI", 7f);
            CoinTypeInfoUI.Intance.SelectClickeCoinTypeAudio(-1);
        }
        /// <summary>
        /// 推出房间
        /// </summary>
        public void ReturnToHall()
        {
            if (App.GameData.GStatus != GameStatus.PlayAndConfine)
            {
                if (!GameBackUItf.gameObject.activeSelf)
                    GameBackUItf.gameObject.SetActive(true);
            }
            else
            {
               Instance.NoteText_Show("游戏正在进行中，请稍后！！！");
            }
            
        }
        /// <summary>
        /// 结果调用
        /// </summary>
        public void HideSettleMentUI()
        {
            if (SettleMentUItf.gameObject.activeSelf)
                SettleMentUItf.gameObject.SetActive(false);
            GameScene.Instance.ClearPai();
            CoinTypeInfoUI.Intance.SelectClickeCoinTypeAudio(UserInfoUI.Instance.GameConfig.CoinType);

        }
        //
        /// <summary>
        ///退出大厅
        /// </summary>
        public void SurReturnHall()//按钮
        {
            App.QuitGame();
        }
        /// <summary>
        /// 取消返回游戏
        /// </summary>
        public void CancleReturnHall()//按钮
        {
            if (GameBackUItf.gameObject.activeSelf)
                GameBackUItf.gameObject.SetActive(false);
        }
        /// <summary>
        /// 隐藏列表UI
        /// </summary>
        public void HideBtmUI()
        {
            UerInfoCountDownLuziUI.Intance.ClickeUIFun();
        }
        /// <summary>
        /// 下不了注文本显示
        /// </summary>
        /// <param name="str"></param>
        public void NoteText_Show(string str = "")
        {
            if (note_textTF.gameObject.activeSelf)
                note_textTF.gameObject.SetActive(false);
            note_textTF.gameObject.SetActive(true);
            if (str != "")
            {
                Text _text = note_textTF.GetComponent<Text>();
                if (_text == null) return;
                _text.text = str;
            }
        }
        public void OnSetting()
        {
            SettingPnl.Instance.Show(true);
        }

    }
}

