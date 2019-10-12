using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongAnimation : MonoBehaviour
    {
        /// <summary>
        /// 麻将上升动画
        /// </summary>
        public void ActionMahRise(float duration = 0.02f)
        {
            transform.DOLocalMoveY(MiscUtility.MahjongSize.y * 0.5f + 0.1f, duration);
        }

        /// <summary>
        /// 麻将下落动画
        /// </summary>
        public void ActionMahDropDown(float duration = 0.02f)
        {
            transform.DOLocalMoveY(MiscUtility.MahjongSize.y * 0.5f, duration);
        }
    }
}