using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class GameBackUI : MonoBehaviour
    {
        public static GameBackUI Instance;
        public Transform GameBackUibg;

        protected void Awake()
        {
            Instance = this;
        }

        public void ShowSelf()
        {
            if (!GameBackUibg.gameObject.activeSelf)
                GameBackUibg.gameObject.SetActive(true);
        }

        public void HideSelf()
        {
            if (GameBackUibg.gameObject.activeSelf)
                GameBackUibg.gameObject.SetActive(false);
        }

    }
}

