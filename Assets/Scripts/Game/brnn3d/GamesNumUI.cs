using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class GamesNumUI : MonoBehaviour
    {
        public static GamesNumUI Instance;

        public Text JuText;
        public Text BaText;

        //获取组件
        protected void Awake()
        {
            Instance = this;
        }

        public void SetGamesNumUI()
        {
            if (JuText != null)
                JuText.text = App.GetGameData<GlobalData>().Frame + "";
            if (BaText != null)
                BaText.text = App.GetGameData<GlobalData>().Bundle + "";
        }

    }
}

