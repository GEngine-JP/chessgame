using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.Manager;
using Assets.Scripts.Game.lswc.UI.Item;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Windows
{
    /// <summary>
    /// 下注面板
    /// </summary>
    public class LSBetWindow : MonoBehaviour
    {

        public UITweener Tween;

        public Text TotalGold;

        public Text TotalBet;

        public Image ChangeAnteBtnImage;

        public Text ChangeAnteText;

        [HideInInspector]
        public List<LSBetItem> BetItems;

        private bool _isShow=false;

        private void Awake()
        {
            BetItems = new List<LSBetItem>();
        }

        private void Start()
        {
           FindChild();
           Tween.SetOnFinished(OnHideFinished);
        }

        void FindChild()
        {
            Tween = transform.FindChild("LeftBottom").GetComponent<UITweener>();
            TotalGold = transform.FindChild("LeftBottom/TotalGold").GetComponent<Text>();
            ChangeAnteBtnImage= transform.FindChild("LeftBottom/ChangeAnteBtn").GetComponent<Image>();
            TotalBet = transform.FindChild("LeftBottom/TotalBet").GetComponent<Text>();
            ChangeAnteText = transform.FindChild("LeftBottom/ChangeAnteBtn/Text").GetComponent<Text>();
        }

        public void SetBetWindow()
        {
            foreach (var lsBetItem in BetItems)
            {
                lsBetItem.InitItem();
            }
            SetTotalBets();
            SetTotalGold();
        }

        public void SetTotalGold()
        {
            TotalGold.text = (App.GetGameData<GlobalData>().TotalGold - App.GetGameData<GlobalData>().TotalBets).ToString();
        }

        public void SetTotalBets()
        {
            TotalBet.text = App.GetGameData<GlobalData>().TotalBets.ToString();
        }

        public void Show()
        {
            _isShow = true;
            gameObject.SetActive(true);
            Tween.PlayForward();
        }

        public void Hide()
        {
            _isShow = false;
            Tween.PlayReverse();
        }

        private void OnHideFinished()
        {
            if (!_isShow)
            {
                gameObject.SetActive(false);
            }   
        }

        public void RefreshItems()
        {
            foreach (var lsBetItem in BetItems)
            {
                lsBetItem.RefreshItem();
            }
        }

        public void ChangeAnte()
        {
            ChangeAnteText.text = App.GetGameData<GlobalData>().GetNowAnte();

            ChangeAnteBtnImage.overrideSprite = App.GetGameData<GlobalData>().GetNowAnteSprite();
        }
    }
}
