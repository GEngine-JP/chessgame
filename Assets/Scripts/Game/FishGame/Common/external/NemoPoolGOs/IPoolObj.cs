using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs
{
    public interface IPoolObj {

        GameObject Prefab
        {
            get;
            set;

        }
        /// <summary>
        /// ����
        /// </summary>
        void On_Reuse(GameObject prefab);

        /// <summary>
        /// ����
        /// </summary>
        void On_Recycle();
    }
}
