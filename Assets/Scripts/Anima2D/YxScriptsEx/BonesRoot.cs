using Anima2D;
using UnityEngine;

namespace Assets.Scripts.Common.components
{
    /// <summary>
    /// 骨骼
    /// </summary>
    public class BonesRoot : MonoBehaviour
    {
        public int Order;
        public SpriteMeshInstance[] Bones;
        public SpriteRenderer[] EspecialSkin;

        protected void Awake()
        {
            var len = Bones.Length;
            for (var i = 0; i < len; i++)
            {
                var bones = Bones[i];
                bones.sortingOrder += Order;
            }
            len = EspecialSkin.Length;
            for (var i = 0; i < len; i++)
            {
                var skin = EspecialSkin[i];
                skin.sortingOrder += Order;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Add All SpriteMeshInstance")]
        protected void AddAllChildAdapter()
        {
            Bones = transform.GetComponentsInChildren<SpriteMeshInstance>(true);
        }

        /// <summary>
        /// 
        /// </summary>
        [ContextMenu("Add All SpriteRenderer")]
        protected void AddAllSpriteRenderer()
        {
            EspecialSkin = transform.GetComponentsInChildren<SpriteRenderer>(true);
        }
    }
}
