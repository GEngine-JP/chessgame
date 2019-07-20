using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class DicAnimotor : MonoBehaviour
    {
        public static DicAnimotor Instance;
        protected void Awake()
        {
            Instance = this;
        }
        public void AnimotorJieSu()
        {
            DicMode.Instance.ShowPNumber();
        }
    }
}

