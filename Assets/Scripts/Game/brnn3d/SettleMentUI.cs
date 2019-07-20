using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn3d
{
    public class SettleMentUI : MonoBehaviour
    {
        public static SettleMentUI Instance;
        public Text SelfWinText;
        public Text SelfBackText;
        public Text BankerWinText;
        public Text BankerBackText;

        public Transform SettleMent;

        protected void Awake()
        {
            Instance = this;
        }

        public void SetSettleMentUI()
        {
            if (!SettleMent.gameObject.activeSelf)
                SettleMent.gameObject.SetActive(true);
            if (SelfWinText != null)
            {
                SelfWinText.text = App.GetGameData<GlobalData>().ResultUserTotal + "";
                if (App.GetGameData<GlobalData>().ResultUserTotal > 0)
                {
                    MusicManager.Instance.Play("win");
                }
                else
                {
                    MusicManager.Instance.Play("lost");
                }
            }
            else
            {
                if (SelfWinText != null) SelfWinText.text = "";
            }
            if (BankerWinText != null)
                BankerWinText.text = App.GetGameData<GlobalData>().ResultBnakerTotal + "";
            else
            {
                if (BankerWinText != null) BankerWinText.text = "";
            }
        }

        public void HideSettleMentUI()
        {
            Invoke("WaitToHideSettelMentUI", 8f);
        }

        void WaitToHideSettelMentUI()
        {
            if (SettleMent.gameObject.activeSelf)
                SettleMent.gameObject.SetActive(false);
            BeiShuMode.Instance.PlayBeiShuEff();
        }


    }
}

