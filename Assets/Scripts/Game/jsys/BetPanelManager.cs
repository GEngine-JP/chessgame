using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jsys
{
    public class BetPanelManager : MonoBehaviour
    {

        public static BetPanelManager Instance;

        //我的金币

        public Text MyMoneyText;
        //彩金

        public Text WiningText;
        //得分

        public Text GetMoneyText;
        //切换筹码

        public Text ChipText;
        //下注各动物按钮

        public Button[] BetButtons;
        //清空按钮

        public Button ClearButton;
        //续投按钮

        public Button GoOnButton;
        //各区域押注数量

        public Text[] BetTexts;
        //各区域倍数

        public Text[] MultipleTexts;
        //临时假数据，切换筹码

        public Transform Uitf;

        public Transform Hidetf;

        public Vector3 RealHideTf;

        protected void Start()
        {
            Instance = this;
            _bb = new int[12];
            ChipText.transform.parent.GetComponent<Button>().onClick.AddListener(OnClickBtn);
        }
        //按钮事件
        private void OnClickBtn()
        {
            ClickBtn();
        }

        private bool ClickBtn()
        {
            bool panDuan = false;
            if (App.GetGameData<GlobalData>().StartBet)
            {
                if (Num[BetIndex] > App.GetGameData<GlobalData>().UserMoney)
                {
                    Instance.ShowBetButton(false);
                }
                else
                {
                    Instance.ShowBetButton(true);
                    panDuan = true;
                }
            }
            return panDuan;
        }

        //新一把游戏开始
        public void GameBeginXizhu()
        {
            //Debug.Log("所有游戏状态回到最初");
            MusicManager.Instance.Stop();
//            MusicManager.Instance.PlayBacksound("Beijing");
            //隐藏结算UI
            //TurnGroupsManager.Instance.GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Bet;  
            ResultUIManager.Instance.HideJieSuanUI();
            AnimationManager.Instance.HideBetPanel();
            //倍率显示
            TurnGroupsManager.Instance.GameConfig.Imultiplying = App.GetGameData<GlobalData>().Multiplying;

            //筹码显示归零
            //ShowChipTextUI(Chip);自己注释的
            ButtonUIInit();
            SetMoney(App.GetGameData<GlobalData>().UserMoney);
            //总筹码显示           
            Instance.ShowIgetMoney(0);
            Instance.ShowiWiningText(0);
            ModelManager.Instance.ChangeToHaidi();
        }
        //新一把游戏下注界面清零
        public void ButtonUIInit()
        {
            for (int i = 0; i < 12; i++)
            {
                _bb[i] = 0;
                BetTexts[i].text = "0";
            }
        }

        //显示彩金Text
        public void ShowiWiningText(int iwining)
        {
            WiningText.text = iwining + "";
        }
        //显示得分切换Text
        public void ShowIgetMoney(long igetmoney)
        {
            GetMoneyText.text = igetmoney + "";
        }

        //游戏等待状态
        public void Gamewaitshow()
        {
            MusicManager.Instance.Stop();
            AudioPlay.Instance.PlaySounds("Xiazhu");
            TurnGroupsManager.Instance.GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Bet;
            Instance.ShowBetButton(false);
            //显示押注面板
            if (!TurnGroupsManager.Instance.GameConfig.IsBetPanelOnShow)
            {
                ShowUI();
            }
        }
        //倍率显示
        public void ShowImultiply(int[] imultiplying)
        {
            for (int i = 0; i < 12; i++)
            {
                MultipleTexts[i].text = "x" + imultiplying[i];
            }
        }

        /// <summary>
        ///显示押注可操作状态
        /// </summary>
        /// <param name="isCanBet"></param>
        public void ShowBetButton(bool isCanBet)
        {
            for (int i = 0; i < BetButtons.Length; i++)
            {
                BetButtons[i].interactable = isCanBet;
            }
            GoOnButton.interactable = isCanBet;
            ClearButton.interactable = isCanBet;
        }
        //显示金币
        public void SetMoney(long money)
        {
            MyMoneyText.text = money + "";
        }

        //显示下注面板
        public void ShowUI()
        {
            transform.DOMove(Uitf.position, 1f).SetEase(Ease.OutBounce);
            TurnGroupsManager.Instance.GameConfig.IsBetPanelOnShow = true;
        }
        //隐藏下注面板
        public void HideUI()
        {
            transform.DOMove(Hidetf.position, 0.5f);
            TurnGroupsManager.Instance.GameConfig.IsBetPanelOnShow = false;
        }

        /// <summary>
        /// 发送下注
        /// </summary>
        public void SendBet()
        {
            bool isNull = false;
            foreach (int i in _bb)
            {
                if (i != 0)
                {
                    isNull = true;
                }
            }
            if (isNull)
            {
                for (int i = 0; i < _bb.Length; i++)
                {
                    _xuYa[i] = _bb[i];
                }
                App.GetRServer<GameServer>().ClickedToSend(_bb);
                App.GameData.GStatus = GameStatus.PlayAndConfine;
            }
            for (int index = 0; index < _bb.Length; index++)
            {
                _bb[index] = 0;
            }
            _isXuya = true;
        }
        //押注数据
        private int[] _bb;
        //续压数据
        private int[] _xuYa = new int[12];
        private bool _isClear;
        //押注数据显示
        public void ShowBetData(int num)
        {
            if (_isClear)
            {
                for (int i = 0; i < _bb.Length; i++)
                {
                    _bb[i] = _xuYa[i];
                }
                _isClear = false;
            }
            _bb[num] += Num[BetIndex];

            BetTexts[num].text = _bb[num] + "";
            MyMoneyText.text = (App.GetGameData<GlobalData>().UserMoney - Num[BetIndex]) + "";
        }
        //下注控件
        private bool _isXuya = true;
        public void Beting(int num)
        {
            if (num <= 11)
            {
                AudioPlay.Instance.PlaySounds("Xiazhu");
                Debug.Log("num" + num);
                if (ClickBtn())
                {
                    ShowBetData(num);
                    App.GetGameData<GlobalData>().UserMoney -= Num[BetIndex];
                }
                _isXuya = true;
            }
            else if (num == 12)
            {
                AudioPlay.Instance.PlaySounds("Xuya");

                for (int i = 0; i < _bb.Length; i++)
                {
                    BetTexts[i].text = _xuYa[i] + "";
                    App.GetGameData<GlobalData>().UserMoney -= _xuYa[i];
                    _bb[i] = _xuYa[i];
                    _isClear = true;
                }
                if (_isXuya)
                {
                    MyMoneyText.text = App.GetGameData<GlobalData>().UserMoney + "";
                    _isXuya = false;
                }
            }
            else if (num == 13)
            {
                AudioPlay.Instance.PlaySounds("Xiazhu");
                for (int index = 0; index < _bb.Length; index++)
                {
                    App.GetGameData<GlobalData>().UserMoney += _bb[index];
                }
                ButtonUIInit();
                _isClear = false;
                MyMoneyText.text = App.GetGameData<GlobalData>().UserMoney + "";
                _isXuya = true;
            }
        }
        //筹码的数值
        public int[] Num;
        //筹码数值的索引
        public int BetIndex;
        //切换筹码
        public void ChangeChips()
        {
            BetIndex++;
            if (BetIndex >= Num.Length)
            {
                BetIndex = 0;
            }
            ChipText.text = Num[BetIndex] + "";
            AudioPlay.Instance.PlaySounds("Qiehuan");
        }
    }
}
