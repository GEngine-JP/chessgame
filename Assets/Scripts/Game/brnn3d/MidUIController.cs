using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class MidUIController : MonoBehaviour
    {
        public static MidUIController Instance;
        protected void Awake()
        {
            Instance = this;
        }

        public void GameBackUINoClicked()
        {
            GameBackUI.Instance.HideSelf();
        }

    }

}
