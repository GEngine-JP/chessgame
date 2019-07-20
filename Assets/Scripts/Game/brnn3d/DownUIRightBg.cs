using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class DownUIRightBg : MonoBehaviour
    {
        public static DownUIRightBg Instance;
        public Transform[] CoinTypeEffs = new Transform[6];

        protected void Awake()
        {
            Instance = this;
        }

        public void ShowCoinTypeEffect(int index)
        {
            for (int i = 0; i < 6; i++)
            {
                if (i == index)
                    CoinTypeEffs[i].gameObject.SetActive(true);
                else
                    CoinTypeEffs[i].gameObject.SetActive(false);
            }
        }

    }
}

