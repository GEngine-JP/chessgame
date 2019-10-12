using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PoolsComponent : BaseComponent
    {
        private Dictionary<string, ObjectPoolBase> mMahObjectPool = new Dictionary<string, ObjectPoolBase>();

        public override void OnInitalization()
        {
        }

        public T GetPool<T>(string poolName) where T : ObjectPoolBase
        {
            T pool = null;
            if (mMahObjectPool.ContainsKey(poolName))
            {
                pool = mMahObjectPool[poolName] as T;
            }
            else
            {
                pool = PoolUitlity.CreateObjectPool<T>(poolName).ExSetParent(transform);
                mMahObjectPool[poolName] = pool;
            }
            return pool;
        }

        public void ClearPool<T>(string poolName) where T : ObjectPoolBase
        {
            ObjectPoolBase pool;
            if (mMahObjectPool.TryGetValue(poolName, out pool))
            {
                mMahObjectPool.Remove(poolName);
                DestroyImmediate(pool.gameObject);
            }
        }
    }
}