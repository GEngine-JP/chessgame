using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jsys
{
    public class HandselManager : MonoBehaviour
    {
        public static HandselManager Instance;

        public Text BonuText;

        private float _currTimer;

        private float timerInver = 0.05f;

        protected void Awake()
        {
            Instance = this;
        }

        //设置彩金数量
        public void SetIwiningText(int iwining)
        {
            BonuText.text = iwining + "";
        }

        protected void Update()
        {
            if (TurnGroupsManager.Instance.GameConfig.TurnTableState == (int)GameConfig.GoldSharkState.Marquee)
            {
                _currTimer += Time.deltaTime;
                if (_currTimer > timerInver)
                {
                    TurnGroupsManager.Instance.GameConfig.BonuNumber = Random.Range(5000, 50000);
                    BonuText.text = TurnGroupsManager.Instance.GameConfig.BonuNumber + "";
                    BetPanelManager.Instance.ShowiWiningText(TurnGroupsManager.Instance.GameConfig.BonuNumber);
                    _currTimer = 0;
                }
            }
        }
    }
}
