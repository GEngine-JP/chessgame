using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahHandCtrl : MonoBehaviour, IMahHandCtrl
    {
        public IMahHand OnIni()
        {
            IMahHand hand;
            switch (MahjongUtility.GameKey)
            {
                default: hand = gameObject.AddComponent<MahHand>(); break;
            }
            return hand;
        }
    }
}