using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongTable : MahjongTablePart, ISceneInitCycle
    {
        public bool IsCustom;
        public MeshRenderer TableMesh;
        public Transform TableLift;
        public float LiftOffset;
        public MeshRenderer[] Lifts;

        public void OnSceneInitCycle()
        {
            SwitchTableSkin();
        }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
        }

        /// <summary>
        /// 切换麻将桌子皮肤
        /// </summary>
        public void SwitchTableSkin()
        {
            if (IsCustom) return;

            var assetsName = "TableSkin_" + MahjongUtility.MahjongTableColor;
            var texture = GameUtils.GetAssets<Texture>(assetsName);
            if (texture != null)
            {
                TableMesh.material.mainTexture = texture;
                for (int i = 0; i < Lifts.Length; i++)
                {
                    Lifts[i].material.mainTexture = texture;
                }
            }
        }

        /// <summary>
        /// 麻将机下移动画
        /// </summary>
        public Tween TableDownAnimation(float time)
        {
            return TableLift.transform.DOLocalMoveY(LiftOffset, time);
        }

        /// <summary>
        /// 麻将机上升动画
        /// </summary>
        public Tween TableUpAnimation(float time)
        {
            return TableLift.transform.DOLocalMoveY(0, time);
        }
    }
}