using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PoolUitlity
    {
        public const string Po_EffectObject = "EffectObject";   

        public static T CreateObjectPool<T>(string name) where T : ObjectPoolBase
        {
            GameObject obj = new GameObject("[" + name + "-Pool]");
            return obj.AddComponent<T>();
        }
    }
}
